using FerOmega.Entities.AbstractSyntax;
using FerOmega.Entities.InternalSyntax;

namespace FerOmega.Providers.Abstractions
{
    public interface ISqlProvider
    {
        string Convert(Tree<AbstractToken> tree);
    }
}