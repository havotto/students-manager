using Havotto.StudentsManager.Data;
using Havotto.StudentsManager.Dto;
using Havotto.StudentsManager.Mappers;
using Havotto.StudentsManager.Util;
using Microsoft.EntityFrameworkCore;

namespace Havotto.StudentsManager.Services;

public class StudentService(StudentsDbContext db)
{
    public async Task<Result<IReadOnlyList<StudentResponse>>> GetAllAsync(CancellationToken ct)
    {
        var students = await db.Students
            .AsNoTracking()
            .OrderBy(s => s.Id)
            .ToListAsync(ct);
        return Result<IReadOnlyList<StudentResponse>>.Success(students.ConvertAll(s => s.ToResponse()));
    }

    public async Task<Result<StudentResponse>> GetByIdAsync(int id, CancellationToken ct)
    {
        var student = await db.Students.AsNoTracking().FirstOrDefaultAsync(s => s.Id == id, ct);
        return student is null ? NotFound(id) : student.ToResponse();
    }

    public async Task<Result<StudentResponse>> CreateAsync(StudentRequest request, CancellationToken ct)
    {
        var student = request.ToEntity();
        db.Students.Add(student);
        await db.SaveChangesAsync(ct);
        return student.ToResponse();
    }

    public async Task<Result<StudentResponse>> UpdateAsync(int id, StudentRequest request, CancellationToken ct)
    {
        var student = await db.Students.FindAsync([id], ct);
        if (student is null)
        {
            return NotFound(id);
        }

        request.ApplyTo(student);
        await db.SaveChangesAsync(ct);
        return student.ToResponse();
    }

    public async Task<Result<StudentResponse>> DeleteAsync(int id, CancellationToken ct)
    {
        var student = await db.Students.FindAsync([id], ct);
        if (student is null)
        {
            return NotFound(id);
        }

        db.Students.Remove(student);
        await db.SaveChangesAsync(ct);
        return student.ToResponse();
    }

    private static Error NotFound(int id) =>
        new(ErrorType.NotFound, "Student not found", $"Student with ID {id} was not found.");
}
