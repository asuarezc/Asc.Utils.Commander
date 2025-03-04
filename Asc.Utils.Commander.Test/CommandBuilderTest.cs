using Xunit.Abstractions;

namespace Asc.Utils.Commander.Test;

public class CommandBuilderTest(ITestOutputHelper testOutputHelper)
{
    #region When null stuff

    [Fact]
    public void NullJob()
    {
        Action? job = null;
        ArgumentNullException exception = Assert.Throws<ArgumentNullException>(() => Commander.Instance.GetCommandBuilder().Job(job!));
        Assert.Equal("Value cannot be null. (Parameter 'job')", exception.Message);
        Assert.Equal("job", exception.ParamName);
    }

    [Fact]
    public void NullAsyncJob()
    {
        ArgumentNullException exception = Assert.Throws<ArgumentNullException>(() => Commander.Instance.GetCommandBuilder().Job(null!));
        Assert.Equal("Value cannot be null. (Parameter 'job')", exception.Message);
        Assert.Equal("job", exception.ParamName);
    }

    [Fact]
    public void GenericNullJob()
    {
        Func<string>? job = null;
        ArgumentNullException exception = Assert.Throws<ArgumentNullException>(() => Commander.Instance.GetCommandBuilder<string>().Job(job!));
        Assert.Equal("Value cannot be null. (Parameter 'job')", exception.Message);
        Assert.Equal("job", exception.ParamName);
    }

    [Fact]
    public void GenericNullAsyncJob()
    {
        Func<Task<string>>? job = null;
        ArgumentNullException exception = Assert.Throws<ArgumentNullException>(() => Commander.Instance.GetCommandBuilder<string>().Job(job!));
        Assert.Equal("Value cannot be null. (Parameter 'job')", exception.Message);
        Assert.Equal("job", exception.ParamName);
    }

    [Fact]
    public void NullOnSuccess()
    {
        Action? onSuccess = null;
        ArgumentNullException exception = Assert.Throws<ArgumentNullException>(() => Commander.Instance.GetCommandBuilder().OnSuccess(onSuccess!));
        Assert.Equal("Value cannot be null. (Parameter 'onSuccess')", exception.Message);
        Assert.Equal("onSuccess", exception.ParamName);
    }

    [Fact]
    public void NullAsyncJOnSuccess()
    {
        ArgumentNullException exception = Assert.Throws<ArgumentNullException>(() => Commander.Instance.GetCommandBuilder().OnSuccess(null!));
        Assert.Equal("Value cannot be null. (Parameter 'onSuccess')", exception.Message);
        Assert.Equal("onSuccess", exception.ParamName);
    }

    [Fact]
    public void GenericNullOnSuccess()
    {
        Action<string>? onSuccess = null;
        ArgumentNullException exception = Assert.Throws<ArgumentNullException>(() => Commander.Instance.GetCommandBuilder<string>().OnSuccess(onSuccess!));
        Assert.Equal("Value cannot be null. (Parameter 'onSuccess')", exception.Message);
        Assert.Equal("onSuccess", exception.ParamName);
    }

    [Fact]
    public void GenericNullAsyncOnSuccess()
    {
        Func<string, Task>? onSuccess = null;
        ArgumentNullException exception = Assert.Throws<ArgumentNullException>(() => Commander.Instance.GetCommandBuilder<string>().OnSuccess(onSuccess!));
        Assert.Equal("Value cannot be null. (Parameter 'onSuccess')", exception.Message);
        Assert.Equal("onSuccess", exception.ParamName);
    }

    [Fact]
    public void NullOnFailure()
    {
        Action<Exception>? onFailure = null;
        ArgumentNullException exception = Assert.Throws<ArgumentNullException>(() => Commander.Instance.GetCommandBuilder().OnFailure(onFailure!));
        Assert.Equal("Value cannot be null. (Parameter 'onFailure')", exception.Message);
        Assert.Equal("onFailure", exception.ParamName);
    }

    [Fact]
    public void NullAsyncOnFailure()
    {
        ArgumentNullException exception = Assert.Throws<ArgumentNullException>(() => Commander.Instance.GetCommandBuilder().OnFailure<Exception>(null!));
        Assert.Equal("Value cannot be null. (Parameter 'onFailure')", exception.Message);
        Assert.Equal("onFailure", exception.ParamName);
    }

