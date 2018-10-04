namespace FerOmega.Services
{
    internal class OperatorToken
    {
        internal const string NonExistingOperator = "";

        public string Current { get; set; }

        public string Next { get; set; }

        public string Previous { get; set; }

        public OperatorToken(string[] tokens, int i)
        {
            Previous = i == 0 ? NonExistingOperator : tokens[i - 1].Trim();
            Current = tokens[i].Trim();
            Next = i == tokens.Length - 1 ? NonExistingOperator : tokens[i + 1].Trim();
        }

        public override string ToString()
        {
            return $"{Previous} {Current} {Next}";
        }
    }
}