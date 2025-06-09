using System;

namespace CustomHtmlParser
{
    class Program
    {
        static void Main(string[] args)
        {
            string html = @"<html>
                <head>
                    <title>Test Page</title>
                </head>
                <body>
                    <h1 id='main-title' class='header'>Welcome</h1>
                    <p>This is a test paragraph.</p>
                    <img src='image.jpg' alt='Test Image' />
                    <div class='container'>
                        <p>Nested paragraph</p>
                        <a href='https://example.com'>Link</a>
                    </div>
                </body>
            </html>";

            Console.WriteLine("=== HTML Parser Demo ===\n");

            var parser = new HtmlParser();
            var root = parser.Parse(html);

            Console.WriteLine("1. Iterator Pattern Demo:");
            DemoIterator(root);

            Console.WriteLine("\n2. Command Pattern Demo:");
            DemoCommand(root);

            Console.WriteLine("\n3. Template Method Demo:");
            DemoTemplateMethod(root);

            Console.WriteLine("\n4. Visitor Pattern Demo:");
            DemoVisitor(root);

            Console.WriteLine("\n5. State Pattern Demo:");
            DemoState();
        }

        static void DemoIterator(HtmlElement root)
        {
            var document = new HtmlDocument(root);

            Console.WriteLine("Depth-First Traversal:");
            document.SetIteratorType(IteratorType.DepthFirst);
            var iterator = document.CreateIterator();
            while (iterator.HasNext())
            {
                var element = iterator.Next();
                Console.WriteLine($"  {element.TagName}");
            }

            Console.WriteLine("\nBreadth-First Traversal:");
            document.SetIteratorType(IteratorType.BreadthFirst);
            iterator = document.CreateIterator();
            while (iterator.HasNext())
            {
                var element = iterator.Next();
                Console.WriteLine($"  {element.TagName}");
            }
        }

        static void DemoCommand(HtmlElement root)
        {
            var invoker = new HtmlCommandInvoker();
            var newElement = new HtmlElement("span");
            newElement.TextContent = "New Element";

            var bodyElement = FindElementByTag(root, "body");
            if (bodyElement != null)
            {
                Console.WriteLine("Adding new element...");
                var addCommand = new AddElementCommand(bodyElement, newElement);
                invoker.ExecuteCommand(addCommand);
                Console.WriteLine($"Body children count: {bodyElement.Children.Count}");

                Console.WriteLine("Undoing...");
                invoker.Undo();
                Console.WriteLine($"Body children count: {bodyElement.Children.Count}");

                Console.WriteLine("Redoing...");
                invoker.Redo();
                Console.WriteLine($"Body children count: {bodyElement.Children.Count}");
            }
        }

        static void DemoTemplateMethod(HtmlElement root)
        {
            Console.WriteLine("Pretty HTML:");
            var prettyRenderer = new PrettyHtmlRenderer();
            var prettyHtml = prettyRenderer.Render(root);
            Console.WriteLine(prettyHtml);

            Console.WriteLine("\nMinified HTML:");
            var minifiedRenderer = new MinifiedHtmlRenderer();
            var minifiedHtml = minifiedRenderer.Render(root);
            Console.WriteLine(minifiedHtml);
        }

        static void DemoVisitor(HtmlElement root)
        {
            var validationVisitor = new HtmlValidationVisitor();
            root.Accept(validationVisitor);
            Console.WriteLine($"Validation errors: {validationVisitor.Errors.Count}");
            foreach (var error in validationVisitor.Errors)
            {
                Console.WriteLine($"  - {error}");
            }

            var statsVisitor = new HtmlStatisticsVisitor();
            root.Accept(statsVisitor);
            Console.WriteLine($"\nTotal elements: {statsVisitor.TotalElements}");
            Console.WriteLine($"Max depth: {statsVisitor.MaxDepth}");
            Console.WriteLine("Top tags:");
            foreach (var tag in statsVisitor.TagCounts)
            {
                Console.WriteLine($"  {tag.Key}: {tag.Value}");
            }
        }

        static void DemoState()
        {
            Console.WriteLine("Parsing simple HTML with State pattern:");
            var parser = new HtmlParser();
            var simpleHtml = "<div class='test'>Hello World</div>";
            var result = parser.Parse(simpleHtml);
            Console.WriteLine($"Parsed tag: {result.TagName}");
            Console.WriteLine($"Parsed class: {result.GetAttribute("class")}");
            Console.WriteLine($"Parsed text: {result.TextContent}");
        }

        static HtmlElement FindElementByTag(HtmlElement root, string tagName)
        {
            if (root.TagName.Equals(tagName, StringComparison.OrdinalIgnoreCase))
                return root;

            foreach (var child in root.Children)
            {
                var found = FindElementByTag(child, tagName);
                if (found != null)
                    return found;
            }

            return null;
        }
    }
}