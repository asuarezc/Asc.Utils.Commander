using Xunit.Abstractions;

namespace Asc.Utils.Commander.Test;

public class ConcurrentCommandProcessorBuilderTest(ITestOutputHelper testOutputHelper)
{
    #region SetMaxThreads

    [Fact]
    public void SetMaxThreads()
    {
        ICommandProcessor commandProcessor = Commander.Instance.GetConcurrentCommandProcessorBuilder()
            .OnAnyJobFailure((Exception _, IExecutedCommand _) => testOutputHelper.WriteLine("Testing SetMaxThreads"))
            .SetMaxThreads(5)
            .Build();

        Assert.NotNull(commandProcessor);
    }

    [Fact]
    public void SetMaxThreads_WhenInvalid()
    {
        Assert.Throws<ArgumentException>(() => Commander.Instance.GetConcurrentCommandProcessorBuilder().SetMaxThreads(-5));
        Assert.Throws<ArgumentException>(() => Commander.Instance.GetConcurrentCommandProcessorBuilder().SetMaxThreads(0));
    }

    [Fact]
    public void SetMaxThreads_WhenAlreadySetted()
    {
        Assert.Throws<InvalidOperationException>(() =>
            Commander.Instance.GetConcurrentCommandProcessorBuilder().SetMaxThreads(5).SetMaxThreads(5));
    }

    #endregion

    #region OnBeforeAnyJob

    [Fact]
    public void OnBeforeAnyJob()
    {
        IConcurrentCommandProcessorBuilder builder = Commander.Instance.GetConcurrentCommandProcessorBuilder()
            .OnBeforeAnyJob(_ => testOutputHelper.WriteLine("Testing OnBeforeAnyJob"));

        Assert.NotNull(builder);
    }

    [Fact]
    public void OnBeforeAnyJob_WhenNullDelegate()
    {
        Action<ICommand>? action = null;

        Assert.Throws<ArgumentNullException>(() =>
        {
            Commander.Instance.GetConcurrentCommandProcessorBuilder()
                .OnBeforeAnyJob(action!);
        });
    }

    [Fact]
    public void OnBeforeAnyJob_WhenThereIsAlreadyADelegateForThat()
    {
        Assert.Throws<InvalidOperationException>(() =>
        {
            Commander.Instance.GetConcurrentCommandProcessorBuilder()
                .OnBeforeAnyJob(_ => testOutputHelper.WriteLine("Testing OnBeforeAnyJob"))
                .OnBeforeAnyJob(_ => testOutputHelper.WriteLine("Testing OnBeforeAnyJob"));
        });
    }

    [Fact]
    public void OnBeforeAnyJobAsync()
    {
        IConcurrentCommandProcessorBuilder builder = Commander.Instance.GetConcurrentCommandProcessorBuilder()
            .OnBeforeAnyJob(async _ => await Task.Run(() => testOutputHelper.WriteLine("Testing OnBeforeAnyJob")));

        Assert.NotNull(builder);
    }

    [Fact]
    public void OnBeforeAnyJobAsync_WhenNullDelegate()
    {
        Func<ICommand, Task>? action = null;

        Assert.Throws<ArgumentNullException>(() =>
        {
            Commander.Instance.GetConcurrentCommandProcessorBuilder()
                .OnBeforeAnyJob(action!);
        });
    }

    [Fact]
    public void OnBeforeAnyJobAsync_WhenThereIsAlreadyADelegateForThat()
    {
        Assert.Throws<InvalidOperationException>(() =>
        {
            Commander.Instance.GetConcurrentCommandProcessorBuilder()
                .OnBeforeAnyJob(async _ => await Task.Run(() => testOutputHelper.WriteLine("Testing OnBeforeAnyJob")))
                .OnBeforeAnyJob(async _ => await Task.Run(() => testOutputHelper.WriteLine("Testing OnBeforeAnyJob")));
        });
    }

    #endregion

    #region OnAnyJobSuccess

    [Fact]
    public void OnAnyJobSuccess()
    {
        IConcurrentCommandProcessorBuilder builder = Commander.Instance.GetConcurrentCommandProcessorBuilder()
            .OnAnyJobSuccess(_ => testOutputHelper.WriteLine("Testing OnAnyJobSuccess"));

        Assert.NotNull(builder);
    }

    [Fact]
    public void OnAnyJobSuccess_WhenNullDelegate()
    {
        Action<IExecutedCommand>? action = null;

        Assert.Throws<ArgumentNullException>(() =>
        {
            Commander.Instance.GetConcurrentCommandProcessorBuilder()
                .OnAnyJobSuccess(action!);
        });
    }

    [Fact]
    public void OnAnyJobSuccess_WhenThereIsAlreadyADelegateForThat()
    {
        Assert.Throws<InvalidOperationException>(() =>
        {
            Commander.Instance.GetConcurrentCommandProcessorBuilder()
                .OnAnyJobSuccess(_ => testOutputHelper.WriteLine("Testing OnAnyJobSuccess"))
                .OnAnyJobSuccess(_ => testOutputHelper.WriteLine("Testing OnAnyJobSuccess"));
        });
    }

