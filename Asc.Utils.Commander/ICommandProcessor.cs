namespace Asc.Utils.Commander;

/// <summary>
/// Surprise: processes commands
/// </summary>
public interface ICommandProcessor
{
    /// <summary>
    /// You won't believe it but it processes a command
    /// </summary>
    void ProcessCommand(ICommand command);
}
