

namespace TemporalWarehouse.Api.Application.Interfaces;

public interface IBackgroundTaskQueue
{
    void QueueBackgroundTaskItem(Func<CancellationToken, Task> taskItem);
    Task<Func<CancellationToken, Task>> DequeueAsync(CancellationToken cancellationToken);
}