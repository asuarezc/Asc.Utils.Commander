namespace Asc.Utils.Commander.Implementation;

internal abstract class CommandParameter(object? value) : ICommandParameter
{
    internal object? objectValue = value;

    public T OfType<T>()
    {
        if (objectValue is null)
            throw new InvalidOperationException("Cannot convert null into anything");

        return (T)Convert.ChangeType(objectValue, typeof(T));
    }
}

internal class CommandParameter<T> : CommandParameter
{
    public T Value { get; private set; }

    public CommandParameter(T value) : base(value)
    {
        ArgumentNullException.ThrowIfNull(value, nameof(value));

        Value = value;
        objectValue = value;
    }
}
