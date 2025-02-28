namespace Asc.Utils.Commander.Implementation;

internal class SequentialCommandProcessorBuilder : ISequentialCommandProcessorBuilder
{
    private DefaultCommandDelegate? onBeforeJobDelegate;
    private DefaultExecutedCommandDelegate? onSuccessDelegate;
    private readonly List<DefaultExceptionCommandDelegate> onFailureDelegates = [];
    private DefaultExecutedCommandDelegate? onFinallyDelegate;

    public ISequentialCommandProcessorBuilder OnBeforeAnyJob(Action<ICommand> onBeforeJob)
    {
        ArgumentNullException.ThrowIfNull(onBeforeJob, nameof(onBeforeJob));
        CommandProcessorBuilderValidator.ThrowIfThereIsAlreadyADelegateForThat(onBeforeJobDelegate);

        onBeforeJobDelegate = new DefaultCommandDelegate(onBeforeJob);

        return this;
    }

    public ISequentialCommandProcessorBuilder OnBeforeAnyJob(Func<ICommand, Task> onBeforeJob)
    {
        ArgumentNullException.ThrowIfNull(onBeforeJob, nameof(onBeforeJob));
        CommandProcessorBuilderValidator.ThrowIfThereIsAlreadyADelegateForThat(onBeforeJobDelegate);

        onBeforeJobDelegate = new DefaultCommandDelegate(onBeforeJob);

        return this;
    }

    public ISequentialCommandProcessorBuilder OnAnyJobSuccess(Action<IExecutedCommand> onSuccess)
    {
        ArgumentNullException.ThrowIfNull(onSuccess, nameof(onSuccess));
        CommandProcessorBuilderValidator.ThrowIfThereIsAlreadyADelegateForThat(onSuccessDelegate);

        onSuccessDelegate = new DefaultExecutedCommandDelegate(onSuccess);

        return this;
    }

    public ISequentialCommandProcessorBuilder OnAnyJobSuccess(Func<IExecutedCommand, Task> onSuccess)
    {
        ArgumentNullException.ThrowIfNull(onSuccess, nameof(onSuccess));
        CommandProcessorBuilderValidator.ThrowIfThereIsAlreadyADelegateForThat(onSuccessDelegate);

        onSuccessDelegate = new DefaultExecutedCommandDelegate(onSuccess);

        return this;
    }

    public ISequentialCommandProcessorBuilder OnAnyJobFailure<TException>(Action<TException, IExecutedCommand> onFailure)
        where TException : Exception
    {
        ArgumentNullException.ThrowIfNull(onFailure, nameof(onFailure));
        CommandProcessorBuilderValidator.ThrowIfThereIsAlreadyADelegateForThat<TException>(onFailureDelegates);

        onFailureDelegates.Add(new DefaultExceptionCommandDelegate<TException>(onFailure));

        return this;
    }

    public ISequentialCommandProcessorBuilder OnAnyJobFailure<TException>(Func<TException, IExecutedCommand, Task> onFailure)
        where TException : Exception
    {
        ArgumentNullException.ThrowIfNull(onFailure, nameof(onFailure));
        CommandProcessorBuilderValidator.ThrowIfThereIsAlreadyADelegateForThat<TException>(onFailureDelegates);

        onFailureDelegates.Add(new DefaultExceptionCommandDelegate<TException>(onFailure));

        return this;
    }

    public ISequentialCommandProcessorBuilder OnAfterAnyJob(Action<IExecutedCommand> onFinally)
    {
        ArgumentNullException.ThrowIfNull(onFinally, nameof(onFinally));
        CommandProcessorBuilderValidator.ThrowIfThereIsAlreadyADelegateForThat(onFinallyDelegate);

        onFinallyDelegate = new DefaultExecutedCommandDelegate(onFinally);

        return this;
    }

    public ISequentialCommandProcessorBuilder OnAfterAnyJob(Func<IExecutedCommand, Task> onFinally)
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

        CommandProcessorConfiguration processorConfiguration = new()
        {
            OnBeforeJobDelegate = onBeforeJobDelegate,
            OnSuccessDelegate = onSuccessDelegate,
            OnFailureDelegates = onFailureDelegates,
            OnFinallyDelegate = onFinallyDelegate
        };

        return new SequentialCommandProcessor(processorConfiguration);
    }
}
