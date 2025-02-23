namespace Asc.Utils.Commander.Implementation;

internal class CommandBuilder : ICommandBuilder
{
    private string? id;
    private CommandDelegate? jobDelegate = null;
    private CommandDelegate? onSuccessDelegate = null;
    private CommandDelegate? onFinallyDelegate = null;
    private readonly List<ExceptionCommandDelegate> onFailureDelegates = [];
    private readonly Dictionary<string, string> commandParameters = [];

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

    public ICommandBuilder AddOrReplaceParameter(string key, string value)
    {
        if (string.IsNullOrEmpty(key))
            throw new ArgumentNullException(nameof(key));

        if (!commandParameters.TryAdd(key, value))
            commandParameters[key] = value;

        return this;
    }

    public ICommandBuilder SetId(string id)
    {
        if (string.IsNullOrEmpty(id))
            throw new ArgumentNullException(nameof(id));

        if (!string.IsNullOrEmpty(this.id))
            throw new InvalidOperationException("Command already have an Id");

        this.id = id;

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
    private CommandJobDelegate<TResult>? jobDelegate = null;
    private CommandOnSuccessDelegate<TResult>? onSuccessDelegate = null;
    private CommandDelegate? onFinallyDelegate = null;
    private readonly List<ExceptionCommandDelegate> onFailureDelegates = [];
    private readonly Dictionary<string, string> commandParameters = [];

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

    public ICommandBuilder<TResult> AddOrReplaceParameter(string key, string value)
    {
        if (string.IsNullOrEmpty(key))
            throw new ArgumentNullException(nameof(key));

        if (!commandParameters.TryAdd(key, value))
            commandParameters[key] = value;

        return this;
    }

    public ICommandBuilder<TResult> SetId(string id)
    {
        if (string.IsNullOrEmpty(id))
            throw new ArgumentNullException(nameof(id));

        if (!string.IsNullOrEmpty(this.id))
            throw new InvalidOperationException("Command already have an Id");

        this.id = id;

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