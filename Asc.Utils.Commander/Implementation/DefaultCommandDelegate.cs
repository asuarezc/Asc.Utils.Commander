namespace Asc.Utils.Commander.Implementation;

internal class DefaultCommandDelegate
{
    internal Action<ICommand>? SyncronousDelegate { get; private set; }

    internal Func<ICommand, Task>? AsyncronousDelegate { get; private set; }

    internal bool CanExecute() => AsyncronousDelegate is not null || SyncronousDelegate is not null;

    internal DefaultCommandDelegate(Action<ICommand>? syncronousDelegate) => SyncronousDelegate = syncronousDelegate;

    internal DefaultCommandDelegate(Func<ICommand, Task> asyncronousDelegate) => AsyncronousDelegate = asyncronousDelegate;

    internal async Task ExecuteAsync(ICommand command)
    {
        if (AsyncronousDelegate is not null)
        {
            await AsyncronousDelegate(command);
            return;
        }

        if (SyncronousDelegate is not null)
        {
            await Task.Run(() => SyncronousDelegate(command));
            return;
        }
    }
}

internal class DefaultExecutedCommandDelegate
{
    internal Action<IExecutedCommand>? SyncronousDelegate { get; private set; }

    internal Func<IExecutedCommand, Task>? AsyncronousDelegate { get; private set; }

    internal bool CanExecute() => SyncronousDelegate is not null || AsyncronousDelegate is not null;

    internal DefaultExecutedCommandDelegate(Action<IExecutedCommand>? syncronousDelegate) => SyncronousDelegate = syncronousDelegate;

    internal DefaultExecutedCommandDelegate(Func<IExecutedCommand, Task>? asyncronousDelegate) => AsyncronousDelegate = asyncronousDelegate;

    internal async Task ExecuteAsync(IExecutedCommand command)
    {
        if (AsyncronousDelegate is not null)
        {
            await AsyncronousDelegate(command);
            return;
        }

        if (SyncronousDelegate is not null)
        {
            await Task.Run(() => SyncronousDelegate(command));
            return;
        }
    }
}

internal abstract class DefaultExceptionCommandDelegate
{
    internal Type ExceptionType { get; set; }

    internal Action<Exception, IExecutedCommand>? BaseSyncronousDelegate { get; private set; }

    internal Func<Exception, IExecutedCommand, Task>? BaseAsyncronousDelegate { get; private set; }

    public DefaultExceptionCommandDelegate(Type exceptionType, Action<Exception, IExecutedCommand> baseSyncronousDelegate)
    {
        ExceptionType = exceptionType;
        BaseSyncronousDelegate = baseSyncronousDelegate;
    }

    public DefaultExceptionCommandDelegate(Type exceptionType, Func<Exception, IExecutedCommand, Task> baseAsyncronousDelegate)
    {
        ExceptionType = exceptionType;
        BaseAsyncronousDelegate = baseAsyncronousDelegate;
    }

    internal bool CanExecute() => BaseSyncronousDelegate is not null || BaseAsyncronousDelegate is not null;

    internal async Task ExecuteAsync(Exception ex, IExecutedCommand command)
    {
        if (BaseAsyncronousDelegate is not null)
        {
            await BaseAsyncronousDelegate(ex, command);
            return;
        }

        if (BaseSyncronousDelegate is not null)
        {
            await Task.Run(() => BaseSyncronousDelegate(ex, command));
            return;
        }
    }
}

internal class DefaultExceptionCommandDelegate<TException> : DefaultExceptionCommandDelegate where TException : Exception
{
    internal Action<TException, IExecutedCommand>? SyncronousDelegate { get; private set; }

    internal Func<TException, IExecutedCommand, Task>? AsyncronousDelegate { get; private set; }

    internal DefaultExceptionCommandDelegate(Action<TException, IExecutedCommand> syncronousDelegate) : base(typeof(TException), (Action<Exception, IExecutedCommand>)syncronousDelegate)
    {
        SyncronousDelegate = syncronousDelegate;
    }

    internal DefaultExceptionCommandDelegate(Func<TException, IExecutedCommand, Task> asyncronousDelegate) : base(typeof(TException), (Func<Exception, IExecutedCommand, Task>)asyncronousDelegate)
    {
        AsyncronousDelegate = asyncronousDelegate;
    }
}