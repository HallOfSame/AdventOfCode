using Helpers.Extensions;
using Helpers.FileReaders;
using Helpers.Maps;
using Helpers.Structure;

namespace PuzzleDays
{
    public class Day10 : SingleExecutionPuzzle<Day10.ExecState>
    {
        public record ExecState(Dictionary<Coordinate, int?> Map);

        public override PuzzleInfo Info => new(2024, 10, "Hoof It");

        protected override async Task<ExecState> LoadInputState(string puzzleInput)
        {
            var coords = await new GridFileReader().ReadFromString(puzzleInput.Trim());

            // . only appears in the examples, but it helps for running them
            return new ExecState(coords.ToDictionary(x => x.Coordinate, x => x.Value == '.'
                                                                                 ? default(int?)
                                                                                 : int.Parse(x.Value.ToString())));
        }

        protected override async Task<string> ExecutePuzzlePartOne()
        {
            var trailheads = InitialState.Map.Where(x => x.Value == 0).ToList();

            var totalScore = 0;

            foreach (var trailhead in trailheads)
            {
                totalScore += GetTrailheadScore(trailhead.Key);
            }

            return totalScore.ToString();
        }

        protected override async Task<string> ExecutePuzzlePartTwo()
        {
            throw new NotImplementedException();
        }

        private int GetTrailheadScore(Coordinate start)
        {
            var score = 0;
            var queue = new Queue<Coordinate>();
            queue.Enqueue(start);
            var visited = new HashSet<Coordinate>();

            while (queue.Count > 0) 
            {
                var current = queue.Dequeue();
                var currentValue = InitialState.Map[current]!.Value;

                if (currentValue == 9)
                {
                    score++;
                    continue;
                }

                var validNeighbors = current.GetNeighbors().Where(x => InitialState.Map.TryGetValue(x, out var neighborVal) && neighborVal == currentValue + 1);

                foreach(var neighbor in validNeighbors) 
                {   
                    if (visited.Add(neighbor))
                    {
                        queue.Enqueue(neighbor);
                    }
                }
            }

            return score;
        }
    }
}
