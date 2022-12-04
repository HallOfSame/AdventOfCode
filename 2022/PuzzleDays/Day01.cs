using Helpers.FileReaders;
using Helpers.Heaps;
using Helpers.Structure;

namespace PuzzleDays;

public class Day01Problem : ProblemBase
{
    protected override async Task<string> SolvePartOneInternal()
    {
        // Peek so we can run part 2 easier
        return elves.Peek()
                    .TotalCalories.ToString();
    }

    protected override async Task<string> SolvePartTwoInternal()
    {
        var one = elves.Dequeue();

        var two = elves.Dequeue();

        var three = elves.Dequeue();

        return (one.TotalCalories + two.TotalCalories + three.TotalCalories).ToString();
    }

    public override async Task ReadInput()
    {
        var strings = await new StringFileReader().ReadInputFromFile();

        elves = MaxHeap.CreateMaxHeap<Elf>();

        var currentFood = new List<int>();

        void AddElfToHeap()
        {
            var newElf = new Elf(currentFood);
            elves.Enqueue(newElf, newElf.TotalCalories);
            currentFood.Clear();
        }

        foreach (var str in strings)
        {
            if (string.IsNullOrEmpty(str))
            {
                AddElfToHeap();
                continue;
            }

            var food = int.Parse(str);
            currentFood.Add(food);
        }

        AddElfToHeap();
    }

    private PriorityQueue<Elf, int> elves;
}

class Elf
{
    public int TotalCalories { get; }

    public Elf(IEnumerable<int> meals)
    {
        TotalCalories = meals.Sum();
    }
}