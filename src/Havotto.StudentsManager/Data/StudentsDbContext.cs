using Havotto.StudentsManager.Entities;
using Microsoft.EntityFrameworkCore;

namespace Havotto.StudentsManager.Data;

public class StudentsDbContext(DbContextOptions<StudentsDbContext> options) : DbContext(options)
{
    public DbSet<Student> Students => Set<Student>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Student>(student =>
        {
            student.Property(s => s.Name).HasMaxLength(100);
            student.Property(s => s.ClassName).HasMaxLength(50);
        });
    }
}
