using Asc.Utils.Needle;
using System.Text;
using Xunit.Abstractions;

namespace Asc.Utils.Commander.Test;

public class SequentialCommandProcessorTest(ITestOutputHelper testOutputHelper)
{
    [Fact]
    public async Task MostBasicIntendedUse()
    {
        string result = string.Empty;

        ICommandProcessor commandProcessor = Commander.Instance.GetSequentialCommandProcessorBuilder()
            .OnAnyJobFailure((Exception ex, IExecutedCommand command) =>
            {
                testOutputHelper.WriteLine($"{command.Id} failed, Exception message is: \"{ex.Message}\"");
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
    public async Task ExecutedCommand()
    {
        string result = string.Empty;
        string executedCommandResult = string.Empty;

        ICommandProcessor commandProcessor = Commander.Instance.GetSequentialCommandProcessorBuilder()
            .OnAnyJobFailure((Exception ex, IExecutedCommand command) =>
            {
                testOutputHelper.WriteLine($"{command.Id} failed, Exception message is: \"{ex.Message}\"");
            })
            .OnAfterAnyJob(executedCommand =>
            {
                executedCommandResult = executedCommand.CommandResult.ToString();
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
        Assert.Equal("Succeeded", executedCommandResult);
    }

    [Fact]
    public async Task GenericCommand()
    {
        string result = string.Empty;

        ICommandProcessor commandProcessor = Commander.Instance.GetSequentialCommandProcessorBuilder()
            .OnAnyJobFailure((Exception ex, IExecutedCommand command) =>
            {
                testOutputHelper.WriteLine($"{command.Id} failed, Exception message is: \"{ex.Message}\"");
            })
            .Build();

        ICommand command = Commander.Instance.GetCommandBuilder<string>()
            .Job(() => "test")
            .OnSuccess(jobResult =>
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
                testOutputHelper.WriteLine($"{command.Id} failed, Exception message is: \"{ex.Message}\"");
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
                testOutputHelper.WriteLine($"{command.Id} failed, Exception message is: \"{ex.Message}\"");
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
            .OnAnyJobFailure((Exception _, IExecutedCommand command) =>
            {
                result = command.Id;
            })
            .Build();

        ICommand command = Commander.Instance.GetCommandBuilder()
            .Job(() => throw new InvalidOperationException())
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
            .OnBeforeAnyJob(_ =>
            {
                builder.Append('A');
            })
            .OnAnyJobSuccess(_ =>
            {
                builder.Append('D');
            })
            .OnAnyJobFailure((Exception ex, IExecutedCommand command) =>
            {
                testOutputHelper.WriteLine($"{command.Id} failed, Exception message is: \"{ex.Message}\"");
            })
            .OnAfterAnyJob(_ =>
            {
                builder.Append('F');
            })
            .Build();

        ICommand command = Commander.Instance.GetCommandBuilder()
            .Job(() =>
            {
                builder.Append('B');
            })
            .OnSuccess(() =>
            {
                builder.Append('C');
            })
            .OnFinally(() =>
            {
                builder.Append('E');
            })
            .SetId(nameof(command))
            .Build();

        commandProcessor.ProcessCommand(command);

        await Task.Delay(100);

        // ReSharper disable once StringLiteralTypo
        Assert.Equal("ABCDEF", builder.ToString());
    }

    [Fact]
    public async Task CorrectOrderOfExecutionWithException_1()
    {
        StringBuilder builder = new();

        ICommandProcessor commandProcessor = Commander.Instance.GetSequentialCommandProcessorBuilder()
            .OnBeforeAnyJob(_ =>
            {
                builder.Append('A');
            })
            .OnAnyJobFailure((Exception _, IExecutedCommand _) =>
            {
                builder.Append('D');
            })
            .OnAfterAnyJob(_ =>
            {
                builder.Append('F');
            })
            .Build();

        ICommand command = Commander.Instance.GetCommandBuilder()
            .Job(() =>
            {
                builder.Append('B');
                throw new InvalidOperationException();
            })
            .OnFailure((Exception _) =>
            {
                builder.Append('C');
            })
            .OnFinally(() =>
            {
                builder.Append('E');
            })
            .SetId(nameof(command))
            .Build();

        commandProcessor.ProcessCommand(command);

        await Task.Delay(100);

        // ReSharper disable once StringLiteralTypo
        Assert.Equal("ABCDEF", builder.ToString());
    }

    [Fact]
    public async Task CorrectOrderOfExecutionWithException_2()
    {
        StringBuilder builder = new();

        ICommandProcessor commandProcessor = Commander.Instance.GetSequentialCommandProcessorBuilder()
            .OnBeforeAnyJob(_ =>
            {
                builder.Append('A');
            })
            .OnAnyJobFailure((Exception _, IExecutedCommand _) =>
            {
                builder.Append('D');
            })
            .OnAfterAnyJob(_ =>
            {
                builder.Append('F');
            })
            .Build();

        ICommand command = Commander.Instance.GetCommandBuilder()
            .Job(() =>
            {
                builder.Append('B');
                throw new InvalidOperationException();
            })
            .OnFailure((InvalidOperationException _) =>
            {
                builder.Append('C');
            })
            .OnFailure((Exception _) =>
            {
                builder.Append("This should be not executed");
            })
            .OnFinally(() =>
            {
                builder.Append('E');
            })
            .SetId(nameof(command))
            .Build();

        commandProcessor.ProcessCommand(command);

        await Task.Delay(100);

        // ReSharper disable once StringLiteralTypo
        Assert.Equal("ABCDEF", builder.ToString());
    }

    [Fact]
    public async Task CorrectOrderOfExecutionWithException_3()
    {
        StringBuilder builder = new();

        ICommandProcessor commandProcessor = Commander.Instance.GetSequentialCommandProcessorBuilder()
            .OnBeforeAnyJob(_ =>
            {
                builder.Append('A');
            })
            .OnAnyJobFailure((InvalidOperationException _, IExecutedCommand _) =>
            {
                builder.Append('D');
            })
            .OnAnyJobFailure((Exception _, IExecutedCommand _) =>
            {
                builder.Append("This should be not executed");
            })
            .OnAfterAnyJob(_ =>
            {
                builder.Append('F');
            })
            .Build();

        ICommand command = Commander.Instance.GetCommandBuilder()
            .Job(() =>
            {
                builder.Append('B');
                throw new InvalidOperationException();
            })
            .OnFailure((InvalidOperationException _) =>
            {
                builder.Append('C');
            })
            .OnFailure((Exception _) =>
            {
                builder.Append("This should be not executed");
            })
            .OnFinally(() =>
            {
                builder.Append('E');
            })
            .SetId(nameof(command))
            .Build();

        commandProcessor.ProcessCommand(command);

        await Task.Delay(100);

        // ReSharper disable once StringLiteralTypo
        Assert.Equal("ABCDEF", builder.ToString());
    }

    [Fact]
    public async Task CorrectOrderOfExecution_AllAsync()
    {
        StringBuilder builder = new();

        ICommandProcessor commandProcessor = Commander.Instance.GetSequentialCommandProcessorBuilder()
            .OnBeforeAnyJob(async _ =>
            {
                await Task.Run(() => builder.Append('A'));
            })
            .OnAnyJobSuccess(async _ =>
            {
                await Task.Run(() => builder.Append('D'));
            })
            .OnAnyJobFailure(async (Exception ex, IExecutedCommand command) =>
            {
                await Task.Run(() => testOutputHelper.WriteLine($"{command.Id} failed, Exception message is: \"{ex.Message}\""));
            })
            .OnAfterAnyJob(async _ =>
            {
                await Task.Run(() => builder.Append('F'));
            })
            .Build();

        ICommand command = Commander.Instance.GetCommandBuilder()
            .Job(async () =>
            {
                await Task.Run(() => builder.Append('B'));
            })
            .OnSuccess(async () =>
            {
                await Task.Run(() => builder.Append('C'));
            })
            .OnFailure(async (InvalidOperationException _) =>
            {
                await Task.Run(() => builder.Append("This should be not executed"));
            })
            .OnFinally(async () =>
            {
                await Task.Run(() => builder.Append('E'));
            })
            .SetId(nameof(command))
            .Build();

        commandProcessor.ProcessCommand(command);

        await Task.Delay(100);

        // ReSharper disable once StringLiteralTypo
        Assert.Equal("ABCDEF", builder.ToString());
    }

    [Fact]
    public async Task ParametersAndElapsedTime()
    {
        string result = string.Empty;
        TimeSpan? elapsedTime = null;

        ICommandProcessor commandProcessor = Commander.Instance.GetSequentialCommandProcessorBuilder()
            .OnAnyJobSuccess(command =>
            {
                result = command.Parameters["param1"].OfType<string>();
                elapsedTime = command.JobElapsedTime;
            })
            .OnAnyJobFailure(async (Exception ex, IExecutedCommand command) =>
            {
                await Task.Run(() => testOutputHelper.WriteLine($"{command.Id} failed, Exception message is: \"{ex.Message}\""));
            })
            .Build();

        ICommand command = Commander.Instance.GetCommandBuilder()
            .Job(() =>
            {
                testOutputHelper.WriteLine("Testing ParametersAndElapsedTime");
            })
            .AddOrReplaceParameter("param1", "test")
            .SetId(nameof(command))
            .Build();

        commandProcessor.ProcessCommand(command);

        await Task.Delay(100);

        Assert.Equal("test", result);
        Assert.NotNull(elapsedTime);
    }
}