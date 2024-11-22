using System;
using System.Threading.Tasks;
using Helpers.Exceptions;
using Helpers.Interfaces;

namespace Helpers.Structure
{
    public abstract class StepExecutionPuzzle<TExecutionState> : ExecutionPuzzle<StepExecutionResult, TExecutionState>, IStepExecutionPuzzle where TExecutionState : IExecutionState
    {
        private readonly JsonStateCopier stateCopier = new();

        protected TExecutionState? CurrentState;

        public Task ResetToInitialState()
        {
            CurrentState = stateCopier.Copy(InitialState);
            return Task.CompletedTask;
        }

        public Task RevertState(IExecutionState state)
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

        protected override async Task<StepExecutionResult> ExecutePuzzlePartOne()
        {
            StepExecutionResult result;

            do
            {
                result = await ExecuteStepPartOne();
            } while (!result.IsCompleted);

            return result;
        }

        protected override async Task<StepExecutionResult> ExecutePuzzlePartTwo()
        {
            StepExecutionResult result;

            do
            {
                result = await ExecuteStepPartTwo();
            } while (!result.IsCompleted);

            return result;
        }

        protected abstract Task<(bool isComplete, string? result)> ExecutePuzzleStepPartOne();
        protected abstract Task<(bool isComplete, string? result)> ExecutePuzzleStepPartTwo();
    }
}
