using System;

namespace Helpers.Interfaces;

/// <summary>
/// Represents a puzzle that executes the solver in a step by step process that can be ran one step at a time.
/// </summary>
public interface IStepExecutionPuzzle<out TStepExecutionResult> : IExecutablePuzzle<TStepExecutionResult> where TStepExecutionResult : IStepExecutionResult {}

/// <summary>
/// To represent the return from doing one step of work.
/// </summary>
public interface IStepExecutionResult
{
    /// <summary>
    /// Set if an error occurred during this step.
    /// </summary>
    Exception? Exception { get; init; }
    
    /// <summary>
    /// <c>true</c> when this was the final step.
    /// </summary>
    bool IsCompleted { get; init; }

    /// <summary>
    /// Set when <see cref="IsCompleted"/> is <c>true</c>.
    /// TODO maybe set this on each step, idk if that would be useful.
    /// </summary>
    string? Result { get; init; }

    /// <summary>
    /// Current state after processing this step.
    /// </summary>
    IExecutionState CurrentState { get; init; }
}

/// <summary>
/// To tag a puzzle executor that works in steps.
/// Idea being to be able to run one step at a time and visualize in between.
/// </summary>
public interface ISingleStepExecutable<out TStepExecutionResult> where TStepExecutionResult : IStepExecutionResult
{
    /// <summary>
    /// Reset the internal state to the starting point.
    /// </summary>
    void ResetToInitialState();

    /// <summary>
    /// Used to revert the internal state to <paramref name="state"/>.
    /// </summary>
    void RevertState(IExecutionState state);

    /// <summary>
    /// Run a single step for part 1 and pause.
    /// </summary>
    TStepExecutionResult ExecuteStepPartOne();

    /// <summary>
    /// Run a single step for part 2 and pause.
    /// </summary>
    TStepExecutionResult ExecuteStepPartTwo();
}