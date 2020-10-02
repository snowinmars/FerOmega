using System.Collections;
using FerOmega.Entities.InternalSyntax.Enums;
using NUnit.Framework;

namespace FerOmega.Tests.Tokenization
{
    internal class EscapingGrammarTests : BaseTest
    {
        private static IEnumerable EscapingGrammarCases
        {
            get
            {
                yield return new TestCaseData("[&]&[&]",
                                              new[]
                                              {
                                                  "[&]", "&", "[&]",
                                              }).SetName(nameof(OperatorType.BitwiseAnd));

                yield return new TestCaseData("[&&&]&&[&&]",
                                              new[]
                                              {
                                                  "[&&&]", "&&", "[&&]",
                                              }).SetName(nameof(OperatorType.And));

                yield return new TestCaseData("[andrey]and[sand]",
                                              new[]
                                              {
                                                  "[andrey]", "and", "[sand]",
                                              }).SetName(nameof(OperatorType.And) + "Word");

                yield return new TestCaseData("[|]|[|]",
                                              new[]
                                              {
                                                  "[|]", "|", "[|]",
                                              }).SetName(nameof(OperatorType.BitwiseOr));

                yield return new TestCaseData("[|||]||[||]",
                                              new[]
                                              {
                                                  "[|||]", "||", "[||]",
                                              }).SetName(nameof(OperatorType.Or));

                yield return new TestCaseData("[orrey]or[sor]",
                                              new[]
                                              {
                                                  "[orrey]", "or", "[sor]",
                                              }).SetName(nameof(OperatorType.Or) + "Word");

                yield return new TestCaseData("[+]+[++]",
                                              new[]
                                              {
                                                  "[+]", "+", "[++]",
                                              }).SetName(nameof(OperatorType.Plus));

                yield return new TestCaseData("[-]-[--]",
                                              new[]
                                              {
                                                  "[-]", "-", "[--]",
                                              }).SetName(nameof(OperatorType.Minus));

                yield return new TestCaseData("[*]*[**]",
                                              new[]
                                              {
                                                  "[*]", "*", "[**]",
                                              }).SetName(nameof(OperatorType.Multiple));

                yield return new TestCaseData("[/]/[//]",
                                              new[]
                                              {
                                                  "[/]", "/", "[//]",
                                              }).SetName(nameof(OperatorType.Divide));

                yield return new TestCaseData("[%]%[%%]",
                                              new[]
                                              {
                                                  "[%]", "%", "[%%]",
                                              }).SetName(nameof(OperatorType.Reminder));

                yield return new TestCaseData("[^]^[^^]",
                                              new[]
                                              {
                                                  "[^]", "^", "[^^]",
                                              }).SetName(nameof(OperatorType.Xor));

                yield return new TestCaseData("[=]=[=]",
                                              new[]
                                              {
                                                  "[=]", "=", "[=]",
                                              }).SetName(nameof(OperatorType.Equals) + "Length1");

                yield return new TestCaseData("[==]==[=]",
                                              new[]
                                              {
                                                  "[==]", "==", "[=]",
                                              }).SetName(nameof(OperatorType.Equals) + "Length2");

                yield return new TestCaseData("[===]===[=]",
                                              new[]
                                              {
                                                  "[===]", "===", "[=]",
                                              }).SetName(nameof(OperatorType.Equals) + "Length3");

                yield return new TestCaseData("[!=]!=[!!=]",
                                              new[]
                                              {
                                                  "[!=]", "!=", "[!!=]",
                                              }).SetName(nameof(OperatorType.NotEquals) + "C");

                yield return new TestCaseData("[<>]<>[><><]",
                                              new[]
                                              {
                                                  "[<>]", "<>", "[><><]",
                                              }).SetName(nameof(OperatorType.NotEquals) + "Sql");

                yield return new TestCaseData("[>>]>[>]",
                                              new[]
                                              {
                                                  "[>>]", ">", "[>]",
                                              }).SetName(nameof(OperatorType.GreaterThan));

                yield return new TestCaseData("[>=>=]>=[>==]",
                                              new[]
                                              {
                                                  "[>=>=]", ">=", "[>==]",
                                              }).SetName(nameof(OperatorType.GreaterOrEqualsThan));

                yield return new TestCaseData("[<<]<[<]",
                                              new[]
                                              {
                                                  "[<<]", "<", "[<]",
                                              }).SetName(nameof(OperatorType.LesserThan));

                yield return new TestCaseData("[<=<=]<=[<==]",
                                              new[]
                                              {
                                                  "[<=<=]", "<=", "[<==]",
                                              }).SetName(nameof(OperatorType.LesserOrEqualsThan));

                yield return new TestCaseData("[in]in[in]",
                                              new[]
                                              {
                                                  "[in]", "in", "[in]",
                                              }).SetName(nameof(OperatorType.InRange));
            }
        }

        [Test]
        [TestCaseSource(nameof(EscapingGrammarCases))]
        public void EscapingGrammar(string equation, string[] expectedTokens)
        {
            var actualTokens = TokenizationService.Tokenizate(equation);
            TokenizationHelper.Test(expectedTokens, actualTokens);
        }
    }
}