using System.Text;

namespace Asc.Utils.Commander.Test;

public class SomeFuntionalTests
{
    [Fact]
    public void IntendedUse1()
    {
        StringBuilder stringBuilder = new();
        ISequentialCommandProcessorBuilder commandProcessorBuilder = Commander.Instance.GetSequentialCommandProcessorBuilder();
        ICommandBuilder commandBuilder = Commander.Instance.GetCommandBuilder();

        ICommandProcessor commandProcessor = commandProcessorBuilder
            .OnAnyJobFailure((Exception ex, IExecutedCommand command) =>
            {
                stringBuilder.AppendLine($"{command.Id} failed, Exception message is: \"{ex.Message}\"");
            })
            .Build();

        ICommand command = commandBuilder
            .Job(() =>
            {
                stringBuilder.AppendLine("Job1");
            })
            .SetId("Job1")
            .Build();

        void OnIsRunningChanged(object? sender, bool isRunning)
        {
            if (isRunning)
                return;

            Assert.Equal("Job1", stringBuilder.ToString());
            commandProcessor.IsRunningChanged -= OnIsRunningChanged;
        };

        commandProcessor.IsRunningChanged += OnIsRunningChanged;
        commandProcessor.ProcessCommand(command);
    }
}
