using System;
using System.Threading.Tasks;

namespace Helpers.Interfaces;

/// <summary>
/// Result of executing part one or two of a puzzle.
/// </summary>
public interface IExecutionResult
{
    public string Result { get; set; }
    public Exception? Exception { get; set; }
}

/// <summary>
/// Represents a puzzle that does not have a set steppable process and runs the solver in one action.
/// </summary>
public interface ISingleExecutionPuzzle : IExecutablePuzzle<IExecutionResult> {}

/// <summary>
/// Basic interface to represent a class can execute a puzzle.
/// </summary>
public interface IExecutablePuzzle<out TProcessingResult>
{
    TProcessingResult ExecutePartOne();

    TProcessingResult ExecutePartTwo();

    /// <summary>
    /// Loads the puzzle input as a string.
    /// I think this will be safe to assume, basically every puzzle is a (possibly multiline) string at the end of the day.
    /// </summary>
    Task LoadInput(string puzzleInput);
}