    [Fact]
    public void GenericNullOnFailure()
    {
        Action<Exception>? onFailure = null;
        ArgumentNullException exception = Assert.Throws<ArgumentNullException>(() => Commander.Instance.GetCommandBuilder<string>().OnFailure(onFailure!));
        Assert.Equal("Value cannot be null. (Parameter 'onFailure')", exception.Message);
        Assert.Equal("onFailure", exception.ParamName);
    }

    [Fact]
    public void GenericNullAsyncOnFailure()
    {
        Func<Exception, Task>? onFailure = null;
        ArgumentNullException exception = Assert.Throws<ArgumentNullException>(() => Commander.Instance.GetCommandBuilder<string>().OnFailure(onFailure!));
        Assert.Equal("Value cannot be null. (Parameter 'onFailure')", exception.Message);
        Assert.Equal("onFailure", exception.ParamName);
    }

    [Fact]
    public void NullOnFinally()
    {
        Action? onFinally = null;
        ArgumentNullException exception = Assert.Throws<ArgumentNullException>(() => Commander.Instance.GetCommandBuilder().OnFinally(onFinally!));
        Assert.Equal("Value cannot be null. (Parameter 'onFinally')", exception.Message);
        Assert.Equal("onFinally", exception.ParamName);
    }

    [Fact]
    public void NullAsyncOnFinally()
    {
        ArgumentNullException exception = Assert.Throws<ArgumentNullException>(() => Commander.Instance.GetCommandBuilder().OnFinally(null!));
        Assert.Equal("Value cannot be null. (Parameter 'onFinally')", exception.Message);
        Assert.Equal("onFinally", exception.ParamName);
    }

    [Fact]
    public void GenericNullOnFinally()
    {
        Action? onFinally = null;
        ArgumentNullException exception = Assert.Throws<ArgumentNullException>(() => Commander.Instance.GetCommandBuilder<string>().OnFinally(onFinally!));
        Assert.Equal("Value cannot be null. (Parameter 'onFinally')", exception.Message);
        Assert.Equal("onFinally", exception.ParamName);
    }

    [Fact]
    public void GenericNullAsyncOnFinally()
    {
        ArgumentNullException exception = Assert.Throws<ArgumentNullException>(() => Commander.Instance.GetCommandBuilder<string>().OnFinally(null!));
        Assert.Equal("Value cannot be null. (Parameter 'onFinally')", exception.Message);
        Assert.Equal("onFinally", exception.ParamName);
    }

    [Fact]
    public void AddOrReplaceParameter_WhenNullKey()
    {
        ArgumentNullException exception = Assert.Throws<ArgumentNullException>(() =>
            Commander.Instance.GetCommandBuilder().AddOrReplaceParameter<string>(null!, null!)
        );
        Assert.Equal("Value cannot be null. (Parameter 'key')", exception.Message);
        Assert.Equal("key", exception.ParamName);
    }

    [Fact]
    public void GenericAddOrReplaceParameter_WhenNullKey()
    {
        ArgumentNullException exception = Assert.Throws<ArgumentNullException>(() =>
            Commander.Instance.GetCommandBuilder<string>().AddOrReplaceParameter<string>(null!, null!)
        );
        Assert.Equal("Value cannot be null. (Parameter 'key')", exception.Message);
        Assert.Equal("key", exception.ParamName);
    }

    [Fact]
    public void AddOrReplaceParameter_WhenKeyIsEmpty()
    {
        ArgumentNullException exception = Assert.Throws<ArgumentNullException>(() =>
            Commander.Instance.GetCommandBuilder().AddOrReplaceParameter<string>(string.Empty, null!)
        );
        Assert.Equal("Value cannot be null. (Parameter 'key')", exception.Message);
        Assert.Equal("key", exception.ParamName);
    }

    [Fact]
    public void GenericAddOrReplaceParameter_WhenKeyIsEmpty()
    {
        ArgumentNullException exception = Assert.Throws<ArgumentNullException>(() =>
            Commander.Instance.GetCommandBuilder<string>().AddOrReplaceParameter<string>(string.Empty, null!)
        );
        Assert.Equal("Value cannot be null. (Parameter 'key')", exception.Message);
        Assert.Equal("key", exception.ParamName);
    }

