namespace Helpers.Interfaces;

public interface IPuzzleContainer
{
    IPuzzle GetPuzzle(int year, int day);
}