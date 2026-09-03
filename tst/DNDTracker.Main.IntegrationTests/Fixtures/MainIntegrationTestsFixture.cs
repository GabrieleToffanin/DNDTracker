using DNDTracker.Outbound.PostgresDb.Database.Postgres;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.PostgreSql;
using Testcontainers.RabbitMq;
using Xunit;

namespace DNDTracker.Main.IntegrationTests.Fixtures;

public class MainIntegrationTestsFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgreSqlContainer;
    private readonly RabbitMqContainer _rabbitMqContainer;
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

        _rabbitMqContainer = new RabbitMqBuilder()
            .WithImage("rabbitmq:3-management")
            .WithUsername("guest")
            .WithPassword("guest")
            .WithPortBinding(5672, true)
            .WithPortBinding(15672, true)
            .Build();
    }

    public async Task InitializeAsync()
    {
        await _postgreSqlContainer.StartAsync();
        await _rabbitMqContainer.StartAsync();
        
        _factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                // NetPub's connection factory is configured while services are registered, so the
                // broker endpoint must be visible before Build(): UseSetting flows through the args.
                builder.UseSetting("RabbitMQ:Host", _rabbitMqContainer.Hostname);
                builder.UseSetting("RabbitMQ:Port", _rabbitMqContainer.GetMappedPublicPort(5672).ToString());

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
                        { "ConnectionStrings:DefaultConnection", connectionString },
                        { "RabbitMQ:Host", _rabbitMqContainer.Hostname },
                        { "RabbitMQ:Port", _rabbitMqContainer.GetMappedPublicPort(5672).ToString() },
                    }!);
                });
            });

        using var scope = _factory.Services.CreateScope();
        var database = scope.ServiceProvider.GetRequiredService<DNDTrackerPostgresDbContext>();
        await database.Database.MigrateAsync();
    }
    
    public HttpClient CreateClient()
    {
        return _factory.CreateClient();
    }
    
    public async Task DisposeAsync()
    {
        await _postgreSqlContainer.DisposeAsync();
        await _rabbitMqContainer.DisposeAsync();
        await _factory.DisposeAsync();
    }
}