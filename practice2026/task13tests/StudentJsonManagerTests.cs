using task13;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Xunit;
public class StudentJsonManagerTests
{
    private readonly StudentJsonManager _studentJsonManager = new StudentJsonManager();
    [Fact]
    public void Serialize_IgnoreNull()
    {
        var student = new Student
        {
            FirstName = "Дмитрий",
            LastName = "Патлин",
            BirthDate = new DateTime(2006, 12, 28),
            Grades = null
        };
        var json = _studentJsonManager.Serialize(student);
        Assert.Contains("\"FirstName\": \"Дмитрий\"", json);
        Assert.Contains("\"LastName\": \"Патлин\"", json);
        Assert.DoesNotContain("\"Grades\"", json);
    }
    [Fact]
    public void Deserialize_ValidJson_ReturnObject()
    {
        var json = @"{
            ""FirstName"": ""Дмитрий"",
            ""LastName"": ""Патлин"",
            ""BirthDate"": ""2006-12-28""
            }";

        var student = _studentJsonManager.Deserialize(json);
        Assert.NotNull(student);
        Assert.Equal("Дмитрий", student.FirstName);
        Assert.Equal(new DateTime(2006,12,28),student.BirthDate);
        Assert.Null(student.Grades);
    }
    [Fact]
    public void Deserialize_InvalidDate_ThrowsException()
    {
        var json = @"{
            ""FirstName"": ""АА"",
            ""LastName"": ""ББ"",
            ""BirthDate"": ""28-12-2006""
        }";
        Assert.Throws<JsonException>(() => _studentJsonManager.Deserialize(json));
    }
    [Fact]
    public void Deserialize_MissingName()
    {
        var json = @"{
            ""LastName"": ""Патлин"",
            ""BirthDate"": ""2006-12-28""
            }";
        var ex = Assert.Throws<ArgumentException>(() => _studentJsonManager.Deserialize(json));
        Assert.Contains("Поля FirstName и LastName обязательны для заполнения", ex.Message);
    }
    [Fact]
    public void SaveAndLoad()
    {
        var filePath = "test_student.json";
        var student = new Student
        {
            FirstName = "Дмитрий",
            LastName = "Патлин",
            BirthDate = new DateTime(2006, 12, 28),
            Grades = new List<Subject>
            {
                new Subject { Name = "Математический анализ", Grade = 91 },
                new Subject {Name = "Дискретная математика", Grade = 1}
            }
        };
        try
        {
            _studentJsonManager.SaveToFile(student, filePath);
            var loadedStudent = _studentJsonManager.LoadFromFile(filePath);
            Assert.Equal(student.FirstName, loadedStudent.FirstName);
            Assert.Equal(student.LastName, loadedStudent.LastName);
            Assert.NotNull(loadedStudent.Grades);
            Assert.Equal(2, loadedStudent.Grades.Count);
        }
        finally
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
    }
}

