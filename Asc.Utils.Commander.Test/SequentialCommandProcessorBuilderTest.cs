using Xunit.Abstractions;

namespace Asc.Utils.Commander.Test;

public class SequentialCommandProcessorBuilderTest(ITestOutputHelper testOutputHelper)
{
    #region OnBeforeAnyJob

    [Fact]
    public void OnBeforeAnyJob()
    {
        ISequentialCommandProcessorBuilder builder = Commander.Instance.GetSequentialCommandProcessorBuilder()
            .OnBeforeAnyJob(_ => testOutputHelper.WriteLine("Testing OnBeforeAnyJob"));

        Assert.NotNull(builder);
    }

    [Fact]
    public void OnBeforeAnyJob_WhenNullDelegate()
    {
        Action<ICommand>? action = null;

        Assert.Throws<ArgumentNullException>(() =>
        {
            Commander.Instance.GetSequentialCommandProcessorBuilder()
                .OnBeforeAnyJob(action!);
        });
    }

    [Fact]
    public void OnBeforeAnyJob_WhenThereIsAlreadyADelegateForThat()
    {
        Assert.Throws<InvalidOperationException>(() =>
        {
            Commander.Instance.GetSequentialCommandProcessorBuilder()
                .OnBeforeAnyJob(_ => testOutputHelper.WriteLine("Testing OnBeforeAnyJob"))
                .OnBeforeAnyJob(_ => testOutputHelper.WriteLine("Testing OnBeforeAnyJob"));
        });
    }

    [Fact]
    public void OnBeforeAnyJobAsync()
    {
        ISequentialCommandProcessorBuilder builder = Commander.Instance.GetSequentialCommandProcessorBuilder()
            .OnBeforeAnyJob(async _ => await Task.Run(() => testOutputHelper.WriteLine("Testing OnBeforeAnyJob")));

        Assert.NotNull(builder);
    }

    [Fact]
    public void OnBeforeAnyJobAsync_WhenNullDelegate()
    {
        Func<ICommand, Task>? action = null;

        Assert.Throws<ArgumentNullException>(() =>
        {
            Commander.Instance.GetSequentialCommandProcessorBuilder()
                .OnBeforeAnyJob(action!);
        });
    }

    [Fact]
    public void OnBeforeAnyJobAsync_WhenThereIsAlreadyADelegateForThat()
    {
        Assert.Throws<InvalidOperationException>(() =>
        {
            Commander.Instance.GetSequentialCommandProcessorBuilder()
                .OnBeforeAnyJob(async _ => await Task.Run(() => testOutputHelper.WriteLine("Testing OnBeforeAnyJob")))
                .OnBeforeAnyJob(async _ => await Task.Run(() => testOutputHelper.WriteLine("Testing OnBeforeAnyJob")));
        });
    }

    #endregion

    #region OnAnyJobSuccess

    [Fact]
    public void OnAnyJobSuccess()
    {
        ISequentialCommandProcessorBuilder builder = Commander.Instance.GetSequentialCommandProcessorBuilder()
            .OnAnyJobSuccess(_ => testOutputHelper.WriteLine("Testing OnAnyJobSuccess"));

        Assert.NotNull(builder);
    }

    [Fact]
    public void OnAnyJobSuccess_WhenNullDelegate()
    {
        Action<IExecutedCommand>? action = null;

        Assert.Throws<ArgumentNullException>(() =>
        {
            Commander.Instance.GetSequentialCommandProcessorBuilder()
                .OnAnyJobSuccess(action!);
        });
    }

    [Fact]
    public void OnAnyJobSuccess_WhenThereIsAlreadyADelegateForThat()
    {
        Assert.Throws<InvalidOperationException>(() =>
        {
            Commander.Instance.GetSequentialCommandProcessorBuilder()
                .OnAnyJobSuccess(_ => testOutputHelper.WriteLine("Testing OnAnyJobSuccess"))
                .OnAnyJobSuccess(_ => testOutputHelper.WriteLine("Testing OnAnyJobSuccess"));
        });
    }

