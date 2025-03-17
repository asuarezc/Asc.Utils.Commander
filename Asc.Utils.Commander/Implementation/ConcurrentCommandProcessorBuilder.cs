namespace Asc.Utils.Commander.Implementation;

internal class ConcurrentCommandProcessorBuilder : IConcurrentCommandProcessorBuilder
{
    private DefaultCommandDelegate? onBeforeJobDelegate;
    private DefaultExecutedCommandDelegate? onSuccessDelegate;
    private readonly List<DefaultExceptionCommandDelegate> onFailureDelegates = [];
    private DefaultExecutedCommandDelegate? onFinallyDelegate;
    private int? maxNumberOfCommandsProcessedSimultaneosly;

    public IConcurrentCommandProcessorBuilder SetMaxThreads(
        int maxThreads)
    {
        if (maxThreads <= 0)
            throw new ArgumentException("Value must be greater than 0", nameof(maxThreads));

        CommandProcessorBuilderValidator.ThrowIfThereIsAlreadyADelegateForThat(
            this.maxNumberOfCommandsProcessedSimultaneosly
        );

        this.maxNumberOfCommandsProcessedSimultaneosly = maxThreads;

        return this;
    }

    public IConcurrentCommandProcessorBuilder OnBeforeAnyJob(Action<ICommand> onBeforeJob)
    {
        ArgumentNullException.ThrowIfNull(onBeforeJob, nameof(onBeforeJob));
        CommandProcessorBuilderValidator.ThrowIfThereIsAlreadyADelegateForThat(onBeforeJobDelegate);

        onBeforeJobDelegate = new DefaultCommandDelegate(onBeforeJob);

        return this;
    }

    public IConcurrentCommandProcessorBuilder OnBeforeAnyJob(Func<ICommand, Task> onBeforeJob)
    {
        ArgumentNullException.ThrowIfNull(onBeforeJob, nameof(onBeforeJob));
        CommandProcessorBuilderValidator.ThrowIfThereIsAlreadyADelegateForThat(onBeforeJobDelegate);

        onBeforeJobDelegate = new DefaultCommandDelegate(onBeforeJob);

        return this;
    }

    public IConcurrentCommandProcessorBuilder OnAnyJobSuccess(Action<IExecutedCommand> onSuccess)
    {
        ArgumentNullException.ThrowIfNull(onSuccess, nameof(onSuccess));
        CommandProcessorBuilderValidator.ThrowIfThereIsAlreadyADelegateForThat(onSuccessDelegate);

        onSuccessDelegate = new DefaultExecutedCommandDelegate(onSuccess);

        return this;
    }

    public IConcurrentCommandProcessorBuilder OnAnyJobSuccess(Func<IExecutedCommand, Task> onSuccess)
    {
        ArgumentNullException.ThrowIfNull(onSuccess, nameof(onSuccess));
        CommandProcessorBuilderValidator.ThrowIfThereIsAlreadyADelegateForThat(onSuccessDelegate);

        onSuccessDelegate = new DefaultExecutedCommandDelegate(onSuccess);

        return this;
    }

    public IConcurrentCommandProcessorBuilder OnAnyJobFailure<TException>(Action<TException, IExecutedCommand> onFailure) where TException : Exception
    {
        ArgumentNullException.ThrowIfNull(onFailure, nameof(onFailure));
        CommandProcessorBuilderValidator.ThrowIfThereIsAlreadyADelegateForThat<TException>(onFailureDelegates);

        onFailureDelegates.Add(new DefaultExceptionCommandDelegate<TException>(onFailure));

        return this;
    }

    public IConcurrentCommandProcessorBuilder OnAnyJobFailure<TException>(Func<TException, IExecutedCommand, Task> onFailure) where TException : Exception
    {
        ArgumentNullException.ThrowIfNull(onFailure, nameof(onFailure));
        CommandProcessorBuilderValidator.ThrowIfThereIsAlreadyADelegateForThat<TException>(onFailureDelegates);

        onFailureDelegates.Add(new DefaultExceptionCommandDelegate<TException>(onFailure));

        return this;
    }

    public IConcurrentCommandProcessorBuilder OnAfterAnyJob(Action<IExecutedCommand> onFinally)
    {
        ArgumentNullException.ThrowIfNull(onFinally, nameof(onFinally));
        CommandProcessorBuilderValidator.ThrowIfThereIsAlreadyADelegateForThat(onFinallyDelegate);

        onFinallyDelegate = new DefaultExecutedCommandDelegate(onFinally);

        return this;
    }

    public IConcurrentCommandProcessorBuilder OnAfterAnyJob(Func<IExecutedCommand, Task> onFinally)
    {
        ArgumentNullException.ThrowIfNull(onFinally, nameof(onFinally));
        CommandProcessorBuilderValidator.ThrowIfThereIsAlreadyADelegateForThat(onFinallyDelegate);

        onFinallyDelegate = new DefaultExecutedCommandDelegate(onFinally);

        return this;
    }

    public ICommandProcessor Build()
    {
        if (onFailureDelegates is null
            || onFailureDelegates.Count == 0
            || !onFailureDelegates.Any(it => it.ExceptionType is not null && it.ExceptionType == typeof(Exception)))
        {
            throw new InvalidOperationException("An on failure delegate for Exception instances is mandatory");
        }

        ConcurrentCommandProcessorConfiguration processorConfiguration = new()
        {
            OnBeforeJobDelegate = onBeforeJobDelegate,
            OnSuccessDelegate = onSuccessDelegate,
            OnFailureDelegates = onFailureDelegates,
            OnFinallyDelegate = onFinallyDelegate,
            MaxThreads = maxNumberOfCommandsProcessedSimultaneosly ?? Environment.ProcessorCount,
        };

        return new ConcurrentCommandProcessor(processorConfiguration);
    }
}
