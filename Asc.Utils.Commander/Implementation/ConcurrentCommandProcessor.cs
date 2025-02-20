using Asc.Utils.Needle;
using System.Collections.Concurrent;

namespace Asc.Utils.Commander.Implementation;

internal class ConcurrentCommandProcessor(ConcurrentCommandProcessorConfiguration configuration) : ICommandProcessor
{
    private readonly ConcurrentCommandProcessorConfiguration? configuration = configuration;
    private readonly ConcurrentQueue<ICommand> pendingCommands = new();

    private readonly INeedleWorkerSlim worker = Pincushion.Instance.GetSemaphoreWorkerSlim(
        maxThreads: configuration.MaxThreads ?? Environment.ProcessorCount,
        cancelPendingJobsIfAnyOtherFails: false
    );

    private Task? processUntilQueueIsEmptyTask = null;

    #region ICommandProcessor implementation

    public CommandExecutionMode ExecutionMode => CommandExecutionMode.Concurrent;

    public bool IsRunning => processUntilQueueIsEmptyTask is not null && processUntilQueueIsEmptyTask.Status == TaskStatus.Running;

    public event EventHandler<bool>? IsRunningChanged;

    public void ProcessCommand(ICommand command)
    {
        pendingCommands.Enqueue(command);

        if (!IsRunning)
        {
            IsRunningChanged?.Invoke(this, true);
            processUntilQueueIsEmptyTask = Task.Run(ProcessUntilQueueIsEmptyAsync);
        }
    }

    #endregion

    private async Task ProcessUntilQueueIsEmptyAsync()
    {
        do
        {
            if (configuration is null)
                throw new InvalidOperationException("Cannot process command without a configuration");

            if (!pendingCommands.TryDequeue(out ICommand? command))
                throw new InvalidOperationException("Cannot dequeue command");

            if (command is null)
                throw new InvalidOperationException("Cannot process a null command");

            if (command is not CommandBase commandBase)
                throw new InvalidOperationException("Cannot process a null command");

            worker.AddJob(async () => await commandBase.RunAsync(configuration));
        } while (!pendingCommands.IsEmpty);

        await worker.RunAsync();

        if (!pendingCommands.IsEmpty)
            await ProcessUntilQueueIsEmptyAsync();

        IsRunningChanged?.Invoke(this, false);
    }
}
