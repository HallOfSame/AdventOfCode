using System;
using System.Threading.Tasks;

using IntCodeInterpreter;
using IntCodeInterpreter.Input;

namespace Day05
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            var ops = await new FileInputParser().ReadOperationsFromFile("PuzzleInput.txt");

            var interpreter = new Interpreter();

            interpreter.ProcessOperations(ops,
                                          1,
                                          (x) =>
                                          {
                                              if (x != 0)
                                              {
                                                  // We don't really know how many outputs there will be
                                                  // So just write everything not 0. If there's more than 1 the interpreter is wrong
                                                  Console.WriteLine($"Output: {x}");
                                              }
                                          });
        }
    }
}
