using System.ComponentModel.DataAnnotations;

namespace Havotto.StudentsManager.Dto;

public record StudentResponse(int Id, string Name, int Age, string ClassName);

/// <summary>Request body for both creating and updating a student.</summary>
public record StudentRequest(
    [property: Required, StringLength(100, MinimumLength = 1)] string Name,
    [property: Range(1, 150)] int Age,
    [property: Required, StringLength(50, MinimumLength = 1)] string ClassName);
