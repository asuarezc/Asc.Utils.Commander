namespace Asc.Utils.Commander.Implementation;

internal class CommandBuilder : ICommandBuilder
{
    private string? id;
    private CommandDelegate? jobDelegate;
    private CommandDelegate? onSuccessDelegate;
    private CommandDelegate? onFinallyDelegate;
    private readonly List<ExceptionCommandDelegate> onFailureDelegates = [];
    private readonly Dictionary<string, ICommandParameter> commandParameters = [];

    public ICommandBuilder Job(Action job)
    {
        ArgumentNullException.ThrowIfNull(job, nameof(job));
        CommandBuilderValidator.ThrowIfThereIsAlreadyADelegateForThat(jobDelegate);

        jobDelegate = new CommandDelegate(job);

        return this;
    }

    public ICommandBuilder Job(Func<Task> job)
    {
        ArgumentNullException.ThrowIfNull(job, nameof(job));
        CommandBuilderValidator.ThrowIfThereIsAlreadyADelegateForThat(jobDelegate);

        jobDelegate = new CommandDelegate(job);

        return this;
    }

    public ICommandBuilder OnSuccess(Action onSuccess)
    {
        ArgumentNullException.ThrowIfNull(onSuccess, nameof(onSuccess));
        CommandBuilderValidator.ThrowIfThereIsAlreadyADelegateForThat(onSuccessDelegate);

        onSuccessDelegate = new CommandDelegate(onSuccess);

        return this;
    }

    public ICommandBuilder OnSuccess(Func<Task> onSuccess)
    {
        ArgumentNullException.ThrowIfNull(onSuccess, nameof(onSuccess));
        CommandBuilderValidator.ThrowIfThereIsAlreadyADelegateForThat(onSuccessDelegate);

        onSuccessDelegate = new CommandDelegate(onSuccess);

        return this;
    }

    public ICommandBuilder OnFailure<TException>(Action<TException> onFailure) where TException : Exception
    {
        ArgumentNullException.ThrowIfNull(onFailure, nameof(onFailure));
        CommandBuilderValidator.ThrowIfThereIsAlreadyADelegateForThat<TException>(onFailureDelegates);

        onFailureDelegates.Add(new ExceptionCommandDelegate<TException>(onFailure));

        return this;
    }

    public ICommandBuilder OnFailure<TException>(Func<TException, Task> onFailure) where TException : Exception
    {
        ArgumentNullException.ThrowIfNull(onFailure, nameof(onFailure));
        CommandBuilderValidator.ThrowIfThereIsAlreadyADelegateForThat<TException>(onFailureDelegates);

        onFailureDelegates.Add(new ExceptionCommandDelegate<TException>(onFailure));

        return this;
    }

    public ICommandBuilder OnFinally(Action onFinally)
    {
        ArgumentNullException.ThrowIfNull(onFinally, nameof(onFinally));
        CommandBuilderValidator.ThrowIfThereIsAlreadyADelegateForThat(onFinallyDelegate);

        onFinallyDelegate = new CommandDelegate(onFinally);

        return this;
    }

    public ICommandBuilder OnFinally(Func<Task> onFinally)
    {
        ArgumentNullException.ThrowIfNull(onFinally, nameof(onFinally));
        CommandBuilderValidator.ThrowIfThereIsAlreadyADelegateForThat(onFinallyDelegate);

        onFinallyDelegate = new CommandDelegate(onFinally);

        return this;
    }

    public ICommandBuilder AddOrReplaceParameter<T>(string key, T value)
    {
        if (string.IsNullOrEmpty(key))
            throw new ArgumentNullException(nameof(key));

        CommandParameter parameter = new CommandParameter<T>(value);

        if (!commandParameters.TryAdd(key, parameter))
            commandParameters[key] = parameter;

        return this;
    }

    public ICommandBuilder SetId(string commandId)
    {
        if (string.IsNullOrEmpty(commandId))
            throw new ArgumentNullException(nameof(commandId));

        if (!string.IsNullOrEmpty(this.id))
            throw new InvalidOperationException("Command already have an Id");

        this.id = commandId;

        return this;
    }

    public ICommand Build()
    {
        if (jobDelegate is null)
            throw new InvalidOperationException("Job delegate is mandatory");

        if (string.IsNullOrEmpty(id))
            throw new InvalidOperationException("Id is mandatory");

        return new Command(jobDelegate, onSuccessDelegate, onFailureDelegates, onFinallyDelegate, id, commandParameters);
    }
}

