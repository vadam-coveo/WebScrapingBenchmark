namespace WebScrapingBenchmark.Framework.Logging
{
        public static class ConsoleLogger 
        {
            private static object _lock = new object();

            public static void Debug(string message)
            {
                WriteLine(message, ConsoleColor.Gray);
            }

            public static void Info(string message)
            {
                WriteLine(message, ConsoleColor.White);
            }

            public static void Warn(string message)
            {
                WriteLine(message, ConsoleColor.Yellow);
            }

            public static void Error(string message, Exception? exception = null)
            {
                WriteLine(message, ConsoleColor.Red);

                if (exception == null) return;

                WriteLine(exception.Message, ConsoleColor.DarkRed);
                if (exception.StackTrace != null)
                    WriteLine(exception.StackTrace, ConsoleColor.DarkRed);
            }

            public static void WriteLine(string message, ConsoleColor color = ConsoleColor.White, int tabIndentation = 0)
            {
                lock (_lock)
                {
                    var indentation = tabIndentation > 1 ? string.Concat(Enumerable.Repeat("   ", tabIndentation)) : "";
                    Console.ForegroundColor = color;

                    Console.WriteLine($"{indentation}{message}");

                    Console.ForegroundColor = ConsoleColor.White;
                }

            }

            
        }
}
