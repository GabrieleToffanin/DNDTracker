using DNDTracker.Application.UseCases.Campaigns.GetCampaign;
using Scalar.AspNetCore;

namespace DNDTracker.Presentation;

public partial class Program
{

    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers();
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
        configuration.RegisterServicesFromAssembly(typeof(GetCampaignByIdHandler).Assembly);
    }
}