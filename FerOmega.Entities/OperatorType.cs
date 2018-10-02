namespace FerOmega.Entities
{
    public enum OperatorType
    {
        Variable,

        /// <summary>
        ///     <para>
        ///         <c>a eq 1</c>
        ///     </para>
        ///     <para>
        ///         <c>a == 1</c>
        ///     </para>
        /// </summary>
        Equals,

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
        NotEquals,

        /// <summary>
        ///     <para>
        ///         <c>!a</c>
        ///     </para>
        ///     <para>
        ///         <c>not a</c>
        ///     </para>
        /// </summary>
        Not,

        /// <summary>
        ///     <para>
        ///         <c>a > 5</c>
        ///     </para>
        ///     <para>
        ///         <c>a gt 5</c>
        ///     </para>
        /// </summary>
        GreaterThan,

        /// <summary>
        ///     <para>
        ///         <c>a &lt; 5</c>
        ///     </para>
        ///     <para>
        ///         <c>a lt 5</c>
        ///     </para>
        /// </summary>
        LesserThan,

        /// <summary>
        ///     <para>
        ///         <c>a >= 5</c>
        ///     </para>
        ///     <para>
        ///         <c>a geq 5</c>
        ///     </para>
        /// </summary>
        GreaterOrEqualsThan,

        /// <summary>
        ///     <para>
        ///         <c>a &lt;= 5</c>
        ///     </para>
        ///     <para>
        ///         <c>a leq 5</c>
        ///     </para>
        /// </summary>
        LesserOrEqualsThan,

        /// <summary>
        ///     <para>
        ///         <c>a in (5, 7, 8)</c>
        ///     </para>
        /// </summary>
        InRange,

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
        And,

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
        Or,

        /// <summary>
        ///     <para>
        ///         <c>a ^ b</c>
        ///     </para>
        ///     <para>
        ///         <c>a xor b</c>
        ///     </para>
        /// </summary>
        Xor,

        /// <summary>
        ///     <para>
        ///         <c>a con example</c>
        ///     </para>
        /// </summary>
        Contains,

        /// <summary>
        ///     <para>
        ///         <c>a stw example</c>
        ///     </para>
        /// </summary>
        StartsWith,

        /// <summary>
        ///     <para>
        ///         <c>a edw example</c>
        ///     </para>
        /// </summary>
        EndsWith,

        /// <summary>
        ///     <para>
        ///         <c>emp a</c>
        ///     </para>
        /// </summary>
        Empty,

        /// <summary>
        ///     <para>
        ///         <c>nep a</c>
        ///     </para>
        /// </summary>
        NotEmpty,

        /// <summary>
        ///     <para>
        ///         <c>+5</c>
        ///     </para>
        /// </summary>
        UnaryPlus,

        /// <summary>
        ///     <para>
        ///         <c>a + 5</c>
        ///     </para>
        /// </summary>
        Plus,

        /// <summary>
        ///     <para>
        ///         <c>-5</c>
        ///     </para>
        /// </summary>
        UnaryMinus,

        /// <summary>
        ///     <para>
        ///         <c>a - 5</c>
        ///     </para>
        /// </summary>
        Minus,

        /// <summary>
        ///     <para>
        ///         <c>a * 2</c>
        ///     </para>
        /// </summary>
        Multiple,

        /// <summary>
        ///     <para>
        ///         <c>a % 2</c>
        ///     </para>
        /// </summary>
        Reminder,

        /// <summary>
        ///     <para>
        ///         <c>a / 2</c>
        ///     </para>
        /// </summary>
        Divide,

        /// <summary>
        ///     <para>
        ///         <c>~a</c>
        ///     </para>
        /// </summary>
        Invert,

        /// <summary>
        /// (
        /// </summary>
        OpenRoundBracket,

        /// <summary>
        /// )
        /// </summary>
        CloseRoundBracket,

        /// <summary>
        /// {
        /// </summary>
        OpenCurlyBracket,

        /// <summary>
        /// }
        /// </summary>
        CloseCurlyBracket,

        /// <summary>
        /// [
        /// </summary>
        OpenSquareBracket,

        /// <summary>
        /// ]
        /// </summary>
        CloseSquareBracket,
    }
}