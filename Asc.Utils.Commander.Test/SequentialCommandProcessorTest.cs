using Asc.Utils.Needle;
using System.Text;

namespace Asc.Utils.Commander.Test;

public class SequentialCommandProcessorTest
{
    [Fact]
    public async Task MostBasicIntendedUse()
    {
        string result = string.Empty;

        ICommandProcessor commandProcessor = Commander.Instance.GetSequentialCommandProcessorBuilder()
            .OnAnyJobFailure((Exception ex, IExecutedCommand command) =>
            {
                Console.WriteLine($"{command.Id} failed, Exception message is: \"{ex.Message}\"");
            })
            .Build();

        ICommand command = Commander.Instance.GetCommandBuilder()
            .Job(() =>
            {
                result = "Job1";
            })
            .SetId(nameof(command))
            .Build();

        commandProcessor.ProcessCommand(command);

        await Task.Delay(100);

        Assert.Equal("Job1", result);
    }

    [Fact]
    public async Task GenericCommand()
    {
        string result = string.Empty;

        ICommandProcessor commandProcessor = Commander.Instance.GetSequentialCommandProcessorBuilder()
            .OnAnyJobFailure((Exception ex, IExecutedCommand command) =>
            {
                Console.WriteLine($"{command.Id} failed, Exception message is: \"{ex.Message}\"");
            })
            .Build();

        ICommand command = Commander.Instance.GetCommandBuilder<string>()
            .Job(() =>
            {
                return "test";
            })
            .OnSuccess((string jobResult) =>
            {
                result = jobResult;
            })
            .SetId(nameof (command))
            .Build();

        commandProcessor.ProcessCommand(command);

        await Task.Delay(100);

        Assert.Equal("test", result);
    }

    [Fact]
    public async Task AsyncJob()
    {
        string result = string.Empty;

        ICommandProcessor commandProcessor = Commander.Instance.GetSequentialCommandProcessorBuilder()
            .OnAnyJobFailure((Exception ex, IExecutedCommand command) =>
            {
                Console.WriteLine($"{command.Id} failed, Exception message is: \"{ex.Message}\"");
            })
            .Build();

        ICommand command = Commander.Instance.GetCommandBuilder()
            .Job(async () =>
            {
                await Task.Run(() => { result = "Job1"; });
            })
            .SetId(nameof(command))
            .Build();

        commandProcessor.ProcessCommand(command);

        await Task.Delay(100);

        Assert.Equal("Job1", result);
    }

    [Fact]
    public async Task BatchOfCommands()
    {
        int result = 0;

        ICommandProcessor commandProcessor = Commander.Instance.GetSequentialCommandProcessorBuilder()
            .OnAnyJobFailure((Exception ex, IExecutedCommand command) =>
            {
                Console.WriteLine($"{command.Id} failed, Exception message is: \"{ex.Message}\"");
            })
            .Build();

        ICommand command = Commander.Instance.GetCommandBuilder()
            .Job(() =>
            {
                result++;
            })
            .SetId(nameof(command))
            .Build();

        using INeedleWorkerSlim worker = Pincushion.Instance.GetParallelWorkerSlim();

        for (int i = 0; i <= 9; i++)
            worker.AddJob(() => commandProcessor.ProcessCommand(command));

        await worker.RunAsync();
        await Task.Delay(100);

        Assert.Equal(10, result);
    }

    [Fact]
    public async Task OnAnyJobFailure()
    {
        string result = string.Empty;

        ICommandProcessor commandProcessor = Commander.Instance.GetSequentialCommandProcessorBuilder()
            .OnAnyJobFailure((Exception ex, IExecutedCommand command) =>
            {
                result = command.Id;
            })
            .Build();

        ICommand command = Commander.Instance.GetCommandBuilder()
            .Job(() =>
            {
                throw new InvalidOperationException();
            })
            .SetId(nameof(command))
            .Build();

        commandProcessor.ProcessCommand(command);

        await Task.Delay(100);

        Assert.Equal(nameof(command), result);
    }

    [Fact]
    public async Task CorrectOrderOfExecution()
    {
        StringBuilder builder = new();

        ICommandProcessor commandProcessor = Commander.Instance.GetSequentialCommandProcessorBuilder()
            .OnBeforeAnyJob((ICommand command) =>
            {
                builder.Append('A');
            })
            .OnAnyJobSuccess((IExecutedCommand command) =>
            {
                builder.Append('C');
            })
            .OnAnyJobFailure((Exception ex, IExecutedCommand command) =>
            {
                Console.WriteLine($"{command.Id} failed, Exception message is: \"{ex.Message}\"");
            })
            .OnAfterAnyJob((IExecutedCommand command) =>
            {
                builder.Append('E');
            })
            .Build();

        ICommand command = Commander.Instance.GetCommandBuilder()
            .Job(() =>
            {
                builder.Append('B');
            })
            .OnSuccess(() =>
            {
                builder.Append('D');
            })
            .OnFinally(() =>
            {
                builder.Append('F');
            })
            .SetId(nameof(command))
            .Build();

        commandProcessor.ProcessCommand(command);

        await Task.Delay(100);

        Assert.Equal("ABCDEF", builder.ToString());
    }

    [Fact]
    public async Task CorrectOrderOfExecutionWithException()
    {
        StringBuilder builder = new();

        ICommandProcessor commandProcessor = Commander.Instance.GetSequentialCommandProcessorBuilder()
            .OnBeforeAnyJob((ICommand command) =>
            {
                builder.Append('A');
            })
            .OnAnyJobFailure((Exception ex, IExecutedCommand command) =>
            {
                builder.Append('C');
            })
            .OnAfterAnyJob((IExecutedCommand command) =>
            {
                builder.Append('E');
            })
            .Build();

        ICommand command = Commander.Instance.GetCommandBuilder()
            .Job(() =>
            {
                builder.Append('B');
                throw new InvalidOperationException();
            })
            .OnFailure((Exception ex) =>
            {
                builder.Append('D');
            })
            .OnFinally(() =>
            {
                builder.Append('F');
            })
            .SetId(nameof(command))
            .Build();

        commandProcessor.ProcessCommand(command);

        await Task.Delay(100);

        Assert.Equal("ABCDEF", builder.ToString());
    }
}