    [Fact]
    public void SetId_WhenIdIsNull()
    {
        ArgumentNullException exception = Assert.Throws<ArgumentNullException>(() =>
            Commander.Instance.GetCommandBuilder().SetId(null!)
        );
        Assert.Equal("Value cannot be null. (Parameter 'commandId')", exception.Message);
        Assert.Equal("commandId", exception.ParamName);
    }

    [Fact]
    public void GenericSetId_WhenIdIsNull()
    {
        ArgumentNullException exception = Assert.Throws<ArgumentNullException>(() =>
            Commander.Instance.GetCommandBuilder<string>().SetId(null!)
        );
        Assert.Equal("Value cannot be null. (Parameter 'commandId')", exception.Message);
        Assert.Equal("commandId", exception.ParamName);
    }

    [Fact]
    public void SetId_WhenIdIsEmpty()
    {
        ArgumentNullException exception = Assert.Throws<ArgumentNullException>(() =>
            Commander.Instance.GetCommandBuilder().SetId(string.Empty)
        );
        Assert.Equal("Value cannot be null. (Parameter 'commandId')", exception.Message);
        Assert.Equal("commandId", exception.ParamName);
    }

    [Fact]
    public void GenericSetId_WhenIdIsEmpty()
    {
        ArgumentNullException exception = Assert.Throws<ArgumentNullException>(() =>
            Commander.Instance.GetCommandBuilder<string>().SetId(string.Empty)
        );
        Assert.Equal("Value cannot be null. (Parameter 'commandId')", exception.Message);
        Assert.Equal("commandId", exception.ParamName);
    }

    #endregion

    #region Mandatory Stuff

    [Fact]
    public void JobIsMandatory()
    {
        ICommandBuilder commandBuilder = Commander.Instance.GetCommandBuilder();

        InvalidOperationException exception = Assert.Throws<InvalidOperationException>(() => commandBuilder.Build());
        Assert.Equal("Job delegate is mandatory", exception.Message);
    }

    [Fact]
    public void IdIsMandatory()
    {
        ICommandBuilder commandBuilder = Commander.Instance.GetCommandBuilder()
            .Job(() => testOutputHelper.WriteLine("Testing IdIsMandatory"));

        InvalidOperationException exception = Assert.Throws<InvalidOperationException>(() => commandBuilder.Build());
        Assert.Equal("Id is mandatory", exception.Message);
    }

    [Fact]
    public void GenericJobIsMandatory()
    {
        ICommandBuilder<string> commandBuilder = Commander.Instance.GetCommandBuilder<string>();

        InvalidOperationException exception = Assert.Throws<InvalidOperationException>(() => commandBuilder.Build());
        Assert.Equal("Job delegate is mandatory", exception.Message);
    }

    [Fact]
    public void GenericIdIsMandatory()
    {
        ICommandBuilder<string> commandBuilder = Commander.Instance.GetCommandBuilder<string>()
            .Job(() => string.Empty)
            .OnSuccess(result => testOutputHelper.WriteLine($"Result is {result}"));

        InvalidOperationException exception = Assert.Throws<InvalidOperationException>(() => commandBuilder.Build());
        Assert.Equal("Id is mandatory", exception.Message);
    }

    [Fact]
    public void OnSuccessIsMandatoryWhenGenericCommand()
    {
        ICommandBuilder<string> commandBuilder = Commander.Instance.GetCommandBuilder<string>()
            .Job(() => string.Empty)
            .SetId("test");

        InvalidOperationException exception = Assert.Throws<InvalidOperationException>(() => commandBuilder.Build());
        Assert.Equal("On success delegate is mandatory", exception.Message);
    }

    #endregion

    #region Duplicate stuff

    [Fact]
    public void DuplicateJob_1()
    {
        InvalidOperationException exception = Assert.Throws<InvalidOperationException>(() =>
            Commander.Instance.GetCommandBuilder()
                .Job(() => testOutputHelper.WriteLine("Testing DuplicateJob"))
                .Job(() => testOutputHelper.WriteLine("Testing DuplicateJob"))
        );

        Assert.Equal("There is already a delegate for that", exception.Message);
    }

