using System;
using System.Threading.Tasks;

using IntCodeInterpreter.Input;

namespace Day02
{
    internal class Program
    {
        #region Class Methods

        public static async Task Main(string[] args)
        {
            var operations = await new FileInputParser().ReadOperationsFromFile("PuzzleInput.txt");

            // Set 1202 error
            operations[1] = 12;
            operations[2] = 2;

            var outputList = new IntCodeInterpreter.IntCodeInterpreter().ProcessOperations(operations);

            Console.WriteLine($"Value at 0 after running program: {outputList[0]}.");
            Console.WriteLine($"Output: {string.Join(", ", outputList)}.");
        }

        #endregion
    }
}