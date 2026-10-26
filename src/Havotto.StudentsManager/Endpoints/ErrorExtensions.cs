using Havotto.StudentsManager.Util;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Havotto.StudentsManager.Endpoints;

public static class ErrorExtensions
{
    public static ProblemHttpResult ToProblem(this Error error)
    {
        var statusCode = error.Type switch
        {
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            _ => StatusCodes.Status500InternalServerError,
        };
        return TypedResults.Problem(title: error.Title, detail: error.Detail, statusCode: statusCode);
    }
}
