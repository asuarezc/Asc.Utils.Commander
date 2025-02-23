namespace Asc.Utils.Commander;

public interface ICommand
{
    string Id { get; }

    IReadOnlyDictionary<string, string> Parameters { get; }
}

public interface IExecutedCommand : ICommand
{
    TimeSpan JobElapsedTime { get; }

    ExecutedCommandResult CommandResult { get; }
}