internal class CommandBuilder<TResult> : ICommandBuilder<TResult>
{
    private string? id;
    private CommandJobDelegate<TResult>? jobDelegate;
    private CommandOnSuccessDelegate<TResult>? onSuccessDelegate;
    private CommandDelegate? onFinallyDelegate;
    private readonly List<ExceptionCommandDelegate> onFailureDelegates = [];
    private readonly Dictionary<string, ICommandParameter> commandParameters = [];

    public ICommandBuilder<TResult> Job(Func<TResult> job)
    {
        ArgumentNullException.ThrowIfNull(job, nameof(job));
        CommandBuilderValidator.ThrowIfThereIsAlreadyADelegateForThat(jobDelegate);

        jobDelegate = new CommandJobDelegate<TResult>(job);

        return this;
    }

    public ICommandBuilder<TResult> Job(Func<Task<TResult>> job)
    {
        ArgumentNullException.ThrowIfNull(job, nameof(job));
        CommandBuilderValidator.ThrowIfThereIsAlreadyADelegateForThat(jobDelegate);

        jobDelegate = new CommandJobDelegate<TResult>(job);

        return this;
    }

    public ICommandBuilder<TResult> OnSuccess(Action<TResult> onSuccess)
    {
        ArgumentNullException.ThrowIfNull(onSuccess, nameof(onSuccess));
        CommandBuilderValidator.ThrowIfThereIsAlreadyADelegateForThat(onSuccessDelegate);

        onSuccessDelegate = new CommandOnSuccessDelegate<TResult>(onSuccess);

        return this;
    }

    public ICommandBuilder<TResult> OnSuccess(Func<TResult, Task> onSuccess)
    {
        ArgumentNullException.ThrowIfNull(onSuccess, nameof(onSuccess));
        CommandBuilderValidator.ThrowIfThereIsAlreadyADelegateForThat(onSuccessDelegate);

        onSuccessDelegate = new CommandOnSuccessDelegate<TResult>(onSuccess);

        return this;
    }

    public ICommandBuilder<TResult> OnFailure<TException>(Action<TException> onFailure) where TException : Exception
    {
        ArgumentNullException.ThrowIfNull(onFailure, nameof(onFailure));
        CommandBuilderValidator.ThrowIfThereIsAlreadyADelegateForThat<TException>(onFailureDelegates);

        onFailureDelegates.Add(new ExceptionCommandDelegate<TException>(onFailure));

        return this;
    }

    public ICommandBuilder<TResult> OnFailure<TException>(Func<TException, Task> onFailure) where TException : Exception
    {
        ArgumentNullException.ThrowIfNull(onFailure, nameof(onFailure));
        CommandBuilderValidator.ThrowIfThereIsAlreadyADelegateForThat<TException>(onFailureDelegates);

        onFailureDelegates.Add(new ExceptionCommandDelegate<TException>(onFailure));

        return this;
    }

    public ICommandBuilder<TResult> OnFinally(Action onFinally)
    {
        ArgumentNullException.ThrowIfNull(onFinally, nameof(onFinally));
        CommandBuilderValidator.ThrowIfThereIsAlreadyADelegateForThat(onFinallyDelegate);

        onFinallyDelegate = new CommandDelegate(onFinally);

        return this;
    }

    public ICommandBuilder<TResult> OnFinally(Func<Task> onFinally)
    {
        ArgumentNullException.ThrowIfNull(onFinally, nameof(onFinally));
        CommandBuilderValidator.ThrowIfThereIsAlreadyADelegateForThat(onFinallyDelegate);

        onFinallyDelegate = new CommandDelegate(onFinally);

        return this;
    }

    public ICommandBuilder<TResult> AddOrReplaceParameter<T>(string key, T value)
    {
        if (string.IsNullOrEmpty(key))
            throw new ArgumentNullException(nameof(key));

        CommandParameter parameter = new CommandParameter<T>(value);

        if (!commandParameters.TryAdd(key, parameter))
            commandParameters[key] = parameter;

        return this;
    }

    public ICommandBuilder<TResult> SetId(string commandId)
    {
        if (string.IsNullOrEmpty(commandId))
            throw new ArgumentNullException(nameof(commandId));

        if (!string.IsNullOrEmpty(this.id))
            throw new InvalidOperationException("Command already have an Id");

        this.id = commandId;

        return this;
    }

    public ICommand Build()
    {
        if (jobDelegate is null)
            throw new InvalidOperationException("Job delegate is mandatory");

        if (onSuccessDelegate is null)
            throw new InvalidOperationException("On success delegate is mandatory");

        if (string.IsNullOrEmpty(id))
            throw new InvalidOperationException("Id is mandatory");

        return new Command<TResult>(jobDelegate, onSuccessDelegate, onFailureDelegates, onFinallyDelegate, id, commandParameters);
    }
}