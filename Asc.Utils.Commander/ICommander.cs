namespace Asc.Utils.Commander;

public interface ICommander
{
    ICommandBuilder GetCommandBuilder();

    ICommandBuilder<TResult> GetCommandBuilder<TResult>();

    ISequentialCommandProcessorBuilder GetSequentialCommandProcessorBuilder();

    IConcurrentCommandProcessorBuilder GetConcurrentCommandProcessorBuilder();
}
