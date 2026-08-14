using System.Diagnostics;
using DNDTracker.Application.Behaviors;
using DNDTracker.Application.Queries.UseCases.GetCampaign;
using MediatR;
using DNDTracker.Application.UseCases.Campaigns.CreateCampaign;
using DNDTracker.Domain;
using DNDTracker.Domain.Campaigns;
using DNDTracker.Inbound.AmqpAdapter;
using DNDTracker.Inbound.RestAdapter.Controllers;
using DNDTracker.Main.Middleware;
using DNDTracker.Outbound.RabbitMq;
using DNDTracker.Outbound.RabbitMq.Configuration;
using DNDTracker.Outbound.RabbitMq.Messaging;
using DNDTracker.Outbound.PostgresDb.Database.Postgres;
using DNDTracker.Outbound.PostgresDb.Repositories;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Scalar.AspNetCore;
using Serilog;
using Serilog.Sinks.OpenTelemetry;

namespace DNDTracker.Main;

public class Program
{
    public static async Task Main(string[] args)
    {
        Activity.DefaultIdFormat = ActivityIdFormat.W3C;
        Activity.ForceDefaultIdFormat = true;

        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

        string otlpEndpoint = builder.Configuration["OpenTelemetry:Endpoint"] ?? "http://localhost:4317";
        string serviceName = builder.Configuration["OpenTelemetry:ServiceName"] ?? "DNDTracker";
        string serviceVersion = builder.Configuration["OpenTelemetry:ServiceVersion"] ?? "1.0.0";

        builder.Logging.ClearProviders();
        builder.Host.UseSerilog((context, loggerConfiguration) =>
        {
            loggerConfiguration
                .MinimumLevel.Information()
                .MinimumLevel.Override("Microsoft.AspNetCore", Serilog.Events.LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.EntityFrameworkCore", Serilog.Events.LogEventLevel.Warning)
                .MinimumLevel.Override("Npgsql", Serilog.Events.LogEventLevel.Information)
                .Enrich.FromLogContext()
                .Enrich.WithProperty("service.name", serviceName)
                .Enrich.WithProperty("service.version", serviceVersion)
                .WriteTo.Console(
                    outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties}{NewLine}{Exception}")
                .WriteTo.OpenTelemetry(options =>
                {
                    options.Endpoint = otlpEndpoint.Replace("4317", "4318") + "/v1/logs";
                    options.Protocol = OtlpProtocol.HttpProtobuf;
                    options.ResourceAttributes = new Dictionary<string, object>
                    {
                        ["service.name"] = serviceName,
                        ["service.version"] = serviceVersion
                    };
                });
        });

        ResourceBuilder resourceBuilder = ResourceBuilder.CreateDefault()
            .AddService(serviceName: serviceName, serviceVersion: serviceVersion);

        builder.Services.AddOpenTelemetry()
            .WithTracing(tracing => tracing
                .SetResourceBuilder(resourceBuilder)
                .AddAspNetCoreInstrumentation(opts =>
                {
                    opts.RecordException = true;
                    opts.Filter = ctx => !ctx.Request.Path.StartsWithSegments("/health")
                                     && !ctx.Request.Path.StartsWithSegments("/metrics");
                })
                .AddHttpClientInstrumentation()
                .AddEntityFrameworkCoreInstrumentation()
                .AddSource("Npgsql")
                .AddSource(TracingPipelineBehavior<object, object>.ActivitySource.Name)
                .AddSource(RabbitMqTelemetry.ActivitySourceName)
                .AddOtlpExporter(opts => opts.Endpoint = new Uri(otlpEndpoint)))
            .WithMetrics(metrics => metrics
                .SetResourceBuilder(resourceBuilder)
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddRuntimeInstrumentation()
                .AddPrometheusExporter()
                .AddOtlpExporter(opts => opts.Endpoint = new Uri(otlpEndpoint)));

        builder.Services.AddDbContext<DNDTrackerPostgresDbContext>(options =>
        {
            options.UseNpgsql(builder.Configuration["ConnectionStrings:DefaultConnection"]);
            options.EnableSensitiveDataLogging(builder.Environment.IsDevelopment());
            options.EnableDetailedErrors(builder.Environment.IsDevelopment());
        });

        AssemblyPart inboundRestAdapterPart = new(typeof(CampaignController).Assembly);

        builder.Services.AddControllers()
            .PartManager.ApplicationParts.Add(inboundRestAdapterPart);

        builder.Services.AddOpenApi();
        builder.Services.AddMediatR(ConfigureMediatR);
        builder.Services.AddScoped(
            typeof(IPipelineBehavior<,>),
            typeof(TracingPipelineBehavior<,>));
        builder.Services.AddScoped<ICampaignRepository, PostgreCampaignRepository>();
        builder.Services.Configure<RabbitMqConfiguration>(
            builder.Configuration.GetSection("RabbitMQ"));
        builder.Services.AddRabbitMqMessaging();
        builder.Services.AddAmqpAdapter();

        builder.Services.Configure<BackpressureOptions>(
            builder.Configuration.GetSection("Backpressure"));
        builder.Services.AddSingleton<BackpressureOptions>(provider =>
            provider.GetRequiredService<Microsoft.Extensions.Options.IOptions<BackpressureOptions>>().Value);
        builder.Services.AddProblemDetails();
        builder.Services.AddExceptionHandler<NotFoundExceptionHandler>();

        WebApplication app = builder.Build();

        bool initMode = string.Equals(
            builder.Configuration["DNDTRACKER_INIT"],
            "true",
            StringComparison.OrdinalIgnoreCase)
            || Array.Exists(args, arg => string.Equals(arg, "--init", StringComparison.OrdinalIgnoreCase));

        if (initMode)
        {
            await InfrastructureBootstrapper.InitializeAsync(app);
            return;
        }

        app.MapOpenApi();
        app.MapScalarApiReference(options =>
        {
            options
                .WithTitle("DNDTracker API")
                .WithTheme(ScalarTheme.Mars)
                .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
        });

        if (!string.Equals(builder.Configuration["ASPNETCORE_ENVIRONMENT"], "Docker"))
        {
            app.UseHttpsRedirection();
        }

        app.UseExceptionHandler();
        app.UseMiddleware<BackpressureMiddleware>();
        app.UseAuthorization();
        app.MapControllers();
        app.MapGet("/health", () => Results.Ok(new { status = "healthy", timestamp = DateTime.UtcNow }));
        app.MapPrometheusScrapingEndpoint();

        app.Run();
    }

    private static void ConfigureMediatR(MediatRServiceConfiguration configuration)
    {
        configuration.RegisterServicesFromAssembly(typeof(GetCampaignByNameHandler).Assembly);
        configuration.RegisterServicesFromAssembly(typeof(CreateCampaignCommandHandler).Assembly);
    }
}
