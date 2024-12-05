using Helpers.Drawing;
using Helpers.Extensions;
using Helpers.FileReaders;
using Helpers.Interfaces;
using Helpers.Maps;
using Helpers.Structure;

namespace PuzzleDays
{
    public class Day04 : SingleExecutionPuzzle<Day04.ExecState>, IVisualize2d
    {
        public record ExecState(Dictionary<Coordinate, char> Input);

        public override PuzzleInfo Info => new(2024, 04, "Ceres Search");
        protected override async Task<ExecState> LoadInputState(string puzzleInput)
        {
            var coordinates = await new GridFileReader().ReadFromString(puzzleInput);

            return new ExecState(coordinates.ToDictionary(x => x.Coordinate, x => x.Value));
        }

        private HashSet<Coordinate> coordinatesContainingXmas = [];

        protected override async Task<string> ExecutePuzzlePartOne()
        {
            var xCharactersToStartFrom = InitialState.Input.Where(x => x.Value == 'X');
            coordinatesContainingXmas = [];
            var total = 0;

            foreach (var startPoint in xCharactersToStartFrom)
            {
                var (count, found) = FindXmasFromStartPoint(startPoint.Key);
                coordinatesContainingXmas.UnionWith(found);
                total += count;
            }

            return total.ToString();
        }

        protected override async Task<string> ExecutePuzzlePartTwo()
        {
            throw new NotImplementedException();
        }

        private (int foundCount, HashSet<Coordinate> foundLoc) FindXmasFromStartPoint(Coordinate start)
        {
            var coordinatesInXmas = new HashSet<Coordinate>();
            var foundCount = 0;
            var neighbors = start.GetNeighbors(true);

            foreach (var neighbor in neighbors)
            {
                if (!CoordinateIsChar(neighbor, 'M'))
                {
                    continue;
                }

                var direction = (Direction)neighbors.IndexOf(neighbor);
                var potentialA = neighbor.Move(direction);

                if (!CoordinateIsChar(potentialA, 'A'))
                {
                    continue;
                }

                var potentialS = potentialA.Move(direction);

                if (!CoordinateIsChar(potentialS, 'S'))
                {
                    continue;
                }

                foundCount++;
                coordinatesInXmas.Add(start);
                coordinatesInXmas.Add(neighbor);
                coordinatesInXmas.Add(potentialA);
                coordinatesInXmas.Add(potentialS);
            }

            return (foundCount, coordinatesInXmas);
        }

        private bool CoordinateIsChar(Coordinate coordinate, char testChar)
        {
            return InitialState.Input.TryGetValue(coordinate, out var actualChar) && actualChar == testChar;
        }

        public DrawableCoordinate[] GetCoordinates()
        {
            return InitialState.Input.Select(x => new DrawableCoordinate
                {
                    X = x.Key.X,
                    Y = x.Key.Y,
                    Text = x.Value.ToString(),
                    Color = coordinatesContainingXmas.Contains(x.Key) ? "yellow" : null
                })
                .ToArray();
        }
    }
}
