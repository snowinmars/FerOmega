using System.Collections.Generic;
using FerOmega.Entities;

namespace FerOmega.Abstractions
{
    public interface IGrammarService
    {
        string[] BracketsDenotations { get; }

        string[] OperatorDenotations { get; }

        IList<Operator> Operators { get; }

        Operator Get(OperatorType operatorType);

        Operator[] GetOperatorsForSection(GrammarSectionType grammarSectionType);

        Operator[] GetPossibleOperators(string denotation);

        bool IsBracket(AbstractToken @operator);

        bool IsBracket(OperatorType operatorType);

        bool IsBracket(string denotation);

        bool IsCloseBracket(AbstractToken @operator);

        bool IsCloseBracket(OperatorType operatorType);

        bool IsCloseBracket(string denotation);

        bool IsOpenBracket(AbstractToken @operator);

        bool IsOpenBracket(string denotation);

        bool IsOpenBracket(OperatorType operatorType);

        bool IsOperand(string input);

        bool IsOperator(string input);

        bool IsUniqueByArity(string denotation, ArityType arity);

        bool IsUniqueByDenotation(string denotation);

        bool IsUniqueByFixity(string denotation, FixityType fixity);
    }
}