    [Fact]
    public void OnAnyJobSuccessAsync()
    {
        ISequentialCommandProcessorBuilder builder = Commander.Instance.GetSequentialCommandProcessorBuilder()
            .OnAnyJobSuccess(async _ => await Task.Run(() => testOutputHelper.WriteLine("Testing OnAnyJobSuccess")));

        Assert.NotNull(builder);
    }

    [Fact]
    public void OnAnyJobSuccessAsync_WhenNullDelegate()
    {
        Func<IExecutedCommand, Task>? action = null;

        Assert.Throws<ArgumentNullException>(() =>
        {
            Commander.Instance.GetSequentialCommandProcessorBuilder()
                .OnAnyJobSuccess(action!);
        });
    }

    [Fact]
    public void OnAnyJobSuccessAsync_WhenThereIsAlreadyADelegateForThat()
    {
        Assert.Throws<InvalidOperationException>(() =>
        {
            Commander.Instance.GetSequentialCommandProcessorBuilder()
                .OnAnyJobSuccess(async _ => await Task.Run(() => testOutputHelper.WriteLine("Testing OnAnyJobSuccess")))
                .OnAnyJobSuccess(async _ => await Task.Run(() => testOutputHelper.WriteLine("Testing OnAnyJobSuccess")));
        });
    }

    #endregion

    #region OnAnyJobFailure_WhenException

    [Fact]
    public void OnAnyJobFailure()
    {
        ISequentialCommandProcessorBuilder builder = Commander.Instance.GetSequentialCommandProcessorBuilder()
            .OnAnyJobFailure((Exception _, IExecutedCommand _) => testOutputHelper.WriteLine("Testing OnAnyJobFailure"));

        Assert.NotNull(builder);
    }

    [Fact]
    public void OnAnyJobFailure_WhenNullDelegate()
    {
        Action<Exception, IExecutedCommand>? action = null;

        Assert.Throws<ArgumentNullException>(() =>
        {
            Commander.Instance.GetSequentialCommandProcessorBuilder()
                .OnAnyJobFailure(action!);
        });
    }

    [Fact]
    public void OnAnyJobFailure_WhenThereIsAlreadyADelegateForThat()
    {
        Assert.Throws<InvalidOperationException>(() =>
        {
            Commander.Instance.GetSequentialCommandProcessorBuilder()
                .OnAnyJobFailure((Exception _, IExecutedCommand _) => testOutputHelper.WriteLine("Testing OnAnyJobFailure"))
                .OnAnyJobFailure((Exception _, IExecutedCommand _) => testOutputHelper.WriteLine("Testing OnAnyJobFailure"));
        });
    }

