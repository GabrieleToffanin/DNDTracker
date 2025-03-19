using System.ComponentModel;
using DNDTracker.Outbounx.PostgresDb.Database.Postgres;
using DNDTracker.Outbounx.PostgresDb.Repositories;
using DNDTracker.Domain.Campaigns;
using DNDTracker.Domain.Tests.Behaviors;
using JetBrains.Annotations;
using Testcontainers.PostgreSql;

namespace DNDTracker.BackendInfrastructure.PostgresDb.Tests;

/// <summary>
/// Provides an implementation of the campaign repository for tests within a PostgreSQL database context.
/// This class is used to validate repository behavior by extending the CampaignRepositorySpecification.
/// </summary>
[Category("Specification")]
[TestSubject(typeof(ICampaignRepository))]
public class PostgresCampaignRepository : CampaignRepositorySpecification, IAsyncLifetime
{
    private DNDTrackerPostgresDbContext Context { get; set; }
    private PostgreSqlContainer PostgresContainer { get; } = new PostgreSqlBuilder()
        .WithPassword("test")
        .WithPortBinding(5432, true)
        .WithName("PostgresIntegrationTest")
        .WithUsername("test")
        .WithDatabase("testdb")
        .Build();

    private PostgreCampaignRepository CreateRepository()
    {
        PostgreCampaignRepository repository = new(Context);

        return repository;
    }

    public async Task InitializeAsync()
    {
        await PostgresContainer.StartAsync();
        Context = new DNDTrackerPostgresDbContext(PostgresContainer.GetConnectionString());
        _campaignRepository = CreateRepository();
        await Context.Database.EnsureCreatedAsync();
    }

    public async Task DisposeAsync()
    {
        await Context.Database.EnsureDeletedAsync();
        await PostgresContainer.StopAsync();
    }
}