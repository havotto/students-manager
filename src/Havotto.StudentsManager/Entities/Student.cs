namespace Havotto.StudentsManager.Entities;

public class Student
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public int Age { get; set; }
    public required string ClassName { get; set; }
}
