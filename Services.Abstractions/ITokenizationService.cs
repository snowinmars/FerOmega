namespace FerOmega.Services.Abstractions
{
    public interface ITokenizationService
    {
        string[] Tokenizate(string equation);
    }
}