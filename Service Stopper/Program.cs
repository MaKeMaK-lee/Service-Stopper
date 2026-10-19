using WindowsServices;

namespace Service_Stopper
{
    class Program
    {
        static async Task Main(string[] args)
        {
            IEnumerable<string> names;

            if (args.Length > 0)
                names = args;
            else
                names = ConfigureNames();

            await StopAll(names);

            Console.WriteLine($"Нажмите любую клавишу для выхода.");
            Console.ReadKey();
        }

        static IEnumerable<string> ConfigureNames()
        {
            IEnumerable<string> names;
            string filename = "ServiceList.txt";

            names = Files.ReadLinesFromFile(filename);

            if (!names.Any())
            {
                Console.WriteLine($"Список целевых служб пуст.");
                Console.WriteLine();
                Console.WriteLine($"Вы можете ввести их вручную.");
                Console.WriteLine($"Ожидается ввод имён целевых служб или пустой строки для подтверждения.");
                names = Utilities.ReadConsoleLinesUntilEmptyLine();
            }

            return names;
        }

        static async Task StopAll(IEnumerable<string> names)
        {
            await Task.WhenAll(names.Select(StopServiceByName));
        }

        static async Task StopServiceByName(string serviceName)
        {
            var manager = new WindowsServiceManager();

            try
            {
                bool stopped = await manager.StopServiceAsync(
                    serviceName,
                    TimeSpan.FromSeconds(30));

                if (stopped)
                    ColoredConsole.WriteLine($"[{serviceName}] Служба остановлена.", ConsoleColor.DarkGreen);
                else
                    ColoredConsole.WriteLine($"[{serviceName}] Служба уже была остановлена.", ConsoleColor.DarkYellow);
            }
            catch (Exception ex)
            {
                ColoredConsole.WriteLine($"[{serviceName}] Ошибка: {ex.Message}", ConsoleColor.Red);
            }
        }
    }
}
