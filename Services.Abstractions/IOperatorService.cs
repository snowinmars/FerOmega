using Entities.InternalSyntax;

namespace Services.Abstractions
{
    public interface IOperatorService
    {
        Operator Resolve(StringToken token, Operator[] possibleOperators);
    }
}