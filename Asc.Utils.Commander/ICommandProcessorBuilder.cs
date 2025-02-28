namespace Asc.Utils.Commander;

/// <summary>
/// A builder to get a <see cref="ICommandProcessor"/> instance
/// </summary>
public interface ICommandProcessorBuilder
{
    ICommandProcessor Build();
}

/// <summary>
/// A builder to get a <see cref="ICommandProcessor"/> that processes commands sequentially
/// </summary>
public interface ISequentialCommandProcessorBuilder : ICommandProcessorBuilder
{
    /// <summary>
    /// Defines a syncronous delegate to be executed just before executing the job of a <see cref="ICommand"/>
    /// </summary>
    ISequentialCommandProcessorBuilder OnBeforeAnyJob(Action<ICommand> onBeforeJob);

    /// <summary>
    /// Defines an asyncronous delegate to be executed just before executing the job of a <see cref="ICommand"/>
    /// </summary>
    ISequentialCommandProcessorBuilder OnBeforeAnyJob(Func<ICommand, Task> onBeforeJob);

    /// <summary>
    /// Defines a syncronous delegate to be executed just after successful execution of a <see cref="ICommand"/> job
    /// </summary>
    ISequentialCommandProcessorBuilder OnAnyJobSuccess(Action<IExecutedCommand> onSuccess);

    /// <summary>
    /// Defines an asyncronous delegate to be executed just after successful execution of a <see cref="ICommand"/> job
    /// </summary>
    ISequentialCommandProcessorBuilder OnAnyJobSuccess(Func<IExecutedCommand, Task> onSuccess);

    /// <summary>
    /// Adds a syncronous delegate to perform on failure operations when any <see cref="ICommand"/> job throws an exception of <typeparamref name="TException"/> type
    /// </summary>
    ISequentialCommandProcessorBuilder OnAnyJobFailure<TException>(Action<TException, IExecutedCommand> onFailure)
        where TException : Exception;

    /// <summary>
    /// Adds an asyncronous delegate to perform on failure operations when any <see cref="ICommand"/> job throws an exception of <typeparamref name="TException"/> type
    /// </summary>
    ISequentialCommandProcessorBuilder OnAnyJobFailure<TException>(Func<TException, IExecutedCommand, Task> onFailure)
        where TException : Exception;

    /// <summary>
    /// Defines a syncronous delegate to be executed right after the execution of a <see cref="ICommand"/> job regardless of whether the result was successful or not
    /// </summary>
    ISequentialCommandProcessorBuilder OnAfterAnyJob(Action<IExecutedCommand> onAfterJob);

    /// <summary>
    /// Defines an asyncronous delegate to be executed right after the execution of a <see cref="ICommand"/> job regardless of whether the result was successful or not
    /// </summary>
    ISequentialCommandProcessorBuilder OnAfterAnyJob(Func<IExecutedCommand, Task> onAfterJob);
}

/// <summary>
/// A builder to get a <see cref="ICommandProcessor"/> that processes commands simultaneously
/// </summary>
public interface IConcurrentCommandProcessorBuilder : ICommandProcessorBuilder
{
    /// <summary>
    /// Sets the maximum number of commands that can be processed simultaneously
    /// </summary>
    IConcurrentCommandProcessorBuilder SetMaxThreads(
        int maxThreads);

    /// <summary>
    /// Defines a syncronous delegate to be executed just before executing the job of a <see cref="ICommand"/>
    /// </summary>
    IConcurrentCommandProcessorBuilder OnBeforeAnyJob(Action<ICommand> onBeforeJob);

    /// <summary>
    /// Defines an asyncronous delegate to be executed just before executing the job of a <see cref="ICommand"/>
    /// </summary>
    IConcurrentCommandProcessorBuilder OnBeforeAnyJob(Func<ICommand, Task> onBeforeJob);

    /// <summary>
    /// Defines a syncronous delegate to be executed just after successful execution of a <see cref="ICommand"/> job
    /// </summary>
    IConcurrentCommandProcessorBuilder OnAnyJobSuccess(Action<IExecutedCommand> onSuccess);

    /// <summary>
    /// Defines an asyncronous delegate to be executed just after successful execution of a <see cref="ICommand"/> job
    /// </summary>
    IConcurrentCommandProcessorBuilder OnAnyJobSuccess(Func<IExecutedCommand, Task> onSuccess);

    /// <summary>
    /// Adds a syncronous delegate to perform on failure operations when any <see cref="ICommand"/> job throws an exception of <typeparamref name="TException"/> type
    /// </summary>
    IConcurrentCommandProcessorBuilder OnAnyJobFailure<TException>(Action<TException, IExecutedCommand> onFailure)
        where TException : Exception;

    /// <summary>
    /// Adds an asyncronous delegate to perform on failure operations when any <see cref="ICommand"/> job throws an exception of <typeparamref name="TException"/> type
    /// </summary>
    IConcurrentCommandProcessorBuilder OnAnyJobFailure<TException>(Func<TException, IExecutedCommand, Task> onFailure)
        where TException : Exception;

    /// <summary>
    /// Defines a syncronous delegate to be executed right after the execution of a <see cref="ICommand"/> job regardless of whether the result was successful or not
    /// </summary>
    IConcurrentCommandProcessorBuilder OnAfterAnyJob(Action<IExecutedCommand> onAfterJob);

    /// <summary>
    /// Defines an asyncronous delegate to be executed right after the execution of a <see cref="ICommand"/> job regardless of whether the result was successful or not
    /// </summary>
    IConcurrentCommandProcessorBuilder OnAfterAnyJob(Func<IExecutedCommand, Task> onAfterJob);
}