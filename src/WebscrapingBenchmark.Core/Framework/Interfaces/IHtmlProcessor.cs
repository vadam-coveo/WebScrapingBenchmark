
using Coveo.Connectors.Utilities.Web.WebScraping;

namespace WebscrapingBenchmark.Core.Framework.Interfaces
{
    public interface IHtmlProcessor
    {
        string GetHtmlBody();
        IEnumerable<string> Extract(Selector selector);
        bool Remove(Selector selector);
    }
}
