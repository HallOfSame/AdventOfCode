using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Helpers.Exceptions;
using Helpers.Interfaces;
using InputStorageDatabase;

namespace Helpers.Structure
{
    public abstract class SingleExecutionPuzzle<TExecutionState> : ExecutionPuzzle<ExecutionResult, TExecutionState>, ISingleExecutionPuzzle
    {}

    public abstract class ExecutionPuzzle<TProcessingResult, TExecutionState> : IExecutablePuzzle<TProcessingResult> where TProcessingResult : ExecutionResult, new()
    {
        private TExecutionState? initialState;

        protected TExecutionState InitialState => initialState ?? throw new InvalidOperationException("Accessed initial state before load.");

        public async Task<TProcessingResult> ExecutePartOne()
        {
            return await ExecuteAndCatch(ExecutePuzzlePartOne);
        }

        public async Task<TProcessingResult> ExecutePartTwo()
        {
            return await ExecuteAndCatch(ExecutePuzzlePartTwo);
        }

        private static async Task<TProcessingResult> ExecuteAndCatch(Func<Task<string>> executeMethod)
        {
            var sw = new Stopwatch();
            sw.Start();

            try
            {
                var result = await executeMethod();
                sw.Stop();
                return new TProcessingResult
                {
                    Result = result,
                    ElapsedTime = sw.Elapsed
                };
            }
            catch (Exception ex)
            {
                sw.Stop();
                return new TProcessingResult
                {
                    Exception = ex,
                    ElapsedTime = sw.Elapsed
                };
            }
        }

        public async Task LoadInput(string puzzleInput, PuzzleInputType inputType)
        {
            try
            {
                initialState = await LoadInputState(puzzleInput, inputType);
            }
            catch (Exception ex)
            {
                throw new InputLoadException(ex);
            }
        }

        public abstract PuzzleInfo Info { get; }

        protected abstract Task<TExecutionState> LoadInputState(string puzzleInput, PuzzleInputType inputType);
        protected abstract Task<string> ExecutePuzzlePartOne();
        protected abstract Task<string> ExecutePuzzlePartTwo();
    }
}
