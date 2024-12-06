using Helpers.Drawing;
using Helpers.FileReaders;
using Helpers.Interfaces;
using Helpers.Maps;
using Helpers.Structure;

namespace PuzzleDays
{
    public class Day06 : StepExecutionPuzzle<Day06.ExecState>, IVisualize2d
    {
        public record ExecState(Coordinate GuardPosition, Direction GuardFacing, Dictionary<Coordinate, char> Map, HashSet<Coordinate> PartOneVisited, HashSet<Coordinate> Part2BlockPositions, Coordinate OriginalGuardPos);

        public override PuzzleInfo Info => new(2024, 6, "Guard Gallivant");
        public override bool ResetOnNewPart => false;

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
            return new ExecState(guardPosition.Coordinate, guardFacing, map, guardVisited, [], guardPosition.Coordinate);
        }

        protected override async Task<(bool isComplete, string? result)> ExecutePuzzleStepPartOne()
        {
            var (position, direction) = GetGuardNextLocation(CurrentState.GuardPosition, CurrentState.GuardFacing);

            var isComplete = !CurrentState.Map.ContainsKey(position);
            var totalVisited = CurrentState.PartOneVisited.Count;

            if (isComplete)
            {
                // Skip adding the out-of-bounds coordinate to the map
                return (true, totalVisited.ToString());
            }

            CurrentState.PartOneVisited.Add(position);
            CurrentState = CurrentState with { GuardPosition = position, GuardFacing = direction };
            return (false, totalVisited.ToString());
        }

        protected override async Task<(bool isComplete, string? result)> ExecutePuzzleStepPartTwo()
        {
            if (CurrentState.PartOneVisited.Count <= 1)
            {
                throw new InvalidOperationException("Run part one first");
            }

            foreach (var possibleAddedBlock in CurrentState.PartOneVisited)
            {
                if (possibleAddedBlock == CurrentState.OriginalGuardPos)
                {
                    // Per the problem, this isn't valid
                    continue;
                }

                if (ResultsInLoop(CurrentState.OriginalGuardPos, Direction.North, possibleAddedBlock, []))
                {
                    CurrentState.Part2BlockPositions.Add(possibleAddedBlock);
                }
            }

            return (true, CurrentState.Part2BlockPositions.Count.ToString());
        }

        private bool ResultsInLoop(Coordinate currentPosition, Direction currentDirection, Coordinate addedBlock, HashSet<(Coordinate, Direction)> visitedCoordinates)
        {
            while (true)
            {
                var nextStep = GetGuardNextLocation(currentPosition, currentDirection, addedBlock);

                if (!CurrentState.Map.ContainsKey(nextStep.position))
                {
                    // If we leave the map, it can't be a loop
                    return false;
                }

                if (!visitedCoordinates.Add(nextStep))
                {
                    // We have been here with the current direction
                    return true;
                }

                currentPosition = nextStep.position;
                currentDirection = nextStep.direction;
            }
        }

        private (Coordinate position, Direction direction) GetGuardNextLocation(Coordinate currentPosition, Direction currentDirection, Coordinate? treatAsBlock = null)
        {
            var nextPosition = currentPosition.GetDirection(currentDirection);

            if (!CurrentState.Map.TryGetValue(nextPosition, out var nextPositionChar))
            {
                // If we are out of the map, keep going
                return (nextPosition, currentDirection);
            }

            var isBlocked = nextPositionChar == '#' || (treatAsBlock is not null && nextPosition == treatAsBlock);

            if (!isBlocked)
            {
                // Valid to keep moving this way
                return (nextPosition, currentDirection);
            }

            // We need to turn right, but we don't move forward yet
            var newDirection = currentDirection.TurnRight90();
            return (currentPosition, newDirection);
        }

        public DrawableCoordinate[] GetCoordinates()
        {
            var toDraw =
                CurrentState.Map.ToDictionary(x => x.Key,
                                              x =>
                                              {
                                                  if (CurrentState.Part2BlockPositions.Contains(x.Key))
                                                  {
                                                      return 'O';
                                                  }

                                                  return CurrentState.PartOneVisited.Contains(x.Key) ? 'X' : x.Value;
                                              });

            toDraw[CurrentState.GuardPosition] = CurrentState.GuardFacing.ToChar();

            return toDraw.ToDrawableCoordinates(x => x == CurrentState.GuardPosition ? "blue" : null,
                                                x => CurrentState.Part2BlockPositions.Contains(x) ? "green" : null);
        }
    }
}
