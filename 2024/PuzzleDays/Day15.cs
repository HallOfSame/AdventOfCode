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
    public override bool ResetOnNewPart => false;
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
        
        return (CurrentState.CurrentMoveIndex == CurrentState.Moves.Count, GetGpsLevelOfBoxes().ToString(CultureInfo.InvariantCulture));
    }

    protected override async Task<(bool isComplete, string? result)> ExecutePuzzleStepPartTwo()
    {
        throw new NotImplementedException();
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

    private decimal GetGpsLevelOfBoxes()
    {
        var height = CurrentState.Map.Max(x => x.Key.Y);
        
        var boxes = CurrentState.Map.Where(x => x.Value == 'O').Select(x => x.Key);

        return boxes.Select(x => ((height - x.Y) * 100) + x.X)
            .Sum();
    }

    public DrawableCoordinate[] GetCoordinates()
    {
        return CurrentState.Map.ToDrawableCoordinates();
    }
}