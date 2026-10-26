using System.Net;
using System.Net.Http.Json;
using Havotto.StudentsManager.Dto;
using Microsoft.AspNetCore.Mvc;

namespace Havotto.StudentsManager.Tests;

public class StudentsApiTests : ApiTestBase
{
    private const string Url = "/api/students";

    private async Task<StudentResponse> CreateStudentAsync(string name = "Alice", int age = 20, string className = "10A")
    {
        var response = await Client.PostAsJsonAsync(Url, new StudentRequest(name, age, className), Ct);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<StudentResponse>(Ct))!;
    }

    [Fact(DisplayName = "Should return an empty list when no students exist")]
    public async Task ShouldReturnEmptyList()
    {
        // Act
        var response = await Client.GetAsync(Url, Ct);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var students = await response.Content.ReadFromJsonAsync<List<StudentResponse>>(Ct);
        Assert.NotNull(students);
        Assert.Empty(students);
    }

    [Fact(DisplayName = "Should return all students")]
    public async Task ShouldReturnAllStudents()
    {
        // Arrange
        var alice = await CreateStudentAsync("Alice", 20, "10A");
        var bob = await CreateStudentAsync("Bob", 21, "10B");

        // Act
        var response = await Client.GetAsync(Url, Ct);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var students = await response.Content.ReadFromJsonAsync<List<StudentResponse>>(Ct);
        Assert.Equal([alice, bob], students);
    }

    [Fact(DisplayName = "Should create a student and return 201 with a Location header")]
    public async Task ShouldCreateStudent()
    {
        // Arrange
        var request = new StudentRequest("Alice", 20, "10A");

        // Act
        var response = await Client.PostAsJsonAsync(Url, request, Ct);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var created = await response.Content.ReadFromJsonAsync<StudentResponse>(Ct);
        Assert.NotNull(created);
        Assert.True(created.Id > 0);
        Assert.Equal(new StudentResponse(created.Id, "Alice", 20, "10A"), created);
        Assert.Equal($"{Url}/{created.Id}", response.Headers.Location?.AbsolutePath);
    }

    [Theory(DisplayName = "Should return 400 ValidationProblemDetails when create request is invalid")]
    [InlineData("", 20, "10A", "Name")]
    [InlineData("Alice", 0, "10A", "Age")]
    [InlineData("Alice", 151, "10A", "Age")]
    [InlineData("Alice", 20, "", "ClassName")]
    public async Task ShouldReturn400WhenCreateInvalid(string name, int age, string className, string invalidField)
    {
        // Arrange
        var request = new StudentRequest(name, age, className);

        // Act
        var response = await Client.PostAsJsonAsync(Url, request, Ct);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var problem = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>(Ct);
        Assert.NotNull(problem);
        Assert.Contains(problem.Errors.Keys, key => key.Equals(invalidField, StringComparison.OrdinalIgnoreCase));
    }

    [Fact(DisplayName = "Should return a student by ID")]
    public async Task ShouldReturnStudentById()
    {
        // Arrange
        var created = await CreateStudentAsync();

        // Act
        var response = await Client.GetAsync($"{Url}/{created.Id}", Ct);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(created, await response.Content.ReadFromJsonAsync<StudentResponse>(Ct));
    }

    [Fact(DisplayName = "Should return 404 ProblemDetails when student ID does not exist")]
    public async Task ShouldReturn404WhenNotFound()
    {
        // Act
        var response = await Client.GetAsync($"{Url}/999", Ct);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>(Ct);
        Assert.NotNull(problem);
        Assert.Equal(404, problem.Status);
    }

    [Fact(DisplayName = "Should update an existing student")]
    public async Task ShouldUpdateStudent()
    {
        // Arrange
        var created = await CreateStudentAsync();
        var request = new StudentRequest("Alice Smith", 21, "11B");

        // Act
        var response = await Client.PutAsJsonAsync($"{Url}/{created.Id}", request, Ct);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var expected = new StudentResponse(created.Id, "Alice Smith", 21, "11B");
        Assert.Equal(expected, await response.Content.ReadFromJsonAsync<StudentResponse>(Ct));
        var fetched = await Client.GetFromJsonAsync<StudentResponse>($"{Url}/{created.Id}", Ct);
        Assert.Equal(expected, fetched);
    }

    [Fact(DisplayName = "Should return 404 ProblemDetails when updating a non-existent student")]
    public async Task ShouldReturn404WhenUpdatingMissing()
    {
        // Act
        var response = await Client.PutAsJsonAsync($"{Url}/999", new StudentRequest("Alice", 20, "10A"), Ct);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>(Ct);
        Assert.NotNull(problem);
        Assert.Equal(404, problem.Status);
    }

    [Fact(DisplayName = "Should return 400 ValidationProblemDetails when update request is invalid")]
    public async Task ShouldReturn400WhenUpdateInvalid()
    {
        // Arrange
        var created = await CreateStudentAsync();

        // Act
        var response = await Client.PutAsJsonAsync($"{Url}/{created.Id}", new StudentRequest("", 20, "10A"), Ct);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var problem = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>(Ct);
        Assert.NotNull(problem);
        Assert.NotEmpty(problem.Errors);
    }

    [Fact(DisplayName = "Should delete a student and return 204")]
    public async Task ShouldDeleteStudent()
    {
        // Arrange
        var created = await CreateStudentAsync();

        // Act
        var response = await Client.DeleteAsync($"{Url}/{created.Id}", Ct);

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        var getResponse = await Client.GetAsync($"{Url}/{created.Id}", Ct);
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    [Fact(DisplayName = "Should return 404 ProblemDetails when deleting a non-existent student")]
    public async Task ShouldReturn404WhenDeletingMissing()
    {
        // Act
        var response = await Client.DeleteAsync($"{Url}/999", Ct);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>(Ct);
        Assert.NotNull(problem);
        Assert.Equal(404, problem.Status);
    }
}
