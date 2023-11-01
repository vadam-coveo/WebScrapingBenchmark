namespace WebScrapingBenchmark.Framework.HtmlProcessors
{
    public interface IHtmlProcessorFactory
    {
        IHtmlProcessor AngleSharpHtmlProcessor(string html);

        IHtmlProcessor GetHtmlAgilityPackHtmlProcessor(string html);

        void Release(IHtmlProcessor processor);
    }
}
