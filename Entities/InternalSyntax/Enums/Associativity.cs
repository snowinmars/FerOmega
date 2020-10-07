namespace FerOmega.Entities.InternalSyntax.Enums
{
    public enum Associativity
    {
        Left = 1,

        Right = 2,

        // f.e. in "1, 2" operator "," really doesn't care about it's associativity due to it has no operands
        Ambivalent = 3,
    }
}