using FerOmega.Entities.InternalSyntax;
using FerOmega.Entities.InternalSyntax.Enums;

namespace FerOmega.Services.Abstractions
{
    public interface IGrammarService<T>
        where T : IGrammarConfig
    {
        Operator[] Operators { get; }

        string OperatorsRegex { get; }

        string[] OperatorDenotations { get; }

        Operator Separator { get; }

        Operator OpenEscapeOperator { get; }

        Operator CloseEscapeOperator { get; }

        Operator OpenPriorityBracket { get; }

        Operator ClosePriorityBracket { get; }

        string EnsureEscaped(string value);

        string EnsureUnescaped(string value);

        Operator[] GetPossibleOperators(string denotation, Arity? arity = null, Fixity? fixity = null);

        bool IsOperand(string input);

        bool IsOperator(string input);

        bool IsUniqueByArity(string denotation, Arity arity);

        bool IsUniqueByDenotation(string denotation);

        bool IsUniqueByFixity(string denotation, Fixity fixity);
    }
}