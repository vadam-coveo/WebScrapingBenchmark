using Coveo.Connectors.Utilities.Web.WebScraping;
using HtmlAgilityPack;
using HtmlAgilityPack.CssSelectors.NetCore;
using WebscrapingBenchmark.Core.Framework.Interfaces;

namespace WebscrapingBenchmark.NewStrategiesExecutor.HtmlProcessors
{
    // this version should be more efficient than the old code we have
    internal class RevampedHtmlAgilityHtmlProcessor : IHtmlProcessor
    {
        private HtmlDocument _htmlDocument;

        public RevampedHtmlAgilityHtmlProcessor(string html)
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
            var output = new List<string>();

            foreach (var node in Select(selector))
            {
                output.Add(ProcessorHelper.GetValueOrElement(node, ProcessorHelper.GetPathInfo(selector)));
            }

            return selector.RemoveDuplicates ? output.Distinct() : output;
        }

        public bool Remove(Selector selector)
        {
            bool removed = false;
            var nodes = Select(selector);
            if (nodes == null)
            {
                return false;
            }

            foreach (var node in Select(selector))
            {
                removed = true;
                node.Remove();
            }
            return removed;
        }

        private IEnumerable<HtmlNode> Select(Selector selector)
        {
            if (selector.Type == SelectorType.CSS)
            {
                return _htmlDocument.QuerySelectorAll(selector.Path);
            }

            return _htmlDocument.DocumentNode.SelectNodes(selector.Path);
        }
    }
}
