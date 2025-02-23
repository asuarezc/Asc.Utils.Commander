﻿using Asc.Utils.Needle;
using System.Text;

namespace Asc.Utils.Commander.Test;

public class ConcurrentCommandProcessorTest
{
    [Fact]
    public async Task ConcurrentExecution()
    {
        Lock locker = new();
        string result = string.Empty;
        TimeSpan total = TimeSpan.Zero;

        ICommandProcessor commandProcessor = Commander.Instance.GetConcurrentCommandProcessorBuilder()
            .SetMaxNumberOfCommandsProcessedSimultaneosly(20)
            .OnAnyJobSuccess((IExecutedCommand) =>
            {
                locker.Enter();

                try { total += IExecutedCommand.JobElapsedTime; }
                finally { locker.Exit(); }
            })
            .OnAnyJobFailure((Exception ex, IExecutedCommand command) =>
            {
                Console.WriteLine($"{command.Id} failed, Exception message is: \"{ex.Message}\"");
            })
            .Build();

        ICommand command = Commander.Instance.GetCommandBuilder()
            .Job(async () =>
            {
                await Task.Delay(TimeSpan.FromSeconds(1));
            })
            .SetId(nameof(command))
            .Build();

        for (int i = 0; i <= 9; i++)
            commandProcessor.ProcessCommand(command);

        await Task.Delay(TimeSpan.FromSeconds(2));

        Assert.True(total > TimeSpan.FromSeconds(10));
        Assert.True(total < TimeSpan.FromSeconds(11));
    }

    [Fact]
    public async Task MostBasicIntendedUse()
    {
        string result = string.Empty;

        ICommandProcessor commandProcessor = Commander.Instance.GetConcurrentCommandProcessorBuilder()
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

        ICommandProcessor commandProcessor = Commander.Instance.GetConcurrentCommandProcessorBuilder()
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

        ICommandProcessor commandProcessor = Commander.Instance.GetConcurrentCommandProcessorBuilder()
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

        ICommandProcessor commandProcessor = Commander.Instance.GetConcurrentCommandProcessorBuilder()
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

        ICommandProcessor commandProcessor = Commander.Instance.GetConcurrentCommandProcessorBuilder()
            .OnBeforeAnyJob((ICommand command) =>
            {
                builder.Append('A');
            })
            .OnAnyJobSuccess((IExecutedCommand command) =>
            {
                builder.Append('D');
            })
            .OnAnyJobFailure((Exception ex, IExecutedCommand command) =>
            {
                Console.WriteLine($"{command.Id} failed, Exception message is: \"{ex.Message}\"");
            })
            .OnAfterAnyJob((IExecutedCommand command) =>
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

        Assert.Equal("ABCDEF", builder.ToString());
    }

    [Fact]
    public async Task CorrectOrderOfExecutionWithException_1()
    {
        StringBuilder builder = new();

        ICommandProcessor commandProcessor = Commander.Instance.GetConcurrentCommandProcessorBuilder()
            .OnBeforeAnyJob((ICommand command) =>
            {
                builder.Append('A');
            })
            .OnAnyJobFailure((Exception ex, IExecutedCommand command) =>
            {
                builder.Append('D');
            })
            .OnAfterAnyJob((IExecutedCommand command) =>
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
            .OnFailure((Exception ex) =>
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

        Assert.Equal("ABCDEF", builder.ToString());
    }

    [Fact]
    public async Task CorrectOrderOfExecutionWithException_2()
    {
        StringBuilder builder = new();

        ICommandProcessor commandProcessor = Commander.Instance.GetConcurrentCommandProcessorBuilder()
            .OnBeforeAnyJob((ICommand command) =>
            {
                builder.Append('A');
            })
            .OnAnyJobFailure((Exception ex, IExecutedCommand command) =>
            {
                builder.Append('D');
            })
            .OnAfterAnyJob((IExecutedCommand command) =>
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
            .OnFailure((InvalidOperationException ex) =>
            {
                builder.Append('C');
            })
            .OnFailure((Exception ex) =>
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

        Assert.Equal("ABCDEF", builder.ToString());
    }

    [Fact]
    public async Task CorrectOrderOfExecutionWithException_3()
    {
        StringBuilder builder = new();

        ICommandProcessor commandProcessor = Commander.Instance.GetConcurrentCommandProcessorBuilder()
            .OnBeforeAnyJob((ICommand command) =>
            {
                builder.Append('A');
            })
            .OnAnyJobFailure((InvalidOperationException ex, IExecutedCommand command) =>
            {
                builder.Append('D');
            })
            .OnAnyJobFailure((Exception ex, IExecutedCommand command) =>
            {
                builder.Append("This should be not executed");
            })
            .OnAfterAnyJob((IExecutedCommand command) =>
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
            .OnFailure((InvalidOperationException ex) =>
            {
                builder.Append('C');
            })
            .OnFailure((Exception ex) =>
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

        Assert.Equal("ABCDEF", builder.ToString());
    }

    [Fact]
    public async Task CorrectOrderOfExecution_AllAsync()
    {
        StringBuilder builder = new();

        ICommandProcessor commandProcessor = Commander.Instance.GetConcurrentCommandProcessorBuilder()
            .OnBeforeAnyJob(async (ICommand command) =>
            {
                await Task.Run(() => builder.Append('A'));
            })
            .OnAnyJobSuccess(async (IExecutedCommand command) =>
            {
                await Task.Run(() => builder.Append('D'));
            })
            .OnAnyJobFailure(async (Exception ex, IExecutedCommand command) =>
            {
                await Task.Run(() => Console.WriteLine($"{command.Id} failed, Exception message is: \"{ex.Message}\""));
            })
            .OnAfterAnyJob(async (IExecutedCommand command) =>
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
            .OnFinally(async () =>
            {
                await Task.Run(() => builder.Append('E'));
            })
            .SetId(nameof(command))
            .Build();

        commandProcessor.ProcessCommand(command);

        await Task.Delay(100);

        Assert.Equal("ABCDEF", builder.ToString());
    }

    [Fact]
    public async Task ParametersAndElapsedTime()
    {
        string result = string.Empty;
        TimeSpan? elapsedTime = null;

        ICommandProcessor commandProcessor = Commander.Instance.GetConcurrentCommandProcessorBuilder()
            .OnAnyJobSuccess((IExecutedCommand command) =>
            {
                result = command.Parameters.FirstOrDefault().Value;
                elapsedTime = command.JobElapsedTime;
            })
            .OnAnyJobFailure(async (Exception ex, IExecutedCommand command) =>
            {
                await Task.Run(() => Console.WriteLine($"{command.Id} failed, Exception message is: \"{ex.Message}\""));
            })
            .Build();

        ICommand command = Commander.Instance.GetCommandBuilder()
            .Job(() =>
            {
                Console.WriteLine("Testing ParametersAndElapsedTime");
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
