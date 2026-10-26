using Havotto.StudentsManager.Dto;
using Havotto.StudentsManager.Entities;

namespace Havotto.StudentsManager.Mappers;

public static class StudentMapper
{
    public static StudentResponse ToResponse(this Student student) =>
        new(student.Id, student.Name, student.Age, student.ClassName);

    public static Student ToEntity(this StudentRequest request) =>
        new() { Name = request.Name, Age = request.Age, ClassName = request.ClassName };

    public static void ApplyTo(this StudentRequest request, Student student)
    {
        student.Name = request.Name;
        student.Age = request.Age;
        student.ClassName = request.ClassName;
    }
}
