namespace Asc.Utils.Commander.Implementation;

internal class DefaultCommandDelegate
{
    internal Action<ICommand>? SyncronousDelegate { get; }

    internal Func<ICommand, Task>? AsyncronousDelegate { get; }

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
        }
    }
}

internal class DefaultExecutedCommandDelegate
{
    internal Action<IExecutedCommand>? SyncronousDelegate { get; }

    internal Func<IExecutedCommand, Task>? AsyncronousDelegate { get; }

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
        }
    }
}

internal abstract class DefaultExceptionCommandDelegate
{
    internal Type? ExceptionType { get; set; }

    internal virtual Task RunAsync(Exception? ex, IExecutedCommand command)
    {
        throw new InvalidOperationException("Use derived type");
    }
}

internal class DefaultExceptionCommandDelegate<TException> : DefaultExceptionCommandDelegate where TException : Exception
{
    internal Action<TException, IExecutedCommand>? SyncronousDelegate { get; }

    internal Func<TException, IExecutedCommand, Task>? AsyncronousDelegate { get; }

    private DefaultExceptionCommandDelegate()
    {
        ExceptionType = typeof(TException);
    }

    internal DefaultExceptionCommandDelegate(Action<TException, IExecutedCommand> syncronousDelegate) : this()
    {
        SyncronousDelegate = syncronousDelegate;
    }

    internal DefaultExceptionCommandDelegate(Func<TException, IExecutedCommand, Task> asyncronousDelegate) : this()
    {
        AsyncronousDelegate = asyncronousDelegate;
    }

    internal override async Task RunAsync(Exception? ex, IExecutedCommand command)
    {
        if (ex is null)
            return;

        await ExecuteAsync((TException)ex, command);
    }

    internal async Task ExecuteAsync(TException ex, IExecutedCommand command)
    {
        if (AsyncronousDelegate is not null)
        {
            await AsyncronousDelegate(ex, command);
            return;
        }

        if (SyncronousDelegate is not null)
        {
            await Task.Run(() => SyncronousDelegate(ex, command));
        }
    }
}