using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using IntCodeInterpreter;
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

            var interpreter = new Interpreter();

            var savedOperations = new List<int>(operations);

            interpreter.ProcessOperations(operations);

            Console.WriteLine($"Value at 0 after running program: {operations[0]}.");

            var noun = 1;

            var verb = 1;

            var output = 0;

            var lastNoun = 0;

            while(true)
            {
                output = TestNounVerb(savedOperations,
                                      noun,
                                      verb,
                                      interpreter);

                if (output < targetValue)
                {
                    noun++;
                }
                else if (output > targetValue)
                {
                    break;
                }
                else
                {
                    return;
                }
            }

            // Since we went over
            noun--;

            while(true)
            {
                output = TestNounVerb(savedOperations,
                                      noun,
                                      verb,
                                      interpreter);

                if (output < targetValue)
                {
                    verb++;
                }
                else if (output > targetValue)
                {
                    return;
                }
                else
                {
                    break;
                }
            }

            Console.WriteLine($"Result value {(noun * 100) + verb}.");
        }

        private const int targetValue = 19690720;

        private static int TestNounVerb(List<int> savedOperations,
                                         int noun,
                                         int verb,
                                         Interpreter interpreter)
        {
            var operations = savedOperations.ToList();

            operations[1] = noun;
            operations[2] = verb;
            
            interpreter.ProcessOperations(operations);

            Console.WriteLine($"{noun}{verb} gives {operations[0]} as opposed to {targetValue}. Diff {operations[0] - targetValue}.");

            return operations[0];
        }

        #endregion
    }
}