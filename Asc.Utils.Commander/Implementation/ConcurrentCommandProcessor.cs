using System.Collections.Concurrent;

namespace Asc.Utils.Commander.Implementation;

internal class ConcurrentCommandProcessor(ConcurrentCommandProcessorConfiguration configuration)
    : ICommandProcessor
{
    private static readonly Lock locker = new();
    private static readonly ReaderWriterLockSlim lockerForMaxConcurrentCommands = new(LockRecursionPolicy.NoRecursion);

    private int numberOfProcessingCommands = 0;
    private readonly ConcurrentCommandProcessorConfiguration? configuration = configuration;
    private readonly ConcurrentQueue<ICommand> pendingCommands = new();
    private Task? processUntilQueueIsEmptyTask = null;
    private Task? lastProccessingCommandTask = null;

    internal int NumberOfProcessingCommands
    {
        get
        {
            lockerForMaxConcurrentCommands.EnterReadLock();

            try { return numberOfProcessingCommands; }
            finally { lockerForMaxConcurrentCommands.ExitReadLock(); }
        }
        set
        {
            lockerForMaxConcurrentCommands.EnterWriteLock();
            
            try { numberOfProcessingCommands = value; }
            finally { lockerForMaxConcurrentCommands.ExitWriteLock(); }
        }
    }

    public void ProcessCommand(ICommand command)
    {
        pendingCommands.Enqueue(command);

        locker.Enter();

        try
        {
            if (processUntilQueueIsEmptyTask is not null)
                return;

            processUntilQueueIsEmptyTask = Task.Run(ProcessUntilQueueIsEmpty);
        }
        finally
        {
            locker.Exit();
        }
    }

    private void ProcessUntilQueueIsEmpty()
    {
        if (configuration is null)
            throw new InvalidOperationException("Cannot process command without a configuration");

        while (!pendingCommands.IsEmpty)
        {
            if (!pendingCommands.TryDequeue(out ICommand? command))
                throw new InvalidOperationException("Cannot dequeue command");

            if (command is null)
                throw new InvalidOperationException("Cannot process a null command");

            if (command is not CommandBase commandBase)
                throw new InvalidOperationException("Cannot process a null command");

            if (NumberOfProcessingCommands < configuration.MaxNumberOfCommandsProcessedSimultaneosly)
            {
                NumberOfProcessingCommands++;

                lastProccessingCommandTask = Task.Run(async () => await RunCommandBaseAsync(commandBase, configuration));
            }
            else
            {
                if (lastProccessingCommandTask is null)
                    throw new InvalidOperationException("Cannot continue a null task");

                lastProccessingCommandTask = lastProccessingCommandTask.ContinueWith(async (Task task) => {
                    NumberOfProcessingCommands++;

                    await RunCommandBaseAsync(commandBase, configuration);
                });
            }
        }

        processUntilQueueIsEmptyTask = null;
    }

    private async Task RunCommandBaseAsync(CommandBase commandBase, ConcurrentCommandProcessorConfiguration configuration)
    {
        try
        {
            await commandBase.RunAsync(configuration);
        }
        finally
        {
            NumberOfProcessingCommands--;
        }
    }
}
