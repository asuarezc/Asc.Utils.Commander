namespace Asc.Utils.Commander;

public interface ICommandProcessorBuilder
{
    ICommandProcessor Build();
}

public interface ISequentialCommandProcessorBuilder : ICommandProcessorBuilder
{
    ISequentialCommandProcessorBuilder AddOnBeforeJobDelegate(Action<ICommand> onBeforeJob);

    ISequentialCommandProcessorBuilder AddOnBeforeJobDelegate(Func<ICommand, Task> onBeforeJob);

    ISequentialCommandProcessorBuilder AddOnSuccessDelegate(Action<IExecutedCommand> onSuccess);

    ISequentialCommandProcessorBuilder AddOnSuccessDelegate(Func<IExecutedCommand, Task> onSuccess);

    ISequentialCommandProcessorBuilder AddOnFailureDelegate<TException>(Action<TException, IExecutedCommand> onFailure) where TException : Exception;

    ISequentialCommandProcessorBuilder AddOnFailureDelegate<TException>(Func<TException, IExecutedCommand, Task> onFailure) where TException : Exception;

    ISequentialCommandProcessorBuilder AddOnFinallyDelegate(Action<IExecutedCommand> onFinally);

    ISequentialCommandProcessorBuilder AddOnFinallyDelegate(Func<IExecutedCommand, Task> onFinally);
}

public interface IConcurrentCommandProcessorBuilder : ICommandProcessorBuilder
{
    IConcurrentCommandProcessorBuilder SetMaxThreads(int maxThreads);

    IConcurrentCommandProcessorBuilder AddOnBeforeJobDelegate(Action<ICommand> onBeforeJob);

    IConcurrentCommandProcessorBuilder AddOnBeforeJobDelegate(Func<ICommand, Task> onBeforeJob);

    IConcurrentCommandProcessorBuilder AddOnSuccessDelegate(Action<IExecutedCommand> onSuccess);

    IConcurrentCommandProcessorBuilder AddOnSuccessDelegate(Func<IExecutedCommand, Task> onSuccess);

    IConcurrentCommandProcessorBuilder AddOnFailureDelegate<TException>(Action<TException, IExecutedCommand> onFailure) where TException : Exception;

    IConcurrentCommandProcessorBuilder AddOnFailureDelegate<TException>(Func<TException, IExecutedCommand, Task> onFailure) where TException : Exception;

    IConcurrentCommandProcessorBuilder AddOnFinallyDelegate(Action<IExecutedCommand> onFinally);

    IConcurrentCommandProcessorBuilder AddOnFinallyDelegate(Func<IExecutedCommand, Task> onFinally);
}