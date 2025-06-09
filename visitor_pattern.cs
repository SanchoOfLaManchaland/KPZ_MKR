using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CustomHtmlParser
{
    public interface IHtmlVisitor
    {
        void Visit(HtmlElement element);
    }

    public class HtmlValidationVisitor : IHtmlVisitor
    {
        public List<string> Errors { get; private set; }
        private readonly HashSet<string> _selfClosingTags = new HashSet<string>
        {
            "area", "base", "br", "col", "embed", "hr", "img", "input", "link", "meta", "param", "source", "track", "wbr"
        };

        public HtmlValidationVisitor()
        {
            Errors = new List<string>();
        }

        public void Visit(HtmlElement element)
        {
            ValidateElement(element);
            
            foreach (var child in element.Children)
            {
                Visit(child);
            }
        }

        private void ValidateElement(HtmlElement element)
        {
            if (string.IsNullOrEmpty(element.TagName))
            {
                Errors.Add("Element has empty tag name");
                return;
            }

            if (_selfClosingTags.Contains(element.TagName.ToLower()) && element.Children.Count > 0)
            {
                Errors.Add($"Self-closing tag '{element.TagName}' cannot have children");
            }

            ValidateAttributes(element);
            ValidateSpecificTags(element);
        }

        private void ValidateAttributes(HtmlElement element)
        {
            foreach (var attr in element.Attributes)
            {
                if (string.IsNullOrEmpty(attr.Key))
                {
                    Errors.Add($"Element '{element.TagName}' has attribute with empty name");
                }
            }
        }

        private void ValidateSpecificTags(HtmlElement element)
        {
            switch (element.TagName.ToLower())
            {
                case "img":
                    if (!element.Attributes.ContainsKey("src"))
                    {
                        Errors.Add("img element must have 'src' attribute");
                    }
                    if (!element.Attributes.ContainsKey("alt"))
                    {
                        Errors.Add("img element should have 'alt' attribute for accessibility");
                    }
                    break;
                case "a":
                    if (!element.Attributes.ContainsKey("href"))
                    {
                        Errors.Add("a element should have 'href' attribute");
                    }
                    break;
            }
        }
    }

    public class HtmlStatisticsVisitor : IHtmlVisitor
    {
        public Dictionary<string, int> TagCounts { get; private set; }
        public int TotalElements { get; private set; }
        public int TotalTextLength { get; private set; }
        public int MaxDepth { get; private set; }
        private int _currentDepth;

        public HtmlStatisticsVisitor()
        {
            TagCounts = new Dictionary<string, int>();
            TotalElements = 0;
            TotalTextLength = 0;
            MaxDepth = 0;
            _currentDepth = 0;
        }

        public void Visit(HtmlElement element)
        {
            _currentDepth++;
            MaxDepth = Math.Max(MaxDepth, _currentDepth);
            
            TotalElements++;
            
            if (TagCounts.ContainsKey(element.TagName))
            {
                TagCounts[element.TagName]++;
            }
            else
            {
                TagCounts[element.TagName] = 1;
            }

            if (!string.IsNullOrEmpty(element.TextContent))
            {
                TotalTextLength += element.TextContent.Length;
            }

            foreach (var child in element.Children)
            {
                Visit(child);
            }
            
            _currentDepth--;
        }

        public string GetReport()
        {
            var sb = new StringBuilder();
            sb.AppendLine("HTML Statistics Report:");
            sb.AppendLine($"Total Elements: {TotalElements}");
            sb.AppendLine($"Total Text Length: {TotalTextLength}");
            sb.AppendLine($"Maximum Depth: {MaxDepth}");
            sb.AppendLine("Tag Counts:");
            
            foreach (var tag in TagCounts.OrderByDescending(x => x.Value))
            {
                sb.AppendLine($"  {tag.Key}: {tag.Value}");
            }
            
            return sb.ToString();
        }
    }

    public class HtmlSearchVisitor : IHtmlVisitor
    {
        private readonly string _searchTag;
        private readonly string _searchAttribute;
        private readonly string _searchValue;
        public List<HtmlElement> FoundElements { get; private set; }

        public HtmlSearchVisitor(string searchTag = null, string searchAttribute = null, string searchValue = null)
        {
            _searchTag = searchTag?.ToLower();
            _searchAttribute = searchAttribute?.ToLower();
            _searchValue = searchValue;
            FoundElements = new List<HtmlElement>();
        }

        public void Visit(HtmlElement element)
        {
            bool matches = true;

            if (_searchTag != null && element.TagName.ToLower() != _searchTag)
            {
                matches = false;
            }

            if (_searchAttribute != null && _searchValue != null)
            {
                var attrValue = element.GetAttribute(_searchAttribute);
                if (attrValue != _searchValue)
                {
                    matches = false;
                }
            }
            else if (_searchAttribute != null)
            {
                if (!element.Attributes.ContainsKey(_searchAttribute))
                {
                    matches = false;
                }
            }

            if (matches)
            {
                FoundElements.Add(element);
            }

            foreach (var child in element.Children)
            {
                Visit(child);
            }
        }
    }

    public class HtmlTransformVisitor : IHtmlVisitor
    {
        private readonly Func<HtmlElement, bool> _condition;
        private readonly Action<HtmlElement> _transformation;

        public HtmlTransformVisitor(Func<HtmlElement, bool> condition, Action<HtmlElement> transformation)
        {
            _condition = condition;
            _transformation = transformation;
        }

        public void Visit(HtmlElement element)
        {
            if (_condition(element))
            {
                _transformation(element);
            }

            foreach (var child in element.Children)
            {
                Visit(child);
            }
        }
    }

    public static class HtmlElementExtensions
    {
        public static void Accept(this HtmlElement element, IHtmlVisitor visitor)
        {
            visitor.Visit(element);
        }
    }
}