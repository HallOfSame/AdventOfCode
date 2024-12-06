using Helpers.Drawing;
using Helpers.FileReaders;
using Helpers.Interfaces;
using Helpers.Maps;
using Helpers.Structure;

namespace PuzzleDays
{
    public class Day06 : StepExecutionPuzzle<Day06.ExecState>, IVisualize2d
    {
        public record ExecState(Coordinate GuardPosition, Direction GuardFacing, Dictionary<Coordinate, char> Map, HashSet<Coordinate> GuardVisited);

        public override PuzzleInfo Info => new(2024, 6, "Guard Gallivant");
        protected override async Task<ExecState> LoadInitialState(string puzzleInput)
        {
            var coordinates = await new GridFileReader().ReadFromString(puzzleInput);

            List<(char mapChar, Direction matchingDirection)> guardIndicators =
                [('^', Direction.North), ('>', Direction.East), ('v', Direction.South), ('<', Direction.West)];

            var map = coordinates.ToDictionary(x => x.Coordinate, x => x.Value);
            var guardPosition = coordinates.Single(x => guardIndicators.Any(indicator => indicator.mapChar == x.Value));
            var guardFacing = guardIndicators.Single(x => x.mapChar == guardPosition.Value)
                .matchingDirection;
            var guardVisited = new HashSet<Coordinate> { guardPosition.Coordinate };
            map[guardPosition.Coordinate] = '.';
            return new ExecState(guardPosition.Coordinate, guardFacing, map, guardVisited);
        }

        protected override async Task<(bool isComplete, string? result)> ExecutePuzzleStepPartOne()
        {
            var (position, direction) = GetGuardNextLocation();

            var isComplete = !CurrentState.Map.ContainsKey(position);

            if (isComplete)
            {
                return (true, CurrentState.GuardVisited.Count.ToString());
            }

            CurrentState.GuardVisited.Add(position);
            CurrentState = CurrentState with { GuardPosition = position, GuardFacing = direction };
            return (false, CurrentState.GuardVisited.Count.ToString());
        }

        protected override async Task<(bool isComplete, string? result)> ExecutePuzzleStepPartTwo()
        {
            throw new NotImplementedException();
        }

        private (Coordinate, Direction) GetGuardNextLocation()
        {
            var nextPosition = CurrentState.GuardPosition.GetDirection(CurrentState.GuardFacing);

            if (!CurrentState.Map.TryGetValue(nextPosition, out var atNextPosition) || atNextPosition != '#')
            {
                return (nextPosition, CurrentState.GuardFacing);
            }

            // Turn right if blocked
            var newDirection = CurrentState.GuardFacing.TurnRight90();
            return (CurrentState.GuardPosition, newDirection);
        }

        public DrawableCoordinate[] GetCoordinates()
        {
            var toDraw =
                CurrentState.Map.ToDictionary(x => x.Key,
                                              x => CurrentState.GuardVisited.Contains(x.Key) ? 'X' : x.Value);

            toDraw[CurrentState.GuardPosition] = CurrentState.GuardFacing.ToChar();

            return toDraw.ToDrawableCoordinates(x => x == CurrentState.GuardPosition ? "blue" : null);
        }
    }
}
