using System;

namespace Helpers.Interfaces;

/// <summary>
/// Result of executing part one or two of a puzzle.
/// </summary>
public interface IExecutionResult
{
    public string Result { get; set; }
    public Exception? Exception { get; set; }
    public bool IsCompleted { get; }
}

/// <summary>
/// Basic interface to represent a class can execute a puzzle.
/// </summary>
public interface IExecutablePuzzle
{
    IExecutionResult ExecutePartOne();

    IExecutionResult ExecutePartTwo();

    /// <summary>
    /// Loads the puzzle input as a string.
    /// I think this will be safe to assume, basically every puzzle is a (possibly multiline) string at the end of the day.
    /// </summary>
    void LoadInput(string puzzleInput);
}