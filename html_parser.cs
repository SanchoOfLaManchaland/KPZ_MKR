using System;
using System.Collections.Generic;
using System.Text;

namespace CustomHtmlParser
{
    public class HtmlParser
    {
        private HtmlParserState _currentState;
        private StringBuilder _textBuffer;
        private StringBuilder _tagBuffer;
        private StringBuilder _attributeNameBuffer;
        private StringBuilder _attributeValueBuffer;
        private Stack<HtmlElement> _elementStack;
        private HtmlElement _rootElement;
        private char _quoteChar;

        public char QuoteChar => _quoteChar;

        public HtmlParser()
        {
            _textBuffer = new StringBuilder();
            _tagBuffer = new StringBuilder();
            _attributeNameBuffer = new StringBuilder();
            _attributeValueBuffer = new StringBuilder();
            _elementStack = new Stack<HtmlElement>();
            _currentState = new TextState(this);
            _quoteChar = '\0';
        }

        public HtmlElement Parse(string html)
        {
            Reset();

            foreach (char ch in html)
            {
                _currentState.ParseCharacter(ch);
            }

            FlushTextBuffer();

            return _rootElement;
        }

        public void SetState(HtmlParserState state)
        {
            _currentState = state;
        }

        public void AppendToTextBuffer(char ch)
        {
            _textBuffer.Append(ch);
        }

        public void AppendToTagBuffer(char ch)
        {
            _tagBuffer.Append(ch);
        }

        public void AppendToAttributeNameBuffer(char ch)
        {
            _attributeNameBuffer.Append(ch);
        }

        public void AppendToAttributeValueBuffer(char ch)
        {
            _attributeValueBuffer.Append(ch);
        }

        public void SetQuoteChar(char ch)
        {
            _quoteChar = ch;
        }

        public void FlushTextBuffer()
        {
            if (_textBuffer.Length > 0)
            {
                string text = _textBuffer.ToString().Trim();
                if (!string.IsNullOrEmpty(text) && _elementStack.Count > 0)
                {
                    _elementStack.Peek().TextContent = text;
                }
                _textBuffer.Clear();
            }
        }

        public void CreateOpeningTag()
        {
            string tagName = _tagBuffer.ToString();
            var element = new HtmlElement(tagName);

            foreach (var attr in _pendingAttributes)
            {
                element.SetAttribute(attr.Key, attr.Value);
            }
            _pendingAttributes.Clear();

            if (_elementStack.Count == 0)
            {
                _rootElement = element;
            }
            else
            {
                _elementStack.Peek().AddChild(element);
            }

            _elementStack.Push(element);
            ClearBuffers();
        }

        public void CreateClosingTag()
        {
            string tagName = _tagBuffer.ToString();

            if (_elementStack.Count > 0)
            {
                var currentElement = _elementStack.Peek();
                if (currentElement.TagName.Equals(tagName, StringComparison.OrdinalIgnoreCase))
                {
                    _elementStack.Pop();
                }
            }

            ClearBuffers();
        }

        public void CreateSelfClosingTag()
        {
            string tagName = _tagBuffer.ToString();
            var element = new HtmlElement(tagName);

            foreach (var attr in _pendingAttributes)
            {
                element.SetAttribute(attr.Key, attr.Value);
            }
            _pendingAttributes.Clear();

            if (_elementStack.Count == 0)
            {
                _rootElement = element;
            }
            else
            {
                _elementStack.Peek().AddChild(element);
            }

            ClearBuffers();
        }

        public void AddCurrentAttribute()
        {
            if (_attributeNameBuffer.Length > 0)
            {
                string name = _attributeNameBuffer.ToString();
                string value = _attributeValueBuffer.ToString();
                _pendingAttributes[name] = value;
            }

            _attributeNameBuffer.Clear();
            _attributeValueBuffer.Clear();
            _quoteChar = '\0';
        }

        private Dictionary<string, string> _pendingAttributes = new Dictionary<string, string>();

        private void Reset()
        {
            _elementStack.Clear();
            _rootElement = null;
            _currentState = new TextState(this);
            _pendingAttributes.Clear();
            ClearBuffers();
        }

        private void ClearBuffers()
        {
            _tagBuffer.Clear();
            _attributeNameBuffer.Clear();
            _attributeValueBuffer.Clear();
            _quoteChar = '\0';
        }
    }
}