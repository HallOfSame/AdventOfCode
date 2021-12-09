using Spectre.Console;
using System.Threading.Tasks;

namespace Helpers.Structure
{
    public abstract class ProblemBase
    {
        public async Task SolvePartOne()
        {
            var partOneResult = await SolvePartOneInternal();

            AnsiConsole.MarkupLine($"[blue]Part One Result: {partOneResult}.[/]");
        }

        protected abstract Task<string> SolvePartOneInternal();

        public async Task SolvePartTwo()
        {
            var partTwoResult = await SolvePartTwoInternal();

            AnsiConsole.MarkupLine($"[blue]Part Two Result: {partTwoResult}.[/]");
        }

        protected abstract Task<string> SolvePartTwoInternal();

        public abstract Task ReadInput();
    }
}
