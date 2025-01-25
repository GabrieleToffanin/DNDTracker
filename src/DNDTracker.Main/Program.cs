using DNDTracker.Application.Queries.UseCases.GetCampaign;
using DNDTracker.BackendInfrastructure.PostgresDb.Database.Postgres;
using DNDTracker.Inbound.RestAdapter.Controllers;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

namespace DNDTracker.Main;

public partial class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddDbContext<DNDTrackerPostgresDbContext>(options =>
        {
            options.UseNpgsql(builder.Configuration["ConnectionStrings:DNDTrackerPostgres"]);
        });

        AssemblyPart inboundRestAdapterPart = new AssemblyPart(typeof(CampaignController).Assembly);
        
        builder.Services.AddControllers()
            .PartManager.ApplicationParts.Add(inboundRestAdapterPart);
        builder.Services.AddOpenApi();
        builder.Services.AddMediatR(ConfigureMediatR);

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