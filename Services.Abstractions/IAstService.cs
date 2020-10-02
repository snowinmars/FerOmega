using Entities.AbstractSyntax;
using Entities.InternalSyntax;

namespace Services.Abstractions
{
    public interface IAstService
    {
        Tree<AbstractToken> Convert(string[] tokens);
    }
}