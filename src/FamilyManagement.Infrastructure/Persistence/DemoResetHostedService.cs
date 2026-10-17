using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using FamilyManagement.Application.Interfaces;

namespace FamilyManagement.Infrastructure.Persistence;

public class DemoResetHostedService : BackgroundService
{
    private readonly IDemoDataSeeder _demoDataSeeder;
    private readonly IConfiguration _configuration;
    private readonly ILogger<DemoResetHostedService> _logger;

    public DemoResetHostedService(
        IDemoDataSeeder demoDataSeeder,
        IConfiguration configuration,
        ILogger<DemoResetHostedService> logger)
    {
        _demoDataSeeder = demoDataSeeder;
        _configuration = configuration;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            await _demoDataSeeder.EnsureSeededAsync(stoppingToken);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _logger.LogError(ex, "Failed to ensure demo sandbox data on startup.");
        }

        var intervalHours = _configuration.GetValue<int?>("Auth:DemoResetIntervalHours") ?? 24;
        if (intervalHours <= 0)
        {
            intervalHours = 24;
        }

        using var timer = new PeriodicTimer(TimeSpan.FromHours(intervalHours));
        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            try
            {
                await _demoDataSeeder.ResetAndSeedAsync(stoppingToken);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                _logger.LogError(ex, "Periodic 24h demo sandbox reset failed.");
            }
        }
    }
}
