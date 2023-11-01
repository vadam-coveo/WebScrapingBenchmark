namespace WebScrapingBenchmark.Framework.HtmlProcessors
{
    public interface IHtmlProcessorFactory
    {
        IHtmlProcessor GetAngleSharpHtmlProcessor(string html);

        IHtmlProcessor GetHtmlAgilityPackHtmlProcessor(string html);

        void Release(IHtmlProcessor processor);
    }
}
