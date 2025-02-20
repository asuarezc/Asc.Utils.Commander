using System.Collections.Concurrent;

namespace Asc.Utils.Commander.Implementation;

internal class SequentialCommandProcessor : ICommandProcessor
{
    private readonly CommandProcessorConfiguration? configuration = null;
    private readonly ConcurrentQueue<ICommand> pendingCommands = new();
    private Task? proccessFirstCommandTask = null;

    public CommandExecutionMode ExecutionMode => CommandExecutionMode.Sequential;

    public bool IsRunning { get; private set; } = false;

    public event EventHandler<bool>? IsRunningChanged;

    internal SequentialCommandProcessor(CommandProcessorConfiguration configuration)
    {
        this.configuration = configuration;
    }

    public void ProcessCommand(ICommand command)
    {
        pendingCommands.Enqueue(command);

        if (proccessFirstCommandTask is null || proccessFirstCommandTask.IsCompleted)
            proccessFirstCommandTask = Task.Run(ProccessFirstCommand);
    }

    private async Task ProccessFirstCommand()
    {
        if (configuration is null)
            throw new InvalidOperationException("Cannot process command without a configuration");

        if (!pendingCommands.TryDequeue(out ICommand? command))
            throw new InvalidOperationException("Cannot dequeue command");

        if (command is null)
            throw new InvalidOperationException("Cannot process a null command");

        if (command is not CommandBase commandBase)
            throw new InvalidOperationException("Cannot process a null command");

        IsRunning = true;
        IsRunningChanged?.Invoke(this, true);

        try
        {
            await commandBase.RunAsync(configuration);
        }
        finally
        {
            IsRunning = false;
            IsRunningChanged?.Invoke(this, false);
        }

        if (!pendingCommands.IsEmpty)
            await ProccessFirstCommand();
    }
}
