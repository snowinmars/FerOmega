using System.Text.RegularExpressions;

using FerOmega.Abstractions;
using FerOmega.Entities;
using FerOmega.Entities.RedBlack;
using FerOmega.Services;

using Newtonsoft.Json;

namespace FerOmega.Tests
{
    internal class SmokeEquationGenerator : ISmokeEquationGenerator
    {
        public class Equation
        {
            public int Id { get; set; }

            public string InfixForm { get; set; }

            public string RevertedPolishForm { get; set; }

            public string ShortTreeForm { get; set; }

            public Tree<AbstractToken> ShortTreeFormAsTree => JsonConvert.DeserializeObject<ShortToken>(ShortTreeForm).ToTree();

            public Equation DeSpacify()
            {
                InfixForm = SmokeEquationGenerator.DeSpacify(InfixForm);
                RevertedPolishForm = SmokeEquationGenerator.DeSpacify(RevertedPolishForm);
                ShortTreeForm = SmokeEquationGenerator.DeSpacify(ShortTreeForm);

                return this;
            }
        }

        internal readonly IGrammarService GrammarService;

        public SmokeEquationGenerator()
        {
            GrammarService = new GrammarService();
        }

        // TODO: [DT] 
        public static string DeSpacify(string input)
        {
            return Regex.Replace(input, "\\s", "");
        }

        public Equation[] GetEquations()
        {
            return new[]
            {
                ConstructEquation0(),
                ConstructEquation1(),
                ConstructEquation2(),
                ConstructEquation3(),
                ConstructEquation4(),
                ConstructEquation5(),
                ConstructEquation6(),
                ConstructEquation7(),
                ConstructEquation8(),
            };
        }

        private Equation ConstructEquation0()
        {
            var equation = new Equation
            {
                Id = 1,
                InfixForm = "[5] - [3]",
                RevertedPolishForm = "[5] [3] -",
                ShortTreeForm = @"
{
  ""OperatorType"": 21,
  ""Value"": null,
  ""Children"": [
    {
      ""OperatorType"": 1,
      ""Value"": ""[5]"",
      ""Children"": []
    },
    {
      ""OperatorType"": 1,
      ""Value"": ""[3]"",
      ""Children"": []
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
      ""OperatorType"": 32,
      ""Value"": null,
      ""Children"": [
        {
          ""OperatorType"": 1,
          ""Value"": ""[5]"",
          ""Children"": []
        }
      ]
    },
    {
      ""OperatorType"": 1,
      ""Value"": ""[3]"",
      ""Children"": []
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
      ""OperatorType"": 21,
      ""Value"": null,
      ""Children"": [
        {
          ""OperatorType"": 1,
          ""Value"": ""[7]"",
          ""Children"": []
        },
        {
          ""OperatorType"": 1,
          ""Value"": ""[3]"",
          ""Children"": []
        }
      ]
    },
    {
      ""OperatorType"": 1,
      ""Value"": ""[6]"",
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
      ""OperatorType"": 22,
      ""Value"": null,
      ""Children"": [
        {
          ""OperatorType"": 24,
          ""Value"": null,
          ""Children"": [
            {
              ""OperatorType"": 1,
              ""Value"": ""[6]"",
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
          ""Value"": ""[8]"",
          ""Children"": []
        }
      ]
    },
    {
      ""OperatorType"": 1,
      ""Value"": ""[3]"",
      ""Children"": []
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
                InfixForm = "[2] + [3] * [4]",
                RevertedPolishForm = "[2]  [3]  [4]  *  + ",
                ShortTreeForm = @"
{
  ""OperatorType"": 19,
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
          ""OperatorType"": 1,
          ""Value"": ""[3]"",
          ""Children"": []
        },
        {
          ""OperatorType"": 1,
          ""Value"": ""[4]"",
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
      ""OperatorType"": 21,
      ""Value"": null,
      ""Children"": [
        {
          ""OperatorType"": 21,
          ""Value"": null,
          ""Children"": [
            {
              ""OperatorType"": 1,
              ""Value"": ""[17]"",
              ""Children"": []
            },
            {
              ""OperatorType"": 24,
              ""Value"": null,
              ""Children"": [
                {
                  ""OperatorType"": 22,
                  ""Value"": null,
                  ""Children"": [
                    {
                      ""OperatorType"": 1,
                      ""Value"": ""[5]"",
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
          ""Value"": ""[2]"",
          ""Children"": []
        }
      ]
    },
    {
      ""OperatorType"": 24,
      ""Value"": null,
      ""Children"": [
        {
          ""OperatorType"": 1,
          ""Value"": ""[4]"",
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
      ""OperatorType"": 1,
      ""Value"": ""[5]"",
      ""Children"": []
    },
    {
      ""OperatorType"": 24,
      ""Value"": null,
      ""Children"": [
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
                  ""Value"": ""[7]"",
                  ""Children"": []
                },
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
                      ""Value"": ""[3]"",
                      ""Children"": []
                    }
                  ]
                }
              ]
            },
            {
              ""OperatorType"": 21,
              ""Value"": null,
              ""Children"": [
                {
                  ""OperatorType"": 1,
                  ""Value"": ""[6]"",
                  ""Children"": []
                },
                {
                  ""OperatorType"": 1,
                  ""Value"": ""[4]"",
                  ""Children"": []
                }
              ]
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
  ]
}
",
            };

            return equation.DeSpacify();
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
      ""OperatorType"": 1,
      ""Value"": ""[4]"",
      ""Children"": []
    },
    {
      ""OperatorType"": 19,
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
              ""Value"": ""[1]"",
              ""Children"": []
            }
          ]
        },
        {
          ""OperatorType"": 22,
          ""Value"": null,
          ""Children"": [
            {
              ""OperatorType"": 1,
              ""Value"": ""[4]"",
              ""Children"": []
            },
            {
              ""OperatorType"": 19,
              ""Value"": null,
              ""Children"": [
                {
                  ""OperatorType"": 1,
                  ""Value"": ""[2]"",
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
        }
      ]
    }
  ]
}

",
            };

            return equation.DeSpacify();
        }

        private Equation ConstructEquation8()
        {
            var equation = new Equation
            {
                Id = 8,
                InfixForm = " (( [341] * [46] ) + ( [40] + ( + ( - [528] ) ) ) ) + [546]",
                RevertedPolishForm = "[341] [46] * [40] [528] - + + + [546] +",
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
							""OperatorType"": 1,
							""Value"": 341,
							""Children"": []
						},
						{
							""OperatorType"": 1,
							""Value"": 46,
							""Children"": []
						},
					]
				},

				{
					""OperatorType"": 19,
					""Value"": null,
					""Children"": [
						{
							""OperatorType"": 1,
							""Value"": 40,
							""Children"": []
						},
						{
							""OperatorType"": 18,
							""Value"": null,
							""Children"": [
								{
									""OperatorType"": 20,
									""Value"": null,
									""Children"": [
										{
											""OperatorType"": 1,
											""Value"": 528,
											""Children"": []
										},
									]
								},
							]
						},
					]
				},
			]
		},
		{
			""OperatorType"": 1,
			""Value"": 546,
			""Children"": []
		}
	]
}
",
            };

            return equation.DeSpacify();
        }
    }
}