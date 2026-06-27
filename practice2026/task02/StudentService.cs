using System;
using System.Collections.Generic;
using System.Linq;

namespace task02
{
    public class StudentService
    {
        List<Student> students;

        public StudentService(List<Student> Students)
        {
            students = Students;

        }

        public IEnumerable<Student> GetStudentsByFaculty(string FacultyName)
        {
            return from student in students
                   where student.Faculty == FacultyName
                   select student;
        }

        public IEnumerable<Student> GetStudentsWithMinAverageGrade(double minAverage)
        {
            return from student in students
                   where student.Grades.Average() > minAverage
                   select student;
        }

        public IEnumerable<Student> GetStudentsOrderedByName()
        {
            return from student in students
                   orderby student.Name
                   select student;
        }
        public IEnumerable<IGrouping<string,Student>> GetStudentsGroupedByFaculty() {
            return from student in students
                   group student by student.Faculty
                   into studentGroup
                   select studentGroup;
        }
        public string GetHighestAverageFaculty()
        {
            return (from student in students
                    group student by student.Faculty into studentGroup
                    orderby (from student in studentGroup
                             select student.Grades.Average()).ToArray().Average() descending
                    select studentGroup).ToArray()[0].Key;
        }
    }
}
