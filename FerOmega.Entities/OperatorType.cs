namespace FerOmega.Entities
{
    public enum OperatorType
    {
        Literal = 1,

        /// <summary>
        ///     <para>
        ///         <c>a eq 1</c>
        ///     </para>
        ///     <para>
        ///         <c>a == 1</c>
        ///     </para>
        /// </summary>
        Equals = 2,

        /// <summary>
        ///     <para>
        ///         <c>a != 1</c>
        ///     </para>
        ///     <para>
        ///         <c>a &lt;> 1</c>
        ///     </para>
        ///     <para>
        ///         <c>a neq 1</c> or
        ///     </para>
        /// </summary>
        NotEquals = 3,

        /// <summary>
        ///     <para>
        ///         <c>!a</c>
        ///     </para>
        ///     <para>
        ///         <c>not a</c>
        ///     </para>
        /// </summary>
        Not = 4,

        /// <summary>
        ///     <para>
        ///         <c>a > 5</c>
        ///     </para>
        ///     <para>
        ///         <c>a gt 5</c>
        ///     </para>
        /// </summary>
        GreaterThan = 5,

        /// <summary>
        ///     <para>
        ///         <c>a &lt; 5</c>
        ///     </para>
        ///     <para>
        ///         <c>a lt 5</c>
        ///     </para>
        /// </summary>
        LesserThan = 6,

        /// <summary>
        ///     <para>
        ///         <c>a >= 5</c>
        ///     </para>
        ///     <para>
        ///         <c>a geq 5</c>
        ///     </para>
        /// </summary>
        GreaterOrEqualsThan = 7,

        /// <summary>
        ///     <para>
        ///         <c>a &lt;= 5</c>
        ///     </para>
        ///     <para>
        ///         <c>a leq 5</c>
        ///     </para>
        /// </summary>
        LesserOrEqualsThan = 8,

        /// <summary>
        ///     <para>
        ///         <c>a in (5, 7, 8)</c>
        ///     </para>
        /// </summary>
        InRange = 9,

        /// <summary>
        ///     <para>
        ///         <c>a & b</c>
        ///     </para>
        ///     <para>
        ///         <c>a && b</c>
        ///     </para>
        ///     <para>
        ///         <c>a and b</c>
        ///     </para>
        /// </summary>
        And = 10,

        /// <summary>
        ///     <para>
        ///         <c>a | b</c>
        ///     </para>
        ///     <para>
        ///         <c>a || b</c>
        ///     </para>
        ///     <para>
        ///         <c>a or b</c>
        ///     </para>
        /// </summary>
        Or = 11,

        /// <summary>
        ///     <para>
        ///         <c>a ^ b</c>
        ///     </para>
        ///     <para>
        ///         <c>a xor b</c>
        ///     </para>
        /// </summary>
        Xor = 12,

        /// <summary>
        ///     <para>
        ///         <c>a con example</c>
        ///     </para>
        /// </summary>
        Contains = 13,

        /// <summary>
        ///     <para>
        ///         <c>a stw example</c>
        ///     </para>
        /// </summary>
        StartsWith = 14,

        /// <summary>
        ///     <para>
        ///         <c>a edw example</c>
        ///     </para>
        /// </summary>
        EndsWith = 15,

        /// <summary>
        ///     <para>
        ///         <c>emp a</c>
        ///     </para>
        /// </summary>
        Empty = 16,

        /// <summary>
        ///     <para>
        ///         <c>nep a</c>
        ///     </para>
        /// </summary>
        NotEmpty = 17,

        /// <summary>
        ///     <para>
        ///         <c>+5</c>
        ///     </para>
        /// </summary>
        UnaryPlus = 18,

        /// <summary>
        ///     <para>
        ///         <c>a + 5</c>
        ///     </para>
        /// </summary>
        Plus = 19,

        /// <summary>
        ///     <para>
        ///         <c>-5</c>
        ///     </para>
        /// </summary>
        UnaryMinus = 20,

        /// <summary>
        ///     <para>
        ///         <c>a - 5</c>
        ///     </para>
        /// </summary>
        Minus = 21,

        /// <summary>
        ///     <para>
        ///         <c>a * 2</c>
        ///     </para>
        /// </summary>
        Multiple = 22,

        /// <summary>
        ///     <para>
        ///         <c>a % 2</c>
        ///     </para>
        /// </summary>
        Reminder = 23,

        /// <summary>
        ///     <para>
        ///         <c>a / 2</c>
        ///     </para>
        /// </summary>
        Divide = 24,

        /// <summary>
        ///     <para>
        ///         <c>~a</c>
        ///     </para>
        /// </summary>
        Invert = 25,

        /// <summary>
        /// (
        /// </summary>
        OpenRoundBracket = 26,

        /// <summary>
        /// )
        /// </summary>
        CloseRoundBracket = 27,

        /// <summary>
        /// {
        /// </summary>
        OpenCurlyBracket = 28,

        /// <summary>
        /// }
        /// </summary>
        CloseCurlyBracket = 29,

        /// <summary>
        /// [
        /// </summary>
        OpenSquareBracket = 30,

        /// <summary>
        /// ]
        /// </summary>
        CloseSquareBracket = 31,

        /// <summary>
        ///     <para>
        ///         <c>5!</c>
        ///     </para>
        /// </summary>
        Factorial = 32,
    }
}