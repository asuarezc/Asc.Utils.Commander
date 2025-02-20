using System.Diagnostics;

namespace Asc.Utils.Commander.Implementation;

internal abstract class CommandBase(
    List<ExceptionCommandDelegate> onFailureDelegates,
    CommandDelegate? onFinallyDelegate,
    string id = "")
{
    public string Id { get; private set; } = id;

    public bool HasOnFailureDelegates => OnFailureDelegates is not null && OnFailureDelegates.Count > 0;

    internal List<ExceptionCommandDelegate> OnFailureDelegates { get; private set; } = onFailureDelegates;

    internal CommandDelegate? OnFinallyDelegate { get; private set; } = onFinallyDelegate;

    protected async Task ManageExceptionAsync(Exception ex)
    {
        if (OnFailureDelegates is null || OnFailureDelegates.Count == 0)
            return;

        Type exType = ex.GetType();

        IEnumerable<ExceptionCommandDelegate>? onTypedFailures = OnFailureDelegates.Where(it => it.ExceptionType.Equals(exType));

        if (onTypedFailures is null || !onTypedFailures.Any())
            return;

        foreach (var onTypedFailure in onTypedFailures.Where(it => it.CanExecute()))
            await onTypedFailure.ExecuteAsync(ex);
    }

    protected async Task ManageExceptionAsync(Exception ex, List<DefaultExceptionCommandDelegate> delegates, TimeSpan jobExecutionTime)
    {
        if (delegates is null || delegates.Count == 0)
            return;

        Type exType = ex.GetType();

        IEnumerable<DefaultExceptionCommandDelegate>? onTypedFailures = delegates.Where(it => it.ExceptionType.Equals(exType));

        if (onTypedFailures is null || !onTypedFailures.Any())
            return;

        ExecutedCommand executedCommand = new(jobExecutionTime, ExecutedCommandResult.Failed, Id);

        foreach (var onTypedFailure in onTypedFailures.Where(it => it.CanExecute()))
            await onTypedFailure.ExecuteAsync(ex, executedCommand);
    }

    internal virtual Task RunAsync(CommandProcessorConfiguration configuration)
    {
        throw new InvalidOperationException("Use a derived type");
    }
}

internal class Command(
    CommandDelegate? jobDelegate,
    CommandDelegate? onSuccessDelegate,
    List<ExceptionCommandDelegate> onFailureDelegates,
    CommandDelegate? onFinallyDelegate,
    string id = "") : CommandBase(onFailureDelegates, onFinallyDelegate, id), ICommand
{
    internal CommandDelegate? JobDelegate { get; private set; } = jobDelegate;

    internal CommandDelegate? OnSuccessDelegate { get; private set; } = onSuccessDelegate;

    internal override async Task RunAsync(CommandProcessorConfiguration configuration)
    {
        if (JobDelegate is null || !JobDelegate.CanExecute())
            return;

        bool success = true;

        if (configuration.OnBeforeJobDelegate is not null && configuration.OnBeforeJobDelegate.CanExecute())
            await configuration.OnBeforeJobDelegate.ExecuteAsync(this);

        Stopwatch stopwatch = Stopwatch.StartNew();

        try
        {
            await JobDelegate.ExecuteAsync();
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            success = false;

            if (configuration.OnFailureDelegates is not null)
                await ManageExceptionAsync(ex, configuration.OnFailureDelegates, stopwatch.Elapsed);

            await ManageExceptionAsync(ex);
        }
        finally
        {
            if (stopwatch.IsRunning)
                stopwatch.Stop();

            ExecutedCommand executedCommand = new(stopwatch.Elapsed, ExecutedCommandResult.Succeeded, Id);

            if (success)
            {
                if (configuration.OnSuccessDelegate is not null && configuration.OnSuccessDelegate.CanExecute())
                    await configuration.OnSuccessDelegate.ExecuteAsync(executedCommand);

                if (OnSuccessDelegate is not null && OnSuccessDelegate.CanExecute())
                    await OnSuccessDelegate.ExecuteAsync();
            }

            if (configuration.OnFinallyDelegate is not null && configuration.OnFinallyDelegate.CanExecute())
                await configuration.OnFinallyDelegate.ExecuteAsync(executedCommand);

            if (OnFinallyDelegate is not null && OnFinallyDelegate.CanExecute())
                await OnFinallyDelegate.ExecuteAsync();
        }
    }
}

internal class Command<TResult>(
    CommandJobDelegate<TResult>? jobDelegate,
    CommandOnSuccessDelegate<TResult>? onSuccessDelegate,
    List<ExceptionCommandDelegate> onFailureDelegates,
    CommandDelegate? onFinallyDelegate,
    string id = "") : CommandBase(onFailureDelegates, onFinallyDelegate, id), ICommand
{
    internal CommandJobDelegate<TResult>? JobDelegate { get; private set; } = jobDelegate;

    internal CommandOnSuccessDelegate<TResult>? OnSuccessDelegate { get; private set; } = onSuccessDelegate;

    internal override async Task RunAsync(CommandProcessorConfiguration configuration)
    {
        if (JobDelegate is null || !JobDelegate.CanExecute())
            return;

        bool success = true;
        TResult? result = default;

        if (configuration.OnBeforeJobDelegate is not null && configuration.OnBeforeJobDelegate.CanExecute())
            await configuration.OnBeforeJobDelegate.ExecuteAsync(this);

        Stopwatch stopwatch = Stopwatch.StartNew();

        try
        {
            result = await JobDelegate.ExecuteAsync();
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            success = false;

            if (configuration.OnFailureDelegates is not null)
                await ManageExceptionAsync(ex, configuration.OnFailureDelegates, stopwatch.Elapsed);

            await ManageExceptionAsync(ex);
        }
        finally
        {
            if (stopwatch.IsRunning)
                stopwatch.Stop();

            ExecutedCommand executedCommand = new(stopwatch.Elapsed, ExecutedCommandResult.Succeeded, Id);

            if (success)
            {
                if (configuration.OnSuccessDelegate is not null && configuration.OnSuccessDelegate.CanExecute())
                    await configuration.OnSuccessDelegate.ExecuteAsync(executedCommand);

                if (OnSuccessDelegate is not null && OnSuccessDelegate.CanExecute() && result is not null)
                    await OnSuccessDelegate.ExecuteAsync(result);
            }

            if (configuration.OnFinallyDelegate is not null && configuration.OnFinallyDelegate.CanExecute())
                await configuration.OnFinallyDelegate.ExecuteAsync(executedCommand);

            if (OnFinallyDelegate is not null && OnFinallyDelegate.CanExecute())
                await OnFinallyDelegate.ExecuteAsync();
        }
    }
}

internal class ExecutedCommand : IExecutedCommand
{
    private string? id;

    public ExecutedCommand(
        TimeSpan jobElapsedTime,
        ExecutedCommandResult commandResult,
        string id)
    {
        JobElapsedTime = jobElapsedTime;
        CommandResult = commandResult;
        Id = id;
    }

    public TimeSpan JobElapsedTime { get; private set; }

    public ExecutedCommandResult CommandResult { get; private set; }

    public string Id { get => id is not null ? id : string.Empty; private set => id = value; }
}
