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
}