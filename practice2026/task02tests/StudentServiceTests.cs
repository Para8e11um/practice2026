using Xunit;
using System.Collections;
using System.Linq;
using task02;

public class StudentServiceTests
{
    private List<Student> _testStudents;
    private StudentService _studentService;
    public StudentServiceTests()
    {
        _testStudents = new List<Student>
        {
            new Student { Name = "Иван", Faculty = "ФИТ", Grades = new List<int> {5, 4, 5}},
            new Student { Name = "Анна", Faculty = "ФИТ", Grades = new List<int> {3,4,3}},
            new Student { Name = "Петр", Faculty = "Экономика", Grades = new List<int> {5,5,5}}
        };
        _studentService = new StudentService(_testStudents);
    }
    [Fact]
    public void Test1()
    {
        var result = _studentService.GetStudentsByFaculty("ФИТ").ToList();
        Assert.Equal(2, result.Count);
        Assert.True(result.All(s => s.Faculty == "ФИТ"));
    }
    [Fact]
    public void Test2()
    {
        var result = _studentService.GetHighestAverageFaculty();
        Assert.Equal("Экономика", result);
    }
    [Fact]
    public void Test3()
    {
        var result = _studentService.GetStudentsWithMinAverageGrade(4.5).ToList();
        Assert.Equal(2, result.Count);
        Assert.Contains(result, s => s.Name == "Петр");
        Assert.Contains(result, s => s.Name == "Иван");
    }
    [Fact]
    public void Test4()
    {
        var result = _studentService.GetStudentsOrderedByName().ToList();
        Assert.Equal(result[0].Name, "Анна");
        Assert.Equal(result[1].Name, "Иван");
        Assert.Equal(result[2].Name, "Петр");
    }
    [Fact]
    public void Test5()
    {
        var result = _studentService.GetStudentsGroupedByFaculty().ToList();
        Assert.Equal(2, result.Count());
        var fitGroup = result.FirstOrDefault(g => g.Key == "ФИТ");
        Assert.NotNull(fitGroup);
        Assert.Equal(2, fitGroup.Count());
        var ecoGroup = result.FirstOrDefault(g => g.Key == "Экономика");
        Assert.NotNull(ecoGroup);
        Assert.Single(ecoGroup);
    }
}
