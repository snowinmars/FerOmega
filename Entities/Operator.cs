using System;

namespace Entities
{
    public class Operator : AbstractToken
    {
        // TODO: [DT] 
        public Operator(ArityType arity,
            AssociativityType associativity,
            OperatorType operatorType,
            FixityType fixityType,
            GrammarSectionType grammarSectionType,
            params string[] denotations)
            : base(operatorType, -1)
        {
            Arity = arity;
            Associativity = associativity;
            Denotations = denotations;
            Fixity = fixityType;
            GrammarSectionType = grammarSectionType;
        }

        public ArityType Arity { get; set; }

        public AssociativityType Associativity { get; set; }

        public string[] Denotations { get; set; }

        public FixityType Fixity { get; set; }

        public GrammarSectionType GrammarSectionType { get; set; }

        public string MainDenotation => Denotations[0];

        public Operator DeepClone()
        {
            return new Operator(Arity, Associativity, OperatorType, Fixity, GrammarSectionType, Denotations)
            {
                Priority = Priority,
            };
        }

        public override string ToString()
        {
            var arityExample = "";

            switch (Arity)
            {
            case ArityType.Unary:
                switch (Fixity)
                {
                case FixityType.Prefix:
                    arityExample = $"{MainDenotation}a";

                    break;
                case FixityType.Postfix:
                    arityExample = $"a{MainDenotation}";

                    break;
                case FixityType.Infix:
                case FixityType.Circumflex:
                case FixityType.PostCircumflex:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
                }

                break;
            case ArityType.Binary:
                arityExample = $"a {MainDenotation} b";

                break;
            case ArityType.Nulary:
            case ArityType.Ternary:
            case ArityType.Kvatery:
                break;
            case ArityType.Multiarity:
                arityExample = $"{MainDenotation} ...";

                break;
            default:
                throw new ArgumentOutOfRangeException();
            }

            return $"{OperatorType}, f.e. {arityExample}";
        }
    }
}