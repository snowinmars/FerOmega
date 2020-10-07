namespace FerOmega.Entities.InternalSyntax.Enums
{
    public enum Arity
    {
        /// ?
        Nulary = 0,

        /// +1
        Unary = 1,

        /// 1 + 2
        Binary = 2,

        /// 1 ? 2 : 3
        Ternary = 3,

        /// ?
        Kvatery = 4,

        /// ( 1, 2, 3)
        Multiarity = 5,
    }
}