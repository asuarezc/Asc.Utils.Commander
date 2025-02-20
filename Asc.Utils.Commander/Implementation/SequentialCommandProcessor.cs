using System.Collections.Concurrent;

namespace Asc.Utils.Commander.Implementation;

internal class SequentialCommandProcessor : ICommandProcessor
{
    private readonly CommandProcessorConfiguration? configuration = null;
    private readonly ConcurrentQueue<ICommand> pendingCommands = new();
    private Task? processUntilQueueIsEmptyTask = null;

    #region ICommandProcessor implementation

    public CommandExecutionMode ExecutionMode => CommandExecutionMode.Sequential;

    public bool IsRunning =>
        processUntilQueueIsEmptyTask is not null
        && processUntilQueueIsEmptyTask.Status == TaskStatus.Running;

    public event EventHandler<bool>? IsRunningChanged;

    public void ProcessCommand(ICommand command)
    {
        pendingCommands.Enqueue(command);

        if (!IsRunning)
        {
            IsRunningChanged?.Invoke(this, true);
            processUntilQueueIsEmptyTask = Task.Run(ProcessUntilQueueIsEmpty);
        }
    }

    #endregion

    internal SequentialCommandProcessor(CommandProcessorConfiguration configuration)
    {
        this.configuration = configuration;
    }

    private async Task ProcessUntilQueueIsEmpty()
    {
        if (configuration is null)
            throw new InvalidOperationException("Cannot process command without a configuration");

        if (!pendingCommands.TryDequeue(out ICommand? command))
            throw new InvalidOperationException("Cannot dequeue command");

        if (command is null)
            throw new InvalidOperationException("Cannot process a null command");

        if (command is not CommandBase commandBase)
            throw new InvalidOperationException("Cannot process a null command");

        await commandBase.RunAsync(configuration);

        if (!pendingCommands.IsEmpty)
            await ProcessUntilQueueIsEmpty();

        IsRunningChanged?.Invoke(this, false);
    }
}
