namespace WebScrapingBenchmark.Framework.Reporting
{
    public class ConsoleReportingEntry
    {
        public string Metric { get; set; }
        public string Average { get; set; }
        public string Fastest { get; set; }
        public string FastestName { get; set; }
        public string Slowest { get; set; }
        public string SlowestName { get; set; }

        public ConsoleReportingEntry(string metric, string average, string fastest, string fastestName, string slowest, string slowestName)
        {
            Metric = metric;
            Average = average;
            Fastest = fastest;
            FastestName = fastestName;
            Slowest = slowest;
            SlowestName = slowestName;
        }
    }
}
