using System.Diagnostics;

namespace Asc.Utils.Commander.Implementation;

internal abstract class CommandBase(
    List<ExceptionCommandDelegate> onFailureDelegates,
    CommandDelegate? onFinallyDelegate,
    string id,
    Dictionary<string, string>? commandParameters = null)
{
    public string Id { get; private set; } = id;

    public bool HasOnFailureDelegates => OnFailureDelegates is not null && OnFailureDelegates.Count > 0;

    public IReadOnlyDictionary<string, string> Parameters => commandParameters.AsReadOnly();

    internal Dictionary<string, string> commandParameters = commandParameters ?? [];

    internal List<ExceptionCommandDelegate> OnFailureDelegates { get; private set; } = onFailureDelegates;

    internal CommandDelegate? OnFinallyDelegate { get; private set; } = onFinallyDelegate;

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
    string id,
    Dictionary<string, string>? commandParameters = null)
    : CommandBase(onFailureDelegates, onFinallyDelegate, id, commandParameters), ICommand
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

            await ExceptionManager.ManageExceptionAsync(
                exception: ex,
                delegates: OnFailureDelegates,
                defaultDelegates: configuration.OnFailureDelegates,
                jobElapsedTime: stopwatch.Elapsed,
                Id
            );
        }
        finally
        {
            if (stopwatch.IsRunning)
                stopwatch.Stop();

            ExecutedCommand executedCommand = new(
                stopwatch.Elapsed,
                success ? ExecutedCommandResult.Succeeded : ExecutedCommandResult.Failed,
                Id,
                commandParameters
            );

            if (success)
            {
                if (OnSuccessDelegate is not null && OnSuccessDelegate.CanExecute())
                    await OnSuccessDelegate.ExecuteAsync();

                if (configuration.OnSuccessDelegate is not null && configuration.OnSuccessDelegate.CanExecute())
                    await configuration.OnSuccessDelegate.ExecuteAsync(executedCommand);
            }

            if (OnFinallyDelegate is not null && OnFinallyDelegate.CanExecute())
                await OnFinallyDelegate.ExecuteAsync();

            if (configuration.OnFinallyDelegate is not null && configuration.OnFinallyDelegate.CanExecute())
                await configuration.OnFinallyDelegate.ExecuteAsync(executedCommand);
        }
    }
}

internal class Command<TResult>(
    CommandJobDelegate<TResult>? jobDelegate,
    CommandOnSuccessDelegate<TResult>? onSuccessDelegate,
    List<ExceptionCommandDelegate> onFailureDelegates,
    CommandDelegate? onFinallyDelegate,
    string id,
    Dictionary<string, string>? commandParameters = null)
    : CommandBase(onFailureDelegates, onFinallyDelegate, id, commandParameters), ICommand
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

            await ExceptionManager.ManageExceptionAsync(
                exception: ex,
                delegates: OnFailureDelegates,
                defaultDelegates: configuration.OnFailureDelegates,
                jobElapsedTime: stopwatch.Elapsed,
                id: Id
            );
        }
        finally
        {
            if (stopwatch.IsRunning)
                stopwatch.Stop();

            ExecutedCommand executedCommand = new(
                stopwatch.Elapsed,
                success ? ExecutedCommandResult.Succeeded : ExecutedCommandResult.Failed,
                Id,
                commandParameters
            );

            if (success)
            {
                if (OnSuccessDelegate is not null && OnSuccessDelegate.CanExecute() && result is not null)
                    await OnSuccessDelegate.ExecuteAsync(result);

                if (configuration.OnSuccessDelegate is not null && configuration.OnSuccessDelegate.CanExecute())
                    await configuration.OnSuccessDelegate.ExecuteAsync(executedCommand);
            }

            if (OnFinallyDelegate is not null && OnFinallyDelegate.CanExecute())
                await OnFinallyDelegate.ExecuteAsync();

            if (configuration.OnFinallyDelegate is not null && configuration.OnFinallyDelegate.CanExecute())
                await configuration.OnFinallyDelegate.ExecuteAsync(executedCommand);
        }
    }
}

internal class ExecutedCommand : IExecutedCommand
{
    private string? id;

    public ExecutedCommand(
        TimeSpan jobElapsedTime,
        ExecutedCommandResult commandResult,
        string id,
        Dictionary<string, string>? commandParameters = null)
    {
        JobElapsedTime = jobElapsedTime;
        CommandResult = commandResult;
        Id = id;
        this.commandParameters = commandParameters ?? [];
    }

    internal Dictionary<string, string> commandParameters;

    public IReadOnlyDictionary<string, string> Parameters => commandParameters.AsReadOnly();

    public TimeSpan JobElapsedTime { get; private set; }

    public ExecutedCommandResult CommandResult { get; private set; }

    public string Id { get => id is not null ? id : string.Empty; private set => id = value; }
}