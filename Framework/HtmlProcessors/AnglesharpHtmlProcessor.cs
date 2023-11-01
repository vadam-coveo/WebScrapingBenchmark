using AngleSharp.Html.Dom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AngleSharp.Html.Parser;
using WebScrapingBenchmark.Framework.Config;
using AngleSharp.Dom;
using WebScrapingBenchmark.Framework.Logging;
using static WebScrapingBenchmark.Framework.Scrapers.ProcessorHelper;
using ConsoleLogger = WebScrapingBenchmark.Framework.Logging.ConsoleLogger;

namespace WebScrapingBenchmark.Framework.HtmlProcessors
{
    public class AnglesharpHtmlProcessor : IDisposable, IHtmlProcessor
    {
        IHtmlDocument _angleSharpDocument;

        public AnglesharpHtmlProcessor(string html)
        {
            // todo : evaluate anglesharp parser options
            _angleSharpDocument = new HtmlParser().ParseDocument(html);
        }

        public string GetHtmlBody()
        {
            var firstElement = _angleSharpDocument.FirstElementChild;

            if (firstElement == null)
                return "";

            return firstElement.OuterHtml;
        }

        public IEnumerable<string> Extract(Selector selector)
        {
            if (selector.Type == SelectorType.CSS)
            {
                return ExtractWithCss(selector);
            }

            throw new NotImplementedException("ADD XPATH");
        }

        public bool Remove(Selector selector)
        {
            if (selector.Type == SelectorType.CSS)
            {
                return RemoveWithCss(selector);
            }

            throw new NotImplementedException("ADD XPATH");
        }

        private IEnumerable<string> ExtractWithCss(Selector selector)
        {
            try
            {
                var pathInfo = CssPath.Create(selector.Path);
                object extractedValue = null;

                var selectedDomSet = _angleSharpDocument.QuerySelectorAll(pathInfo.Path);

                if (selectedDomSet != null && selectedDomSet.Any())
                {
                    var values = selectedDomSet.Select(node => GetValueOrElement(node, pathInfo)).Where(x => x != null);

                    if (selector.RemoveDuplicates)
                        values = values.Distinct();

                    return values;
                }
                else
                {
                    ConsoleLogger.Debug($"No element found matching the CSS selector: \"{selector}\".");
                }
            }
            catch (Exception ex)
            {
                ConsoleLogger.Error($"Unable to parse {selector.Path}", ex);
            }

            return Enumerable.Empty<string>();
        }

        private bool RemoveWithCss(Selector selector)
        {
            var pathInfo = CssPath.Create(selector.Path);

            var removed = false;
            var selectedDomSet = _angleSharpDocument.QuerySelectorAll(pathInfo.Path);
            if (selectedDomSet != null)
            {
                // For each dom element returned by the CSS expression, remove it from the document.
                foreach (var domObject in selectedDomSet)
                {
                    domObject.Remove();
                    removed = true;
                }
            }

            return removed;
        }

        public void Dispose()
        {
            _angleSharpDocument.Dispose();
        }
    }
}
