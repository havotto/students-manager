using Havotto.StudentsManager;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddStudentsManager();

var app = builder.Build();

app.UseStudentsManager();
app.MapStudentsManager();

app.Run();

public partial class Program;