    [Fact]
    public void OnAnyJobFailureAsync()
    {
        ISequentialCommandProcessorBuilder builder = Commander.Instance.GetSequentialCommandProcessorBuilder()
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
            Commander.Instance.GetSequentialCommandProcessorBuilder()
                .OnAnyJobFailure(action!);
        });
    }

    [Fact]
    public void OnAnyJobFailureAsync_WhenThereIsAlreadyADelegateForThat()
    {
        Assert.Throws<InvalidOperationException>(() =>
        {
            Commander.Instance.GetSequentialCommandProcessorBuilder()
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
        ISequentialCommandProcessorBuilder builder = Commander.Instance.GetSequentialCommandProcessorBuilder()
            .OnAnyJobFailure((InvalidOperationException _, IExecutedCommand _) => testOutputHelper.WriteLine("Testing OnAnyJobFailure"));

        Assert.NotNull(builder);
    }

    [Fact]
    public void OnAnyJobFailure_WhenNullDelegate_InvalidOperationException()
    {
        Action<InvalidOperationException, IExecutedCommand>? action = null;

        Assert.Throws<ArgumentNullException>(() =>
        {
            Commander.Instance.GetSequentialCommandProcessorBuilder()
                .OnAnyJobFailure(action!);
        });
    }

    [Fact]
    public void OnAnyJobFailure_WhenThereIsAlreadyADelegateForThat_InvalidOperationException()
    {
        Assert.Throws<InvalidOperationException>(() =>
        {
            Commander.Instance.GetSequentialCommandProcessorBuilder()
                .OnAnyJobFailure((InvalidOperationException _, IExecutedCommand _) => testOutputHelper.WriteLine("Testing OnAnyJobFailure"))
                .OnAnyJobFailure((InvalidOperationException _, IExecutedCommand _) => testOutputHelper.WriteLine("Testing OnAnyJobFailure"));
        });
    }

    [Fact]
    public void OnAnyJobFailureAsync_InvalidOperationException()
    {
        ISequentialCommandProcessorBuilder builder = Commander.Instance.GetSequentialCommandProcessorBuilder()
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
            Commander.Instance.GetSequentialCommandProcessorBuilder()
                .OnAnyJobFailure(action!);
        });
    }

    [Fact]
    public void OnAnyJobFailureAsync_WhenThereIsAlreadyADelegateForThat_InvalidOperationException()
    {
        Assert.Throws<InvalidOperationException>(() =>
        {
            Commander.Instance.GetSequentialCommandProcessorBuilder()
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
        ISequentialCommandProcessorBuilder builder = Commander.Instance.GetSequentialCommandProcessorBuilder()
            .OnAfterAnyJob(_ => testOutputHelper.WriteLine("Testing OnAfterAnyJob"));

        Assert.NotNull(builder);
    }

    [Fact]
    public void OnAfterAnyJob_WhenNullDelegate()
    {
        Action<IExecutedCommand>? action = null;

        Assert.Throws<ArgumentNullException>(() =>
        {
            Commander.Instance.GetSequentialCommandProcessorBuilder()
                .OnAfterAnyJob(action!);
        });
    }

    [Fact]
    public void OnAfterAnyJob_WhenThereIsAlreadyADelegateForThat()
    {
        Assert.Throws<InvalidOperationException>(() =>
        {
            Commander.Instance.GetSequentialCommandProcessorBuilder()
                .OnAfterAnyJob(_ => testOutputHelper.WriteLine("Testing OnAfterAnyJob"))
                .OnAfterAnyJob(_ => testOutputHelper.WriteLine("Testing OnAfterAnyJob"));
        });
    }

    [Fact]
    public void OnAfterAnyJobAsync()
    {
        ISequentialCommandProcessorBuilder builder = Commander.Instance.GetSequentialCommandProcessorBuilder()
            .OnAfterAnyJob(async _ => await Task.Run(() => testOutputHelper.WriteLine("Testing OnAfterAnyJob")));

        Assert.NotNull(builder);
    }

    [Fact]
    public void OnAfterAnyJobAsync_WhenNullDelegate()
    {
        Func<IExecutedCommand, Task>? action = null;

        Assert.Throws<ArgumentNullException>(() =>
        {
            Commander.Instance.GetSequentialCommandProcessorBuilder()
                .OnAfterAnyJob(action!);
        });
    }

    [Fact]
    public void OnAfterAnyJobAsync_WhenThereIsAlreadyADelegateForThat()
    {
        Assert.Throws<InvalidOperationException>(() =>
        {
            Commander.Instance.GetSequentialCommandProcessorBuilder()
                .OnAfterAnyJob(async _ => await Task.Run(() => testOutputHelper.WriteLine("Testing OnBeforeAnyJob")))
                .OnAfterAnyJob(async _ => await Task.Run(() => testOutputHelper.WriteLine("Testing OnBeforeAnyJob")));
        });
    }

    #endregion

    #region Build

    [Fact]
    public void Build_WhenOK()
    {
        ICommandProcessor processor = Commander.Instance.GetSequentialCommandProcessorBuilder()
            .OnAnyJobFailure((Exception _, IExecutedCommand _) => testOutputHelper.WriteLine("Testing Build_WhenOK"))
            .Build();

        Assert.NotNull(processor);
    }

    [Fact]
    public void Build_WhenKO()
    {
        Assert.Throws<InvalidOperationException>(() => Commander.Instance.GetSequentialCommandProcessorBuilder().Build());
    }

    #endregion
}

