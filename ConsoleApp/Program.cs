﻿using System;
using FerOmega.Services;

namespace FerOmega.ConsoleApp
{
    internal class Program
    {
        static void Main()
        {
            var tokenizeService = new TokenizationService();
            var treeShuntingYardService = new TreeShuntingYardService();
            
            var equation = "[2] + [1]";

            var tokens = tokenizeService.Tokenizate(equation);
            var tree = treeShuntingYardService.Parse(tokens);
        }
    }
}