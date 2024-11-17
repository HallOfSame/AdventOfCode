namespace Helpers.Interfaces;

public interface IStateCopier
{
    TState Copy<TState>(TState state);
}