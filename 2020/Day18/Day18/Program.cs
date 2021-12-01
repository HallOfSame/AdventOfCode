using System;
using System.IO;

namespace Day18
{
    internal class Program
    {
        #region Class Methods

        private static void Main(string[] args)
        {
            var fileLines = File.ReadAllLines("PuzzleInput.txt");

            var parser1 = new ExpressionParser(false);
            var parser2 = new ExpressionParser(true);

            var sum1 = 0L;
            var sum2 = 0L;

            foreach (var line in fileLines)
            {
                var expr1 = parser1.ParseLine(line);

                var result1 = expr1.Evaluate();

                sum1 += result1;

                var expr2 = parser2.ParseLine(line);

                var result2 = expr2.Evaluate();

                sum2 += result2;
            }

            Console.WriteLine($"Sum of expressions: PT1: {sum1} PT2: {sum2}.");
        }

        #endregion
    }
}