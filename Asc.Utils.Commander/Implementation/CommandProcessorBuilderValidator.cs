namespace Asc.Utils.Commander.Implementation;

internal static class CommandProcessorBuilderValidator
{
    internal static void ThrowIfThereIsAlreadyADelegateForThat<TException>(List<DefaultExceptionCommandDelegate> delegates)
    {
        string? typeFullName = typeof(TException).FullName;

        if (string.IsNullOrEmpty(typeFullName))
            throw new InvalidOperationException("Type with no name");

        if (delegates.Any(it =>
            it.ExceptionType is not null
            && !string.IsNullOrEmpty(it.ExceptionType.FullName)
            && it.ExceptionType.FullName == typeFullName))
        {
            throw new InvalidOperationException("There is already a delegate for that");
        }
    }

    internal static void ThrowIfThereIsAlreadyADelegateForThat(DefaultCommandDelegate? @delegate)
    {
        if (@delegate is not null)
            throw new InvalidOperationException("There is already a delegate for that");
    }

    internal static void ThrowIfThereIsAlreadyADelegateForThat(DefaultExecutedCommandDelegate? @delegate)
    {
        if (@delegate is not null)
            throw new InvalidOperationException("There is already a delegate for that");
    }

    internal static void ThrowIfThereIsAlreadyADelegateForThat(int? maxThreads)
    {
        if (maxThreads.HasValue)
            throw new InvalidOperationException("There is already a number of threads limit");
    }
}
