using System.Collections.Concurrent;

namespace Asc.Utils.Commander.Implementation;

internal class ConcurrentCommandProcessor(ConcurrentCommandProcessorConfiguration configuration)
    : ICommandProcessor
{
    private static readonly Lock locker = new();
    private static readonly ReaderWriterLockSlim lockerForMaxConcurrentCommands = new(LockRecursionPolicy.NoRecursion);

    private int numberOfProcessingCommands;
    private readonly ConcurrentCommandProcessorConfiguration? configuration = configuration;
    private readonly ConcurrentQueue<ICommand> pendingCommands = new();
    private Task? processUntilQueueIsEmptyTask;
    private Task? lastProccessingCommandTask;

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

            if (NumberOfProcessingCommands < configuration.MaxThreads)
            {
                NumberOfProcessingCommands++;

                lastProccessingCommandTask = Task.Run(async () => await RunCommandBaseAsync(commandBase, configuration).ConfigureAwait(false));
            }
            else
            {
                if (lastProccessingCommandTask is null)
                    throw new InvalidOperationException("Cannot continue a null task");

                lastProccessingCommandTask = lastProccessingCommandTask.ContinueWith(async _ => {
                    NumberOfProcessingCommands++;

                    await RunCommandBaseAsync(commandBase, configuration).ConfigureAwait(false);
                });
            }
        }

        processUntilQueueIsEmptyTask = null;
    }

    private async Task RunCommandBaseAsync(CommandBase commandBase, ConcurrentCommandProcessorConfiguration processorConfiguration)
    {
        try
        {
            await commandBase.RunAsync(processorConfiguration);
        }
        finally
        {
            NumberOfProcessingCommands--;
        }
    }
}
