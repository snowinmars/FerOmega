using System;
using FerOmega.Entities.InternalSyntax.Enums;

namespace FerOmega.Entities.InternalSyntax
{
    public class Operator : AbstractToken
    {
        public Operator(Arity arity,
            Associativity associativity,
            OperatorType operatorType,
            Fixity fixity,
            params string[] denotations)
            : base(operatorType, -1)

        // priority will be calculated in runtime based on operators declaration in config files
        // otherwise it will be hard to maintain it properly
        {
            Arity = arity;
            Associativity = associativity;
            Denotations = denotations;
            Fixity = fixity;
        }

        public Arity Arity { get; }

        public Associativity Associativity { get; }

        public string[] Denotations { get; }

        public Fixity Fixity { get; }

        public string MainDenotation => Denotations[0];

        public override string ToString()
        {
            string ThrowOor(string paramName, object actualValue, string message)
            {
                throw new ArgumentOutOfRangeException(paramName, actualValue, message);
            }

            var arityExample = Arity switch
            {
                Arity.Unary => Fixity switch
                {
                    Fixity.Prefix => $"{MainDenotation}a",
                    Fixity.Postfix => $"a{MainDenotation}",
                    Fixity.Infix => ThrowOor(nameof(Fixity), Fixity, nameof(Enums.Fixity)),
                    Fixity.Circumflex => ThrowOor(nameof(Fixity), Fixity, nameof(Enums.Fixity)),
                    Fixity.PostCircumflex => ThrowOor(nameof(Fixity), Fixity, nameof(Enums.Fixity)),
                    _ => ThrowOor(nameof(Fixity), Fixity, nameof(Enums.Fixity)),
                },
                Arity.Binary => $"a {MainDenotation} b",
                Arity.Nulary => MainDenotation,
                Arity.Ternary => $"a {MainDenotation} b {MainDenotation} c",
                Arity.Kvatery => "",
                Arity.Multiarity => $"{MainDenotation} ...",
                _ => ThrowOor(nameof(Arity), Arity, nameof(Enums.Arity)),
            };

            return $"{OperatorType},   f.e. {arityExample}";
        }
    }
}
