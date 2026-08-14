using System.ComponentModel;
using DNDTracker.Outbound.PostgresDb.Database.Postgres;
using DNDTracker.Outbound.PostgresDb.Repositories;
using DNDTracker.Domain.Campaigns;
using DNDTracker.Domain.Tests.Behaviors;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
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
        await Context.Database.MigrateAsync();
    }

    public async Task DisposeAsync()
    {
        await Context.Database.EnsureDeletedAsync();
        await PostgresContainer.StopAsync();
    }
}