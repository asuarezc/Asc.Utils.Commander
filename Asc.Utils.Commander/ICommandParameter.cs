namespace Asc.Utils.Commander;

/// <summary>
/// A parameter associated with a <see cref="ICommand"/>
/// </summary>
public interface ICommandParameter
{
    /// <summary>
    /// Returns the parameter value of <typeparamref name="T"/> type
    /// </summary>
    T OfType<T>();
}