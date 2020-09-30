﻿namespace FerOmega.Services.InternalEntities
{
    internal class OperatorToken
    {
        public OperatorToken(string[] tokens, int i)
        {
            Previous = i == 0 ? NonExistingOperator : tokens[i - 1].Trim();
            Current = tokens[i].Trim();
            Next = i == tokens.Length - 1 ? NonExistingOperator : tokens[i + 1].Trim();
        }

        internal const string NonExistingOperator = "NonExistingOperator";

        public string Current { get; set; }

        public string Next { get; set; }

        public string Previous { get; set; }

        public override string ToString()
        {
            return $"{Previous} {Current} {Next}";
        }
    }
}