    [Fact]
    public void DuplicateJob_2()
    {
        InvalidOperationException exception = Assert.Throws<InvalidOperationException>(() =>
            Commander.Instance.GetCommandBuilder()
                .Job(() => testOutputHelper.WriteLine("Testing DuplicateJob"))
                .Job(async () => await Task.Run(() => testOutputHelper.WriteLine("Testing DuplicateJob")))
        );

        Assert.Equal("There is already a delegate for that", exception.Message);
    }

    [Fact]
    public void DuplicateJob_3()
    {
        InvalidOperationException exception = Assert.Throws<InvalidOperationException>(() =>
            Commander.Instance.GetCommandBuilder()
                .Job(async () => await Task.Run(() => testOutputHelper.WriteLine("Testing DuplicateJob")))
                .Job(() => testOutputHelper.WriteLine("Testing DuplicateJob"))
        );

        Assert.Equal("There is already a delegate for that", exception.Message);
    }

    [Fact]
    public void DuplicateJob_4()
    {
        InvalidOperationException exception = Assert.Throws<InvalidOperationException>(() =>
            Commander.Instance.GetCommandBuilder()
                .Job(async () => await Task.Run(() => testOutputHelper.WriteLine("Testing DuplicateJob")))
                .Job(async () => await Task.Run(() => testOutputHelper.WriteLine("Testing DuplicateJob")))
        );

        Assert.Equal("There is already a delegate for that", exception.Message);
    }

    [Fact]
    public void GenericDuplicateJob_1()
    {
        InvalidOperationException exception = Assert.Throws<InvalidOperationException>(() =>
            Commander.Instance.GetCommandBuilder<string>()
                .Job(() => string.Empty)
                .Job(() => string.Empty)
        );

        Assert.Equal("There is already a delegate for that", exception.Message);
    }

    [Fact]
    public void GenericDuplicateJob_2()
    {
        InvalidOperationException exception = Assert.Throws<InvalidOperationException>(() =>
            Commander.Instance.GetCommandBuilder<string>()
                .Job(() => string.Empty)
                .Job(async () =>
                {
                    await Task.Run(() => testOutputHelper.WriteLine("Testing DuplicateJob"));
                    return string.Empty;
                })
        );

        Assert.Equal("There is already a delegate for that", exception.Message);
    }

    [Fact]
    public void GenericDuplicateJob_3()
    {
        InvalidOperationException exception = Assert.Throws<InvalidOperationException>(() =>
            Commander.Instance.GetCommandBuilder<string>()
                .Job(async () =>
                {
                    await Task.Run(() => testOutputHelper.WriteLine("Testing DuplicateJob"));
                    return string.Empty;
                })
                .Job(() => string.Empty)
        );

        Assert.Equal("There is already a delegate for that", exception.Message);
    }

    [Fact]
    public void GenericDuplicateJob_4()
    {
        InvalidOperationException exception = Assert.Throws<InvalidOperationException>(() =>
            Commander.Instance.GetCommandBuilder<string>()
                .Job(async () =>
                {
                    await Task.Run(() => testOutputHelper.WriteLine("Testing DuplicateJob"));
                    return string.Empty;
                })
                .Job(async () =>
                {
                    await Task.Run(() => testOutputHelper.WriteLine("Testing DuplicateJob"));
                    return string.Empty;
                })
        );

        Assert.Equal("There is already a delegate for that", exception.Message);
    }

    [Fact]
    public void DuplicateOnSuccess_1()
    {
        InvalidOperationException exception = Assert.Throws<InvalidOperationException>(() =>
            Commander.Instance.GetCommandBuilder()
                .OnSuccess(() => testOutputHelper.WriteLine("Testing DuplicateOnSuccess"))
                .OnSuccess(() => testOutputHelper.WriteLine("Testing DuplicateOnSuccess"))
        );

        Assert.Equal("There is already a delegate for that", exception.Message);
    }

    [Fact]
    public void DuplicateOnSuccess_2()
    {
        InvalidOperationException exception = Assert.Throws<InvalidOperationException>(() =>
            Commander.Instance.GetCommandBuilder()
                .OnSuccess(() => testOutputHelper.WriteLine("Testing DuplicateOnSuccess"))
                .OnSuccess(async () => await Task.Run(() => testOutputHelper.WriteLine("Testing DuplicateOnSuccess")))
        );

        Assert.Equal("There is already a delegate for that", exception.Message);
    }

