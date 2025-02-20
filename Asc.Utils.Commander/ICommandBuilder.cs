namespace Asc.Utils.Commander;

public interface ICommandBuilder
{
    ICommandBuilder Job(Action job);

    ICommandBuilder Job(Func<Task> job);

    ICommandBuilder OnSuccess(Action onSuccess);

    ICommandBuilder OnSuccess(Func<Task> onSuccess);

    ICommandBuilder OnFailure<TException>(Action<TException> onFailure) where TException : Exception;

    ICommandBuilder OnFailure<TException>(Func<TException, Task> onFailure) where TException : Exception;

    ICommandBuilder OnFinally(Action onFinally);

    ICommandBuilder OnFinally(Func<Task> onFinally);

    ICommandBuilder SetId(string id);

    ICommand Build();
}

public interface ICommandBuilder<TResult>
{
    ICommandBuilder<TResult> Job(Func<TResult> job);

    ICommandBuilder<TResult> Job(Func<Task<TResult>> job);

    ICommandBuilder<TResult> OnSuccess(Action<TResult> onSuccess);

    ICommandBuilder<TResult> OnSuccess(Func<TResult, Task> onSuccess);

    ICommandBuilder<TResult> OnFailure<TException>(Action<TException> onFailure) where TException : Exception;

    ICommandBuilder<TResult> OnFailure<TException>(Func<TException, Task> onFailure) where TException : Exception;

    ICommandBuilder<TResult> OnFinally(Action onFinally);

    ICommandBuilder<TResult> OnFinally(Func<Task> onFinally);

    ICommandBuilder<TResult> SetId(string id);

    ICommand Build();
} 