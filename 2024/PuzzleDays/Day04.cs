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
            var aCharactersToStartFrom = InitialState.Input.Where(x => x.Value == 'A');
            coordinatesContainingXmas = [];
            var total = 0;

            foreach (var startPoint in aCharactersToStartFrom)
            {
                var found = FindMasInXFromStartPoint(startPoint.Key);

                if (found.Count > 0)
                {
                    total++;
                }

                coordinatesContainingXmas.UnionWith(found);
            }

            return total.ToString();
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

        private HashSet<Coordinate> FindMasInXFromStartPoint(Coordinate start)
        {
            var coordinatesInXmas = new HashSet<Coordinate>();

            char[] diagOne = [CharAtCoord(start.Move(Direction.NorthWest)), CharAtCoord(start.Move(Direction.SouthEast))];
            char[] diagTwo = [CharAtCoord(start.Move(Direction.NorthEast)), CharAtCoord(start.Move(Direction.SouthWest))];

            if (IsValidHalf(diagOne) && IsValidHalf(diagTwo))
            {
                // I don't really feel like re-arranging this to add em all
                // So I won't unless I need it to debug
                // It just returns this for drawing the output
                coordinatesInXmas.Add(start);
            }

            return coordinatesInXmas;

            bool IsValidHalf(char[] diag)
            {
                return (diag[0] == 'M' && diag[1] == 'S') || (diag[0] == 'S' && diag[1] == 'M');
            }
        }

        private bool CoordinateIsChar(Coordinate coordinate, char testChar)
        {
            return InitialState.Input.TryGetValue(coordinate, out var actualChar) && actualChar == testChar;
        }

        private char CharAtCoord(Coordinate coordinate)
        {
            // Idk what the default for char is, but it won't match what we test on
            InitialState.Input.TryGetValue(coordinate, out var actualChar);
            return actualChar;
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
