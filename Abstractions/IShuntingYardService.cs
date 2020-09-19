namespace FerOmega.Abstractions
{
    public interface IShuntingYardService<T>
    {
        T Parse(string[] tokens);
    }
}