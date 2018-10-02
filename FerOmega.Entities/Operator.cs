using System;

namespace FerOmega.Entities
{
    public class Operator : AbstractToken
    {
        public Operator(ArityType arity, AssociativityType associativity, OperatorType operatorType, FixityType fixityType, params string[] denotations) : base(operatorType, -1)
        {
            this.Arity = arity;
            this.Associativity = associativity;
            this.Denotations = denotations;
            this.Fixity = fixityType;
        }
        
        public ArityType Arity { get; set; }

        public AssociativityType Associativity { get; set; }

        public string[] Denotations { get; set; }

        public FixityType Fixity { get; set; }

        public string MainDenotation => Denotations[0];


        public override string ToString()
        {
            string arityExample = "";

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
                case ArityType.Multiarity:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return $"{arityExample} ({Associativity}, Priority: {Priority})";
        }
    }
}