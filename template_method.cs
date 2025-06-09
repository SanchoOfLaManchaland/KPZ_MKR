using System.Text;

namespace CustomHtmlParser
{
    public abstract class HtmlRenderer
    {
        public string Render(HtmlElement element)
        {
            var sb = new StringBuilder();
            RenderDocument(sb);
            RenderElement(element, sb, 0);
            RenderFooter(sb);
            return sb.ToString();
        }

        protected abstract void RenderDocument(StringBuilder sb);
        protected abstract void RenderFooter(StringBuilder sb);
        protected abstract string GetIndentation(int level);
        protected abstract void RenderOpeningTag(HtmlElement element, StringBuilder sb, int level);
        protected abstract void RenderClosingTag(HtmlElement element, StringBuilder sb, int level);
        protected abstract void RenderTextContent(string text, StringBuilder sb, int level);
        protected abstract void RenderAttributes(HtmlElement element, StringBuilder sb);

        private void RenderElement(HtmlElement element, StringBuilder sb, int level)
        {
            if (element == null) return;

            RenderOpeningTag(element, sb, level);

            if (!string.IsNullOrEmpty(element.TextContent))
            {
                RenderTextContent(element.TextContent, sb, level + 1);
            }

            foreach (var child in element.Children)
            {
                RenderElement(child, sb, level + 1);
            }

            if (element.Children.Count > 0 || !string.IsNullOrEmpty(element.TextContent))
            {
                RenderClosingTag(element, sb, level);
            }
        }
    }

    public class PrettyHtmlRenderer : HtmlRenderer
    {
        protected override void RenderDocument(StringBuilder sb)
        {
            sb.AppendLine("<!DOCTYPE html>");
        }

        protected override void RenderFooter(StringBuilder sb)
        {
            sb.AppendLine();
        }

        protected override string GetIndentation(int level)
        {
            return new string(' ', level * 2);
        }

        protected override void RenderOpeningTag(HtmlElement element, StringBuilder sb, int level)
        {
            sb.Append(GetIndentation(level));
            sb.Append($"<{element.TagName}");
            RenderAttributes(element, sb);
            
            if (element.Children.Count == 0 && string.IsNullOrEmpty(element.TextContent))
            {
                sb.AppendLine("/>");
            }
            else
            {
                sb.AppendLine(">");
            }
        }

        protected override void RenderClosingTag(HtmlElement element, StringBuilder sb, int level)
        {
            sb.Append(GetIndentation(level));
            sb.AppendLine($"</{element.TagName}>");
        }

        protected override void RenderTextContent(string text, StringBuilder sb, int level)
        {
            sb.Append(GetIndentation(level));
            sb.AppendLine(text);
        }

        protected override void RenderAttributes(HtmlElement element, StringBuilder sb)
        {
            foreach (var attr in element.Attributes)
            {
                sb.Append($" {attr.Key}=\"{attr.Value}\"");
            }
        }
    }

    public class MinifiedHtmlRenderer : HtmlRenderer
    {
        protected override void RenderDocument(StringBuilder sb)
        {
            sb.Append("<!DOCTYPE html>");
        }

        protected override void RenderFooter(StringBuilder sb)
        {
        }

        protected override string GetIndentation(int level)
        {
            return string.Empty;
        }

        protected override void RenderOpeningTag(HtmlElement element, StringBuilder sb, int level)
        {
            sb.Append($"<{element.TagName}");
            RenderAttributes(element, sb);
            
            if (element.Children.Count == 0 && string.IsNullOrEmpty(element.TextContent))
            {
                sb.Append("/>");
            }
            else
            {
                sb.Append(">");
            }
        }

        protected override void RenderClosingTag(HtmlElement element, StringBuilder sb, int level)
        {
            sb.Append($"</{element.TagName}>");
        }

        protected override void RenderTextContent(string text, StringBuilder sb, int level)
        {
            sb.Append(text.Trim());
        }

        protected override void RenderAttributes(HtmlElement element, StringBuilder sb)
        {
            foreach (var attr in element.Attributes)
            {
                sb.Append($" {attr.Key}=\"{attr.Value}\"");
            }
        }
    }

    public class XmlRenderer : HtmlRenderer
    {
        protected override void RenderDocument(StringBuilder sb)
        {
            sb.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
        }

        protected override void RenderFooter(StringBuilder sb)
        {
            sb.AppendLine();
        }

        protected override string GetIndentation(int level)
        {
            return new string('\t', level);
        }

        protected override void RenderOpeningTag(HtmlElement element, StringBuilder sb, int level)
        {
            sb.Append(GetIndentation(level));
            sb.Append($"<{element.TagName}");
            RenderAttributes(element, sb);
            
            if (element.Children.Count == 0 && string.IsNullOrEmpty(element.TextContent))
            {
                sb.AppendLine(" />");
            }
            else
            {
                sb.AppendLine(">");
            }
        }

        protected override void RenderClosingTag(HtmlElement element, StringBuilder sb, int level)
        {
            sb.Append(GetIndentation(level));
            sb.AppendLine($"</{element.TagName}>");
        }

        protected override void RenderTextContent(string text, StringBuilder sb, int level)
        {
            sb.Append(GetIndentation(level));
            sb.AppendLine(text);
        }

        protected override void RenderAttributes(HtmlElement element, StringBuilder sb)
        {
            foreach (var attr in element.Attributes)
            {
                sb.Append($" {attr.Key}=\"{attr.Value}\"");
            }
        }
    }
}