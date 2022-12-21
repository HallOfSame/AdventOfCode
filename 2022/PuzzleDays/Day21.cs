using System.Linq.Expressions;

using Helpers.FileReaders;
using Helpers.Structure;

namespace PuzzleDays
{
    public class Day21 : ProblemBase
    {
        protected override async Task<string> SolvePartOneInternal()
        {
            // Just compile the expression
            // Using a variable only to make part 2 easier
            var result = Expression.Lambda<Func<long, long>>(root,
                                                             humanParamExpression)
                                   .Compile()(humanValue);

            return result.ToString();
        }

        protected override async Task<string> SolvePartTwoInternal()
        {
            var variableDirectionStack = new Stack<char>();

            // Recursively finds the human variable in the expression tree
            // Along the way, we push L or R in to a stack so that we know from the root how to get to the variable
            bool ContainsHumanVariable(BinaryExpression expression)
            {
                if (expression.Left == humanParamExpression)
                {
                    variableDirectionStack.Push('L');
                    return true;
                }

                if (expression.Right == humanParamExpression)
                {
                    variableDirectionStack.Push('R');
                    return true;
                }

                if (expression.Left is BinaryExpression binLeft
                    && ContainsHumanVariable(binLeft))
                {
                    variableDirectionStack.Push('L');
                    return true;
                }

                if (expression.Right is BinaryExpression binRight
                    && ContainsHumanVariable(binRight))
                {
                    variableDirectionStack.Push('R');
                    return true;
                }

                return false;
            }

            // Just evaluates the given expression and returns the result
            long EvaluateExpression(Expression expression)
            {
                return Expression.Lambda<Func<long>>(expression)
                                 .Compile()();
            }

            // Starting from the root of the expression tree, construct a stack that tells us where to find the human variable
            ContainsHumanVariable((BinaryExpression)root);

            var current = (BinaryExpression)root;

            var targetValue = 0L;

            // Follow the stack down
            while (variableDirectionStack.Any())
            {
                var sideWithVariable = variableDirectionStack.Pop();

                var sideToEval = sideWithVariable == 'L'
                                     ? 'R'
                                     : 'L';

                // This is the side of the current expression that does not contain the variable
                // We can evaluate that whole side of the equation, it's all just constant values
                var evaluate = sideToEval == 'L'
                                   ? current.Left
                                   : current.Right;

                if (current == root)
                {
                    // For the very first one, just set our target
                    // Equivalent of setting up the left = right equation
                    // Where evaluate is the side that doesn't contain the variable
                    targetValue = EvaluateExpression(evaluate);
                }
                else
                {
                    // The logic here works like basic math
                    // At this point we have something like:
                    // 24 = (10 + 2) * (11 + puzzleAnswer)
                    // And we are trying to solve for puzzleAnswer
                    // So we can rewrite this as:
                    // (24 / (10 + 2)) = (11 + puzzleAnswer)

                    // And that is essentially what we're doing below, invert the top level operation (* in this example)
                    // Then simplify down the side without the variable
                    // So it becomes (24 / 12) = (11 + puzzleAnswer) => 12 = (11 + puzzleAnswer)
                    // We store the constant side (12 here) in targetValue and keep going
                    // We just keep doing that until one side of the equation is just puzzleAnswer

                    // Figure out how this expression was originally applied so we can invert it
                    var originalOp = current.NodeType switch
                    {
                        ExpressionType.Add => '+',
                        ExpressionType.Subtract => '-',
                        ExpressionType.Multiply => '*',
                        ExpressionType.Divide => '/',
                        _ => throw new ArgumentException("Unexpected node type")
                    };

                    var currentConstant = Expression.Constant(targetValue);

                    var newValueExpression = originalOp switch
                    {
                        // For add, minus, and multiply order doesn't matter, just invert
                        '+' => Expression.Subtract(currentConstant,
                                                   evaluate),
                        '*' => Expression.Divide(currentConstant,
                                                 evaluate),
                        // Order matters for division and subtraction
                        '-' => sideToEval == 'L'
                                   // If it was current = eval - hum
                                   // It becomes eval - current
                                   ? Expression.Subtract(evaluate,
                                                         currentConstant)
                                   // If it was current = hum - eval
                                   // It becomes current + eval
                                   : Expression.Add(currentConstant,
                                                    evaluate),
                        '/' => sideToEval == 'R'
                                   // If it was current = hum / eval
                                   // It becomes current * eval
                                   ? Expression.Multiply(currentConstant,
                                                         evaluate)
                                   // If it was current = eval / hum
                                   // It becomes eval / current
                                   : Expression.Divide(evaluate,
                                                       currentConstant),
                        _ => throw new ArgumentException("Unexpected opToPerform")
                    };

                    targetValue = EvaluateExpression(newValueExpression);
                }

                // Next is whichever side of the tree contains the variable, opposite of evaluate
                var next = sideWithVariable == 'L'
                               ? current.Left
                               : current.Right;

                // If it is a binary expression we just keep going
                // If this cast fails, we're at the end of the stack anyways and next is our variable expression, so the while loop is about to exit
                if (next is BinaryExpression currentAsBin)
                {
                    current = currentAsBin;
                }
            }

            // We reached the end of the stack which means we've simplified from something like 24 = (10 + 2) * (11 + puzzleAnswer)
            // To 1 = puzzleAnswer
            // Where 1 is saved in targetValue
            return targetValue.ToString();
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

                if (monkeyName == "humn")
                {
                    humanParamExpression = Expression.Parameter(typeof(long),
                                                 humanVariableName);
                    expressions[monkeyName] = humanParamExpression;
                    humanValue = long.Parse(expression);
                }
                else if (long.TryParse(expression,
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

        private ParameterExpression humanParamExpression;

        private long humanValue;

        private const string humanVariableName = "humnVar";
    }
}
