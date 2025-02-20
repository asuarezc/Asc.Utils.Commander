namespace Asc.Utils.Commander.Implementation;

internal class CommandDelegate
{
    internal Action? SyncronousDelegate { get; private set; }

    internal Func<Task>? AsyncronousDelegate { get; private set; }

    internal CommandDelegate(Action syncronousDelegate) => SyncronousDelegate = syncronousDelegate;

    internal CommandDelegate(Func<Task> asyncronousDelegate) => AsyncronousDelegate = asyncronousDelegate;

    internal bool CanExecute() => AsyncronousDelegate is not null || SyncronousDelegate is not null;

    internal async Task ExecuteAsync()
    {
        if (AsyncronousDelegate is not null)
        {
            await AsyncronousDelegate();
            return;
        }

        if (SyncronousDelegate is not null)
            await Task.Run(SyncronousDelegate);
    }
}

internal class CommandJobDelegate<TResult>
{
    internal Func<TResult>? SyncronousDelegate { get; private set; }

    internal Func<Task<TResult>>? AsyncronousDelegate { get; private set; }

    internal CommandJobDelegate(Func<TResult> syncronousDelegate) => SyncronousDelegate = syncronousDelegate;

    internal CommandJobDelegate(Func<Task<TResult>> asyncronousDelegate) => AsyncronousDelegate = asyncronousDelegate;

    internal bool CanExecute() => AsyncronousDelegate is not null || SyncronousDelegate is not null;

    internal async Task<TResult> ExecuteAsync()
    {
        TResult? result = default;

        if (AsyncronousDelegate is not null)
            return await AsyncronousDelegate();

        if (SyncronousDelegate is not null)
            await Task.Run(() => result = SyncronousDelegate());

        if (result is null)
            throw new InvalidOperationException("Delegate returned null");

        return result;
    }
}

internal class CommandOnSuccessDelegate<TResult>
{
    internal Action<TResult>? SyncronousDelegate { get; private set; }

    internal Func<TResult, Task>? AsyncronousDelegate { get; private set; }

    internal CommandOnSuccessDelegate(Action<TResult> syncronousDelegate) => SyncronousDelegate = syncronousDelegate;

    internal CommandOnSuccessDelegate(Func<TResult, Task> asyncronousDelegate) => AsyncronousDelegate = asyncronousDelegate;

    internal bool CanExecute() => AsyncronousDelegate is not null || SyncronousDelegate is not null;

    internal async Task ExecuteAsync(TResult result)
    {
        if (AsyncronousDelegate is not null)
        {
            await AsyncronousDelegate(result);
            return;
        }

        if (SyncronousDelegate is not null)
        {
            await Task.Run(() => SyncronousDelegate(result));
            return;
        }
    }
}

internal abstract class ExceptionCommandDelegate
{
    internal Type? ExceptionType { get; set; }

    internal virtual Task RunAsync(Exception ex) { throw new InvalidOperationException("Use derived type"); }

    internal virtual bool CanExecute() => false;
}

internal class ExceptionCommandDelegate<TException> : ExceptionCommandDelegate where TException : Exception
{
    internal Action<TException>? SyncronousDelegate { get; private set; }

    internal Func<TException, Task>? AsyncronousDelegate { get; private set; }

    internal override bool CanExecute() => AsyncronousDelegate is not null || SyncronousDelegate is not null;

    private ExceptionCommandDelegate()
    {
        ExceptionType = typeof(TException);
    }

    internal ExceptionCommandDelegate(Action<TException> syncronousDelegate) : this()
    {
        SyncronousDelegate = syncronousDelegate;
    }

    internal ExceptionCommandDelegate(Func<TException, Task> asyncronousDelegate) : this()
    {
        AsyncronousDelegate = asyncronousDelegate;
    }

    internal override async Task RunAsync(Exception ex)
    {
        if (ex is null)
            return;

        await ExecuteAsync((TException)ex);
    }

    internal async Task ExecuteAsync(TException ex)
    {
        if (AsyncronousDelegate is not null)
        {
            await AsyncronousDelegate(ex);
            return;
        }

        if (SyncronousDelegate is not null)
        {
            await Task.Run(() => SyncronousDelegate(ex));
            return;
        }
    }
}