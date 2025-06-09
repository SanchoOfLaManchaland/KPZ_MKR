using System;
using System.Collections.Generic;
using System.Text;

namespace CustomHtmlParser
{
    public class HtmlElement
    {
        public string TagName { get; set; }
        public Dictionary<string, string> Attributes { get; set; }
        public List<HtmlElement> Children { get; set; }
        public string TextContent { get; set; }
        public HtmlElement Parent { get; set; }

        public HtmlElement(string tagName)
        {
            TagName = tagName;
            Attributes = new Dictionary<string, string>();
            Children = new List<HtmlElement>();
            TextContent = string.Empty;
        }

        public void AddChild(HtmlElement child)
        {
            child.Parent = this;
            Children.Add(child);
        }

        public void SetAttribute(string name, string value)
        {
            Attributes[name] = value;
        }

        public string GetAttribute(string name)
        {
            return Attributes.ContainsKey(name) ? Attributes[name] : null;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append($"<{TagName}");
            
            foreach (var attr in Attributes)
            {
                sb.Append($" {attr.Key}=\"{attr.Value}\"");
            }
            
            if (Children.Count == 0 && string.IsNullOrEmpty(TextContent))
            {
                sb.Append("/>");
            }
            else
            {
                sb.Append(">");
                sb.Append(TextContent);
                
                foreach (var child in Children)
                {
                    sb.Append(child.ToString());
                }
                
                sb.Append($"</{TagName}>");
            }
            
            return sb.ToString();
        }
    }
}