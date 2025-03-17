namespace Asc.Utils.Commander;

/// <summary>
/// A command that can be processed by a <see cref="ICommandProcessor"/>
/// </summary>
public interface ICommand
{
    /// <summary>
    /// Command unique id
    /// </summary>
    string Id { get; }

    /// <summary>
    /// Command parameters
    /// </summary>
    IReadOnlyDictionary<string, ICommandParameter> Parameters { get; }
}

/// <summary>
/// A command whose job has already been executed
/// </summary>
public interface IExecutedCommand : ICommand
{
    /// <summary>
    /// Job execution elapsed time
    /// </summary>
    TimeSpan JobElapsedTime { get; }

    /// <summary>
    /// Command result: succeeded of failed
    /// </summary>
    ExecutedCommandResult CommandResult { get; }
}