using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

using Helpers.FileReaders;
using Helpers.Structure;

namespace PuzzleDays
{
    public class Day21 : ProblemBase
    {
        protected override async Task<string> SolvePartOneInternal()
        {
            var result = Expression.Lambda<Func<long>>(root)
                                   .Compile()();

            return result.ToString();
        }

        protected override async Task<string> SolvePartTwoInternal()
        {
            throw new NotImplementedException();
        }

        public override async Task ReadInput()
        {
            var lines = await new StringFileReader().ReadInputFromFile();

            var expressions = new Dictionary<string, Expression>();

            var dependentExpressions = new Dictionary<string, (string left, string right, string op)>();

            foreach (var line in lines)
            {
                var split = line.Split(':');

                var monkeyName = split[0];

                var expression = split[1]
                    .Trim();

                if (long.TryParse(expression,
                                  out var constant))
                {
                    expressions[monkeyName] = Expression.Constant(constant);
                }
                else
                {
                    var expressionSplit = expression.Split(' ');

                    var left = expressionSplit[0];
                    var op = expressionSplit[1];
                    var right = expressionSplit[2];

                    dependentExpressions[monkeyName] = (left, right, op);
                }
            }

            Expression GetOrCreateExpression(string name)
            {
                if (expressions.TryGetValue(name,
                                            out var existing))
                {
                    return existing;
                }

                var info = dependentExpressions[name];

                var left = GetOrCreateExpression(info.left);
                var right = GetOrCreateExpression(info.right);

                var result = info.op switch
                {
                    "+" => Expression.Add(left,
                                          right),
                    "-" => Expression.Subtract(left,
                                               right),
                    "*" => Expression.Multiply(left,
                                               right),
                    "/" => Expression.Divide(left,
                                             right),
                    _ => throw new ArgumentException("Invalid op")
                };

                expressions[name] = result;
                return result;
            }

            root = GetOrCreateExpression("root");
        }

        private Expression root;
    }
}
