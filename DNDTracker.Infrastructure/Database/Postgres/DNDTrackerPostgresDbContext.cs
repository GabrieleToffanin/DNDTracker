using Microsoft.EntityFrameworkCore;

namespace DNDTracker.Infrastructure.Database.Postgres;

public class DNDTrackerPostgresDbContext(DbContextOptions<DNDTrackerPostgresDbContext> options)
    : DbContext(options);