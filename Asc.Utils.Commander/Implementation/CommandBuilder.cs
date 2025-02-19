namespace Asc.Utils.Commander.Implementation;

internal class CommandBuilder : ICommandBuilder
{
    private string? id;
    private CommandDelegate? jobDelegate = null;
    private CommandDelegate? onSuccessDelegate = null;
    private readonly List<ExceptionCommandDelegate> onFailureDelegates = [];

    public ICommand Build()
    {
        if (jobDelegate is null)
            throw new InvalidOperationException("Job delegate is mandatory");

        if (string.IsNullOrEmpty(id))
            return new Command(jobDelegate, onSuccessDelegate, onFailureDelegates);

        return new Command(jobDelegate, onSuccessDelegate, onFailureDelegates, id);
    }

    public ICommandBuilder Job(Action job)
    {
        ArgumentNullException.ThrowIfNull(job, nameof(job));
        CommandBuilderValidator.ThrowIfThereIsAlreadyAJobDelegate(jobDelegate);

        jobDelegate = new CommandDelegate(job);

        return this;
    }

    public ICommandBuilder Job(Func<Task> job)
    {
        ArgumentNullException.ThrowIfNull(job, nameof(job));
        CommandBuilderValidator.ThrowIfThereIsAlreadyAJobDelegate(jobDelegate);

        jobDelegate = new CommandDelegate(job);

        return this;
    }

    public ICommandBuilder OnFailure<TException>(Action<TException> onFailure) where TException : Exception
    {
        ArgumentNullException.ThrowIfNull(onFailure, nameof(onFailure));
        CommandBuilderValidator.ThrowIfThereIsAlreadyAnOnFailureDelegateWithSameExceptionType<TException>(onFailureDelegates);

        onFailureDelegates.Add(new ExceptionCommandDelegate<TException>(onFailure));

        return this;
    }

    public ICommandBuilder OnFailure<TException>(Func<TException, Task> onFailure) where TException : Exception
    {
        ArgumentNullException.ThrowIfNull(onFailure, nameof(onFailure));
        CommandBuilderValidator.ThrowIfThereIsAlreadyAnOnFailureDelegateWithSameExceptionType<TException>(onFailureDelegates);

        onFailureDelegates.Add(new ExceptionCommandDelegate<TException>(onFailure));

        return this;
    }

    public ICommandBuilder OnSuccess(Action onSuccess)
    {
        ArgumentNullException.ThrowIfNull(onSuccess, nameof(onSuccess));
        CommandBuilderValidator.ThrowIfThereIsAlreadyAnOnSuccessDelegate(onSuccessDelegate);

        onSuccessDelegate = new CommandDelegate(onSuccess);

        return this;
    }

    public ICommandBuilder OnSuccess(Func<Task> onSuccess)
    {
        ArgumentNullException.ThrowIfNull(onSuccess, nameof(onSuccess));
        CommandBuilderValidator.ThrowIfThereIsAlreadyAnOnSuccessDelegate(onSuccessDelegate);

        onSuccessDelegate = new CommandDelegate(onSuccess);

        return this;
    }

    public ICommandBuilder SetId(string id)
    {
        if (string.IsNullOrEmpty(id))
            throw new ArgumentNullException(nameof(id));

        this.id = id;

        return this;
    }
}

internal class CommandBuilder<TResult> : ICommandBuilder<TResult>
{
    private string? id;
    private CommandJobDelegate<TResult>? jobDelegate = null;
    private CommandOnSuccessDelegate<TResult>? onSuccessDelegate = null;
    private readonly List<ExceptionCommandDelegate> onFailureDelegates = [];

    public ICommand<TResult> Build()
    {
        if (jobDelegate is null)
            throw new InvalidOperationException("Job delegate is mandatory");

        if (string.IsNullOrEmpty(id))
            return new Command<TResult>(jobDelegate, onSuccessDelegate, onFailureDelegates);

        return new Command<TResult>(jobDelegate, onSuccessDelegate, onFailureDelegates, id);
    }

    public ICommandBuilder<TResult> Job(Func<TResult> job)
    {
        ArgumentNullException.ThrowIfNull(job, nameof(job));
        CommandBuilderValidator.ThrowIfThereIsAlreadyAJobDelegate(jobDelegate);

        jobDelegate = new CommandJobDelegate<TResult>(job);

        return this;
    }

    public ICommandBuilder<TResult> Job(Func<Task<TResult>> job)
    {
        ArgumentNullException.ThrowIfNull(job, nameof(job));
        CommandBuilderValidator.ThrowIfThereIsAlreadyAJobDelegate(jobDelegate);

        jobDelegate = new CommandJobDelegate<TResult>(job);

        return this;
    }

    public ICommandBuilder<TResult> OnFailure<TException>(Action<TException> onFailure) where TException : Exception
    {
        ArgumentNullException.ThrowIfNull(onFailure, nameof(onFailure));
        CommandBuilderValidator.ThrowIfThereIsAlreadyAnOnFailureDelegateWithSameExceptionType<TException>(onFailureDelegates);

        onFailureDelegates.Add(new ExceptionCommandDelegate<TException>(onFailure));

        return this;
    }

    public ICommandBuilder<TResult> OnFailure<TException>(Func<TException, Task> onFailure) where TException : Exception
    {
        ArgumentNullException.ThrowIfNull(onFailure, nameof(onFailure));
        CommandBuilderValidator.ThrowIfThereIsAlreadyAnOnFailureDelegateWithSameExceptionType<TException>(onFailureDelegates);

        onFailureDelegates.Add(new ExceptionCommandDelegate<TException>(onFailure));

        return this;
    }

    public ICommandBuilder<TResult> OnSuccess(Action<TResult> onSuccess)
    {
        ArgumentNullException.ThrowIfNull(onSuccess, nameof(onSuccess));
        CommandBuilderValidator.ThrowIfThereIsAlreadyAnOnSuccessDelegate(onSuccessDelegate);

        onSuccessDelegate = new CommandOnSuccessDelegate<TResult>(onSuccess);

        return this;
    }

    public ICommandBuilder<TResult> OnSuccess(Func<TResult, Task> onSuccess)
    {
        ArgumentNullException.ThrowIfNull(onSuccess, nameof(onSuccess));
        CommandBuilderValidator.ThrowIfThereIsAlreadyAnOnSuccessDelegate(onSuccessDelegate);

        onSuccessDelegate = new CommandOnSuccessDelegate<TResult>(onSuccess);

        return this;
    }

    public ICommandBuilder<TResult> SetId(string id)
    {
        if (string.IsNullOrEmpty(id))
            throw new ArgumentNullException(nameof(id));

        this.id = id;

        return this;
    }
}