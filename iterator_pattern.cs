using System.Collections;
using System.Collections.Generic;

namespace CustomHtmlParser
{
    public interface IHtmlIterator<T>
    {
        bool HasNext();
        T Next();
        void Reset();
    }

    public interface IHtmlIterable<T>
    {
        IHtmlIterator<T> CreateIterator();
    }

    public class DepthFirstIterator : IHtmlIterator<HtmlElement>
    {
        private Stack<HtmlElement> _stack;
        private HtmlElement _root;

        public DepthFirstIterator(HtmlElement root)
        {
            _root = root;
            Reset();
        }

        public bool HasNext()
        {
            return _stack.Count > 0;
        }

        public HtmlElement Next()
        {
            if (!HasNext()) return null;
            
            var current = _stack.Pop();
            
            for (int i = current.Children.Count - 1; i >= 0; i--)
            {
                _stack.Push(current.Children[i]);
            }
            
            return current;
        }

        public void Reset()
        {
            _stack = new Stack<HtmlElement>();
            if (_root != null)
                _stack.Push(_root);
        }
    }

    public class BreadthFirstIterator : IHtmlIterator<HtmlElement>
    {
        private Queue<HtmlElement> _queue;
        private HtmlElement _root;

        public BreadthFirstIterator(HtmlElement root)
        {
            _root = root;
            Reset();
        }

        public bool HasNext()
        {
            return _queue.Count > 0;
        }

        public HtmlElement Next()
        {
            if (!HasNext()) return null;
            
            var current = _queue.Dequeue();
            
            foreach (var child in current.Children)
            {
                _queue.Enqueue(child);
            }
            
            return current;
        }

        public void Reset()
        {
            _queue = new Queue<HtmlElement>();
            if (_root != null)
                _queue.Enqueue(_root);
        }
    }

    public class HtmlDocument : IHtmlIterable<HtmlElement>
    {
        public HtmlElement Root { get; set; }
        private IteratorType _iteratorType = IteratorType.DepthFirst;

        public HtmlDocument(HtmlElement root)
        {
            Root = root;
        }

        public void SetIteratorType(IteratorType type)
        {
            _iteratorType = type;
        }

        public IHtmlIterator<HtmlElement> CreateIterator()
        {
            switch (_iteratorType)
            {
                case IteratorType.BreadthFirst:
                    return new BreadthFirstIterator(Root);
                default:
                    return new DepthFirstIterator(Root);
            }
        }
    }

    public enum IteratorType
    {
        DepthFirst,
        BreadthFirst
    }
}