namespace WebscrapingBenchmark.Core.Framework.Reporting
{
    public interface IScrapingResultsReporter
    {
        public int Index { get; }
        void ReportResults();
    }
}
