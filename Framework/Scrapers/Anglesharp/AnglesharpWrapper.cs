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

namespace WebScrapingBenchmark.Framework.Scrapers.Anglesharp
{
    public class AnglesharpWrapper
    {
        IHtmlDocument _angleSharpDocument;

        public AnglesharpWrapper()
        {
        }

        public void Parse(string html)
        {
            // todo : evaluate anglesharp parser options
            _angleSharpDocument = new HtmlParser().ParseDocument(html);
        }

        public string Extract(Selector selector)
        {
            if (selector.Type == SelectorType.CSS)
            {
                try
                {
                    var selectedDomSet = _angleSharpDocument.QuerySelectorAll(selector.Path);
                }
                catch (Exception ex)
                {
                    ConsoleLogger.Error($"Unable to parse {selector.Path}", ex);
                }
            }

            return null;
        }
    }
}
