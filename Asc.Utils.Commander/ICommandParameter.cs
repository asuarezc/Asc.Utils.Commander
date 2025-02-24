namespace Asc.Utils.Commander;

public interface ICommandParameter
{
    T OfType<T>();
}