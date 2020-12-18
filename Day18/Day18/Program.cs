using System;
using System.Diagnostics;
using System.IO;

namespace Day18
{
    internal class Program
    {
        #region Class Methods

        private static void Main(string[] args)
        {
            var fileLines = File.ReadAllLines("PuzzleInput.txt");

            var parser = new ExpressionParser();

            var sum = 0L;

            foreach (var line in fileLines)
            {
                var expr = parser.ParseLine(line);

                //7
                var result = expr.Evaluate();

                Console.WriteLine($"{result} FROM {expr.Display()}.");

                sum += result;
            }

            Console.WriteLine($"Sum of expressions: {sum}.");
        }

        #endregion
    }

    public class ExpressionParser
    {
        #region Fields

        private string remainingData;

        #endregion

        #region Instance Methods

        public Expression ParseLine(string input)
        {
            remainingData = input.Trim();

            return GetNextExpression();
        }

        private char ConsumeNextChar()
        {
            var nextChar = remainingData[^1];

            remainingData = remainingData.Substring(0,
                                                    remainingData.Length - 1);

            return nextChar;
        }

        private void ConsumeWhiteSpace()
        {
            while (remainingData.Length > 0
                   && char.IsWhiteSpace(PeekNextChar()))
            {
                ConsumeNextChar();
            }
        }

        private Expression GetArithmeticExpression()
        {
            var right = GetRightExpression();

            if (remainingData.Length == 0
                || PeekNextChar() == '(')
            {
                return right;
            }

            ConsumeWhiteSpace();

            var op = ConsumeNextChar();

            var left = GetNextExpression();

            Expression arithmeticExpression = op switch
            {
                '+' => new AdditionExpression(left,
                                              right),
                '*' => new MultiplicationExpression(left,
                                                    right),
                _ => throw new InvalidOperationException($"Unexpected char {op}.")
            };

            return arithmeticExpression;
        }

        private ConstantExpression GetDigit()
        {
            var digit = string.Empty;

            ConsumeWhiteSpace();

            while (remainingData.Length > 0
                   && char.IsDigit(PeekNextChar()))
            {
                digit += ConsumeNextChar();
            }

            var digitValue = long.Parse(digit);

            return new ConstantExpression(digitValue);
        }

        private Expression GetNextExpression()
        {
            ConsumeWhiteSpace();

            // We only want to return a parentheic expression if the whole thing is wrapped
            // Otherwise it is probably a nested item
            var firstOpen = remainingData.LastIndexOf('(');
            var lastChar = PeekNextChar();

            if (firstOpen == 0
                && lastChar == ')')
            {
                return GetParentheticalExpression();
            }

            return GetArithmeticExpression();
        }

        private Expression GetParentheticalExpression()
        {
            ConsumeWhiteSpace();

            var rightParen = ConsumeNextChar();

            Debug.Assert(rightParen == ')');

            var innerExpression = GetNextExpression();

            ConsumeWhiteSpace();

            var leftParen = ConsumeNextChar();

            Debug.Assert(leftParen == '(');

            return new ParentheticExpression(innerExpression);
        }

        private Expression GetRightExpression()
        {
            ConsumeWhiteSpace();

            var nextChar = PeekNextChar();

            if (char.IsDigit(nextChar))
            {
                return GetDigit();
            }

            return GetParentheticalExpression();
        }

        private char PeekNextChar()
        {
            return remainingData[^1];
        }

        #endregion
    }

    public abstract class Expression
    {
        #region Instance Methods

        public abstract string Display();

        public abstract long Evaluate();

        #endregion
    }

    public class ConstantExpression : Expression
    {
        #region Constructors

        public ConstantExpression(long value)
        {
            Value = value;
        }

        #endregion

        #region Instance Properties

        public long Value { get; }

        #endregion

        #region Instance Methods

        public override string Display()
        {
            return Value.ToString();
        }

        public override long Evaluate()
        {
            return Value;
        }

        #endregion
    }

    public class AdditionExpression : Expression
    {
        #region Constructors

        public AdditionExpression(Expression left,
                                  Expression right)
        {
            Left = left;
            Right = right;
        }

        #endregion

        #region Instance Properties

        public Expression Left { get; }

        public Expression Right { get; }

        #endregion

        #region Instance Methods

        public override string Display()
        {
            return $"{Left.Display()} + {Right.Display()}";
        }

        public override long Evaluate()
        {
            return Left.Evaluate() + Right.Evaluate();
        }

        #endregion
    }

    public class MultiplicationExpression : Expression
    {
        #region Constructors

        public MultiplicationExpression(Expression left,
                                        Expression right)
        {
            Left = left;
            Right = right;
        }

        #endregion

        #region Instance Properties

        public Expression Left { get; }

        public Expression Right { get; }

        #endregion

        #region Instance Methods

        public override string Display()
        {
            return $"{Left.Display()} * {Right.Display()}";
        }

        public override long Evaluate()
        {
            return Left.Evaluate() * Right.Evaluate();
        }

        #endregion
    }

    public class ParentheticExpression : Expression
    {
        #region Constructors

        public ParentheticExpression(Expression inner)
        {
            Inner = inner;
        }

        #endregion

        #region Instance Properties

        public Expression Inner { get; }

        #endregion

        #region Instance Methods

        public override string Display()
        {
            return $"({Inner.Display()})";
        }

        public override long Evaluate()
        {
            return Inner.Evaluate();
        }

        #endregion
    }
}