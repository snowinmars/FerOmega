using Entities.InternalSyntax;
using Entities.InternalSyntax.Enums;

namespace Services.Abstractions
{
    public interface IGrammarService<T>
        where T : IGrammarConfig
    {
        Operator[] Operators { get; }
        
        string[] OperatorDenotations { get; }

        Operator OpenEscapeOperator  { get; }

        Operator CloseEscapeOperator  { get; }
        
        Operator OpenPriorityBracket { get; }
        
        Operator ClosePriorityBracket { get; }

        bool IsOperand(string input);

        bool IsOperator(string input);

        bool IsUniqueByArity(string denotation, Arity arity);

        bool IsUniqueByDenotation(string denotation);

        Operator[] GetPossibleOperators(string denotation);

        bool IsUniqueByFixity(string denotation, Fixity fixity);
    }
}