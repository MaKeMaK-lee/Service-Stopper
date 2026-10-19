namespace Service_Stopper
{
    public class Utilities
    {
        /// <summary>
        /// Читает строки с консоли, пока не будет введена пустая строка.
        /// </summary>
        public static List<string> ReadConsoleLinesUntilEmptyLine()
        {
            var lines = new List<string>();
            string? line;
            while (!string.IsNullOrEmpty(line = Console.ReadLine()))
                lines.Add(line);

            return lines;
        }
    }
}
