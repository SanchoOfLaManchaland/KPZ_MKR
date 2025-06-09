using System.Text;

namespace CustomHtmlParser
{
    public abstract class HtmlParserState
    {
        protected HtmlParser _parser;

        public HtmlParserState(HtmlParser parser)
        {
            _parser = parser;
        }

        public abstract void ParseCharacter(char ch);
    }

    public class TextState : HtmlParserState
    {
        public TextState(HtmlParser parser) : base(parser) { }

        public override void ParseCharacter(char ch)
        {
            if (ch == '<')
            {
                _parser.FlushTextBuffer();
                _parser.SetState(new TagOpenState(_parser));
            }
            else
            {
                _parser.AppendToTextBuffer(ch);
            }
        }
    }

    public class TagOpenState : HtmlParserState
    {
        public TagOpenState(HtmlParser parser) : base(parser) { }

        public override void ParseCharacter(char ch)
        {
            if (ch == '/')
            {
                _parser.SetState(new ClosingTagState(_parser));
            }
            else if (char.IsLetter(ch))
            {
                _parser.AppendToTagBuffer(ch);
                _parser.SetState(new TagNameState(_parser));
            }
        }
    }

    public class TagNameState : HtmlParserState
    {
        public TagNameState(HtmlParser parser) : base(parser) { }

        public override void ParseCharacter(char ch)
        {
            if (char.IsWhiteSpace(ch))
            {
                _parser.SetState(new AttributeNameState(_parser));
            }
            else if (ch == '>')
            {
                _parser.CreateOpeningTag();
                _parser.SetState(new TextState(_parser));
            }
            else if (ch == '/')
            {
                _parser.SetState(new SelfClosingTagState(_parser));
            }
            else
            {
                _parser.AppendToTagBuffer(ch);
            }
        }
    }

    public class AttributeNameState : HtmlParserState
    {
        public AttributeNameState(HtmlParser parser) : base(parser) { }

        public override void ParseCharacter(char ch)
        {
            if (ch == '=')
            {
                _parser.SetState(new AttributeValueStartState(_parser));
            }
            else if (ch == '>')
            {
                _parser.CreateOpeningTag();
                _parser.SetState(new TextState(_parser));
            }
            else if (!char.IsWhiteSpace(ch))
            {
                _parser.AppendToAttributeNameBuffer(ch);
            }
        }
    }

    public class AttributeValueStartState : HtmlParserState
    {
        public AttributeValueStartState(HtmlParser parser) : base(parser) { }

        public override void ParseCharacter(char ch)
        {
            if (ch == '"' || ch == '\'')
            {
                _parser.SetQuoteChar(ch);
                _parser.SetState(new AttributeValueState(_parser));
            }
            else if (!char.IsWhiteSpace(ch))
            {
                _parser.AppendToAttributeValueBuffer(ch);
                _parser.SetState(new AttributeValueState(_parser));
            }
        }
    }

    public class AttributeValueState : HtmlParserState
    {
        public AttributeValueState(HtmlParser parser) : base(parser) { }

        public override void ParseCharacter(char ch)
        {
            if ((_parser.QuoteChar != '\0' && ch == _parser.QuoteChar) || 
                (_parser.QuoteChar == '\0' && char.IsWhiteSpace(ch)))
            {
                _parser.AddCurrentAttribute();
                _parser.SetState(new AttributeNameState(_parser));
            }
            else if (_parser.QuoteChar == '\0' && ch == '>')
            {
                _parser.AddCurrentAttribute();
                _parser.CreateOpeningTag();
                _parser.SetState(new TextState(_parser));
            }
            else
            {
                _parser.AppendToAttributeValueBuffer(ch);
            }
        }
    }

    public class SelfClosingTagState : HtmlParserState
    {
        public SelfClosingTagState(HtmlParser parser) : base(parser) { }

        public override void ParseCharacter(char ch)
        {
            if (ch == '>')
            {
                _parser.CreateSelfClosingTag();
                _parser.SetState(new TextState(_parser));
            }
        }
    }

    public class ClosingTagState : HtmlParserState
    {
        public ClosingTagState(HtmlParser parser) : base(parser) { }

        public override void ParseCharacter(char ch)
        {
            if (ch == '>')
            {
                _parser.CreateClosingTag();
                _parser.SetState(new TextState(_parser));
            }
            else if (!char.IsWhiteSpace(ch))
            {
                _parser.AppendToTagBuffer(ch);
            }
        }
    }
}