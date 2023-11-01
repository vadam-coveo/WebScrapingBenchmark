namespace WebScrapingBenchmark.Framework.HtmlProcessors
{
    public interface IHtmlProcessorFactory
    {
        IHtmlProcessor GetAngleSharpHtmlProcessor(string html, bool xpathSupport);

        IHtmlProcessor GetHtmlAgilityPackHtmlProcessor(string html);

        void Release(IHtmlProcessor processor);
    }
}
