using DNDTracker.Inbound.AmqpAdapter.Consumers;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DNDTracker.Inbound.AmqpAdapter;

public class HeroAddedEventHostedService(
    HeroAddedEventConsumer heroAddedConsumer,
    ILogger<HeroAddedEventHostedService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            logger.LogInformation("🟢 HeroAddedEventHostedService starting");
            await heroAddedConsumer.StartAsync(stoppingToken);
            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
        catch (OperationCanceledException)
        {
            logger.LogInformation("🟡 HeroAddedEventHostedService cancelled");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "❌ HeroAddedEventHostedService failed");
            throw;
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("🛑 HeroAddedEventHostedService stopping");
        await heroAddedConsumer.DisposeAsync();
        await base.StopAsync(cancellationToken);
    }
}
