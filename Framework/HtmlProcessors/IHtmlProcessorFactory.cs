using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebScrapingBenchmark.Framework.HtmlProcessors
{
    public interface IHtmlProcessorFactory
    {
        IHtmlProcessor CreateAnglesharpHtmlProcessor(string html);

        IHtmlProcessor CreateHtmlAgilityPackHtmlProcessor(string html);

        void Release(IHtmlProcessor processor);
    }
}
