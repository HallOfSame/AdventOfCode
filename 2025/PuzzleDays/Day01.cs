using Helpers.Structure;
using InputStorageDatabase;

namespace PuzzleDays;

public class Day01 : SingleExecutionPuzzle<Day01.State>
{
    public override PuzzleInfo Info => new(2025, 01, "Secret Entrance");
    
    protected override Task<string> ExecutePuzzlePartOne()
    {
        var answer = 0;

        foreach (var instruction in InitialState.Instructions)
        {
            // To simplify, ignore when we fully rotate during a move
            var length = instruction.Length % 100;

            var change = instruction.Direction switch
            {
                SpinDirection.Right => length,
                SpinDirection.Left => length * -1,
                _ => throw new InvalidOperationException("Invalid direction")
            };

            var newPosition = InitialState.Location + change;

            newPosition = newPosition switch
            {
                // Handle turning left past 0 but subtracting (newPosition is negative here) from 100
                < 0 => 100 + newPosition,
                // Mod 100 if we turned right from 99
                > 99 => newPosition % 100,
                // Otherwise this is just the right value
                _ => newPosition
            };

            if (newPosition == 0)
            {
                answer++;
            }

            InitialState.Location = newPosition;
        }

        return Task.FromResult(answer.ToString());
    }

    protected override async Task<string> ExecutePuzzlePartTwo()
    {
        throw new NotImplementedException();
    }

    protected override Task<State> LoadInputState(string puzzleInput, PuzzleInputType inputType)
    {
        var lines = puzzleInput.Trim().Split('\n');

        var instructions = new List<SpinInstruction>();

        foreach (var line in lines)
        {
            var direction = line[0] switch
            {
                'L' => SpinDirection.Left,
                'R' => SpinDirection.Right,
                _ => throw new InvalidOperationException($"{line[0]} is not a valid direction")
            };

            var distance = int.Parse(line[1..]);

            instructions.Add(new SpinInstruction(direction, distance));
        }

        return Task.FromResult(new State
        {
            Instructions = instructions,
            Location = 50
        });
    }

    public class State
    {
        public required int Location { get; set; }

        public required List<SpinInstruction> Instructions { get; set; }
    }

    public record SpinInstruction(SpinDirection Direction, int Length);

    public enum SpinDirection
    {
        Left,
        Right
    };
}