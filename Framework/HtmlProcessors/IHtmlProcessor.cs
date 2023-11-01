using WebScrapingBenchmark.Framework.Config;

namespace WebScrapingBenchmark.Framework.HtmlProcessors;

public interface IHtmlProcessor
{
    string GetHtmlBody();
    IEnumerable<string> Extract(Selector selector);
    bool Remove(Selector selector);
}