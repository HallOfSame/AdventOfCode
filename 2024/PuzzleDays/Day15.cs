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
    public record ExecState(Dictionary<Coordinate, char> Map, Coordinate Robot, List<char> Moves, int CurrentMoveIndex);

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
        
        return new ExecState(map, robotLocation, instructions, 0);
    }

    protected override async Task<(bool isComplete, string? result)> ExecutePuzzleStepPartOne()
    {
        var nextInstruction = CurrentState.Moves[CurrentState.CurrentMoveIndex];
        ProcessMove(nextInstruction);
        var newRobotLocation = CurrentState.Map.Single(x => x.Value == '@')
            .Key;
        CurrentState = CurrentState with { Robot = newRobotLocation, CurrentMoveIndex = CurrentState.CurrentMoveIndex + 1 };
        
        return (CurrentState.CurrentMoveIndex == CurrentState.Moves.Count, GetGpsLevelOfBoxes('O').ToString(CultureInfo.InvariantCulture));
    }

    private Dictionary<Coordinate, char>? partTwoMap;

    protected override async Task<(bool isComplete, string? result)> ExecutePuzzleStepPartTwo()
    {
        if (partTwoMap is null)
        {
            partTwoMap = new Dictionary<Coordinate, char>();
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
                
                partTwoMap[new Coordinate(x, coordinate.Y)] = newValues[0];
                partTwoMap[new Coordinate(x + 1, coordinate.Y)] = newValues[1];
            }
        }
        
        var nextInstruction = CurrentState.Moves[CurrentState.CurrentMoveIndex];
        ProcessMovePart2(nextInstruction);
        var newRobotLocation = partTwoMap.Single(x => x.Value == '@')
            .Key;
        CurrentState = CurrentState with { Robot = newRobotLocation, CurrentMoveIndex = CurrentState.CurrentMoveIndex + 1 };
        
        return (CurrentState.CurrentMoveIndex == CurrentState.Moves.Count, GetGpsLevelOfBoxes('[').ToString(CultureInfo.InvariantCulture));
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
        var robotLocation = CurrentState.Robot;
        var moveDirection = CoordinateHelper.ParseDirection(move);
        var inNextSpace = robotLocation.Move(moveDirection);
        
        if (partTwoMap![inNextSpace] == '#')
        {
            // Can't move into a wall
            return;
        }

        if (partTwoMap[inNextSpace] == '.')
        {
            partTwoMap[inNextSpace] = '@';
            partTwoMap[robotLocation] = '.';
            return;
        }

        if (partTwoMap[inNextSpace] == '[' || partTwoMap[inNextSpace] == ']')
        {
            var moveIsValid = true;
            var nexts = new List<Coordinate>
            {
                inNextSpace
            };
            if (partTwoMap[inNextSpace] == '[')
            {
                nexts.Add(inNextSpace.Move(Direction.East));
            }
            else
            {
                nexts.Add(inNextSpace.Move(Direction.West));
            }
            
            var coordinatesToMove = new List<Coordinate> { robotLocation, nexts[0], nexts[1] };
            do
            {
                var nextsCopy = nexts.ToList();
                nexts.Clear();
                foreach (var pieceThatNeedsToMove in nextsCopy)
                {
                    if (!moveIsValid)
                    {
                        break;
                    }
                    
                    var nextSpaceInDirection = pieceThatNeedsToMove.Move(moveDirection);

                    if (partTwoMap[nextSpaceInDirection] == '#')
                    {
                        // Invalid to move into a wall
                        moveIsValid = false;
                        break;
                    }

                    if (partTwoMap[nextSpaceInDirection] == '.')
                    {
                        // Empty space, this is good to move
                        break;
                    }

                    if (partTwoMap[nextSpaceInDirection] == '[' || partTwoMap[nextSpaceInDirection] == ']')
                    {
                        // Another box, need to check both sides of it
                        coordinatesToMove.Add(nextSpaceInDirection);
                        next = nextSpaceInDirection;
                    }
                }
            } while (true);

            if (moveIsValid)
            {
                // Process moves in from the end
                coordinatesToMove.Reverse();

                foreach (var coordinate in coordinatesToMove)
                {
                    partTwoMap[coordinate.GetDirection(moveDirection)] = CurrentState.Map[coordinate];
                    partTwoMap[coordinate] = '.';
                }
            }
        }
    }

    private decimal GetGpsLevelOfBoxes(char boxChar)
    {
        var height = CurrentState.Map.Max(x => x.Key.Y);
        
        var boxes = CurrentState.Map.Where(x => x.Value == boxChar).Select(x => x.Key);

        return boxes.Select(x => ((height - x.Y) * 100) + x.X)
            .Sum();
    }

    public DrawableCoordinate[] GetCoordinates()
    {
        return CurrentState.Map.ToDrawableCoordinates();
    }
}