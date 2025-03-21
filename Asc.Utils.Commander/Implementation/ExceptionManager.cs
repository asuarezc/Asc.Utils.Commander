﻿namespace Asc.Utils.Commander.Implementation;

internal static class ExceptionManager
{
    internal static async Task ManageExceptionAsync(
        Exception? exception,
        List<ExceptionCommandDelegate>? delegates,
        List<DefaultExceptionCommandDelegate>? defaultDelegates,
        TimeSpan jobElapsedTime,
        string id)
    {
        if ((delegates is null || delegates.Count == 0) && (defaultDelegates is null || defaultDelegates.Count == 0))
            ThrowInvalidDueToNoDelegateFoundForCurrent(exception);

        Type? exType = exception?.GetType();
        IExecutedCommand executedCommand = new ExecutedCommand(jobElapsedTime, ExecutedCommandResult.Failed, id);

        ExceptionCommandDelegate? exceptionDelegate = delegates?
            .SingleOrDefault(it => it.ExceptionType is not null && it.ExceptionType == typeof(Exception));

        DefaultExceptionCommandDelegate? defaultExceptionDelegate = defaultDelegates?
            .SingleOrDefault(it => it.ExceptionType is not null && it.ExceptionType == typeof(Exception));

        if (exType is not null && exType.IsSubclassOf(typeof(Exception)))
        {
            ExceptionCommandDelegate? derivedExceptionTypeDelegate = delegates?
                .SingleOrDefault(it => it.ExceptionType is not null && it.ExceptionType == exType);

            DefaultExceptionCommandDelegate? defaultDerivedExceptionTypeDelegate = defaultDelegates?
                .SingleOrDefault(it => it.ExceptionType is not null && it.ExceptionType == exType);

            if (derivedExceptionTypeDelegate is not null)
                await derivedExceptionTypeDelegate.RunAsync(exception);
            else
            {
                if (exceptionDelegate is not null)
                    await exceptionDelegate.RunAsync(exception);
            }

            if (defaultDerivedExceptionTypeDelegate is not null)
                await defaultDerivedExceptionTypeDelegate.RunAsync(exception, executedCommand);
            else
            {
                if (defaultExceptionDelegate is not null)
                    await defaultExceptionDelegate.RunAsync(exception, executedCommand);
            }

            return;
        }

        if (exceptionDelegate is null && defaultExceptionDelegate is null)
            ThrowInvalidDueToNoDelegateFoundForCurrent(exception);
        else
        {
            if (exceptionDelegate is not null)
                await exceptionDelegate.RunAsync(exception);

            if (defaultExceptionDelegate is not null)
                await defaultExceptionDelegate.RunAsync(exception, executedCommand);
        }
    }

    private static void ThrowInvalidDueToNoDelegateFoundForCurrent(Exception? ex)
    {
        throw new InvalidOperationException(
            string.Concat(
                "There is no delegate for current exception, command and command processor.",
                Environment.NewLine,
                "See inner exception for more information."),
            ex
        );
    }
}