using System.Threading.Channels;
using TemporalWarehouse.Api.Application.Interfaces;

namespace TemporalWarehouse.Api.Application.Services;

public class BackgroundTaskQueue : IBackgroundTaskQueue
{
    private readonly Channel<Func<CancellationToken, Task>> _queue;
    public BackgroundTaskQueue(int capacity = 100)
    {
        var options = new BoundedChannelOptions(capacity)
        {
            FullMode = BoundedChannelFullMode.Wait
        };

        _queue = Channel.CreateBounded<Func<CancellationToken, Task>>(options);
    }

    public void QueueBackgroundTaskItem(Func<CancellationToken, Task> taskItem)
    {
        ArgumentNullException.ThrowIfNull(taskItem);
        _queue.Writer.TryWrite(taskItem);
    }

    public async Task<Func<CancellationToken, Task>> DequeueAsync(CancellationToken cancellationToken)
    {
        return await _queue.Reader.ReadAsync(cancellationToken);
    }

}