    [Fact]
    public void DuplicateOnSuccess_3()
    {
        InvalidOperationException exception = Assert.Throws<InvalidOperationException>(() =>
            Commander.Instance.GetCommandBuilder()
                .OnSuccess(async () => await Task.Run(() => testOutputHelper.WriteLine("Testing DuplicateOnSuccess")))
                .OnSuccess(() => testOutputHelper.WriteLine("Testing DuplicateOnSuccess"))
        );

        Assert.Equal("There is already a delegate for that", exception.Message);
    }

    [Fact]
    public void DuplicateOnSuccess_4()
    {
        InvalidOperationException exception = Assert.Throws<InvalidOperationException>(() =>
            Commander.Instance.GetCommandBuilder()
                .OnSuccess(async () => await Task.Run(() => testOutputHelper.WriteLine("Testing DuplicateOnSuccess")))
                .OnSuccess(async () => await Task.Run(() => testOutputHelper.WriteLine("Testing DuplicateOnSuccess")))
        );

        Assert.Equal("There is already a delegate for that", exception.Message);
    }

    [Fact]
    public void GenericDuplicateOnSuccess_1()
    {
        InvalidOperationException exception = Assert.Throws<InvalidOperationException>(() =>
            Commander.Instance.GetCommandBuilder<string>()
                .OnSuccess(_ => testOutputHelper.WriteLine("Testing DuplicateOnSuccess"))
                .OnSuccess(_ => testOutputHelper.WriteLine("Testing DuplicateOnSuccess"))
        );

        Assert.Equal("There is already a delegate for that", exception.Message);
    }

    [Fact]
    public void GenericDuplicateOnSuccess_2()
    {
        InvalidOperationException exception = Assert.Throws<InvalidOperationException>(() =>
            Commander.Instance.GetCommandBuilder<string>()
                .OnSuccess(_ => testOutputHelper.WriteLine("Testing DuplicateOnSuccess"))
                .OnSuccess(async _ => await Task.Run(() => testOutputHelper.WriteLine("Testing DuplicateOnSuccess")))
        );

        Assert.Equal("There is already a delegate for that", exception.Message);
    }

    [Fact]
    public void GenericDuplicateOnSuccess_3()
    {
        InvalidOperationException exception = Assert.Throws<InvalidOperationException>(() =>
            Commander.Instance.GetCommandBuilder<string>()
                .OnSuccess(async _ => await Task.Run(() => testOutputHelper.WriteLine("Testing DuplicateOnSuccess")))
                .OnSuccess(_ => testOutputHelper.WriteLine("Testing DuplicateOnSuccess"))
        );

        Assert.Equal("There is already a delegate for that", exception.Message);
    }

    [Fact]
    public void GenericDuplicateOnSuccess_4()
    {
        InvalidOperationException exception = Assert.Throws<InvalidOperationException>(() =>
            Commander.Instance.GetCommandBuilder<string>()
                .OnSuccess(async _ => await Task.Run(() => testOutputHelper.WriteLine("Testing DuplicateOnSuccess")))
                .OnSuccess(async _ => await Task.Run(() => testOutputHelper.WriteLine("Testing DuplicateOnSuccess")))
        );

        Assert.Equal("There is already a delegate for that", exception.Message);
    }

    [Fact]
    public void DuplicateOnFailure_1()
    {
        InvalidOperationException exception = Assert.Throws<InvalidOperationException>(() =>
            Commander.Instance.GetCommandBuilder()
                .OnFailure((Exception _) => testOutputHelper.WriteLine("Testing DuplicateOnFailure"))
                .OnFailure((Exception _) => testOutputHelper.WriteLine("Testing DuplicateOnFailure"))
        );

        Assert.Equal("There is already a delegate for that", exception.Message);
    }

    [Fact]
    public void DuplicateOnFailure_2()
    {
        InvalidOperationException exception = Assert.Throws<InvalidOperationException>(() =>
            Commander.Instance.GetCommandBuilder()
                .OnFailure((Exception _) => testOutputHelper.WriteLine("Testing DuplicateOnFailure"))
                .OnFailure(async (Exception _) => await Task.Run(() => testOutputHelper.WriteLine("Testing DuplicateOnFailure")))
        );

        Assert.Equal("There is already a delegate for that", exception.Message);
    }

