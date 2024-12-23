using DNDTracker.Application.UseCases.Campaigns.GetCampaign;
using Scalar.AspNetCore;

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

void ConfigureMediatR(MediatRServiceConfiguration configuration)
{
    configuration.RegisterServicesFromAssembly(typeof(GetCampaignByIdHandler).Assembly);
}