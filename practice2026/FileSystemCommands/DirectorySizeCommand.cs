using System;
using System.IO;
using CommandLib;
namespace FileSystemCommands
{
    public class DirectorySizeCommand : ICommand
    {
        private readonly string _path;

        public long Size { get; private set; }

        public DirectorySizeCommand(string path)
        {
            _path = path;
        }
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
