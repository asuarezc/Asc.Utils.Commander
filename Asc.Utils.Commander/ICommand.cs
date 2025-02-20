namespace Asc.Utils.Commander;

public interface ICommand
{
    string Id { get; }
}

public interface IExecutedCommand : ICommand
{
    TimeSpan JobElapsedTime { get; }

    ExecutedCommandResult CommandResult { get; }
}