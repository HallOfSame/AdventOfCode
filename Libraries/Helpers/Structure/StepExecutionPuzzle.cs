using System;
using System.Threading.Tasks;
using Helpers.Exceptions;
using Helpers.Interfaces;

namespace Helpers.Structure
{
    public abstract class StepExecutionPuzzle<TExecutionState> : ExecutionPuzzle<StepExecutionResult, TExecutionState>, IStepExecutionPuzzle
    {
        private readonly JsonStateCopier stateCopier = new();

        private TExecutionState? currentState;

        protected TExecutionState CurrentState
        {
            get => currentState ?? throw new InvalidOperationException("Accessed current state before load");
            set => currentState = value;
        }

        public Task ResetToInitialState()
        {
            CurrentState = stateCopier.Copy(InitialState);
            return Task.CompletedTask;
        }

        public Task RevertState(object state)
        {
            CurrentState = stateCopier.Copy((TExecutionState)state);
            return Task.CompletedTask;
        }

        public async Task<StepExecutionResult> ExecuteStepPartOne()
        {
            try
            {
                var (complete, puzzleResult) = await ExecutePuzzleStepPartOne();

                var result = new StepExecutionResult
                {
                    IsCompleted = complete,
                    Result = puzzleResult,
                    CurrentState = stateCopier.Copy(CurrentState!)
                };

                return result;
            }
            catch (Exception ex)
            {
                return new StepExecutionResult
                {
                    IsCompleted = false,
                    Exception = new UnhandledExecutionException(ex)
                };
            }
        }

        public async Task<StepExecutionResult> ExecuteStepPartTwo()
        {
            try
            {
                var (complete, puzzleResult) = await ExecutePuzzleStepPartTwo();

                var result = new StepExecutionResult
                {
                    IsCompleted = complete,
                    Result = puzzleResult,
                    CurrentState = stateCopier.Copy(CurrentState!)
                };

                return result;
            }
            catch (Exception ex)
            {
                return new StepExecutionResult
                {
                    IsCompleted = false,
                    Exception = new UnhandledExecutionException(ex)
                };
            }
        }

        protected override async Task<string> ExecutePuzzlePartOne()
        {
            StepExecutionResult result;

            do
            {
                result = await ExecuteStepPartOne();
            } while (!result.IsCompleted);

            return result.Result!;
        }

        protected override async Task<string> ExecutePuzzlePartTwo()
        {
            StepExecutionResult result;

            do
            {
                result = await ExecuteStepPartTwo();
            } while (!result.IsCompleted);

            return result.Result!;
        }

        protected sealed override async Task<TExecutionState> LoadInputState(string puzzleInput)
        {
            var state = await LoadInitialState(puzzleInput);
            CurrentState = stateCopier.Copy(state);
            return state;
        }

        protected abstract Task<TExecutionState> LoadInitialState(string puzzleInput);
        protected abstract Task<(bool isComplete, string? result)> ExecutePuzzleStepPartOne();
        protected abstract Task<(bool isComplete, string? result)> ExecutePuzzleStepPartTwo();
    }
}
