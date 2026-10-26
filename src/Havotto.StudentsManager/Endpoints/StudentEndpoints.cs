using Havotto.StudentsManager.Dto;
using Havotto.StudentsManager.Services;
using Havotto.StudentsManager.Util;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Havotto.StudentsManager.Endpoints;

public static class StudentEndpoints
{
    public static IEndpointRouteBuilder MapStudentEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/students").WithTags("Students");

        group.MapGet("/", GetAll);
        group.MapGet("/{id:int}", GetById).WithName(nameof(GetById));
        group.MapPost("/", Create);
        group.MapPut("/{id:int}", Update);
        group.MapDelete("/{id:int}", Delete);

        return app;
    }

    private static async Task<Ok<IReadOnlyList<StudentResponse>>> GetAll(
        StudentService service, CancellationToken ct)
    {
        var result = await service.GetAllAsync(ct);
        return TypedResults.Ok(result.Value);
    }

    private static async Task<Results<Ok<StudentResponse>, ProblemHttpResult>> GetById(
        int id, StudentService service, CancellationToken ct)
    {
        var result = await service.GetByIdAsync(id, ct);
        return result.IsSuccess ? TypedResults.Ok(result.Value) : result.Error!.ToProblem();
    }

    private static async Task<Results<CreatedAtRoute<StudentResponse>, ProblemHttpResult>> Create(
        StudentRequest request, StudentService service, CancellationToken ct)
    {
        var result = await service.CreateAsync(request, ct);
        return result.IsSuccess
            ? TypedResults.CreatedAtRoute(result.Value, nameof(GetById), new { id = result.Value!.Id })
            : result.Error!.ToProblem();
    }

    private static async Task<Results<Ok<StudentResponse>, ProblemHttpResult>> Update(
        int id, StudentRequest request, StudentService service, CancellationToken ct)
    {
        var result = await service.UpdateAsync(id, request, ct);
        return result.IsSuccess ? TypedResults.Ok(result.Value) : result.Error!.ToProblem();
    }

    private static async Task<Results<NoContent, ProblemHttpResult>> Delete(
        int id, StudentService service, CancellationToken ct)
    {
        var result = await service.DeleteAsync(id, ct);
        return result.IsSuccess ? TypedResults.NoContent() : result.Error!.ToProblem();
    }
}
