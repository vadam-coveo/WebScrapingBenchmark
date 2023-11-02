namespace WebScrapingBenchmark.Framework.HtmlProcessors
{
    public interface IHtmlProcessorFactory
    {
        IHtmlProcessor GetAngleSharpHtmlProcessor(string html, bool xpathSupport);

        IHtmlProcessor GetHtmlAgilityPackHtmlProcessor(string html, bool cssSupport);

        IHtmlProcessor GetRevampedHtmlAgilityHtmlProcessor(string html);

        void Release(IHtmlProcessor processor);
    }
}
