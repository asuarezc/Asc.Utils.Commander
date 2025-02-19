namespace Asc.Utils.Commander;

public interface ICommand
{
    string Id { get; }

    bool JobIsAsyncronous { get; }

    bool HasOnSuccesDelegate { get; }

    bool HasOnSuccesAsyncronousDelegate { get; }

    bool HasOnSuccesSyncronousDelegate { get; }

    bool HasOnFailureDelegates { get; }
}

public interface ICommand<TResult>
{
    string Id { get; }

    bool JobIsAsyncronous { get; }

    bool HasOnSuccesDelegate { get; }

    bool HasOnSuccesAsyncronousDelegate { get; }

    bool HasOnSuccesSyncronousDelegate { get; }

    bool HasOnFailureDelegates { get; }
}
