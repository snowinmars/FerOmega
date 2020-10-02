namespace Entities.InternalSyntax
{
    public class StringToken
    {
        public StringToken(string[] tokens, int i)
        {
            Previous = i == 0 ? NonExistingOperator : tokens[i - 1];
            Current = tokens[i];
            Next = i == tokens.Length - 1 ? NonExistingOperator : tokens[i + 1];
        }

        public static readonly string NonExistingOperator = default;

        public string Current { get; set; }

        public string Next { get; set; }

        public string Previous { get; set; }

        public override string ToString()
        {
            return $"{Previous} {Current} {Next}";
        }
    }
}