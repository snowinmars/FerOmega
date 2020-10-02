using System;
using Entities.AbstractSyntax;
using Entities.InternalSyntax;

namespace Providers.Abstractions
{
    public interface ISqlProvider
    {
        string Convert(Tree<AbstractToken> tree);
    }
}