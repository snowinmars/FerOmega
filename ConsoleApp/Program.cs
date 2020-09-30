using System;
using FerOmega.Services;

namespace FerOmega.ConsoleApp
{
    internal class Program
    {
        static void Main()
        {
            var tokenizeService = new TokenizationService();
            var treeShuntingYardService = new AstShuntingYardService();
            
            var equation = "2 + 1 == 3";

            var tokens = tokenizeService.Tokenizate(equation);
            var tree = treeShuntingYardService.Parse(tokens);
        }
    }
}