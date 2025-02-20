﻿namespace Asc.Utils.Commander.Implementation;

internal static class CommandBuilderValidator
{
    internal static void ThrowIfThereIsAlreadyADelegateForThat<TException>(List<ExceptionCommandDelegate> delegates)
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
