using Asc.Utils.Commander.Implementation;

namespace Asc.Utils.Commander;

public interface ICommand
{
    string Id { get; }

    IReadOnlyDictionary<string, ICommandParameter> Parameters { get; }
}

public interface IExecutedCommand : ICommand
{
    TimeSpan JobElapsedTime { get; }

    ExecutedCommandResult CommandResult { get; }
}