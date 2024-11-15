namespace Helpers.Interfaces;

/// <summary>
/// To represent the return from doing one step of work.
/// </summary>
public interface IStepExecutionResult
{
    // TODO maybe a error or Exception here as well?

    bool IsCompleted { get; }
}

/// <summary>
/// To tag a puzzle executor that works in steps.
/// Idea being to be able to run one step at a time and visualize in between.
/// </summary>
public interface ISingleStepExecutable
{
    /// <summary>
    /// Reset the internal state to the starting point.
    /// TODO it would be cool to be able to step forwards and backwards, but that might be hard to manage.
    /// TODO could be do-able if there was some way to flush the state to an object so that we could restore it... maybe JSON?
    /// </summary>
    void ResetToInitialState();

    /// <summary>
    /// Run a single step and pause.
    /// </summary>
    IStepExecutionResult ExecuteStep();

    /// <summary>
    /// Run until execution completes.
    /// </summary>
    IStepExecutionResult ExecuteToCompletion();
}