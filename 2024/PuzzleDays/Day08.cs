using Helpers.Drawing;
using Helpers.Extensions;
using Helpers.FileReaders;
using Helpers.Interfaces;
using Helpers.Maps;
using Helpers.Structure;

namespace PuzzleDays
{
    public class Day08 : SingleExecutionPuzzle<Day08.ExecState>, IVisualize2d
    {
        public record ExecState(Dictionary<Coordinate, char> Map, HashSet<Coordinate> Antinodes);

        public override PuzzleInfo Info => new(2024, 8, "Resonant Collinearity");

        protected override async Task<ExecState> LoadInputState(string puzzleInput)
        {
            var grid = await new GridFileReader().ReadFromString(puzzleInput);

            return new ExecState(grid.ToDictionary(x => x.Coordinate, x => x.Value), []);
        }

        protected override async Task<string> ExecutePuzzlePartOne()
        {
            InitialState.Antinodes.Clear();

            var frequencies = InitialState.Map.Where(x => x.Value != '.')
                .GroupBy(x => x.Value)
                .ToList();

            foreach (var freq in frequencies)
            {
                var antennaPairs = freq.Combinations(2)
                    .ToList();

                foreach (var pair in antennaPairs)
                {
                    var p1 = pair.First();
                    var p2 = pair.Last();

                    var d1 = GetDistanceBetweenCoordinates(p1.Key, p2.Key);
                    var d2 = GetDistanceBetweenCoordinates(p2.Key, p1.Key);

                    var a1 = new Coordinate(p1.Key.X - d1.xDist, p1.Key.Y - d1.yDist);
                    var a2 = new Coordinate(p2.Key.X - d2.xDist, p2.Key.Y - d2.yDist);

                    if (InitialState.Map.ContainsKey(a1))
                    {
                        InitialState.Antinodes.Add(a1);
                    }

                    if (InitialState.Map.ContainsKey(a2))
                    {
                        InitialState.Antinodes.Add(a2);
                    }
                }
            }

            return InitialState.Antinodes.Count
                .ToString();
        }

        protected override async Task<string> ExecutePuzzlePartTwo()
        {
            throw new NotImplementedException();
        }

        private static (decimal xDist, decimal yDist) GetDistanceBetweenCoordinates(Coordinate c1, Coordinate c2)
        {
            var x = c2.X - c1.X;
            var y = c2.Y - c1.Y;
            return (x, y);
        }

        public DrawableCoordinate[] GetCoordinates()
        {
            var draw = InitialState.Map.Select(x => new DrawableCoordinate
                {
                    X = x.Key.X,
                    Y = x.Key.Y,
                    Color = InitialState.Antinodes.Contains(x.Key) ? "red" : null,
                    Text = InitialState.Antinodes.Contains(x.Key) && x.Value == '.' ? "#" : x.Value.ToString()
                })
                .ToArray();
            return draw;
        }
    }
}
