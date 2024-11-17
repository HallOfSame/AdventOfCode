using System;
using System.Threading.Tasks;
using Helpers.Exceptions;
using Helpers.Interfaces;

namespace Helpers.Structure
{
    public abstract class SingleExecutionPuzzle<TExecutionState> : ExecutionPuzzle<IExecutionResult, TExecutionState>
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

        public TProcessingResult ExecutePartOne()
        {
            try
            {
                return ExecutePuzzlePartOne();
            }
            catch (Exception ex)
            {
                throw new UnhandledExecutionException(ex);
            }
        }

        public TProcessingResult ExecutePartTwo()
        {
            try
            {
                return ExecutePuzzlePartTwo();
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

        protected abstract Task<TExecutionState> LoadInputState(string puzzleInput);
        protected abstract TProcessingResult ExecutePuzzlePartOne();
        protected abstract TProcessingResult ExecutePuzzlePartTwo();
    }
}
