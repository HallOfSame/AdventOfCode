﻿using IntCodeInterpreter;
using IntCodeInterpreter.Input;
using System.Text;

var program = await new FileInputParser().ReadOperationsFromFile("PuzzleInput.txt");

// Part 2

var robot = new Robot();

robot.Run(program, PaintColors.Black);

// Part 2
robot = new Robot();
robot.Run(program, PaintColors.White);

class Robot
{
    private Interpreter brain;

    private HashSet<Coordinate> coordinateMap;

    private Direction currentDirection;

    private Coordinate currentCoordinate;

    private int GetColorForCoordinate(Coordinate coordinate)
    {
        if (!coordinateMap.TryGetValue(coordinate, out var existingCoordinate))
        {
            return PaintColors.Black;
        }

        return existingCoordinate.Color;
    }

    private void TurnLeft()
    {
        switch (currentDirection)
        {
            case Direction.Left:
                currentDirection = Direction.Down;
                return;
            case Direction.Right:
                currentDirection = Direction.Up;
                return;
            case Direction.Up:
                currentDirection = Direction.Left;
                return;
            case Direction.Down:
                currentDirection = Direction.Right;
                return;
        }
    }

    private void TurnRight()
    {
        switch (currentDirection)
        {
            case Direction.Left:
                currentDirection = Direction.Up;
                return;
            case Direction.Right:
                currentDirection = Direction.Down;
                return;
            case Direction.Up:
                currentDirection = Direction.Right;
                return;
            case Direction.Down:
                currentDirection = Direction.Left;
                return;
        }
    }

    private void ProcessMove()
    {
        var newX = currentCoordinate.X;
        var newY = currentCoordinate.Y;

        switch (currentDirection)
        {
            case Direction.Left:
                newX -= 1;
                break;
            case Direction.Right:
                newX += 1;
                break;
            case Direction.Up:
                newY += 1;
                break;
            case Direction.Down:
                newY -= 1;
                break;
        }

        // Temp to check the map
        var tempCoordinate = new Coordinate
        {
            X = newX,
            Y = newY,
            Color = PaintColors.Black
        };

        //Console.WriteLine($"Moving to ({newX},{newY}).");

        if (coordinateMap.TryGetValue(tempCoordinate, out var actualCoordinate))
        {
            currentCoordinate = actualCoordinate;
            return;
        }

        coordinateMap.Add(tempCoordinate);

        currentCoordinate = tempCoordinate;
    }

    HashSet<Coordinate> paintedCoordinates = new HashSet<Coordinate>();

    public void Run(List<long> program,
                    int startingColor)
    {
        brain = new Interpreter();

        coordinateMap = new HashSet<Coordinate>();

        currentDirection = Direction.Up;

        currentCoordinate = new Coordinate
        {
            X = 0,
            Y = 0,
            Color = startingColor
        };

        coordinateMap.Add(currentCoordinate);

        var outputIsColor = true;

        brain.ProcessOperations(program, () =>
        {
            // Get input, color of the current panel
            return GetColorForCoordinate(currentCoordinate);
        }, x =>
        {
            // Handle Output
            if (outputIsColor)
            {
                //Console.WriteLine($"Do paint");
                paintedCoordinates.Add(currentCoordinate);
                currentCoordinate.Color = (int)x;
            }
            else
            {
                if (x == 0)
                {
                    TurnLeft();
                }
                else
                {
                    TurnRight();
                }

                // Move forward one
                ProcessMove();
            }

            outputIsColor = !outputIsColor;
        });

        Console.WriteLine($"Number of coordinates in map: {coordinateMap.Count}. Number painted is: {paintedCoordinates.Count}.");

        // Figure out the area size
        var minX = coordinateMap.Min(x => x.X);
        var maxX = coordinateMap.Max(x => x.X);
        var minY = coordinateMap.Min(x => x.Y);
        var maxY = coordinateMap.Max(x => x.Y);

        var stringBuilder = new StringBuilder();
        stringBuilder.AppendLine();

        void DrawBorder()
        {
            stringBuilder.AppendLine(new string(Enumerable.Repeat('-', maxY - minY).ToArray()));
        }

        DrawBorder();

        for(var y = maxY; y >= minY; y--)
        {
            for(var x = minX; x <= maxX; x++)
            {
                var color = GetColorForCoordinate(new Coordinate
                {
                    X = x,
                    Y = y
                });

                stringBuilder.Append(color == PaintColors.White ? '#' : '.');
            }

            stringBuilder.Append(Environment.NewLine);
        }

        DrawBorder();
        stringBuilder.AppendLine();

        Console.WriteLine(stringBuilder.ToString());
    }
}

enum Direction
{
    Up,
    Down,
    Left,
    Right
}

static class PaintColors
{
    public const int Black = 0;

    public const int White = 1;
}

class Coordinate
{
    public int X { get; set; }

    public int Y { get; set; }

    public int Color { get; set; }

    public override bool Equals(object? obj)
    {
        return obj is Coordinate coordinate &&
               X == coordinate.X &&
               Y == coordinate.Y;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(X, Y);
    }
}