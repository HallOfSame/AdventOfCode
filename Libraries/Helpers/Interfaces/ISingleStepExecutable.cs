using System.Threading.Tasks;

namespace Helpers.Interfaces;

/// <summary>
/// Represents a puzzle that executes the solver in a step-by-step process that can be run one step at a time.
/// </summary>
public interface IStepExecutionPuzzle : IExecutablePuzzle<StepExecutionResult>, ISingleStepExecutable {}

/// <summary>
/// To represent the return from doing one step of work.
/// </summary>
public class StepExecutionResult : ExecutionResult
{
    /// <summary>
    /// <c>true</c> when this was the final step.
    /// </summary>
    public bool IsCompleted { get; init; }

    /// <summary>
    /// Current state after processing this step.
    /// Should only be <c>null</c> if we had an exception. (Maybe it shouldn't even be null then?)
    /// </summary>
    public object? CurrentState { get; init; }
}

/// <summary>
/// To tag a puzzle executor that works in steps.
/// Idea being to be able to run one step at a time and visualize in between.
/// </summary>
public interface ISingleStepExecutable
{
    /// <summary>
    /// Reset the internal state to the starting point.
    /// </summary>
    Task ResetToInitialState();

    /// <summary>
    /// Used to revert the internal state to <paramref name="state"/>.
    /// </summary>
    Task RevertState(object state);

    /// <summary>
    /// Run a single step for part 1 and pause.
    /// </summary>
    Task<StepExecutionResult> ExecuteStepPartOne();

    /// <summary>
    /// Run a single step for part 2 and pause.
    /// </summary>
    Task<StepExecutionResult> ExecuteStepPartTwo();
}