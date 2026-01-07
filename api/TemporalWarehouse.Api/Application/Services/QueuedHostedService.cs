
using TemporalWarehouse.Api.Application.Interfaces;


namespace TemporalWarehouse.Api.Application.Services;

public class QueuedHostedService(
                            IBackgroundTaskQueue taskQueue,
                            IServiceProvider serviceProvider,
                            ILogger<QueuedHostedService> logger) : BackgroundService
{
    private readonly IBackgroundTaskQueue _taskQueue = taskQueue;
    private readonly IServiceProvider _serviceProvider = serviceProvider;
    private readonly ILogger<QueuedHostedService> _logger = logger;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Queued Hosted Service running!");
        while (!stoppingToken.IsCancellationRequested)
        {
            var taskItem = await _taskQueue.DequeueAsync(stoppingToken);
            try
            {

                using var scope = _serviceProvider.CreateAsyncScope();
                await taskItem(stoppingToken);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, $"Error occurred executing {taskItem}.", nameof(taskItem));
            }
        }
    }
}
