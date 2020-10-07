namespace FerOmega.Entities.InternalSyntax
{
    public class StringToken
    {
        public StringToken(string[] tokens, int i)
        {
            Previous = i == 0 ? NonExistingOperator : tokens[i - 1];
            Current = tokens[i];
            Next = i == tokens.Length - 1 ? NonExistingOperator : tokens[i + 1];
        }

        public static readonly string NonExistingOperator = "None";

        public string Current { get; }

        public string Next { get; }

        public string Previous { get; }

        public override string ToString()
        {
            return $"{Previous} {Current} {Next}";
        }
    }
}
