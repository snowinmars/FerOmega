using FerOmega.Entities.InternalSyntax;

namespace FerOmega.Services.Abstractions
{
    public interface IOperatorService
    {
        Operator Resolve(StringToken token, Operator[] possibleOperators);
    }
}