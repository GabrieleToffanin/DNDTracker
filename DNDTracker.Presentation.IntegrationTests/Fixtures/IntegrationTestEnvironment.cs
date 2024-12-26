
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Testcontainers.PostgreSql;
using Testcontainers.RabbitMq;

namespace DNDTracker.Presentation.IntegrationTests.Fixtures;

public class IntegrationTestEnvironment
    : WebApplicationFactory<Program> , IAsyncLifetime
{
    public RabbitMqContainer RabbitMqContainer { get; }
    public PostgreSqlContainer PostgresContainer { get; }

    public IntegrationTestEnvironment()
    {
        this.RabbitMqContainer = new RabbitMqBuilder()
            .WithPassword("guest")
            .WithUsername("guest")
            .WithImage("rabbitmq:management")
            .WithName("RabbitMQIntegrationTest")
            .WithPortBinding(5672, true)
            .Build();
        
        this.PostgresContainer = new PostgreSqlBuilder()
            .WithPassword("test")
            .WithPortBinding(5432, true)
            .WithName("PostgresIntegrationTest")
            .WithUsername("test")
            .WithDatabase("testdb")
            .Build();
    }
    
    public async Task InitializeAsync()
    {
        await Task.WhenAll(RabbitMqContainer.StartAsync(), PostgresContainer.StartAsync());
    }

    public async Task DisposeAsync()
    {
        await Task.WhenAll(RabbitMqContainer.StopAsync(), PostgresContainer.StopAsync());
    }
}