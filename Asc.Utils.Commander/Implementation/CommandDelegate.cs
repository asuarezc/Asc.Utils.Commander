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
    internal Type ExceptionType { get; set; }

    internal Action<Exception>? BaseSyncronousDelegate { get; private set; }

    internal Func<Exception, Task>? BaseAsyncronousDelegate { get; private set; }

    public ExceptionCommandDelegate(Type exceptionType, Action<Exception> baseSyncronousDelegate)
    {
        ExceptionType = exceptionType;
        BaseSyncronousDelegate = baseSyncronousDelegate;
    }

    public ExceptionCommandDelegate(Type exceptionType, Func<Exception, Task> baseAsyncronousDelegate)
    {
        ExceptionType = exceptionType;
        BaseAsyncronousDelegate = baseAsyncronousDelegate;
    }

    internal bool CanExecute() => BaseSyncronousDelegate is not null || BaseAsyncronousDelegate is not null;

    internal async Task ExecuteAsync(Exception ex)
    {
        if (BaseAsyncronousDelegate is not null)
        {
            await BaseAsyncronousDelegate(ex);
            return;
        }

        if (BaseSyncronousDelegate is not null)
        {
            await Task.Run(() => BaseSyncronousDelegate(ex));
            return;
        }
    }
}

internal class ExceptionCommandDelegate<TException> : ExceptionCommandDelegate where TException : Exception
{
    internal Action<TException>? SyncronousDelegate { get; private set; }

    internal Func<TException, Task>? AsyncronousDelegate { get; private set; }

    internal ExceptionCommandDelegate(Action<TException> syncronousDelegate) : base(typeof(TException), (Action<Exception>)syncronousDelegate)
    {
        SyncronousDelegate = syncronousDelegate;
    }

    internal ExceptionCommandDelegate(Func<TException, Task> asyncronousDelegate) : base(typeof(TException), (Func<Exception, Task>)asyncronousDelegate)
    {
        AsyncronousDelegate = asyncronousDelegate;
    }
}