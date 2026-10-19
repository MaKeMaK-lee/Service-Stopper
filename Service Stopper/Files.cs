namespace Service_Stopper
{
    internal class Files
    {
        private static void EnsureFileExists(string filePath)
        {
            string? dir = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(dir))
                Directory.CreateDirectory(dir);

            using (new FileStream(filePath, FileMode.OpenOrCreate))
            { }
        }

        public static List<string> ReadLinesFromFile(string filePath)
        {
            EnsureFileExists(filePath);
            return new List<string>(File.ReadAllLines(filePath));
        }
    }
}
