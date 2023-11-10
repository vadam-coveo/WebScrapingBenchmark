# WebScrapingBenchmark

Program for running benchmarks on diferent web-scraping strategies using the same context (url & web-scraping configuration). 

The fundamental principle of benchmarking requires isolating the logic we want to benchmark from external factors. 
Therefore, we have implented a cache for the requests (the cache is being warmed-up at the beginning of the process) and we are adding-up the real request time to the results
in order to speed up the process, and still have consistant results (see ScenarioRunner).


# Project Structure
WebScrapingBenchmark.Main -> this is the main program entry point, it will invoke the 2 other programs
WebScrapingBenchmark.BaselineExecutor -> this program references the connector's repository and runs the exact scraper we have, in isolation with the associated nuget packages and versions
WebScrapingBenchmark.NewStrategyExecutor -> this program is being invoked after the baseline, because it will interpret results from the baseline

This set of programs is meant to be executed along the source-code (never move the bin folders around)

The program creates the WorkFolder and it's sub-directories on first run (excluded via .gitignore)
Here is the structure and what to expect : 
/WorkFolder/Csv -> will contain csv outputs with all the metrics
/WorkFolder/Requests -> will be populated with request cache storage (keep them so that you don't call the real servers each time)
/WorkFolder/Metrics -> will contain jsons of all the metrics involved, this is how the NewStrategyExecutor picks-up metrics from the BaselineExecutor initial run
/WorkFolder/Scenarios -> this is where you should define your scenarios! 

# Project Setup
1 - You will need to copy the Chromium directory from the connector's repo at the root of this project in directory /Chromium
2 - This project won't run if you don't provide scenarios in /WorkFolder/Scenarios
2 - This project needs to reference the connector's project, in particular the Coveo.Connectors.Utilities.Web 

## Details : 
Both repos are expeceted to be in the same parent directory, ie : C:/projects/connectors & C:/projects/WebScrapingBenchmark
Of course, the Coveo.Connectors.Utilities.Web needs to be built prior to working with the Benchmark tool
The WebscrapingBenchmark.Core references a signgle dll (from the connector's build output) ->  Coveo.Connectors.Utilities.Web.dll because we needed some models, that's it! 


## Troubleshooting
If you set a breakpoint in WebScrapingBenchmark.BaselineExecutor or WebScrapingBenchmark.NewStrategyExecutor, it won't hit! 
That's because we are invoking a new processes from Main (because isolation yadayada)... You can change the solution's startup project to the one you wish to debug and run just that one. 
Since results are stored in the filesystem (and overwritten during each run) there won't be any issue doing this. 