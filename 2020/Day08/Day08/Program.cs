using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace Day08
{
    internal class Program
    {
        #region Class Methods

        public static async Task Main(string[] args)
        {
            List<Operation> operations;

            await using (var file = File.OpenRead("PuzzleInput.txt"))
            {
                using var reader = new StreamReader(file);

                operations = await new InputParser().ReadInput(reader);
            }

            var emulator = new Emulator();

            // Pt 1
            var result = emulator.RunOperations(operations,
                                                out _);

            Console.WriteLine($"Result of running operations {result}. Accumulator at {emulator.AccumulatorValue}.");

            // Pt 2

            var sw = new Stopwatch();
            sw.Start();
            var fixer = new OperationFixer(operations);

            var fixedAccValue = fixer.GetAccumulatorForFixedProgram();
            sw.Stop();
            Console.WriteLine($"Result of accumulator with fixed instructions: {fixedAccValue}. In {sw.ElapsedMilliseconds} ms.");

            // Pt 2 inspired by reddit

            sw.Restart();
            var analyzer = new OperationAnalyzer(operations);
            var fixedOps = analyzer.AnalyzeAndFixInstructions();
            emulator.RunOperations(fixedOps,
                                   out _);
            sw.Stop();
            Console.WriteLine($"Accumulator of analyzed {emulator.AccumulatorValue}. In {sw.ElapsedMilliseconds} ms.");
        }

        #endregion
    }
}