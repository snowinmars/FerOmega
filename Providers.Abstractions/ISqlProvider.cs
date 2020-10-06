using System;
using FerOmega.Entities;
using FerOmega.Entities.AbstractSyntax;
using FerOmega.Entities.InternalSyntax;

namespace FerOmega.Providers.Abstractions
{
    public interface ISqlProvider
    {
        (string sql, object[] parameters) Convert(Tree<AbstractToken> tree, 
            params PropertyDef[] properties);
    }
}