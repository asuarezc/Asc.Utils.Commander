using Asc.Utils.Commander.Implementation;

namespace Asc.Utils.Commander;

public sealed class Commander : ICommander
{
    private static readonly Lazy<ICommander> lazyInstance = new(() => new Commander(), LazyThreadSafetyMode.PublicationOnly);

    public static ICommander Instance => lazyInstance.Value;

    private Commander() { }

    public ICommandBuilder GetCommandBuilder() => new CommandBuilder();

    public ICommandBuilder<TResult> GetCommandBuilder<TResult>() => new CommandBuilder<TResult>();

    public IConcurrentCommandProcessorBuilder GetConcurrentCommandProcessorBuilder() => new ConcurrentCommandProcessorBuilder();

    public ISequentialCommandProcessorBuilder GetSequentialCommandProcessorBuilder() => new SequentialCommandProcessorBuilder();
}
