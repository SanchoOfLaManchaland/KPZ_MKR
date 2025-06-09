using System.Collections.Generic;

namespace CustomHtmlParser
{
    public interface IHtmlCommand
    {
        void Execute();
        void Undo();
    }

    public class AddElementCommand : IHtmlCommand
    {
        private HtmlElement _parent;
        private HtmlElement _child;

        public AddElementCommand(HtmlElement parent, HtmlElement child)
        {
            _parent = parent;
            _child = child;
        }

        public void Execute()
        {
            _parent.AddChild(_child);
        }

        public void Undo()
        {
            _parent.Children.Remove(_child);
            _child.Parent = null;
        }
    }

    public class RemoveElementCommand : IHtmlCommand
    {
        private HtmlElement _parent;
        private HtmlElement _child;
        private int _originalIndex;

        public RemoveElementCommand(HtmlElement parent, HtmlElement child)
        {
            _parent = parent;
            _child = child;
            _originalIndex = parent.Children.IndexOf(child);
        }

        public void Execute()
        {
            _parent.Children.Remove(_child);
            _child.Parent = null;
        }

        public void Undo()
        {
            _parent.Children.Insert(_originalIndex, _child);
            _child.Parent = _parent;
        }
    }

    public class SetAttributeCommand : IHtmlCommand
    {
        private HtmlElement _element;
        private string _attributeName;
        private string _newValue;
        private string _oldValue;

        public SetAttributeCommand(HtmlElement element, string attributeName, string newValue)
        {
            _element = element;
            _attributeName = attributeName;
            _newValue = newValue;
            _oldValue = element.GetAttribute(attributeName);
        }

        public void Execute()
        {
            _element.SetAttribute(_attributeName, _newValue);
        }

        public void Undo()
        {
            if (_oldValue == null)
            {
                _element.Attributes.Remove(_attributeName);
            }
            else
            {
                _element.SetAttribute(_attributeName, _oldValue);
            }
        }
    }

    public class SetTextContentCommand : IHtmlCommand
    {
        private HtmlElement _element;
        private string _newContent;
        private string _oldContent;

        public SetTextContentCommand(HtmlElement element, string newContent)
        {
            _element = element;
            _newContent = newContent;
            _oldContent = element.TextContent;
        }

        public void Execute()
        {
            _element.TextContent = _newContent;
        }

        public void Undo()
        {
            _element.TextContent = _oldContent;
        }
    }

    public class HtmlCommandInvoker
    {
        private Stack<IHtmlCommand> _undoStack;
        private Stack<IHtmlCommand> _redoStack;

        public HtmlCommandInvoker()
        {
            _undoStack = new Stack<IHtmlCommand>();
            _redoStack = new Stack<IHtmlCommand>();
        }

        public void ExecuteCommand(IHtmlCommand command)
        {
            command.Execute();
            _undoStack.Push(command);
            _redoStack.Clear();
        }

        public void Undo()
        {
            if (_undoStack.Count > 0)
            {
                var command = _undoStack.Pop();
                command.Undo();
                _redoStack.Push(command);
            }
        }

        public void Redo()
        {
            if (_redoStack.Count > 0)
            {
                var command = _redoStack.Pop();
                command.Execute();
                _undoStack.Push(command);
            }
        }

        public bool CanUndo => _undoStack.Count > 0;
        public bool CanRedo => _redoStack.Count > 0;
    }
}