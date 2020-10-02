using FerOmega.Entities.InternalSyntax;

namespace FerOmega.Services.Abstractions
{
    public interface IGrammarConfig
    {
        Operator[] ConfigOperators();
    }
}