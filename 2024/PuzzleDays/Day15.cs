using System.Globalization;
using Helpers.Drawing;
using Helpers.Extensions;
using Helpers.FileReaders;
using Helpers.Interfaces;
using Helpers.Maps;
using Helpers.Structure;

namespace PuzzleDays;

public class Day15 : StepExecutionPuzzle<Day15.ExecState>, IVisualize2d
{
    public record ExecState(Dictionary<Coordinate, char> Map, Coordinate Robot, List<char> Moves, int CurrentMoveIndex, Dictionary<Coordinate, char> PartTwoMap);

    public override PuzzleInfo Info => new(2024, 15, "Warehouse Woes");
    public override bool ResetOnNewPart => true;
    protected override async Task<ExecState> LoadInitialState(string puzzleInput)
    {
        var inputSplit = puzzleInput.Trim().Split('\n');
        
        var gridLines = new List<string>();
        var instructions = new List<char>();
        var readingGrid = true;
        foreach (var line in inputSplit)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                readingGrid = false;
                continue;
            }

            if (readingGrid)
            {
                gridLines.Add(line);
            }
            else
            {
                instructions.AddRange(line.ToCharArray());
            }
        }
        
        var gridInput = string.Join('\n', gridLines);
        var coordinates = await new GridFileReader().ReadFromString(gridInput);
        var map = coordinates.ToDictionary(c => c.Coordinate, c => c.Value);
        var robotLocation = map.Single(x => x.Value == '@')
            .Key;
        
        return new ExecState(map, robotLocation, instructions, 0, []);
    }

    protected override async Task<(bool isComplete, string? result)> ExecutePuzzleStepPartOne()
    {
        var nextInstruction = CurrentState.Moves[CurrentState.CurrentMoveIndex];
        ProcessMove(nextInstruction);
        var newRobotLocation = CurrentState.Map.Single(x => x.Value == '@')
            .Key;
        CurrentState = CurrentState with { Robot = newRobotLocation, CurrentMoveIndex = CurrentState.CurrentMoveIndex + 1 };
        
        return (CurrentState.CurrentMoveIndex == CurrentState.Moves.Count, GetGpsLevelOfBoxes(CurrentState.Map, 'O').ToString(CultureInfo.InvariantCulture));
    }

    protected override async Task<(bool isComplete, string? result)> ExecutePuzzleStepPartTwo()
    {
        if (CurrentState.PartTwoMap.Count == 0)
        {
            foreach (var (coordinate, value) in CurrentState.Map)
            {
                var x = coordinate.X * 2;
                var newValues = new char[2];

                switch (value)
                {
                    case '#':
                        newValues[0] = '#';
                        newValues[1] = '#';
                        break;
                    case 'O':
                        newValues[0] = '[';
                        newValues[1] = ']';
                        break;
                    case '.':
                        newValues[0] = '.';
                        newValues[1] = '.';
                        break;
                    case '@':
                        newValues[0] = '@';
                        newValues[1] = '.';
                        break;
                    default:
                        throw new InvalidOperationException("Unexpected char in map");
                }
                
                CurrentState.PartTwoMap[new Coordinate(x, coordinate.Y)] = newValues[0];
                CurrentState.PartTwoMap[new Coordinate(x + 1, coordinate.Y)] = newValues[1];
            }
        }
        
        var nextInstruction = CurrentState.Moves[CurrentState.CurrentMoveIndex];
        ProcessMovePart2(nextInstruction);
        var newRobotLocation = CurrentState.PartTwoMap.Single(x => x.Value == '@')
                                           .Key;
        CurrentState = CurrentState with { Robot = newRobotLocation, CurrentMoveIndex = CurrentState.CurrentMoveIndex + 1 };
        var done = CurrentState.CurrentMoveIndex == CurrentState.Moves.Count;
        var result = done ? GetGpsLevelOfBoxes(CurrentState.PartTwoMap, '[').ToString(CultureInfo.InvariantCulture) : nextInstruction.ToString();
        
        return (CurrentState.CurrentMoveIndex == CurrentState.Moves.Count, result);
    }

    private void ProcessMove(char move)
    {
        var robotLocation = CurrentState.Robot;
        var moveDirection = CoordinateHelper.ParseDirection(move);
        var inNextSpace = robotLocation.Move(moveDirection);
        
        if (CurrentState.Map[inNextSpace] == '#')
        {
            // Can't move into a wall
            return;
        }

        if (CurrentState.Map[inNextSpace] == '.')
        {
            CurrentState.Map[inNextSpace] = '@';
            CurrentState.Map[robotLocation] = '.';
            return;
        }

        if (CurrentState.Map[inNextSpace] == 'O')
        {
            var moveIsValid = true;
            var next = inNextSpace;
            var coordinatesToMove = new List<Coordinate> { robotLocation, next };
            do
            {
                var nextSpaceInDirection = next.Move(moveDirection);

                if (CurrentState.Map[nextSpaceInDirection] == '#')
                {
                    // Invalid to move into a wall
                    moveIsValid = false;
                    break;
                }

                if (CurrentState.Map[nextSpaceInDirection] == '.')
                {
                    // Empty space, everything is good to move
                    break;
                }

                if (CurrentState.Map[nextSpaceInDirection] == 'O')
                {
                    // Another box, add it to the list and keep checking
                    coordinatesToMove.Add(nextSpaceInDirection);
                    next = nextSpaceInDirection;
                }
            } while (true);

            if (moveIsValid)
            {
                // Process moves in from the end
                coordinatesToMove.Reverse();

                foreach (var coordinate in coordinatesToMove)
                {
                    CurrentState.Map[coordinate.GetDirection(moveDirection)] = CurrentState.Map[coordinate];
                    CurrentState.Map[coordinate] = '.';
                }
            }
        }
    }
    
    private void ProcessMovePart2(char move)
    {
        var robotLocation = CurrentState.PartTwoMap.Single(x => x.Value == '@').Key;
        var moveDirection = CoordinateHelper.ParseDirection(move);
        var nextSpace = robotLocation.Move(moveDirection);
        var atNextSpace = this.CurrentState.PartTwoMap[nextSpace];
        
        if (atNextSpace == '#')
        {
            // Can't move into a wall
            return;
        }

        if (atNextSpace == '.')
        {
            // Simple robot move
            CurrentState.PartTwoMap[nextSpace] = '@';
            CurrentState.PartTwoMap[robotLocation] = '.';
            return;
        }

        if (atNextSpace == '[' || atNextSpace == ']')
        {
            bool canMove;
            List<Coordinate> coordinatesToMove;

            if (moveDirection is Direction.North or Direction.South)
            {
                var boxCoordinates = new List<Coordinate>(2)
                {
                    nextSpace
                };

                if (atNextSpace == '[')
                {
                    boxCoordinates.Add(nextSpace.GetDirection(Direction.East));
                }
                else
                {
                    boxCoordinates.Add(nextSpace.GetDirection(Direction.West));
                }

                canMove = CanShiftBoxUpOrDown(moveDirection, boxCoordinates, out coordinatesToMove);
            }
            else
            {
                canMove = CanShiftBoxRightOrLeft(moveDirection, nextSpace, out coordinatesToMove);
            }

            if (canMove)
            {
                // Process moves in from the end
                coordinatesToMove.Reverse();

                foreach (var coordinate in coordinatesToMove)
                {
                    CurrentState.PartTwoMap[coordinate.GetDirection(moveDirection)] = CurrentState.PartTwoMap[coordinate];
                    CurrentState.PartTwoMap[coordinate] = '.';
                }

                // Move the robot too
                CurrentState.PartTwoMap[nextSpace] = '@';
                CurrentState.PartTwoMap[robotLocation] = '.';
            }            
        }
    }

    private bool CanShiftBoxRightOrLeft(Direction shiftDirection,
                                        Coordinate firstBoxCoordinate,
                                        out List<Coordinate> allCoordinatesToMove)
    {
        allCoordinatesToMove = [];

        while (true)
        {
            allCoordinatesToMove.Add(firstBoxCoordinate);
            var otherBoxCoordinate = firstBoxCoordinate.GetDirection(shiftDirection);
            allCoordinatesToMove.Add(otherBoxCoordinate);

            var shiftPosition = otherBoxCoordinate.GetDirection(shiftDirection);
            var atShiftPosition = this.CurrentState.PartTwoMap[shiftPosition];

            if (atShiftPosition == '#')
            {
                // Wall, we are blocked
                return false;
            }

            if (atShiftPosition == '.')
            {
                // Empty, we can move
                return true;
            }

            // Otherwise, it is another box to check
            firstBoxCoordinate = shiftPosition;
        }
    }

    private bool CanShiftBoxUpOrDown(Direction shiftDirection,
                                     List<Coordinate> coordinatesToShift,
                                     out List<Coordinate> allCoordinatesToMove)
    {
        allCoordinatesToMove = [];
        // Using a hashset to prevent us double-checking boxes if we start run into both sides of it 
        var hashToShift = coordinatesToShift.ToHashSet();

        while (true)
        {
            var coordinatesToCheckNext = new HashSet<Coordinate>();

            // Check the spot above or below each coordinate
            foreach (var coordinate in hashToShift)
            {
                allCoordinatesToMove.Add(coordinate);
                var shiftPosition = coordinate.GetDirection(shiftDirection);
                var atShiftPosition = this.CurrentState.PartTwoMap[shiftPosition];

                if (atShiftPosition == '#')
                {
                    // Can't move into a wall
                    return false;
                }

                if (atShiftPosition == '.')
                {
                    // This is fine, empty space
                    continue;
                }

                if (atShiftPosition == '[')
                {
                    // Add both pieces of the box to check next
                    coordinatesToCheckNext.Add(shiftPosition);
                    coordinatesToCheckNext.Add(shiftPosition.GetDirection(Direction.East));
                }

                if (atShiftPosition == ']')
                {
                    // Add both pieces of the box to check next
                    coordinatesToCheckNext.Add(shiftPosition);
                    coordinatesToCheckNext.Add(shiftPosition.GetDirection(Direction.West));
                }
            }

            // If there were any boxes in those spots, check we can move them as well
            if (coordinatesToCheckNext.Count > 0)
            {
                hashToShift = coordinatesToCheckNext;
                continue;
            }

            // Only hit if we only found empty space at the end
            return true;
        }
    }

    private decimal GetGpsLevelOfBoxes(Dictionary<Coordinate, char> map, char boxChar)
    {
        var height = map.Max(x => x.Key.Y);
        
        var boxes = map.Where(x => x.Value == boxChar).Select(x => x.Key);

        return boxes.Select(x => ((height - x.Y) * 100) + x.X)
            .Sum();
    }

    public DrawableCoordinate[] GetCoordinates()
    {
        return CurrentState.PartTwoMap.Count > 0 ? CurrentState.PartTwoMap.ToDrawableCoordinates() : CurrentState.Map.ToDrawableCoordinates();
    }
}