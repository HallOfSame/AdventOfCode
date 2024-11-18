namespace InputStorageDatabase.Extensions;

public static class DbContextExtensions
{
    public static IQueryable<PuzzleInput> GetInputsForDay(this AdventOfCodeContext dbContext, int year, int day)
    {
        return dbContext.PuzzleInputs.Where(x => x.Year == year && x.Day == day);
    }
}