    [Fact]
    public void DuplicateOnFailure_3()
    {
        InvalidOperationException exception = Assert.Throws<InvalidOperationException>(() =>
            Commander.Instance.GetCommandBuilder()
                .OnFailure(async (Exception _) => await Task.Run(() => testOutputHelper.WriteLine("Testing DuplicateOnFailure")))
                .OnFailure((Exception _) => testOutputHelper.WriteLine("Testing DuplicateOnFailure"))
        );

        Assert.Equal("There is already a delegate for that", exception.Message);
    }

    [Fact]
    public void DuplicateOnFailure_4()
    {
        InvalidOperationException exception = Assert.Throws<InvalidOperationException>(() =>
            Commander.Instance.GetCommandBuilder()
                .OnFailure(async (Exception _) => await Task.Run(() => testOutputHelper.WriteLine("Testing DuplicateOnFailure")))
                .OnFailure(async (Exception _) => await Task.Run(() => testOutputHelper.WriteLine("Testing DuplicateOnFailure")))
        );

        Assert.Equal("There is already a delegate for that", exception.Message);
    }

    [Fact]
    public void GenericDuplicateOnFailure_1()
    {
        InvalidOperationException exception = Assert.Throws<InvalidOperationException>(() =>
            Commander.Instance.GetCommandBuilder<string>()
                .OnFailure((Exception _) => testOutputHelper.WriteLine("Testing DuplicateOnFailure"))
                .OnFailure((Exception _) => testOutputHelper.WriteLine("Testing DuplicateOnFailure"))
        );

        Assert.Equal("There is already a delegate for that", exception.Message);
    }

    [Fact]
    public void GenericDuplicateOnFailure_2()
    {
        InvalidOperationException exception = Assert.Throws<InvalidOperationException>(() =>
            Commander.Instance.GetCommandBuilder<string>()
                .OnFailure((Exception _) => testOutputHelper.WriteLine("Testing DuplicateOnFailure"))
                .OnFailure(async (Exception _) => await Task.Run(() => testOutputHelper.WriteLine("Testing DuplicateOnFailure")))
        );

        Assert.Equal("There is already a delegate for that", exception.Message);
    }

    [Fact]
    public void GenericDuplicateOnFailure_3()
    {
        InvalidOperationException exception = Assert.Throws<InvalidOperationException>(() =>
            Commander.Instance.GetCommandBuilder<string>()
                .OnFailure(async (Exception _) => await Task.Run(() => testOutputHelper.WriteLine("Testing DuplicateOnFailure")))
                .OnFailure((Exception _) => testOutputHelper.WriteLine("Testing DuplicateOnFailure"))
        );

        Assert.Equal("There is already a delegate for that", exception.Message);
    }

    [Fact]
    public void GenericDuplicateOnFailure_4()
    {
        InvalidOperationException exception = Assert.Throws<InvalidOperationException>(() =>
            Commander.Instance.GetCommandBuilder<string>()
                .OnFailure(async (Exception _) => await Task.Run(() => testOutputHelper.WriteLine("Testing DuplicateOnFailure")))
                .OnFailure(async (Exception _) => await Task.Run(() => testOutputHelper.WriteLine("Testing DuplicateOnFailure")))
        );

        Assert.Equal("There is already a delegate for that", exception.Message);
    }

    [Fact]
    public void DuplicateOnFailure_DerivedType_1()
    {
        InvalidOperationException exception = Assert.Throws<InvalidOperationException>(() =>
            Commander.Instance.GetCommandBuilder()
                .OnFailure((FormatException _) => testOutputHelper.WriteLine("Testing DuplicateOnFailure"))
                .OnFailure((FormatException _) => testOutputHelper.WriteLine("Testing DuplicateOnFailure"))
        );

        Assert.Equal("There is already a delegate for that", exception.Message);
    }

    [Fact]
    public void DuplicateOnFailure_DerivedType_2()
    {
        InvalidOperationException exception = Assert.Throws<InvalidOperationException>(() =>
            Commander.Instance.GetCommandBuilder()
                .OnFailure((FormatException _) => testOutputHelper.WriteLine("Testing DuplicateOnFailure"))
                .OnFailure(async (FormatException _) => await Task.Run(() => testOutputHelper.WriteLine("Testing DuplicateOnFailure")))
        );

        Assert.Equal("There is already a delegate for that", exception.Message);
    }

