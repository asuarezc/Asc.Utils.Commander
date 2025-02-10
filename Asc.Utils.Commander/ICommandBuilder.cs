namespace Asc.Utils.Commander;

public interface ICommandBuilderBase
{
    ICommandBuilder OnSpecificFailure<TException>(Action<TException> onFailure) where TException : Exception;

    ICommandBuilder OnSpecificFailure<TException>(Func<TException, Task> onSuccess) where TException : Exception;

    ICommandBuilder OnFailure(Action<Exception> onFailure);

    ICommandBuilder OnFailure(Func<Exception, Task> onFailure);

    ICommand Build();
}

public interface ICommandBuilder : ICommandBuilderBase
{
    ICommandBuilder Job(Action job);

    ICommandBuilder Job(Func<Task> job);

    ICommandBuilder OnSuccess(Action onSuccess);

    ICommandBuilder OnSuccess(Func<Task> onSuccess);
}

public interface ICommandBuilder<TResult> : ICommandBuilderBase
{
    ICommandBuilder Job(Func<TResult> job);

    ICommandBuilder Job(Func<Task<TResult>> job);

    ICommandBuilder OnSuccess(Action<TResult> onSuccess);

    ICommandBuilder OnSuccess(Func<TResult, Task> onSuccess);
}