using Havotto.StudentsManager.Data;
using Havotto.StudentsManager.Endpoints;
using Havotto.StudentsManager.Services;
using Havotto.StudentsManager.Util;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Havotto.StudentsManager;

public static class StudentsManagerExtensions
{
    public static IServiceCollection AddStudentsManager(this IServiceCollection services)
    {
        services.AddProblemDetails();
        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddValidation();

        // In-memory SQLite lives only as long as its connection is open, so one shared
        // connection is kept for the app lifetime.
        services.AddSingleton(_ =>
        {
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();
            return connection;
        });
        services.AddDbContext<StudentsDbContext>((sp, options) =>
            options.UseSqlite(sp.GetRequiredService<SqliteConnection>()));

        services.AddScoped<StudentService>();
        return services;
    }

    public static WebApplication UseStudentsManager(this WebApplication app)
    {
        using (var scope = app.Services.CreateScope())
        {
            scope.ServiceProvider.GetRequiredService<StudentsDbContext>().Database.EnsureCreated();
        }

        app.UseExceptionHandler();
        app.UseStatusCodePages();
        app.UseDefaultFiles();
        app.UseStaticFiles();
        return app;
    }

    public static WebApplication MapStudentsManager(this WebApplication app)
    {
        app.MapStudentEndpoints();
        return app;
    }
}
