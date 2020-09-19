using FerOmega.Entities;
using FerOmega.Services.InternalEntities;

namespace FerOmega.Services
{
    internal interface IOperatorResolveService
    {
        Operator Resolve(OperatorToken token, Operator[] possibleOperators);
    }
}