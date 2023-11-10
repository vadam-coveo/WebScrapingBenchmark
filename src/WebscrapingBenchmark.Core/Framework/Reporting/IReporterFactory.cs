using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebscrapingBenchmark.Core.Framework.Reporting
{
    public interface IReporterFactory
    {
        IEnumerable<IScrapingResultsReporter> CreateAll();

        void Release(IScrapingResultsReporter reporter);
    }
}
