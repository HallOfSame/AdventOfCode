using System;
using Helpers.Exceptions;
using Helpers.Interfaces;

namespace Helpers.Structure
{
    public abstract class StepExecutionPuzzle<TStepExecutionResult, TExecutionState> : ExecutionPuzzle<TStepExecutionResult, TExecutionState>, IStepExecutionPuzzle<TStepExecutionResult>, ISingleStepExecutable<TStepExecutionResult> where TExecutionState : IExecutionState where TStepExecutionResult : IStepExecutionResult, new()
    {
        private readonly JsonStateCopier stateCopier;

        protected StepExecutionPuzzle()
        {
            stateCopier = new JsonStateCopier();
        }

        protected TExecutionState? CurrentState;

        public void ResetToInitialState()
        {
            CurrentState = stateCopier.Copy(InitialState);
        }

        public void RevertState(IExecutionState state)
        {
            CurrentState = stateCopier.Copy((TExecutionState)state);
        }

        public TStepExecutionResult ExecuteStepPartOne()
        {
            try
            {
                var (complete, puzzleResult) = ExecutePuzzleStepPartOne();

                var result = new TStepExecutionResult
                {
                    IsCompleted = complete,
                    Result = puzzleResult,
                    CurrentState = stateCopier.Copy(CurrentState!)
                };

                return result;
            }
            catch (Exception ex)
            {
                return new TStepExecutionResult
                {
                    IsCompleted = false,
                    Exception = new UnhandledExecutionException(ex)
                };
            }
        }

        public TStepExecutionResult ExecuteStepPartTwo()
        {
            try
            {
                var (complete, puzzleResult) = ExecutePuzzleStepPartTwo();

                var result = new TStepExecutionResult
                {
                    IsCompleted = complete,
                    Result = puzzleResult,
                    CurrentState = stateCopier.Copy(CurrentState!)
                };

                return result;
            }
            catch (Exception ex)
            {
                return new TStepExecutionResult
                {
                    IsCompleted = false,
                    Exception = new UnhandledExecutionException(ex)
                };
            }
        }

        protected override TStepExecutionResult ExecutePuzzlePartOne()
        {
            TStepExecutionResult result;

            do
            {
                result = ExecuteStepPartOne();
            } while (!result.IsCompleted);

            return result;
        }

        protected override TStepExecutionResult ExecutePuzzlePartTwo()
        {
            TStepExecutionResult result;

            do
            {
                result = ExecuteStepPartTwo();
            } while (!result.IsCompleted);

            return result;
        }

        protected abstract (bool isComplete, string? result) ExecutePuzzleStepPartOne();
        protected abstract (bool isComplete, string? result) ExecutePuzzleStepPartTwo();
    }
}
