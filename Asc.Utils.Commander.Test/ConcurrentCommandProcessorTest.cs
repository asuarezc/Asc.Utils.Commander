using Asc.Utils.Needle;
using System.Text;
using Xunit.Sdk;

namespace Asc.Utils.Commander.Test;

public class ConcurrentCommandProcessorTest
{
    private static readonly TestOutputHelper testOutputHelper = new();

    [Fact]
    public async Task ConcurrentExecution()
    {
        Lock locker = new();
        TimeSpan total = TimeSpan.Zero;

        ICommandProcessor commandProcessor = Commander.Instance.GetConcurrentCommandProcessorBuilder()
            .SetMaxThreads(10)
            .OnAnyJobSuccess(executedCommand =>
            {
                locker.Enter();

                try { total += executedCommand.JobElapsedTime; }
                finally { locker.Exit(); }
            })
            //Mandatory
            .OnAnyJobFailure((Exception ex, IExecutedCommand command) =>
            {
                testOutputHelper.WriteLine($"{command.Id} failed, Exception message is: \"{ex.Message}\"");
            })
            .Build();

        ICommand command = Commander.Instance.GetCommandBuilder()
            //Job and SetId are mandatory
            .Job(async () =>
            {
                await Task.Delay(TimeSpan.FromSeconds(1));
            })
            .SetId(nameof(command))
            .Build();

        using INeedleWorkerSlim worker = Pincushion.Instance.GetParallelWorkerSlim();

        for (int i = 0; i < 10; i++)
            worker.AddJob(() => commandProcessor.ProcessCommand(command));

        await worker.RunAsync();

        //We only wait 1.5 secons but total jobs execution time is between 10 and 10.5 seconds
        //This is only possible with simultaneous execution
        await Task.Delay(TimeSpan.FromSeconds(1.5));

        Assert.True(
            total > TimeSpan.FromSeconds(10)
            && total < TimeSpan.FromSeconds(10.5)
        );
    }

    [Fact]
    public async Task ConcurrentExecution_NeedMoreThreds()
    {
        Lock locker = new();
        TimeSpan total = TimeSpan.Zero;

        ICommandProcessor commandProcessor = Commander.Instance.GetConcurrentCommandProcessorBuilder()
            .SetMaxThreads(5)
            .OnAnyJobSuccess(executedCommand =>
            {
                locker.Enter();

                try { total += executedCommand.JobElapsedTime; }
                finally { locker.Exit(); }
            })
            //Mandatory
            .OnAnyJobFailure((Exception ex, IExecutedCommand command) =>
            {
                testOutputHelper.WriteLine($"{command.Id} failed, Exception message is: \"{ex.Message}\"");
            })
            .Build();

        ICommand command = Commander.Instance.GetCommandBuilder()
            //Job and SetId are mandatory
            .Job(async () =>
            {
                await Task.Delay(TimeSpan.FromSeconds(1));
            })
            .SetId(nameof(command))
            .Build();

        using INeedleWorkerSlim worker = Pincushion.Instance.GetParallelWorkerSlim();

        for (int i = 0; i < 10; i++)
            worker.AddJob(() => commandProcessor.ProcessCommand(command));

        await worker.RunAsync();

        await Task.Delay(TimeSpan.FromSeconds(1.5));

        //Same Assert condition as above but false instead of true
        //since the maximum number of commands processed simultaneously has been reduced from 10 to 5
        Assert.False(total > TimeSpan.FromSeconds(10) && total < TimeSpan.FromSeconds(10.5));
    }

    [Fact]
    public async Task MostBasicIntendedUse()
    {
        string result = string.Empty;

        ICommandProcessor commandProcessor = Commander.Instance.GetConcurrentCommandProcessorBuilder()
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
    public async Task GenericCommand()
    {
        string result = string.Empty;

        ICommandProcessor commandProcessor = Commander.Instance.GetConcurrentCommandProcessorBuilder()
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
            .SetId(nameof(command))
            .Build();

        commandProcessor.ProcessCommand(command);

        await Task.Delay(100);

        Assert.Equal("test", result);
    }

    [Fact]
    public async Task AsyncJob()
    {
        string result = string.Empty;

        ICommandProcessor commandProcessor = Commander.Instance.GetConcurrentCommandProcessorBuilder()
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

        ICommandProcessor commandProcessor = Commander.Instance.GetConcurrentCommandProcessorBuilder()
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

        for (int i = 0; i < 10; i++)
            worker.AddJob(() => commandProcessor.ProcessCommand(command));

        await worker.RunAsync();
        await Task.Delay(100);

        Assert.Equal(10, result);
    }

    [Fact]
    public async Task OnAnyJobFailure()
    {
        string result = string.Empty;

        ICommandProcessor commandProcessor = Commander.Instance.GetConcurrentCommandProcessorBuilder()
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

        ICommandProcessor commandProcessor = Commander.Instance.GetConcurrentCommandProcessorBuilder()
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

        ICommandProcessor commandProcessor = Commander.Instance.GetConcurrentCommandProcessorBuilder()
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

        ICommandProcessor commandProcessor = Commander.Instance.GetConcurrentCommandProcessorBuilder()
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

        ICommandProcessor commandProcessor = Commander.Instance.GetConcurrentCommandProcessorBuilder()
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

        ICommandProcessor commandProcessor = Commander.Instance.GetConcurrentCommandProcessorBuilder()
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

        ICommandProcessor commandProcessor = Commander.Instance.GetConcurrentCommandProcessorBuilder()
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
