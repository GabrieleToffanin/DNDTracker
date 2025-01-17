using DNDTracker.BackendInfrastructure.PostgresDb.Database.Postgres;
using DNDTracker.Domain.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Testcontainers.PostgreSql;
using Testcontainers.RabbitMq;

namespace DNDTracker.IntegrationTests.Fixtures;

public abstract class IntegrationTestEnvironment
    : WebApplicationFactory<Program> , IAsyncLifetime
{
    private RabbitMqContainer RabbitMqContainer { get; } = new RabbitMqBuilder()
        .WithPassword("guest")
        .WithUsername("guest")
        .WithImage("rabbitmq:management")
        .WithName("RabbitMQIntegrationTest")
        .WithPortBinding(5672, true)
        .Build();

    private PostgreSqlContainer PostgresContainer { get; } = new PostgreSqlBuilder()
        .WithPassword("test")
        .WithPortBinding(5432, true)
        .WithName("PostgresIntegrationTest")
        .WithUsername("test")
        .WithDatabase("testdb")
        .Build();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        var connectionString = this.PostgresContainer.GetConnectionString();
        
        builder.ConfigureServices(services =>
        {
            // Swap out database implementations for using the container
            services.Remove(services.Single(d => d.ServiceType == typeof(DNDTrackerPostgresDbContext)));
            
            services.AddDbContext<DNDTrackerPostgresDbContext>(options =>
            {
                options.UseNpgsql(connectionString);
            });
            
            InitializeDatabaseData(services);
        });
    }

    private void InitializeDatabaseData(IServiceCollection services)
    {
        var build = services.BuildServiceProvider();

        using var servicesScope = build.CreateScope();

        var current = servicesScope.ServiceProvider;
        var db = current.GetRequiredService<DNDTrackerPostgresDbContext>();
        db.Database.EnsureCreated();

        this.InitializeDatabaseWithCampaignAsync(db);
    }

    private void InitializeDatabaseWithCampaignAsync(DNDTrackerPostgresDbContext db)
    {
        Campaign campaign = Campaign.Create(
            "TestCampaign",
            "TestDescription",
            "TestUrl.jpg",
            DateTime.UtcNow,
            true);
        
        db.Add(campaign);
        
        db.SaveChanges();
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