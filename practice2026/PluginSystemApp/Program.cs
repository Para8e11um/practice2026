using PluginSystem;
namespace PluginSystemApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Ошибка: не указан путь к директории с библиотеками");
                return;
            }
            string pluginsFolder = args[0];
            if (!Directory.Exists(pluginsFolder))
            {
                Console.WriteLine("Ошибка: данной директории не существует");
                return;
            }
            var plguinManager = new PluginManager();
            plguinManager.LoadAndExecutePlugins(pluginsFolder);
            Console.ReadLine();
        }
    }
}
