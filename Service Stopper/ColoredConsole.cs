namespace Service_Stopper
{
    public static class ColoredConsole
    {
        private static readonly object _lock = new();

        public static void WriteLine(string text, ConsoleColor color)
        {
            lock (_lock)
            {
                var old = Console.ForegroundColor;
                try
                {
                    Console.ForegroundColor = color;
                    Console.WriteLine(text);
                }
                finally
                {
                    Console.ForegroundColor = old;
                }
            }
        }
    }

}
