using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using FerOmega.Abstractions;
using FerOmega.Entities;
using FerOmega.Entities.RedBlack;
using FerOmega.Services;

using Newtonsoft.Json;

namespace FerOmega.Tests
{
    internal class EquationGenerator : IEquationGenerator
    {
        internal IGrammarService grammarService;

        public EquationGenerator()
        {
            grammarService = new GrammarService();
        }

        public class Equation
        {
            public int Id { get; set; }

            public string InfixForm { get; set; }

            public string RevertedPolishForm { get; set; }

            public string ShortTreeForm { get; set; }

            public Tree<AbstractToken> ShortTreeFormAsTree => JsonConvert.DeserializeObject<ShortToken>(ShortTreeForm).ToTree();

            public Equation DeSpacify()
            {
                InfixForm = EquationGenerator.DeSpacify(InfixForm);
                RevertedPolishForm = EquationGenerator.DeSpacify(RevertedPolishForm);
                ShortTreeForm = EquationGenerator.DeSpacify(ShortTreeForm);

                return this;
            }
        }

        public static string DeSpacify(string input)
        {
            return Regex.Replace(input, "\\s", "");
        }

        public Equation[] GetEquations()
        {
            return new[]
            {
                ConstructEquation1(),
                ConstructEquation2(),
                ConstructEquation3(),
                ConstructEquation4(),
                ConstructEquation5(),
                ConstructEquation6(),
                ConstructEquation7(),
            };
        }

        // TODO: [DT] 
        private static Random random = new Random(); 

