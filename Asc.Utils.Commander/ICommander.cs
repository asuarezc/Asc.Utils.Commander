namespace Asc.Utils.Commander;

/// <summary>
/// Primary singleton instance to obtain <see cref="ICommand"/>
/// and <see cref="ICommandProcessor"/> builders
/// </summary>
public interface ICommander
{
    /// <summary>
    /// Gets an instance of <see cref="ICommandBuilder"/>, an instance that uses builder pattern
    /// to construct an instance of <see cref="ICommand"/>, which represents a command
    /// that can be processed by an <see cref="ICommandProcessor"/>
    /// </summary>
    ICommandBuilder GetCommandBuilder();

    /// <summary>
    /// Gets an instance of <see cref="ICommandBuilder"/>, an instance that uses builder pattern
    /// to construct an instance of <see cref="ICommand"/>, which represents a command,
    /// whose job returns a <typeparamref name="TResult"/> instance,
    /// that can be processed by an <see cref="ICommandProcessor"/>
    /// </summary>
    /// <typeparam name="TResult">Type returned by Job</typeparam>
    ICommandBuilder<TResult> GetCommandBuilder<TResult>();

    /// <summary>
    /// Get an instance of <see cref="ISequentialCommandProcessorBuilder"/>, an instance that uses
    /// builder pattern to construct an instance of <see cref="ICommandProcessor"/>, an instance used to
    /// process <see cref="ICommand"/> instances. Commands are processed one at a time
    /// </summary>
    ISequentialCommandProcessorBuilder GetSequentialCommandProcessorBuilder();

    /// <summary>
    /// Get an instance of <see cref="IConcurrentCommandProcessorBuilder"/>, an instance that uses
    /// builder pattern to construct an instance of <see cref="ICommandProcessor"/>, an instance used to
    /// process <see cref="ICommand"/> instances. Commands are processed simultaneously
    /// </summary>
    IConcurrentCommandProcessorBuilder GetConcurrentCommandProcessorBuilder();
}
