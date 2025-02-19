namespace Asc.Utils.Commander;

public interface ICommandProcessor
{
    CommandExecutionMode ExecutionMode { get; }

    bool IsRunning { get; }

    void ProcessCommand(ICommand command);

    void ProcessCommand<TResult>(ICommand<TResult> command);
}
