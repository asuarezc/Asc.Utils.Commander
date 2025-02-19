namespace Asc.Utils.Commander.Implementation;

internal static class CommandBuilderValidator
{
    internal static void ThrowIfThereIsAlreadyAnOnFailureDelegateWithSameExceptionType<TException>(IEnumerable<ExceptionCommandDelegate> onFailureDelegates)
    {
        string? typeFullName = typeof(TException).FullName;

        if (string.IsNullOrEmpty(typeFullName))
            throw new InvalidOperationException("Type with no name");

        if (onFailureDelegates.Any(it =>
            it.ExceptionType is not null
            && !string.IsNullOrEmpty(it.ExceptionType.FullName)
            && it.ExceptionType.FullName == typeFullName))
        {
            throw new InvalidOperationException("Exception type already added");
        }
    }

    internal static void ThrowIfThereIsAlreadyAJobDelegate(CommandDelegate? jobDelegate)
    {
        if (jobDelegate is not null)
            throw new InvalidOperationException("There is already a job delegate added");
    }

    internal static void ThrowIfThereIsAlreadyAJobDelegate<TResult>(CommandJobDelegate<TResult>? jobDelegate)
    {
        if (jobDelegate is not null)
            throw new InvalidOperationException("There is already a job delegate added");
    }

    internal static void ThrowIfThereIsAlreadyAnOnSuccessDelegate(CommandDelegate? onSuccessDelegate)
    {
        if (onSuccessDelegate is not null)
            throw new InvalidOperationException("There is already a onSuccess delegate added");
    }

    internal static void ThrowIfThereIsAlreadyAnOnSuccessDelegate<TResult>(CommandOnSuccessDelegate<TResult>? onSuccessDelegate)
    {
        if (onSuccessDelegate is not null)
            throw new InvalidOperationException("There is already a onSuccess delegate added");
    }
}
