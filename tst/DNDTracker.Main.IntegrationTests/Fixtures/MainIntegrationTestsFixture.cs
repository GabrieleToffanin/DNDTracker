using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.PostgreSql;
using Xunit;

namespace DNDTracker.Main.IntegrationTests.Fixtures;

public class MainIntegrationTestsFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgreSqlContainer;
    private WebApplicationFactory<Program> _factory;

    public MainIntegrationTestsFixture()
    {
        _postgreSqlContainer = new PostgreSqlBuilder()
            .WithImage("postgres:15")
            .WithDatabase("dndtracker_test")
            .WithUsername("postgres")
            .WithPassword("postgres")
            .WithPortBinding(5432, true)
            .Build();
    }

    public async Task InitializeAsync()
    {
        await _postgreSqlContainer.StartAsync();
        
        _factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureAppConfiguration((context, config) =>
                {
                    // Clear existing configuration sources
                    config.Sources.Clear();
                
                    // Re-add the default configuration sources
                    config.AddJsonFile("appsettings.json", optional: true)
                        .AddJsonFile($"appsettings.Development.json", optional: true);

                    var connectionString = _postgreSqlContainer.GetConnectionString();
                    // Add the overridden connection string with highest priority
                    config.AddInMemoryCollection(new Dictionary<string, string>
                    {
                        { "ConnectionStrings:DefaultConnection", connectionString }
                    }!);
                });
            });
    }
    
    public HttpClient CreateClient()
    {
        return _factory.CreateClient();
    }
    
    public async Task DisposeAsync()
    {
        await _postgreSqlContainer.DisposeAsync();
        await _factory.DisposeAsync();
    }
}