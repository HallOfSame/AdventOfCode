using System;
using System.Threading.Tasks;
using Helpers.Structure;

namespace Helpers.Interfaces;

/// <summary>
/// Result of executing part one or two of a puzzle.
/// </summary>
public class ExecutionResult
{
    public string? Result { get; init; }
    public Exception? Exception { get; init; }
}

/// <summary>
/// Represents a puzzle that does not have a set step-by-step process and runs the solver in one action.
/// </summary>
public interface ISingleExecutionPuzzle : IExecutablePuzzle<ExecutionResult> {}

/// <summary>
/// Basic interface to represent a class can execute a puzzle.
/// </summary>
public interface IExecutablePuzzle<TProcessingResult> : IPuzzle
{
    Task<TProcessingResult> ExecutePartOne();

    Task<TProcessingResult> ExecutePartTwo();

    /// <summary>
    /// Loads the puzzle input as a string.
    /// I think this will be safe to assume, basically every puzzle is a (possibly multiline) string at the end of the day.
    /// </summary>
    Task LoadInput(string puzzleInput);
}

public interface IPuzzle
{
    PuzzleInfo Info { get; }
}