    [Fact]
    public void DuplicateOnFailure_DerivedType_3()
    {
        InvalidOperationException exception = Assert.Throws<InvalidOperationException>(() =>
            Commander.Instance.GetCommandBuilder()
                .OnFailure(async (FormatException _) => await Task.Run(() => testOutputHelper.WriteLine("Testing DuplicateOnFailure")))
                .OnFailure((FormatException _) => testOutputHelper.WriteLine("Testing DuplicateOnFailure"))
        );

        Assert.Equal("There is already a delegate for that", exception.Message);
    }

    [Fact]
    public void DuplicateOnFailure_DerivedType_4()
    {
        InvalidOperationException exception = Assert.Throws<InvalidOperationException>(() =>
            Commander.Instance.GetCommandBuilder()
                .OnFailure(async (FormatException _) => await Task.Run(() => testOutputHelper.WriteLine("Testing DuplicateOnFailure")))
                .OnFailure(async (FormatException _) => await Task.Run(() => testOutputHelper.WriteLine("Testing DuplicateOnFailure")))
        );

        Assert.Equal("There is already a delegate for that", exception.Message);
    }

    [Fact]
    public void GenericDuplicateOnFailure_DerivedType_1()
    {
        InvalidOperationException exception = Assert.Throws<InvalidOperationException>(() =>
            Commander.Instance.GetCommandBuilder<string>()
                .OnFailure((FormatException _) => testOutputHelper.WriteLine("Testing DuplicateOnFailure"))
                .OnFailure((FormatException _) => testOutputHelper.WriteLine("Testing DuplicateOnFailure"))
        );

        Assert.Equal("There is already a delegate for that", exception.Message);
    }

    [Fact]
    public void GenericDuplicateOnFailure_DerivedType_2()
    {
        InvalidOperationException exception = Assert.Throws<InvalidOperationException>(() =>
            Commander.Instance.GetCommandBuilder<string>()
                .OnFailure((FormatException _) => testOutputHelper.WriteLine("Testing DuplicateOnFailure"))
                .OnFailure(async (FormatException _) => await Task.Run(() => testOutputHelper.WriteLine("Testing DuplicateOnFailure")))
        );

        Assert.Equal("There is already a delegate for that", exception.Message);
    }

    [Fact]
    public void GenericDuplicateOnFailure_DerivedType_3()
    {
        InvalidOperationException exception = Assert.Throws<InvalidOperationException>(() =>
            Commander.Instance.GetCommandBuilder<string>()
                .OnFailure(async (FormatException _) => await Task.Run(() => testOutputHelper.WriteLine("Testing DuplicateOnFailure")))
                .OnFailure((FormatException _) => testOutputHelper.WriteLine("Testing DuplicateOnFailure"))
        );

        Assert.Equal("There is already a delegate for that", exception.Message);
    }

    [Fact]
    public void GenericDuplicateOnFailure_DerivedT_4()
    {
        InvalidOperationException exception = Assert.Throws<InvalidOperationException>(() =>
            Commander.Instance.GetCommandBuilder<string>()
                .OnFailure(async (FormatException _) => await Task.Run(() => testOutputHelper.WriteLine("Testing DuplicateOnFailure")))
                .OnFailure(async (FormatException _) => await Task.Run(() => testOutputHelper.WriteLine("Testing DuplicateOnFailure")))
        );

        Assert.Equal("There is already a delegate for that", exception.Message);
    }

    [Fact]
    public void DuplicateOnFinally_1()
    {
        InvalidOperationException exception = Assert.Throws<InvalidOperationException>(() =>
            Commander.Instance.GetCommandBuilder()
                .OnFinally(() => testOutputHelper.WriteLine("Testing DuplicateOnFinally"))
                .OnFinally(() => testOutputHelper.WriteLine("Testing DuplicateOnFinally"))
        );

        Assert.Equal("There is already a delegate for that", exception.Message);
    }

