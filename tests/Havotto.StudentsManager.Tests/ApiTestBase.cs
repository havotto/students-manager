using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Havotto.StudentsManager.Tests;

/// <summary>
/// xUnit creates a new instance per test, so every test gets its own app and its own in-memory database.
/// </summary>
public abstract class ApiTestBase : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly WebApplicationFactory<Program> _factory;

    protected ApiTestBase()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        _factory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
            builder.ConfigureTestServices(services =>
            {
                services.RemoveAll<SqliteConnection>();
                services.AddSingleton(_connection);
            }));

        Client = _factory.CreateClient();
    }

    protected HttpClient Client { get; }

    protected static CancellationToken Ct => TestContext.Current.CancellationToken;

    public void Dispose()
    {
        Client.Dispose();
        _factory.Dispose();
        _connection.Dispose();
        GC.SuppressFinalize(this);
    }
}
