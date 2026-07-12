using System;
using System.IO;
using CommandLib;
using task07;
namespace FileSystemCommands
{
    [DisplayName("Команда вывода размера директории")]
    public class DirectorySizeCommand : ICommand
    {
        private readonly string _path;
        [DisplayName("Размер директории")]
        public long Size { get; private set; }
        [DisplayName("Конструктор команды вывода размера директории")]
        public DirectorySizeCommand(string path)
        {
            _path = path;
        }
        [DisplayName("Метод исполнения команды вывода размера директории")]
        public void Execute()
        {
            if (!Directory.Exists(_path))
            {
                Console.WriteLine("Каталог не найден: " + _path);
                return;
            }
            Size = CalculateDirectorySize(new DirectoryInfo(_path));
            Console.WriteLine("Размер каталога " + _path + ": " + Size);
        }
        [DisplayName("Метод вычисления размера директории")]
        private long CalculateDirectorySize(DirectoryInfo directoryInfo)
        {
            long size = 0;
            try
            {
                var files = directoryInfo.GetFiles();
                var subdirectories = directoryInfo.GetDirectories();
                foreach (FileInfo file in files)
                {
                    size += file.Length;
                }
                foreach (DirectoryInfo subdirectory in subdirectories)
                {
                    size += CalculateDirectorySize(subdirectory);
                }
            }
            catch (UnauthorizedAccessException) { }
            return size;
        }
    }
}
