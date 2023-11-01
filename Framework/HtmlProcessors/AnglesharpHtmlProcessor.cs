using AngleSharp.Html.Dom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AngleSharp;
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
        IDocument _angleSharpDocument;
        private bool XpathSupport;

        public AnglesharpHtmlProcessor(string html, bool xpathSupport)
        {
            XpathSupport = xpathSupport;
            var config = Configuration.Default;
            if (xpathSupport)
            {
                config.WithXPath();
            }

            var context = BrowsingContext.New(config);
            _angleSharpDocument = context.OpenAsync(virtualResponse => virtualResponse.Content(html)).Result;


            // There are some signature changes compared to what we were doing
            //IHtmlDocument _angleSharpDocument = new HtmlParser().ParseDocument(html);
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
            EnsureXpathIfNeeded(selector);
            return DoExtract(selector);
        }

        public bool Remove(Selector selector)
        {
            EnsureXpathIfNeeded(selector);
            return DoRemove(selector);
        }

        private void EnsureXpathIfNeeded(Selector selector)
        {
            if (selector.Type == SelectorType.XPATH && !XpathSupport)
                throw new ArgumentException("Please add xpathsupport!");
        }

        private string GetFormattedPath(Selector selector)
        {
            if (selector.Type == SelectorType.CSS)
            {
                return selector.Path;
            }

            return $"*[xpath>'{selector.Path}']";
        }

        private IPathInfo GetPathInfo(Selector selector)
        {
            if (selector.Type == SelectorType.CSS)
            {
                return CssPath.Create(selector.Path);
            }
            return XpathInfo.Create(selector.Path);
        }

        private IEnumerable<string> DoExtract(Selector selector)
        {
            try
            {
                var pathInfo = GetPathInfo(selector);
                object extractedValue = null;

                var selectedDomSet = _angleSharpDocument.QuerySelectorAll(GetFormattedPath(selector));

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

        private bool DoRemove(Selector selector)
        {
            var removed = false;
            var selectedDomSet = _angleSharpDocument.QuerySelectorAll(GetFormattedPath(selector));
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
