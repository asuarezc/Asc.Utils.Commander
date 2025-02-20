namespace Asc.Utils.Commander;

public interface ICommandProcessor
{
    CommandExecutionMode ExecutionMode { get; }

    event EventHandler<bool> IsRunningChanged;

    bool IsRunning { get; }

    void ProcessCommand(ICommand command);
}