    [Fact]
    public void OnAnyJobSuccessAsync()
    {
        IConcurrentCommandProcessorBuilder builder = Commander.Instance.GetConcurrentCommandProcessorBuilder()
            .OnAnyJobSuccess(async _ => await Task.Run(() => testOutputHelper.WriteLine("Testing OnAnyJobSuccess")));

        Assert.NotNull(builder);
    }

    [Fact]
    public void OnAnyJobSuccessAsync_WhenNullDelegate()
    {
        Func<IExecutedCommand, Task>? action = null;

        Assert.Throws<ArgumentNullException>(() =>
        {
            Commander.Instance.GetConcurrentCommandProcessorBuilder()
                .OnAnyJobSuccess(action!);
        });
    }

    [Fact]
    public void OnAnyJobSuccessAsync_WhenThereIsAlreadyADelegateForThat()
    {
        Assert.Throws<InvalidOperationException>(() =>
        {
            Commander.Instance.GetConcurrentCommandProcessorBuilder()
                .OnAnyJobSuccess(async _ => await Task.Run(() => testOutputHelper.WriteLine("Testing OnAnyJobSuccess")))
                .OnAnyJobSuccess(async _ => await Task.Run(() => testOutputHelper.WriteLine("Testing OnAnyJobSuccess")));
        });
    }

    #endregion

    #region OnAnyJobFailure_WhenException

    [Fact]
    public void OnAnyJobFailure()
    {
        IConcurrentCommandProcessorBuilder builder = Commander.Instance.GetConcurrentCommandProcessorBuilder()
            .OnAnyJobFailure((Exception _, IExecutedCommand _) => testOutputHelper.WriteLine("Testing OnAnyJobFailure"));

        Assert.NotNull(builder);
    }

    [Fact]
    public void OnAnyJobFailure_WhenNullDelegate()
    {
        Action<Exception, IExecutedCommand>? action = null;

        Assert.Throws<ArgumentNullException>(() =>
        {
            Commander.Instance.GetConcurrentCommandProcessorBuilder()
                .OnAnyJobFailure(action!);
        });
    }

    [Fact]
    public void OnAnyJobFailure_WhenThereIsAlreadyADelegateForThat()
    {
        Assert.Throws<InvalidOperationException>(() =>
        {
            Commander.Instance.GetConcurrentCommandProcessorBuilder()
                .OnAnyJobFailure((Exception _, IExecutedCommand _) => testOutputHelper.WriteLine("Testing OnAnyJobFailure"))
                .OnAnyJobFailure((Exception _, IExecutedCommand _) => testOutputHelper.WriteLine("Testing OnAnyJobFailure"));
        });
    }

    [Fact]
    public void OnAnyJobFailureAsync()
    {
        IConcurrentCommandProcessorBuilder builder = Commander.Instance.GetConcurrentCommandProcessorBuilder()
            .OnAnyJobFailure(async (Exception _, IExecutedCommand _) =>
            {
                await Task.Run(() => testOutputHelper.WriteLine("Testing OnAfterAnyJob"));
            });

        Assert.NotNull(builder);
    }

    [Fact]
    public void OnAnyJobFailureAsync_WhenNullDelegate()
    {
        Func<Exception, IExecutedCommand, Task>? action = null;

        Assert.Throws<ArgumentNullException>(() =>
        {
            Commander.Instance.GetConcurrentCommandProcessorBuilder()
                .OnAnyJobFailure(action!);
        });
    }

    [Fact]
    public void OnAnyJobFailureAsync_WhenThereIsAlreadyADelegateForThat()
    {
        Assert.Throws<InvalidOperationException>(() =>
        {
            Commander.Instance.GetConcurrentCommandProcessorBuilder()
                .OnAnyJobFailure(async (Exception _, IExecutedCommand _) =>
                {
                    await Task.Run(() => testOutputHelper.WriteLine("Testing OnAfterAnyJob"));
                })
                .OnAnyJobFailure(async (Exception _, IExecutedCommand _) =>
                {
                    await Task.Run(() => testOutputHelper.WriteLine("Testing OnAfterAnyJob"));
                });
        });
    }

    #endregion

    #region OnAnyJobFailure_WhenExceptionDerivedType

    [Fact]
    public void OnAnyJobFailure_InvalidOperationException()
    {
        IConcurrentCommandProcessorBuilder builder = Commander.Instance.GetConcurrentCommandProcessorBuilder()
            .OnAnyJobFailure((InvalidOperationException _, IExecutedCommand _) => testOutputHelper.WriteLine("Testing OnAnyJobFailure"));

        Assert.NotNull(builder);
    }

    [Fact]
    public void OnAnyJobFailure_WhenNullDelegate_InvalidOperationException()
    {
        Action<InvalidOperationException, IExecutedCommand>? action = null;

        Assert.Throws<ArgumentNullException>(() =>
        {
            Commander.Instance.GetConcurrentCommandProcessorBuilder()
                .OnAnyJobFailure(action!);
        });
    }

