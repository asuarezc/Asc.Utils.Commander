
namespace Asc.Utils.Commander.Implementation;

internal class CommandBase(
    List<ExceptionCommandDelegate> onFailureDelegates,
    string id = "")
{
    public string Id { get; private set; } = id;

    public bool HasOnFailureDelegates => OnFailureDelegates is not null && OnFailureDelegates.Count > 0;

    internal List<ExceptionCommandDelegate> OnFailureDelegates { get; private set; } = onFailureDelegates;

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
}

internal class Command(
    CommandDelegate? jobDelegate,
    CommandDelegate? onSuccessDelegate,
    List<ExceptionCommandDelegate> onFailureDelegates,
    string id = "") : CommandBase(onFailureDelegates, id), ICommand
{
    public bool JobIsAsyncronous =>
        JobDelegate is not null
        && JobDelegate.AsyncronousDelegate is not null
        && JobDelegate.SyncronousDelegate is null;

    public bool HasOnSuccesDelegate => OnSuccessDelegate is not null;

    public bool HasOnSuccesAsyncronousDelegate =>
        OnSuccessDelegate is not null
        && OnSuccessDelegate.AsyncronousDelegate is not null
        && OnSuccessDelegate.SyncronousDelegate is null;

    public bool HasOnSuccesSyncronousDelegate =>
        OnSuccessDelegate is not null
        && OnSuccessDelegate.SyncronousDelegate is not null
        && OnSuccessDelegate.AsyncronousDelegate is null;

    internal CommandDelegate? JobDelegate { get; private set; } = jobDelegate;

    internal CommandDelegate? OnSuccessDelegate { get; private set; } = onSuccessDelegate;

    internal async Task RunAsync()
    {
        if (JobDelegate is null || !JobDelegate.CanExecute())
            return;

        bool success = true;

        try
        {
            await JobDelegate.ExecuteAsync();
        }
        catch (Exception ex)
        {
            success = false;
            await ManageExceptionAsync(ex);
        }

        if (success && OnSuccessDelegate is not null && OnSuccessDelegate.CanExecute())
            await OnSuccessDelegate.ExecuteAsync();
    }
}

internal class Command<TResult>(
    CommandJobDelegate<TResult>? jobDelegate,
    CommandOnSuccessDelegate<TResult>? onSuccessDelegate,
    List<ExceptionCommandDelegate> onFailureDelegates,
    string id = "") : CommandBase(onFailureDelegates, id), ICommand<TResult>
{
    public bool JobIsAsyncronous =>
        JobDelegate is not null
        && JobDelegate.AsyncronousDelegate is not null
        && JobDelegate.SyncronousDelegate is null;

    public bool HasOnSuccesDelegate => OnSuccessDelegate is not null;

    public bool HasOnSuccesAsyncronousDelegate =>
        OnSuccessDelegate is not null
        && OnSuccessDelegate.AsyncronousDelegate is not null
        && OnSuccessDelegate.SyncronousDelegate is null;

    public bool HasOnSuccesSyncronousDelegate =>
        OnSuccessDelegate is not null
        && OnSuccessDelegate.SyncronousDelegate is not null
        && OnSuccessDelegate.AsyncronousDelegate is null;

    internal CommandJobDelegate<TResult>? JobDelegate { get; private set; } = jobDelegate;

    internal CommandOnSuccessDelegate<TResult>? OnSuccessDelegate { get; private set; } = onSuccessDelegate;

    internal async Task RunAsync()
    {
        if (JobDelegate is null || !JobDelegate.CanExecute())
            return;

        bool success = true;
        TResult? result = default;

        try
        {
            result = await JobDelegate.ExecuteAsync();
        }
        catch (Exception ex)
        {
            success = false;
            await ManageExceptionAsync(ex);
        }

        if (success && OnSuccessDelegate is not null && OnSuccessDelegate.CanExecute() && result is not null)
            await OnSuccessDelegate.ExecuteAsync(result);
    }
}
