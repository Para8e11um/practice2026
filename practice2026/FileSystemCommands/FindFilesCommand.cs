using System;
using System.Collections.Generic;
using System.IO;
using CommandLib;

namespace FileSystemCommands
{
    public class FindFilesCommand : ICommand
    {
        private readonly string _path;
        private readonly string _pattern;
        public List<string> FoundFiles { get; private set; } = new List<string>();

        public FindFilesCommand(string path, string pattern)
        {
            _path = path;
            _pattern = pattern;
        }
        public void Execute()
        {
            FoundFiles.Clear();
            try
            {
                if (!Directory.Exists(_path))
                {
                    Console.WriteLine("Каталог не найден: " + _path);
                    return;
                }
                string[] files = Directory.GetFiles(_path, _pattern, SearchOption.AllDirectories);
                FoundFiles.AddRange(files);
                Console.WriteLine($"Найдено файлов по маске '{_pattern}' в '{_path}': {files.Length}");
                foreach (var file in files)
                {
                    Console.WriteLine("- " + file);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка при поиске файлов: " + ex.Message);
            }
        }
    }
}
