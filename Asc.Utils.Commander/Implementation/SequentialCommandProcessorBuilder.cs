namespace Asc.Utils.Commander.Implementation;

internal class SequentialCommandProcessorBuilder : ISequentialCommandProcessorBuilder
{
    private DefaultCommandDelegate? onBeforeJobDelegate = null;
    private DefaultExecutedCommandDelegate? onSuccessDelegate = null;
    private readonly List<DefaultExceptionCommandDelegate> onFailureDelegates = [];
    private DefaultExecutedCommandDelegate? onFinallyDelegate = null;

    public ISequentialCommandProcessorBuilder AddOnBeforeJobDelegate(Action<ICommand> onBeforeJob)
    {
        ArgumentNullException.ThrowIfNull(onBeforeJob, nameof(onBeforeJob));
        CommandProcessorBuilderValidator.ThrowIfThereIsAlreadyADelegateForThat(onBeforeJobDelegate);

        onBeforeJobDelegate = new DefaultCommandDelegate(onBeforeJob);

        return this;
    }

    public ISequentialCommandProcessorBuilder AddOnBeforeJobDelegate(Func<ICommand, Task> onBeforeJob)
    {
        ArgumentNullException.ThrowIfNull(onBeforeJob, nameof(onBeforeJob));
        CommandProcessorBuilderValidator.ThrowIfThereIsAlreadyADelegateForThat(onBeforeJobDelegate);

        onBeforeJobDelegate = new DefaultCommandDelegate(onBeforeJob);

        return this;
    }

    public ISequentialCommandProcessorBuilder AddOnSuccessDelegate(Action<IExecutedCommand> onSuccess)
    {
        ArgumentNullException.ThrowIfNull(onSuccess, nameof(onSuccess));
        CommandProcessorBuilderValidator.ThrowIfThereIsAlreadyADelegateForThat(onSuccessDelegate);

        onSuccessDelegate = new DefaultExecutedCommandDelegate(onSuccess);

        return this;
    }

    public ISequentialCommandProcessorBuilder AddOnSuccessDelegate(Func<IExecutedCommand, Task> onSuccess)
    {
        ArgumentNullException.ThrowIfNull(onSuccess, nameof(onSuccess));
        CommandProcessorBuilderValidator.ThrowIfThereIsAlreadyADelegateForThat(onSuccessDelegate);

        onSuccessDelegate = new DefaultExecutedCommandDelegate(onSuccess);

        return this;
    }

    public ISequentialCommandProcessorBuilder AddOnFailureDelegate<TException>(Action<TException, IExecutedCommand> onFailure) where TException : Exception
    {
        ArgumentNullException.ThrowIfNull(onFailure, nameof(onFailure));
        CommandProcessorBuilderValidator.ThrowIfThereIsAlreadyADelegateForThat<TException>(onFailureDelegates);

        onFailureDelegates.Add(new DefaultExceptionCommandDelegate<TException>(onFailure));

        return this;
    }

    public ISequentialCommandProcessorBuilder AddOnFailureDelegate<TException>(Func<TException, IExecutedCommand, Task> onFailure) where TException : Exception
    {
        ArgumentNullException.ThrowIfNull(onFailure, nameof(onFailure));
        CommandProcessorBuilderValidator.ThrowIfThereIsAlreadyADelegateForThat<TException>(onFailureDelegates);

        onFailureDelegates.Add(new DefaultExceptionCommandDelegate<TException>(onFailure));

        return this;
    }

    public ISequentialCommandProcessorBuilder AddOnFinallyDelegate(Action<IExecutedCommand> onFinally)
    {
        ArgumentNullException.ThrowIfNull(onFinally, nameof(onFinally));
        CommandProcessorBuilderValidator.ThrowIfThereIsAlreadyADelegateForThat(onFinallyDelegate);

        onFinallyDelegate = new DefaultExecutedCommandDelegate(onFinally);

        return this;
    }

    public ISequentialCommandProcessorBuilder AddOnFinallyDelegate(Func<IExecutedCommand, Task> onFinally)
    {
        ArgumentNullException.ThrowIfNull(onFinally, nameof(onFinally));
        CommandProcessorBuilderValidator.ThrowIfThereIsAlreadyADelegateForThat(onFinallyDelegate);

        onFinallyDelegate = new DefaultExecutedCommandDelegate(onFinally);

        return this;
    }

    public ICommandProcessor Build()
    {
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
