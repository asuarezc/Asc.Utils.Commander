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

            processUntilQueueIsEmptyTask = Task.Run(ProcessUntilQueueIsEmptyAsync);
        }
        finally
        {
            locker.Exit();
        }
    }

    private async Task ProcessUntilQueueIsEmptyAsync()
    {
        if (configuration is null)
            throw new InvalidOperationException("Cannot process command without a configuration");

        while (!pendingCommands.IsEmpty && NumberOfProcessingCommands < configuration.MaxNumberOfCommandsProcessedSimultaneosly)
        {
            if (!pendingCommands.TryDequeue(out ICommand? command))
                throw new InvalidOperationException("Cannot dequeue command");

            if (command is null)
                throw new InvalidOperationException("Cannot process a null command");

            if (command is not CommandBase commandBase)
                throw new InvalidOperationException("Cannot process a null command");

            _ = Task.Run(async () =>
            {
                NumberOfProcessingCommands++;

                try
                {
                    await commandBase.RunAsync(configuration);
                }
                finally
                {
                    NumberOfProcessingCommands--;
                }
            });
        }

        if (!pendingCommands.IsEmpty)
            await ProcessUntilQueueIsEmptyAsync();

        processUntilQueueIsEmptyTask = null;
    }
}
