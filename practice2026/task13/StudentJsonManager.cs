using System;
using System.IO;
using System.Text.Json;
using System.Text.Encodings.Web;
using System.Text.Unicode;

namespace task13
{
    public class StudentJsonManager
    {
        private readonly JsonSerializerOptions _options;
        
        public StudentJsonManager()
        {
            _options = new JsonSerializerOptions()
            {
                WriteIndented = true,
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.Cyrillic)
            };
        }
        public string Serialize(Student student)
        {
            return JsonSerializer.Serialize(student, _options);
        }
        public Student? Deserialize(string json)
        {
            var student = JsonSerializer.Deserialize<Student>(json, _options);
            Validate(student);
            return student;
        }
        public void SaveToFile(Student student,string filePath)
        {
            var json = Serialize(student);
            File.WriteAllText(filePath, json);
        }
        public Student? LoadFromFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("Файл не найден",filePath);
            }
            var json = File.ReadAllText(filePath);
            return Deserialize(json);
        }
        public static void Validate(Student? student)
        {
            if (student == null)
            {
                throw new ArgumentNullException(nameof(student), "Десериализованный объект не может быть null");
            }
            if (string.IsNullOrWhiteSpace(student.FirstName) || string.IsNullOrWhiteSpace(student.LastName)){
                throw new ArgumentException("Поля FirstName и LastName обязательны для заполнения");
            }
            if (student.BirthDate > DateTime.Now)
            {
                throw new ArgumentException("Дата рождения не может быть в будущем");
            }
            if (student.Grades != null){
                foreach(var subject in student.Grades)
                {
                    if (string.IsNullOrWhiteSpace(subject.Name))
                    {
                        throw new ArgumentException("Название предмета обязательно");
                    }
                    if (subject.Grade < 0 || subject.Grade > 100)
                    {
                        throw new ArgumentException($"Некорректная оценка по предмету {subject.Name}");
                    }
                }
            }
        }
    }
}
