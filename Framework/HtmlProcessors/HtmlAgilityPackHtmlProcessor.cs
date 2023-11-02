using HtmlAgilityPack;
using System.Xml.XPath;
using HtmlAgilityPack.CssSelectors.NetCore;
using WebScrapingBenchmark.Framework.Config;
using WebScrapingBenchmark.Framework.Logging;
using WebScrapingBenchmark.Framework.Scrapers;

namespace WebScrapingBenchmark.Framework.HtmlProcessors
{
    public class HtmlAgilityPackHtmlProcessor : IHtmlProcessor
    {
        private HtmlDocument _htmlDocument;
        private bool _cssSupport;

        public HtmlAgilityPackHtmlProcessor(string html, bool cssSupport)
        {
            _cssSupport = cssSupport;
            _htmlDocument = new HtmlDocument();
            _htmlDocument.LoadHtml(html);
        }

        public string GetHtmlBody()
        {
            return _htmlDocument.DocumentNode.OuterHtml;
        }

        public IEnumerable<string> Extract(Selector selector)
        {
            if (selector.Type == SelectorType.XPATH)
                return ExtractWithXpath(selector);

            if(!_cssSupport)
                throw new NotImplementedException("no CSS support");

            return ExtractWithCss(selector);
        }

        public bool Remove(Selector selector)
        {
            if (selector.Type == SelectorType.XPATH)
                return RemoveWithXpath(selector);

            if (!_cssSupport)
                throw new NotImplementedException("no CSS support");

            return RemoveWithCss(selector);
        }


        private IEnumerable<string> ExtractWithXpath(Selector selector)
        {
            var evaluationResult = EvaluateExpression(selector.Path);

            switch (evaluationResult)
            {
                case null:
                    return Enumerable.Empty<string>();

                case XPathNodeIterator selectedNodeSet:
                {
                    var values = selectedNodeSet.Cast<HtmlNodeNavigator>().Select(GetValueOrElement);
                    return values.Distinct().ToArray();
                }
                default:
                    // While evaluating the provided scrapping expression, we received a single object (Or null) instead of an XPathNodeIterator,
                    // we should just return it directly.
                    return new List<string> { evaluationResult.ToString() };
            }
        }

        private bool RemoveWithXpath(Selector selector)
        {
            var removed = false;

            try
            {
                var selectedNodeSet = EvaluateExpression(selector.Path) as XPathNodeIterator;
                if (selectedNodeSet != null && selectedNodeSet.Count > 0)
                {
                    // For each nodes returned by the xpath expression, remove that node from the document.
                    foreach (var node in selectedNodeSet.Cast<HtmlNodeNavigator>().ToArray())
                    {
                        node.CurrentNode.Remove();
                        removed = true;
                    }
                }
            }
            catch (InvalidCastException exception)
            {
                ConsoleLogger.Error("Unable to retrieve a valid iterator to remove DOM elements.", exception);
            }

            return removed;
        }

        private bool RemoveWithCss(Selector selector)
        {
            bool removed = false;
            foreach (var node in _htmlDocument.QuerySelectorAll(selector.Path))
            {
                removed = true;
                node.Remove();
            }
            return removed;
        }

        private IEnumerable<string> ExtractWithCss(Selector selector)
        {
            var output = new List<string>();

            foreach (var node in _htmlDocument.QuerySelectorAll(selector.Path))
            {
                output.Add(ProcessorHelper.GetValueOrElement(node, ProcessorHelper.GetPathInfo(selector)));
            }

            return selector.RemoveDuplicates ? output.Distinct() : output;
        }

        private string GetValueOrElement(HtmlNodeNavigator htmlNode)
        {
            // If the selected node is text or attribute, return the value directly, else
            // we retrieve the entire element's html.
            string value;
            if (htmlNode.NodeType == XPathNodeType.Text || htmlNode.NodeType == XPathNodeType.Attribute)
                value = htmlNode.Value;
            else
                value = htmlNode.CurrentNode.OuterHtml;

            return ProcessorHelper.CleanHtml(value);
        }

        //todo : this one is virtual because the create navigator call could be optimized (if body didn't change, we could keep the same navigator).
        protected virtual object? EvaluateExpression(string expression)
        {
            var navigator = _htmlDocument.CreateNavigator();
            object evaluationResult = null;
            try
            {
                evaluationResult = navigator.Evaluate(expression);
            }
            catch (XPathException ex)
            {
                ConsoleLogger.Error($"Error while trying to extract value using the XPath selector: {expression}.", ex);
            }

            return evaluationResult;
        }


    }
}
