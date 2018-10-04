using System.Collections.Generic;

using FerOmega.Entities;

namespace FerOmega.Abstractions
{
    public interface IGrammarService
    {
        string[] BracketsDenotations { get; }

        Operator CloseEscapeOperator { get; }

        Operator OpenEscapeOperator { get; }

        string[] OperatorDenotations { get; }

        IList<Operator> Operators { get; }

        Operator Get(OperatorType operatorType);

        bool IsBracket(AbstractToken @operator);

        bool IsBracket(OperatorType operatorType);

        bool IsCloseBracket(AbstractToken @operator);

        bool IsCloseBracket(OperatorType operatorType);

        bool IsOpenBracket(AbstractToken @operator);

        bool IsOpenBracket(OperatorType operatorType);

        bool IsOperand(string input);

        bool IsOperator(string input);

        bool IsUniqueByArity(string denotation, ArityType arity);

        bool IsUniqueByFixity(string denotation, FixityType fixity);
    }
}