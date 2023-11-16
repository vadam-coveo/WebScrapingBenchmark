using System.Diagnostics;
using WebscrapingBenchmark.Core.Framework.Helpers;

var directory = Path.Join(FilesystemHelper.Solution.RootPath, "TestCaseData");
var filter = "*.json";

RunProcess(FilesystemHelper.Solution.BaselineExecutorPath, new[] { directory, filter });

RunProcess(FilesystemHelper.Solution.NewStrategiesExecutorPath, new[] { directory, filter });

int RunProcess(string path, string[] arguments)
{
    var start = new ProcessStartInfo
    {
        Arguments = string.Join(' ',arguments),
        FileName = path,
        WindowStyle = ProcessWindowStyle.Normal,
        CreateNoWindow = false,
        WorkingDirectory = new FileInfo(path).DirectoryName
    };

    using var process = Process.Start(start);
    process?.WaitForExit();
    return process?.ExitCode ?? -1;
}


var keyToExit = Console.ReadKey(true);