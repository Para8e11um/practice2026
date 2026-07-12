using System;
using System.Collections.Generic;
using System.IO;
using CommandLib;
using task07;

namespace FileSystemCommands
{
    [DisplayName("Команда поиска файлов в директории")]
    public class FindFilesCommand : ICommand
    {
        private readonly string _path;
        private readonly string _pattern;
        [DisplayName("Список найденных файлов")]
        public List<string> FoundFiles { get; private set; } = new List<string>();
        [DisplayName("Конструктор команды поиска файлов в директории")]
        public FindFilesCommand(string path, string pattern)
        {
            _path = path;
            _pattern = pattern;
        }
        [DisplayName("Метод исполнения команды поиска файлов в директории")]
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
