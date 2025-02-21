using System.Collections.Concurrent;

namespace Asc.Utils.Commander.Implementation;

internal class SequentialCommandProcessor : ICommandProcessor
{
    private static readonly Lock locker = new();

    private readonly CommandProcessorConfiguration? configuration = null;
    private readonly ConcurrentQueue<ICommand> pendingCommands = new();
    private Task? processUntilQueueIsEmptyTask = null;

    public void ProcessCommand(ICommand command)
    {
        pendingCommands.Enqueue(command);

        locker.Enter();

        try
        {
            if (processUntilQueueIsEmptyTask is null || processUntilQueueIsEmptyTask.Status != TaskStatus.Running)
                processUntilQueueIsEmptyTask = Task.Run(ProcessUntilQueueIsEmptyAsync);
        }
        finally
        {
            locker.Exit();
        }
    }

    internal SequentialCommandProcessor(CommandProcessorConfiguration configuration)
    {
        this.configuration = configuration;
    }

    private async Task ProcessUntilQueueIsEmptyAsync()
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
            await ProcessUntilQueueIsEmptyAsync();
    }
}