    [Fact]
    public void DuplicateOnFinally_2()
    {
        InvalidOperationException exception = Assert.Throws<InvalidOperationException>(() =>
            Commander.Instance.GetCommandBuilder()
                .OnFinally(() => testOutputHelper.WriteLine("Testing DuplicateOnFinally"))
                .OnFinally(async () => await Task.Run(() => testOutputHelper.WriteLine("Testing DuplicateOnFinally")))
        );

        Assert.Equal("There is already a delegate for that", exception.Message);
    }

    [Fact]
    public void DuplicateOnFinally_3()
    {
        InvalidOperationException exception = Assert.Throws<InvalidOperationException>(() =>
            Commander.Instance.GetCommandBuilder()
                .OnFinally(async () => await Task.Run(() => testOutputHelper.WriteLine("Testing DuplicateOnFinally")))
                .OnFinally(() => testOutputHelper.WriteLine("Testing DuplicateOnFinally"))
        );

        Assert.Equal("There is already a delegate for that", exception.Message);
    }

    [Fact]
    public void DuplicateOnFinally_4()
    {
        InvalidOperationException exception = Assert.Throws<InvalidOperationException>(() =>
            Commander.Instance.GetCommandBuilder()
                .OnFinally(async () => await Task.Run(() => testOutputHelper.WriteLine("Testing DuplicateOnFinally")))
                .OnFinally(async () => await Task.Run(() => testOutputHelper.WriteLine("Testing DuplicateOnFinally")))
        );

        Assert.Equal("There is already a delegate for that", exception.Message);
    }

    [Fact]
    public void GenericDuplicateOnFinally_1()
    {
        InvalidOperationException exception = Assert.Throws<InvalidOperationException>(() =>
            Commander.Instance.GetCommandBuilder<string>()
                .OnFinally(() => testOutputHelper.WriteLine("Testing DuplicateOnFinally"))
                .OnFinally(() => testOutputHelper.WriteLine("Testing DuplicateOnFinally"))
        );

        Assert.Equal("There is already a delegate for that", exception.Message);
    }

    [Fact]
    public void GenericDuplicateOnFinally_2()
    {
        InvalidOperationException exception = Assert.Throws<InvalidOperationException>(() =>
            Commander.Instance.GetCommandBuilder<string>()
                .OnFinally(() => testOutputHelper.WriteLine("Testing DuplicateOnFinally"))
                .OnFinally(async () => await Task.Run(() => testOutputHelper.WriteLine("Testing DuplicateOnFinally")))
        );

        Assert.Equal("There is already a delegate for that", exception.Message);
    }

    [Fact]
    public void GenericDuplicateOnFinally_3()
    {
        InvalidOperationException exception = Assert.Throws<InvalidOperationException>(() =>
            Commander.Instance.GetCommandBuilder<string>()
                .OnFinally(async () => await Task.Run(() => testOutputHelper.WriteLine("Testing DuplicateOnFinally")))
                .OnFinally(() => testOutputHelper.WriteLine("Testing DuplicateOnFinally"))
        );

        Assert.Equal("There is already a delegate for that", exception.Message);
    }

    [Fact]
    public void GenericDuplicateOnFinally_4()
    {
        InvalidOperationException exception = Assert.Throws<InvalidOperationException>(() =>
            Commander.Instance.GetCommandBuilder<string>()
                .OnFinally(async () => await Task.Run(() => testOutputHelper.WriteLine("Testing DuplicateOnFinally")))
                .OnFinally(async () => await Task.Run(() => testOutputHelper.WriteLine("Testing DuplicateOnFinally")))
        );

        Assert.Equal("There is already a delegate for that", exception.Message);
    }

    [Fact]
    public void SetId_WhenAlreadySetted()
    {
        InvalidOperationException exception = Assert.Throws<InvalidOperationException>(() =>
            Commander.Instance.GetCommandBuilder()
                .SetId("test")
                .SetId("test")
        );

        Assert.Equal("Command already have an Id", exception.Message);
    }

    [Fact]
    public void GenericSetId_WhenAlreadySetted()
    {
        InvalidOperationException exception = Assert.Throws<InvalidOperationException>(() =>
            Commander.Instance.GetCommandBuilder<string>()
                .SetId("test")
                .SetId("test")
        );

        Assert.Equal("Command already have an Id", exception.Message);
    }

    #endregion
}