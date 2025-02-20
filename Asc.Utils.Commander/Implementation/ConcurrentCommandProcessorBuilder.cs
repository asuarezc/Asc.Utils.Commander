namespace Asc.Utils.Commander.Implementation;

internal class ConcurrentCommandProcessorBuilder : IConcurrentCommandProcessorBuilder
{
    private DefaultCommandDelegate? onBeforeJobDelegate = null;
    private DefaultExecutedCommandDelegate? onSuccessDelegate = null;
    private readonly List<DefaultExceptionCommandDelegate> onFailureDelegates = [];
    private DefaultExecutedCommandDelegate? onFinallyDelegate = null;
    private int? maxThreads = null;

    public IConcurrentCommandProcessorBuilder SetMaxThreads(int maxThreads)
    {
        if (maxThreads <= 0 || maxThreads > Environment.ProcessorCount)
            throw new ArgumentException($"Value must be between 1 and {Environment.ProcessorCount}", nameof(maxThreads));

        CommandProcessorBuilderValidator.ThrowIfThereIsAlreadyADelegateForThat(this.maxThreads);
        this.maxThreads = maxThreads;

        return this;
    }

    public IConcurrentCommandProcessorBuilder AddOnBeforeJobDelegate(Action<ICommand> onBeforeJob)
    {
        ArgumentNullException.ThrowIfNull(onBeforeJob, nameof(onBeforeJob));
        CommandProcessorBuilderValidator.ThrowIfThereIsAlreadyADelegateForThat(onBeforeJobDelegate);

        onBeforeJobDelegate = new DefaultCommandDelegate(onBeforeJob);

        return this;
    }

    public IConcurrentCommandProcessorBuilder AddOnBeforeJobDelegate(Func<ICommand, Task> onBeforeJob)
    {
        ArgumentNullException.ThrowIfNull(onBeforeJob, nameof(onBeforeJob));
        CommandProcessorBuilderValidator.ThrowIfThereIsAlreadyADelegateForThat(onBeforeJobDelegate);

        onBeforeJobDelegate = new DefaultCommandDelegate(onBeforeJob);

        return this;
    }

    public IConcurrentCommandProcessorBuilder AddOnSuccessDelegate(Action<IExecutedCommand> onSuccess)
    {
        ArgumentNullException.ThrowIfNull(onSuccess, nameof(onSuccess));
        CommandProcessorBuilderValidator.ThrowIfThereIsAlreadyADelegateForThat(onSuccessDelegate);

        onSuccessDelegate = new DefaultExecutedCommandDelegate(onSuccess);

        return this;
    }

    public IConcurrentCommandProcessorBuilder AddOnSuccessDelegate(Func<IExecutedCommand, Task> onSuccess)
    {
        ArgumentNullException.ThrowIfNull(onSuccess, nameof(onSuccess));
        CommandProcessorBuilderValidator.ThrowIfThereIsAlreadyADelegateForThat(onSuccessDelegate);

        onSuccessDelegate = new DefaultExecutedCommandDelegate(onSuccess);

        return this;
    }

    public IConcurrentCommandProcessorBuilder AddOnFailureDelegate<TException>(Action<TException, IExecutedCommand> onFailure) where TException : Exception
    {
        ArgumentNullException.ThrowIfNull(onFailure, nameof(onFailure));
        CommandProcessorBuilderValidator.ThrowIfThereIsAlreadyADelegateForThat<TException>(onFailureDelegates);

        onFailureDelegates.Add(new DefaultExceptionCommandDelegate<TException>(onFailure));

        return this;
    }

    public IConcurrentCommandProcessorBuilder AddOnFailureDelegate<TException>(Func<TException, IExecutedCommand, Task> onFailure) where TException : Exception
    {
        ArgumentNullException.ThrowIfNull(onFailure, nameof(onFailure));
        CommandProcessorBuilderValidator.ThrowIfThereIsAlreadyADelegateForThat<TException>(onFailureDelegates);

        onFailureDelegates.Add(new DefaultExceptionCommandDelegate<TException>(onFailure));

        return this;
    }

    public IConcurrentCommandProcessorBuilder AddOnFinallyDelegate(Action<IExecutedCommand> onFinally)
    {
        ArgumentNullException.ThrowIfNull(onFinally, nameof(onFinally));
        CommandProcessorBuilderValidator.ThrowIfThereIsAlreadyADelegateForThat(onFinallyDelegate);

        onFinallyDelegate = new DefaultExecutedCommandDelegate(onFinally);

        return this;
    }

    public IConcurrentCommandProcessorBuilder AddOnFinallyDelegate(Func<IExecutedCommand, Task> onFinally)
    {
        ArgumentNullException.ThrowIfNull(onFinally, nameof(onFinally));
        CommandProcessorBuilderValidator.ThrowIfThereIsAlreadyADelegateForThat(onFinallyDelegate);

        onFinallyDelegate = new DefaultExecutedCommandDelegate(onFinally);

        return this;
    }

    public ICommandProcessor Build()
    {
        ConcurrentCommandProcessorConfiguration processorConfiguration = new()
        {
            OnBeforeJobDelegate = onBeforeJobDelegate,
            OnSuccessDelegate = onSuccessDelegate,
            OnFailureDelegates = onFailureDelegates,
            OnFinallyDelegate = onFinallyDelegate,
            MaxThreads = maxThreads,
        };

        throw new NotImplementedException();
    }
}
