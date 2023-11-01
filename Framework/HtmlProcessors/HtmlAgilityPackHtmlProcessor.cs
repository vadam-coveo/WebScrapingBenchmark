using AngleSharp.Html.Dom;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.XPath;
using WebScrapingBenchmark.Framework.Config;
using WebScrapingBenchmark.Framework.Logging;
using WebScrapingBenchmark.Framework.Scrapers;

namespace WebScrapingBenchmark.Framework.HtmlProcessors
{
    public class HtmlAgilityPackHtmlProcessor : IHtmlProcessor
    {
        private HtmlDocument _htmlDocument;

        public HtmlAgilityPackHtmlProcessor(string html)
        {
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
            {
                return ExtractWithXpath(selector);
            }
            throw new NotImplementedException("ADD CSS");
        }

        public bool Remove(Selector selector)
        {
            if (selector.Type == SelectorType.XPATH)
            {
                return RemoveWithXpath(selector);
            }
            throw new NotImplementedException("ADD CSS");
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
