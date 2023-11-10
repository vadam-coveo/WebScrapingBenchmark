using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebscrapingBenchmark.Core.Framework.Reporting
{
    public class ScrapingResultReportingAggregator : IScrapingResultReportingAggregator
    {
        private IReporterFactory ReporterFactory { get; }

        public ScrapingResultReportingAggregator(IReporterFactory reporterFactory)
        {
            ReporterFactory = reporterFactory;
        }

        public void Run()
        {
            var scrapingResultsReporter = ReporterFactory.CreateAll().OrderBy(x => x.Index).ToList();
            foreach (var reporter in scrapingResultsReporter)
            {
                try
                {
                    reporter.ReportResults();
                }
                finally
                {
                    ReporterFactory.Release(reporter);
                }
            }
        }
    }
}
