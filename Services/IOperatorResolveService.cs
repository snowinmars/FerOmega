using Entities;
using Services.InternalEntities;

namespace Services
{
    internal interface IOperatorResolveService
    {
        Operator Resolve(OperatorToken token, Operator[] possibleOperators);
    }
}