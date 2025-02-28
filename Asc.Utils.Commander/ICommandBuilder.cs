namespace Asc.Utils.Commander;

/// <summary>
/// A builder to construc a <see cref="ICommand"/> instance
/// </summary>
public interface ICommandBuilder
{
    /// <summary>
    /// Sets the command job action with a syncronous delegate
    /// </summary>
    ICommandBuilder Job(Action job);

    /// <summary>
    /// Sets the command job action with an asyncronous delegate
    /// </summary>
    ICommandBuilder Job(Func<Task> job);

    /// <summary>
    /// Sets the command on succes action with a syncronous delegate
    /// </summary>
    ICommandBuilder OnSuccess(Action onSuccess);

    /// <summary>
    /// Sets the command on succes action with an asyncronous delegate
    /// </summary>
    ICommandBuilder OnSuccess(Func<Task> onSuccess);

    /// <summary>
    /// Adds a sincronous delegate to perform on failure operations when exception is of <typeparamref name="TException"/> type
    /// </summary>
    ICommandBuilder OnFailure<TException>(Action<TException> onFailure) where TException : Exception; 

    /// <summary>
    /// Adds an asincronous delegate to perform on failure operations when exception is of <typeparamref name="TException"/> type
    /// </summary>
    ICommandBuilder OnFailure<TException>(Func<TException, Task> onFailure) where TException : Exception;

    /// <summary>
    /// Sets a sincronous delegate to perform on finally operations
    /// </summary>
    ICommandBuilder OnFinally(Action onFinally);

    /// <summary>
    /// Sets an asincronous delegate to perform on finally operations
    /// </summary>
    ICommandBuilder OnFinally(Func<Task> onFinally);

    /// <summary>
    /// Adds a paramater of <typeparamref name="T"/> type with a certain <paramref name="key"/> and <paramref name="value"/>
    /// If there is already a param with same key, it updates its value and type
    /// </summary>
    ICommandBuilder AddOrReplaceParameter<T>(string key, T value);

    /// <summary>
    /// Sets command commandId
    /// </summary>
    ICommandBuilder SetId(string commandId);

    /// <summary>
    /// Creates a <see cref="ICommand"/> instance from this <see cref="ICommandBuilder"/>
    /// </summary>
    ICommand Build();
}

/// <summary>
/// A builder to construc a <see cref="ICommand"/> generic instance.
/// Generic type <typeparamref name="TResult"/> is meant to use when a job returns data than OnSuccess delegate needs
/// </summary>
public interface ICommandBuilder<TResult>
{
    /// <summary>
    /// Sets the command job action with a syncronous delegate than returns data of <typeparamref name="TResult"/> type
    /// </summary>
    ICommandBuilder<TResult> Job(Func<TResult> job);

    /// <summary>
    /// Sets the command job action with an asyncronous delegate than returns data of <typeparamref name="TResult"/> type
    /// </summary>
    ICommandBuilder<TResult> Job(Func<Task<TResult>> job);

    /// <summary>
    /// Sets the command on succes action with a syncronous delegate than needs data of <typeparamref name="TResult"/> type
    /// </summary>
    ICommandBuilder<TResult> OnSuccess(Action<TResult> onSuccess);

    /// <summary>
    /// Sets the command on succes action with an asyncronous delegate than needs data of <typeparamref name="TResult"/> type
    /// </summary>
    ICommandBuilder<TResult> OnSuccess(Func<TResult, Task> onSuccess);

    /// <summary>
    /// Adds a sincronous delegate to perform on failure operations when exception is of <typeparamref name="TException"/> type
    /// </summary>
    ICommandBuilder<TResult> OnFailure<TException>(Action<TException> onFailure) where TException : Exception;

    /// <summary>
    /// Adds an asincronous delegate to perform on failure operations when exception is of <typeparamref name="TException"/> type
    /// </summary>
    ICommandBuilder<TResult> OnFailure<TException>(Func<TException, Task> onFailure) where TException : Exception;

    /// <summary>
    /// Sets a sincronous delegate to perform on finally operations
    /// </summary>
    ICommandBuilder<TResult> OnFinally(Action onFinally);

    /// <summary>
    /// Sets an asincronous delegate to perform on finally operations
    /// </summary>
    ICommandBuilder<TResult> OnFinally(Func<Task> onFinally);

    /// <summary>
    /// Adds a paramater of <typeparamref name="T"/> type with a certain <paramref name="key"/> and <paramref name="value"/>
    /// If there is already a param with same key, it updates its value and type
    /// </summary>
    ICommandBuilder<TResult> AddOrReplaceParameter<T>(string key, T value);

    /// <summary>
    /// Sets command commandId
    /// </summary>
    ICommandBuilder<TResult> SetId(string commandId);

    /// <summary>
    /// Creates a <see cref="ICommand"/> generic instance from this <see cref="ICommandBuilder"/> of <typeparamref name="TResult"/>
    /// </summary>
    ICommand Build();
} 