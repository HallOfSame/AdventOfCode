using System;
using System.Threading.Tasks;
using Helpers.Exceptions;
using Helpers.Interfaces;

namespace Helpers.Structure
{
    public abstract class SingleExecutionPuzzle<TExecutionState> : ExecutionPuzzle<ExecutionResult, TExecutionState>
        where TExecutionState : IExecutionState
    {}

    public abstract class ExecutionPuzzle<TProcessingResult, TExecutionState> : IExecutablePuzzle<TProcessingResult> where TExecutionState : IExecutionState
    {
        private TExecutionState? initialState;

        protected TExecutionState InitialState
        {
            get
            {
                return initialState ?? throw new InvalidOperationException("Accessed initial state before load.");
            }
        }

        public async Task<TProcessingResult> ExecutePartOne()
        {
            try
            {
                return await ExecutePuzzlePartOne();
            }
            catch (Exception ex)
            {
                throw new UnhandledExecutionException(ex);
            }
        }

        public async Task<TProcessingResult> ExecutePartTwo()
        {
            try
            {
                return await ExecutePuzzlePartTwo();
            }
            catch (Exception ex)
            {
                throw new UnhandledExecutionException(ex);
            }
        }

        public async Task LoadInput(string puzzleInput)
        {
            try
            {
                initialState = await LoadInputState(puzzleInput);
            }
            catch (Exception ex)
            {
                throw new InputLoadException(ex);
            }
        }

        public abstract PuzzleInfo Info { get; }

        protected abstract Task<TExecutionState> LoadInputState(string puzzleInput);
        protected abstract Task<TProcessingResult> ExecutePuzzlePartOne();
        protected abstract Task<TProcessingResult> ExecutePuzzlePartTwo();
    }
}
