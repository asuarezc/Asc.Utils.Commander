using System.Text;

namespace Asc.Utils.Commander.Test
{
    public class UnitTest1
    {
        [Fact]
        public async Task Test1()
        {
            StringBuilder stringBuilder = new();
            ISequentialCommandProcessorBuilder commandProcessorBuilder = Commander.Instance.GetSequentialCommandProcessorBuilder();
            ICommandBuilder commandBuilder = Commander.Instance.GetCommandBuilder();

            ICommandProcessor commandProcessor = commandProcessorBuilder
                .AddOnBeforeJobDelegate((ICommand command) =>
                {
                    stringBuilder.AppendLine($"Before: {command.Id}");
                })
                .AddOnSuccessDelegate((IExecutedCommand command) =>
                {
                    stringBuilder.AppendLine($"Success: {command.Id}, Elapsed: {command.JobElapsedTime}");
                })
                .AddOnFailureDelegate((Exception ex, IExecutedCommand command) =>
                {
                    stringBuilder.AppendLine($"Failed: {command.Id}, Elapsed: {command.JobElapsedTime}, Exception: {ex.Message}");
                })
                .AddOnFinallyDelegate((IExecutedCommand command) =>
                {
                    stringBuilder.AppendLine($"Finally: {command.Id}, Elapsed: {command.JobElapsedTime}, Result: {command.CommandResult.ToString()}");
                })
                .Build();

            ICommand command = commandBuilder
                .Job(() =>
                {
                    throw new Exception("Bla");
                })
                .OnSuccess(() =>
                {
                    stringBuilder.AppendLine("Success Job1");
                })
                .OnFailure((InvalidOperationException ex) =>
                {
                    stringBuilder.AppendLine("Invalid Job1");
                })
                .OnFailure((Exception ex) =>
                {
                    stringBuilder.AppendLine($"Failed Job1, Exception: {ex.Message}");
                })
                .OnFinally(() =>
                {
                    stringBuilder.AppendLine("Finally Job1");
                })
                .SetId("Job1")
                .Build();

            commandProcessor.ProcessCommand(command);

            await Task.Delay(TimeSpan.FromSeconds(1));

            string result = stringBuilder.ToString();
        }
    }
}
