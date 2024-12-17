using Helpers.FileReaders;
using Helpers.Heaps;
using Helpers.Maps;
using Helpers.Structure;

namespace PuzzleDays
{
    public class Day16 : SingleExecutionPuzzle<Day16.ExecState>
    {
        public record ExecState(Dictionary<Coordinate, char> Map);

        public override PuzzleInfo Info => new(2024, 16, "Reindeer Maze");

        protected override async Task<ExecState> LoadInputState(string puzzleInput)
        {
            var grid = await new GridFileReader().ReadFromString(puzzleInput.Trim());
            return new ExecState(grid.ToDictionary(x => x.Coordinate, x => x.Value));
        }

        protected override async Task<string> ExecutePuzzlePartOne()
        {
            var heapToCheck = MinHeap.CreateMinHeap<(Coordinate location, Direction facing, int score)>();
            var start = InitialState.Map.Single(x => x.Value == 'S');
            var end = InitialState.Map.Single(x => x.Value == 'E');
            var visited = new HashSet<(Coordinate, Direction)>();

            heapToCheck.Enqueue((start.Key, Direction.East, 0), 0);

            while (heapToCheck.Count != 0)
            {
                var next = heapToCheck.Dequeue();

                if (next.location == end.Key)
                {
                    return next.score.ToString();
                }

                var forward = next.location.GetDirection(next.facing);
                var atForwardLocation = InitialState.Map[forward];

                if (atForwardLocation != '#')
                {
                    EnqueueLocation(forward, next.facing, next.score + 1);
                }

                var turnRight = next.facing.TurnRight90();
                EnqueueLocation(next.location, turnRight, next.score + 1000);
                var turnLeft = next.facing.TurnLeft90();
                EnqueueLocation(next.location, turnLeft, next.score + 1000);
            }

            throw new InvalidOperationException("Did not find end of maze");

            void EnqueueLocation(Coordinate location, Direction facing, int newScore)
            {
                if (visited.Add((location, facing)))
                {
                    heapToCheck.Enqueue((location, facing, newScore), newScore);
                }
            }
        }

        protected override async Task<string> ExecutePuzzlePartTwo()
        {
            throw new NotImplementedException();
        }
    }
}
