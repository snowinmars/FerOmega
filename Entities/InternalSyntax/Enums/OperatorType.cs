namespace Entities.InternalSyntax.Enums
{
    public enum OperatorType
    {
        ///  a
        Literal = 1,

        /// a == 2
        Equals,

        /// a != 2
        NotEquals ,

        /// a &gt; 2
        GreaterThan,

        /// a &lt; 2
        LesserThan,

        /// a &lt;= 2
        GreaterOrEqualsThan,

        /// a &gt;= 2
        LesserOrEqualsThan ,

        /// !a
        Not,

        /// a and b
        And ,
        
        /// 10 & 11 = 10
        BitwiseAnd ,

        /// a or b
        Or ,

        ///  10 | 11 = 11
        BitwiseOr,
        
        /// a xor b
        Xor ,

        /// a in ( 2, 3 )
        InRange ,

        /// a.contains(2)
        Contains ,

        /// a.startsWith(2)
        StartsWith ,

        /// a.endsWith(2)
        EndsWith ,
        
        /// +2 
        UnaryPlus,

        /// 3 + 2
        Plus ,

        /// -2 
        UnaryMinus ,

        /// 3 - 2
        Minus ,

        /// 3! = 1 * 2 * 3 = 6 
        Factorial ,
        
        /// 2 * 3
        Multiple ,

        /// 5 % 2 = 1
        Reminder ,

        /// 5 / 2 = 2
        Divide ,

        /// ~3 = ~11 = 00
        Invert ,

        /// ( ...
        OpenPriorityBracket ,

        /// ... )
        ClosePriorityBracket ,
        
        /// [ ...
        OpenEscapeOperator,
        
        /// ... ]
        CloseEscapeOperator,

        /// ,
        Enumerator,
        
        /// ;
        Terminator,
    }
}