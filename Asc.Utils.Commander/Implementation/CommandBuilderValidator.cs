namespace Asc.Utils.Commander.Implementation;

internal static class CommandBuilderValidator
{
    internal static void ThrowIfThereIsAlreadyADelegateForThat<TException>(List<ExceptionCommandDelegate> delegates)
    {
        Type invokingType = typeof(TException);

        if (delegates.Any(it =>
            it.ExceptionType is not null
            && it.ExceptionType == invokingType))
        {
            throw new InvalidOperationException("There is already a delegate for that");
        }
    }

    internal static void ThrowIfThereIsAlreadyADelegateForThat<TResult>(CommandOnSuccessDelegate<TResult>? @delegate)
    {
        if (@delegate is not null)
            throw new InvalidOperationException("There is already a delegate for that");
    }

    internal static void ThrowIfThereIsAlreadyADelegateForThat(CommandDelegate? @delegate)
    {
        if (@delegate is not null)
            throw new InvalidOperationException("There is already a delegate for that");
    }

    internal static void ThrowIfThereIsAlreadyADelegateForThat<TResult>(CommandJobDelegate<TResult>? @delegate)
    {
        if (@delegate is not null)
            throw new InvalidOperationException("There is already a delegate for that");
    }
}
