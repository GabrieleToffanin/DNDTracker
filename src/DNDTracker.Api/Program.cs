using DNDTracker.Application.UseCases.Campaigns.GetCampaign;
using DNDTracker.BackendInfrastructure.PostgresDb.Database.Postgres;
using DNDTracker.Infrastructure.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

namespace DNDTracker.Api;

public partial class Program
{

    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddDbContext<DNDTrackerPostgresDbContext>(options =>
        {
            options.UseNpgsql(builder.Configuration["ConnectionStrings:DNDTrackerPostgres"]);
        });

        builder.Services.AddControllers();
        builder.Services.AddOpenApi();
        builder.Services.AddMediatR(ConfigureMediatR);
        builder.Services.AddInfrastructure();

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.MapScalarApiReference(options =>
            {
                options
                    .WithTheme(ScalarTheme.Mars)
                    .WithTitle("DNDTracker API")
                    .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
            });
        }

        app.UseHttpsRedirection();
        app.MapControllers();

        app.Run();
    }
    
    static void ConfigureMediatR(MediatRServiceConfiguration configuration)
    {
        configuration.RegisterServicesFromAssembly(typeof(GetCampaignByNameHandler).Assembly);
    }
}