namespace Asc.Utils.Commander;

public interface ICommandProcessorBuilder
{
    ICommandProcessor Build();
}

public interface ISequentialCommandProcessorBuilder : ICommandProcessorBuilder
{
    ISequentialCommandProcessorBuilder OnBeforeAnyJob(Action<ICommand> onBeforeJob);

    ISequentialCommandProcessorBuilder OnBeforeAnyJob(Func<ICommand, Task> onBeforeJob);

    ISequentialCommandProcessorBuilder OnAnyJobSuccess(Action<IExecutedCommand> onSuccess);

    ISequentialCommandProcessorBuilder OnAnyJobSuccess(Func<IExecutedCommand, Task> onSuccess);

    ISequentialCommandProcessorBuilder OnAnyJobFailure<TException>(Action<TException, IExecutedCommand> onFailure)
        where TException : Exception;

    ISequentialCommandProcessorBuilder OnAnyJobFailure<TException>(Func<TException, IExecutedCommand, Task> onFailure)
        where TException : Exception;

    ISequentialCommandProcessorBuilder OnAfterAnyJob(Action<IExecutedCommand> onAfterJob);

    ISequentialCommandProcessorBuilder OnAfterAnyJob(Func<IExecutedCommand, Task> onAfterJob);
}

public interface IConcurrentCommandProcessorBuilder : ICommandProcessorBuilder
{
    IConcurrentCommandProcessorBuilder SetMaxNumberOfCommandsProcessedSimultaneosly(
        int maxNumberOfCommandsProcessedSimultaneosly);

    IConcurrentCommandProcessorBuilder OnBeforeAnyJob(Action<ICommand> onBeforeJob);

    IConcurrentCommandProcessorBuilder OnBeforeAnyJob(Func<ICommand, Task> onBeforeJob);

    IConcurrentCommandProcessorBuilder OnAnyJobSuccess(Action<IExecutedCommand> onSuccess);

    IConcurrentCommandProcessorBuilder OnAnyJobSuccess(Func<IExecutedCommand, Task> onSuccess);

    IConcurrentCommandProcessorBuilder OnAnyJobFailure<TException>(Action<TException, IExecutedCommand> onFailure)
        where TException : Exception;

    IConcurrentCommandProcessorBuilder OnAnyJobFailure<TException>(Func<TException, IExecutedCommand, Task> onFailure)
        where TException : Exception;

    IConcurrentCommandProcessorBuilder OnAfterAnyJob(Action<IExecutedCommand> onAfterJob);

    IConcurrentCommandProcessorBuilder OnAfterAnyJob(Func<IExecutedCommand, Task> onAfterJob);
}