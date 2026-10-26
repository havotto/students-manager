using System.Net;

namespace Havotto.StudentsManager.Tests;

public class FrontendTests : ApiTestBase
{
    [Fact(DisplayName = "Should serve the SPA at the root URL")]
    public async Task ShouldServeSpa()
    {
        // Act
        var response = await Client.GetAsync("/", Ct);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("text/html", response.Content.Headers.ContentType?.MediaType);
        Assert.Contains("Students Manager", await response.Content.ReadAsStringAsync(Ct));
    }
}
