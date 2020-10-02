namespace Entities.InternalSyntax.Enums
{
    public enum Fixity
    {
        /// operand OPERATOR operand
        /// f.e. 2 + 2
        Infix,

        /// OPERATOR operand
        /// f.e. !2
        Prefix,

        /// operand OPERATOR
        /// f.e. 2!
        Postfix,

        /// OPERATOR operand OPERATOR
        /// f.e. ( 2 )
        Circumflex,

        /// operand OPERATOR operand OPERATOR
        /// f.e. arr [ 1 ]
        PostCircumflex,
    }
}