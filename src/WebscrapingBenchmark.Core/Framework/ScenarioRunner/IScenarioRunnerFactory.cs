using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebscrapingBenchmark.Core.Framework.ScenarioRunner
{
    public interface IScenarioRunnerFactory
    {
        IEnumerable<IScenarioRunner> CreateAll();
        void Release(IScenarioRunner scenarioRunner);
    }
}
