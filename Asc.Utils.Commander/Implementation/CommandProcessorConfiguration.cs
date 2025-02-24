namespace Asc.Utils.Commander.Implementation;

internal class CommandProcessorConfiguration
{
    internal DefaultCommandDelegate? OnBeforeJobDelegate { get; set; } = null;

    internal DefaultExecutedCommandDelegate? OnSuccessDelegate { get; set; } = null;

    internal List<DefaultExceptionCommandDelegate> OnFailureDelegates { get; set; } = [];

    internal DefaultExecutedCommandDelegate? OnFinallyDelegate { get; set; } = null;
}

internal class ConcurrentCommandProcessorConfiguration : CommandProcessorConfiguration
{
    internal int MaxNumberOfCommandsProcessedSimultaneosly { get; set; }
}