        public Equation GetAlgebraEquation()
        {
            var operators = grammarService.GetOperatorsForSection(GrammarSectionType.Algebra | GrammarSectionType.Equality | GrammarSectionType.Inequality);
            var operatorsCount = random.Next(5, 7);

            for (int i = 0; i < operatorsCount; i++)
            {
                var @operator = operators[random.Next(0, operators.Length - 1)];

                int operandsCount;
                switch (@operator.Arity)
                {
                    case ArityType.Unary:
                        operandsCount = 1;
                        break;
                    case ArityType.Binary:
                        operandsCount = 2;
                        break;
                    case ArityType.Nulary:
                    case ArityType.Ternary:
                    case ArityType.Kvatery:
                    case ArityType.Multiarity:
                        throw new NotSupportedException();
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                switch (@operator.Fixity)
                {
                    case FixityType.Infix:
                        Func<string, string, string, string> pattern = (left, op, right) => $"{left} {op} {right}";
                        break;
                    case FixityType.Prefix:
                        Func<string, string, string, string> pattern = (left, op, right) => $"{left} {op} {right}";
                        break;
                    case FixityType.Postfix:
                        break;
                    case FixityType.Circumflex:
                        break;
                    case FixityType.PostCircumflex:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                for (int j = 0; j < operandsCount; j++)
                {
                    var operand = random.Next(-1024, 1024).ToString();


                }
            }
        }

        public IEnumerable<Equation> GetAlgebraEquations(int count)
        {
            for (int i = 0; i < count; i++)
            {
                yield return GetAlgebraEquation();
            }
        }

        private Equation ConstructEquation7()
        {
            var equation = new Equation
            {
                Id = 7,
                InfixForm = "[4] + ( [3] + [1] + [4] * ( [2] + [3] ) )",
                RevertedPolishForm = "[4]  [3]  [1]  +  [4]  [2]  [3]  +  *  +  + ",
                ShortTreeForm = @"
{
  ""OperatorType"": 19,
  ""Value"": null,
  ""Children"": [
    {
      ""OperatorType"": 19,
      ""Value"": null,
      ""Children"": [
        {
          ""OperatorType"": 22,
          ""Value"": null,
          ""Children"": [
            {
              ""OperatorType"": 19,
              ""Value"": null,
              ""Children"": [
                {
                  ""OperatorType"": 1,
                  ""Value"": ""[3]"",
                  ""Children"": []
                },
                {
                  ""OperatorType"": 1,
                  ""Value"": ""[2]"",
                  ""Children"": []
                }
              ]
            },
            {
              ""OperatorType"": 1,
              ""Value"": ""[4]"",
              ""Children"": []
            }
          ]
        },
        {
          ""OperatorType"": 19,
          ""Value"": null,
          ""Children"": [
            {
              ""OperatorType"": 1,
              ""Value"": ""[1]"",
              ""Children"": []
            },
            {
              ""OperatorType"": 1,
              ""Value"": ""[3]"",
              ""Children"": []
            }
          ]
        }
      ]
    },
    {
      ""OperatorType"": 1,
      ""Value"": ""[4]"",
      ""Children"": []
    }
  ]
}

",
            };

            return equation.DeSpacify();
        }

        private Equation ConstructEquation6()
        {
            var equation = new Equation
            {
                Id = 6,
                InfixForm = "[5] + ( [7] - [2] * [3] ) * ( [6] - [4] ) / [2]",
                RevertedPolishForm = "[5]  [7]  [2]  [3]  *  -  [6]  [4]  -  *  [2]  /  + ",
                ShortTreeForm = @"
{
  ""OperatorType"": 19,
  ""Value"": null,
  ""Children"": [
    {
      ""OperatorType"": 24,
      ""Value"": null,
      ""Children"": [
        {
          ""OperatorType"": 1,
          ""Value"": ""[2]"",
          ""Children"": []
        },
        {
          ""OperatorType"": 22,
          ""Value"": null,
          ""Children"": [
            {
              ""OperatorType"": 21,
              ""Value"": null,
              ""Children"": [
                {
                  ""OperatorType"": 1,
                  ""Value"": ""[4]"",
                  ""Children"": []
                },
                {
                  ""OperatorType"": 1,
                  ""Value"": ""[6]"",
                  ""Children"": []
                }
              ]
            },
            {
              ""OperatorType"": 21,
              ""Value"": null,
              ""Children"": [
                {
                  ""OperatorType"": 22,
                  ""Value"": null,
                  ""Children"": [
                    {
                      ""OperatorType"": 1,
                      ""Value"": ""[3]"",
                      ""Children"": []
                    },
                    {
                      ""OperatorType"": 1,
                      ""Value"": ""[2]"",
                      ""Children"": []
                    }
                  ]
                },
                {
                  ""OperatorType"": 1,
                  ""Value"": ""[7]"",
                  ""Children"": []
                }
              ]
            }
          ]
        }
      ]
    },
    {
      ""OperatorType"": 1,
      ""Value"": ""[5]"",
      ""Children"": []
    }
  ]
}
",
            };

            return equation.DeSpacify();
        }

        private Equation ConstructEquation5()
        {
            var equation = new Equation
            {
                Id = 5,
                InfixForm = "[17] - [5] * [6] / [3] - [2] + [4] / [7]",
                RevertedPolishForm = "[17]  [5]  [6]  *  [3]  /  -  [2]  -  [4]  [7]  /  + ",
                ShortTreeForm = @"
{
  ""OperatorType"": 19,
  ""Value"": null,
  ""Children"": [
    {
      ""OperatorType"": 24,
      ""Value"": null,
      ""Children"": [
        {
          ""OperatorType"": 1,
          ""Value"": ""[7]"",
          ""Children"": []
        },
        {
          ""OperatorType"": 1,
          ""Value"": ""[4]"",
          ""Children"": []
        }
      ]
    },
    {
      ""OperatorType"": 21,
      ""Value"": null,
      ""Children"": [
        {
          ""OperatorType"": 1,
          ""Value"": ""[2]"",
          ""Children"": []
        },
        {
          ""OperatorType"": 21,
          ""Value"": null,
          ""Children"": [
            {
              ""OperatorType"": 24,
              ""Value"": null,
              ""Children"": [
                {
                  ""OperatorType"": 1,
                  ""Value"": ""[3]"",
                  ""Children"": []
                },
                {
                  ""OperatorType"": 22,
                  ""Value"": null,
                  ""Children"": [
                    {
                      ""OperatorType"": 1,
                      ""Value"": ""[6]"",
                      ""Children"": []
                    },
                    {
                      ""OperatorType"": 1,
                      ""Value"": ""[5]"",
                      ""Children"": []
                    }
                  ]
                }
              ]
            },
            {
              ""OperatorType"": 1,
              ""Value"": ""[17]"",
              ""Children"": []
            }
          ]
        }
      ]
    }
  ]
}
",
            };

            return equation.DeSpacify();
        }

        private Equation ConstructEquation4()
        {
            var equation = new Equation
            {
                Id = 4,
                InfixForm = "[2] + [2] * [2]",
                RevertedPolishForm = "[2]  [2]  [2]  *  + ",
                ShortTreeForm = @"
{
  ""OperatorType"": 19,
  ""Value"": null,
  ""Children"": [
    {
      ""OperatorType"": 22,
      ""Value"": null,
      ""Children"": [
        {
          ""OperatorType"": 1,
          ""Value"": ""[2]"",
          ""Children"": []
        },
        {
          ""OperatorType"": 1,
          ""Value"": ""[2]"",
          ""Children"": []
        }
      ]
    },
    {
      ""OperatorType"": 1,
      ""Value"": ""[2]"",
      ""Children"": []
    }
  ]
}
",
            };

            return equation.DeSpacify();
        }

        private Equation ConstructEquation3()
        {
            var equation = new Equation
            {
                Id = 3,
                InfixForm = "[6] / [2] * [8] / [3]",
                RevertedPolishForm = "[6]  [2]  /  [8]  *  [3]  / ",
                ShortTreeForm = @"
{
  ""OperatorType"": 24,
  ""Value"": null,
  ""Children"": [
    {
      ""OperatorType"": 1,
      ""Value"": ""[3]"",
      ""Children"": []
    },
    {
      ""OperatorType"": 22,
      ""Value"": null,
      ""Children"": [
        {
          ""OperatorType"": 1,
          ""Value"": ""[8]"",
          ""Children"": []
        },
        {
          ""OperatorType"": 24,
          ""Value"": null,
          ""Children"": [
            {
              ""OperatorType"": 1,
              ""Value"": ""[2]"",
              ""Children"": []
            },
            {
              ""OperatorType"": 1,
              ""Value"": ""[6]"",
              ""Children"": []
            }
          ]
        }
      ]
    }
  ]
}
",
            };

            return equation.DeSpacify();
        }

        private Equation ConstructEquation2()
        {
            var equation = new Equation
            {
                Id = 2,
                InfixForm = "[7] - [3] + [6]",
                RevertedPolishForm = "[7] [3] - [6] +",
                ShortTreeForm = @"
{
  ""OperatorType"": 19,
  ""Value"": null,
  ""Children"": [
    {
      ""OperatorType"": 1,
      ""Value"": ""[6]"",
      ""Children"": []
    },
    {
      ""OperatorType"": 21,
      ""Value"": null,
      ""Children"": [
        {
          ""OperatorType"": 1,
          ""Value"": ""[3]"",
          ""Children"": []
        },
        {
          ""OperatorType"": 1,
          ""Value"": ""[7]"",
          ""Children"": []
        }
      ]
    }
  ]
}
",
            };

            return equation.DeSpacify();
        }

        private Equation ConstructEquation1()
        {
            var equation = new Equation
            {
                Id = 1,
                InfixForm = "[5]! + [3]",
                RevertedPolishForm = "[5] ! [3] +",
                ShortTreeForm = @"
{
  ""OperatorType"": 19,
  ""Value"": null,
  ""Children"": [
    {
      ""OperatorType"": 1,
      ""Value"": ""[3]"",
      ""Children"": []
    },
    {
      ""OperatorType"": 32,
      ""Value"": null,
      ""Children"": [
        {
          ""OperatorType"": 1,
          ""Value"": ""[5]"",
          ""Children"": []
        }
      ]
    }
  ]
}
",
            };

            return equation.DeSpacify();
        }
    }
}