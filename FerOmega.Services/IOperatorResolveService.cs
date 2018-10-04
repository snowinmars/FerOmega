using FerOmega.Entities;

namespace FerOmega.Services
{
    internal interface IOperatorResolveService
    {
        Operator Resolve(OperatorToken token, Operator[] possibleOperators);
    }
}