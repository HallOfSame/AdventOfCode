using System;
using System.Diagnostics;

using Day18.Expressions;

namespace Day18
{
    public class ExpressionParser
    {
        private readonly bool isPartTwo;

        public ExpressionParser(bool isPartTwo)
        {
            this.isPartTwo = isPartTwo;
        }

        #region Fields

        // The remaining data from the input we haven't consumed yet
        private string remainingData;

        #endregion

        #region Instance Methods

        // Parse the input in to an expression
        public Expression ParseLine(string input)
        {
            remainingData = input.Trim();

            return GetNextExpression();
        }

        // Remove next char from the end of the remaining data and return it
        private char ConsumeNextChar()
        {
            var nextChar = remainingData[^1];

            remainingData = remainingData.Substring(0,
                                                    remainingData.Length - 1);

            return nextChar;
        }

        // Consume any whitespace characters
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
            // Start with the right side of things
            var right = GetRightExpression();

            // If out of data or we hit a paren then return
            if (remainingData.Length == 0
                || PeekNextChar() == '(')
            {
                return right;
            }

            ConsumeWhiteSpace();

            // Get the op next
            var op = ConsumeNextChar();

            // Then get the entire expression for the left side
            var left = GetNextExpression();

            if (isPartTwo
                && left is MultiplicationExpression multLeft
                && op == '+')
            {
                // Part two logic states that addition has precedence
                // So what we can do here is rewrite the expression in progress to give addition that precedence
                // The right side of the multiplication should be its right expression + the additions right
                // Then we return a multiplication of the left and this new addition expression
                // You might be thinking to yourself, there's no way that's gonna work in all case
                // But it does (at least for my puzzle input). Hell yeah
                var actualRight = new AdditionExpression(multLeft.Right,
                                                         right);
                return new MultiplicationExpression(multLeft.Left,
                                                    actualRight);
            }

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

        // Handles only getting the next digit (or parenthetic expression)
        // Because of order of ops we don't want to process too far here
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

        // Checks the next char without consuming it
        private char PeekNextChar()
        {
            return remainingData[^1];
        }

        #endregion
    }
}