using FerOmega.Entities.AbstractSyntax;
using FerOmega.Entities.InternalSyntax;

namespace FerOmega.Services.Abstractions
{
    public interface IAstService
    {
        Tree<AbstractToken> Convert(string[] tokens);
    }
}