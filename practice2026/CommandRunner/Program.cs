using CommandLib;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
namespace CommandRunner
{
    internal class Program
    {
        static void Main()
        {
            string targetDir = AppDomain.CurrentDomain.BaseDirectory;
            string dllPath = Path.GetFullPath(Path.Combine(targetDir, "..", "..", "..", "..", "FileSystemCommands", "bin", "Debug", "net9.0", "FileSystemCommands.dll"));
            if (!File.Exists(dllPath))
            {
                dllPath = Path.Combine(targetDir, "FileSystemCommands.dll");
                if (!File.Exists(dllPath))
                {
                    Console.WriteLine($"Сборка не найдена в: " + dllPath);
                    return;
                }
            }
            try
            {
                Assembly assembly = Assembly.LoadFrom(dllPath);

                var commandTypes = assembly.GetTypes().Where(t => typeof(ICommand).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);
                foreach (var commandType in commandTypes)
                {
                    Console.WriteLine("Инициализация и выполнение " + commandType.Name);
                    ICommand command = null;
                    if (commandType.Name == "DirectorySizeCommand")
                    {
                        command = (ICommand)Activator.CreateInstance(commandType, targetDir);
                    }
                    else if (commandType.Name == "FindFilesCommand")
                    {
                        command = (ICommand)(Activator.CreateInstance(commandType, targetDir, "*.dll"));
                    }
                    command?.Execute();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка:" + ex.Message);
            }
        }
    }
}