    [Fact]
    public void OnAnyJobFailure_WhenThereIsAlreadyADelegateForThat_InvalidOperationException()
    {
        Assert.Throws<InvalidOperationException>(() =>
        {
            Commander.Instance.GetConcurrentCommandProcessorBuilder()
                .OnAnyJobFailure((InvalidOperationException _, IExecutedCommand _) => testOutputHelper.WriteLine("Testing OnAnyJobFailure"))
                .OnAnyJobFailure((InvalidOperationException _, IExecutedCommand _) => testOutputHelper.WriteLine("Testing OnAnyJobFailure"));
        });
    }

    [Fact]
    public void OnAnyJobFailureAsync_InvalidOperationException()
    {
        IConcurrentCommandProcessorBuilder builder = Commander.Instance.GetConcurrentCommandProcessorBuilder()
            .OnAnyJobFailure(async (InvalidOperationException _, IExecutedCommand _) =>
            {
                await Task.Run(() => testOutputHelper.WriteLine("Testing OnAfterAnyJob"));
            });

        Assert.NotNull(builder);
    }

    [Fact]
    public void OnAnyJobFailureAsync_WhenNullDelegate_InvalidOperationException()
    {
        Func<InvalidOperationException, IExecutedCommand, Task>? action = null;

        Assert.Throws<ArgumentNullException>(() =>
        {
            Commander.Instance.GetConcurrentCommandProcessorBuilder()
                .OnAnyJobFailure(action!);
        });
    }

    [Fact]
    public void OnAnyJobFailureAsync_WhenThereIsAlreadyADelegateForThat_InvalidOperationException()
    {
        Assert.Throws<InvalidOperationException>(() =>
        {
            Commander.Instance.GetConcurrentCommandProcessorBuilder()
                .OnAnyJobFailure(async (InvalidOperationException _, IExecutedCommand _) =>
                {
                    await Task.Run(() => testOutputHelper.WriteLine("Testing OnAfterAnyJob"));
                })
                .OnAnyJobFailure(async (InvalidOperationException _, IExecutedCommand _) =>
                {
                    await Task.Run(() => testOutputHelper.WriteLine("Testing OnAfterAnyJob"));
                });
        });
    }

    #endregion

    #region OnAfterAnyJob

    [Fact]
    public void OnAfterAnyJob()
    {
        IConcurrentCommandProcessorBuilder builder = Commander.Instance.GetConcurrentCommandProcessorBuilder()
            .OnAfterAnyJob(_ => testOutputHelper.WriteLine("Testing OnAfterAnyJob"));

        Assert.NotNull(builder);
    }

    [Fact]
    public void OnAfterAnyJob_WhenNullDelegate()
    {
        Action<IExecutedCommand>? action = null;

        Assert.Throws<ArgumentNullException>(() =>
        {
            Commander.Instance.GetConcurrentCommandProcessorBuilder()
                .OnAfterAnyJob(action!);
        });
    }

    [Fact]
    public void OnAfterAnyJob_WhenThereIsAlreadyADelegateForThat()
    {
        Assert.Throws<InvalidOperationException>(() =>
        {
            Commander.Instance.GetConcurrentCommandProcessorBuilder()
                .OnAfterAnyJob(_ => testOutputHelper.WriteLine("Testing OnAfterAnyJob"))
                .OnAfterAnyJob(_ => testOutputHelper.WriteLine("Testing OnAfterAnyJob"));
        });
    }

    [Fact]
    public void OnAfterAnyJobAsync()
    {
        IConcurrentCommandProcessorBuilder builder = Commander.Instance.GetConcurrentCommandProcessorBuilder()
            .OnAfterAnyJob(async _ => await Task.Run(() => testOutputHelper.WriteLine("Testing OnAfterAnyJob")));

        Assert.NotNull(builder);
    }

    [Fact]
    public void OnAfterAnyJobAsync_WhenNullDelegate()
    {
        Func<IExecutedCommand, Task>? action = null;

        Assert.Throws<ArgumentNullException>(() =>
        {
            Commander.Instance.GetConcurrentCommandProcessorBuilder()
                .OnAfterAnyJob(action!);
        });
    }

    [Fact]
    public void OnAfterAnyJobAsync_WhenThereIsAlreadyADelegateForThat()
    {
        Assert.Throws<InvalidOperationException>(() =>
        {
            Commander.Instance.GetConcurrentCommandProcessorBuilder()
                .OnAfterAnyJob(async _ => await Task.Run(() => testOutputHelper.WriteLine("Testing OnBeforeAnyJob")))
                .OnAfterAnyJob(async _ => await Task.Run(() => testOutputHelper.WriteLine("Testing OnBeforeAnyJob")));
        });
    }

    #endregion

    #region Build

    [Fact]
    public void Build_WhenOK()
    {
        ICommandProcessor processor = Commander.Instance.GetConcurrentCommandProcessorBuilder()
            .OnAnyJobFailure((Exception _, IExecutedCommand _) => testOutputHelper.WriteLine("Testing Build_WhenOK"))
            .Build();

        Assert.NotNull(processor);
    }

    [Fact]
    public void Build_WhenKO()
    {
        Assert.Throws<InvalidOperationException>(() => Commander.Instance.GetConcurrentCommandProcessorBuilder().Build());
    }

    #endregion
}
