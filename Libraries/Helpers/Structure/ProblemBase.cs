using System;

using Spectre.Console;
using System.Threading.Tasks;

namespace Helpers.Structure
{
    [Obsolete($"Replaced with ExecutionPuzzle and StepExecutionPuzzle")]
    public abstract class ProblemBase
    {
        public async Task<string> SolvePartOne()
        {
            var partOneResult = await SolvePartOneInternal();

            AnsiConsole.MarkupLine($"[blue]Part One Result: {partOneResult}.[/]");

            return partOneResult;
        }

        protected abstract Task<string> SolvePartOneInternal();

        public async Task<string> SolvePartTwo()
        {
            var partTwoResult = await SolvePartTwoInternal();

            AnsiConsole.MarkupLine($"[blue]Part Two Result: {partTwoResult}.[/]");

            return partTwoResult;
        }

        protected abstract Task<string> SolvePartTwoInternal();

        public abstract Task ReadInput();
    }
}
