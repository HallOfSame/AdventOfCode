using System;
using System.Collections.Generic;
using System.Linq;
using Helpers.Drawing;

namespace Helpers.Maps;

public static class CoordinateHelper
{
    #region Class Methods

    public static decimal ManhattanDistance(Coordinate c1,
                                        Coordinate c2)
    {
        return Math.Abs(c1.X - c2.X) + Math.Abs(c1.Y - c2.Y);
    }

    public static Coordinate GetDirection(this Coordinate c, Direction d, int steps = 1)
    {
        var updatedCoordinate = new Coordinate(c.X, c.Y);

        switch (d)
        {
            case Direction.North:
                updatedCoordinate.Y += steps;
                break;
            case Direction.South:
                updatedCoordinate.Y -= steps;
                break;
            case Direction.East:
                updatedCoordinate.X += steps;
                break;
            case Direction.West:
                updatedCoordinate.X -= steps;
                break;
            default:
                throw new NotImplementedException();
        }

        return updatedCoordinate;
    }

    public static Direction TurnRight90(this Direction currentDirection)
    {
        return currentDirection switch
        {
            Direction.North => Direction.East,
            Direction.East => Direction.South,
            Direction.South => Direction.West,
            Direction.West => Direction.North,
            _ => throw new NotImplementedException("Haven't done diagonals")
        };
    }

    public static Direction TurnLeft90(this Direction currentDirection)
    {
        return currentDirection switch
        {
            Direction.North => Direction.West,
            Direction.East => Direction.North,
            Direction.South => Direction.East,
            Direction.West => Direction.South,
            _ => throw new NotImplementedException("Haven't done diagonals")
        };
    }

    public static char ToChar(this Direction direction)
    {
        return direction switch
        {
            Direction.North => '^',
            Direction.East => '>',
            Direction.South => 'v',
            Direction.West => '<',
            _ => throw new NotImplementedException("Haven't done diagonals")
        };
    }

    public static Direction ParseDirection(char direction)
    {
        return direction switch
        {
            '^' => Direction.North,
            '>' => Direction.East,
            'v' => Direction.South,
            '<' => Direction.West,
            _ => throw new NotImplementedException("Diagonal not supported")
        };
    }

    public static IEnumerable<Direction> InfiniteDirections(List<Direction> directionOrder)
    {
        var currentIndex = 0;

        while (true)
        {
            if (currentIndex >= directionOrder.Count)
            {
                currentIndex = 0;
            }

            yield return directionOrder[currentIndex];

            currentIndex++;
        }
    }

    public static DrawableCoordinate[] ToDrawableCoordinates(this Dictionary<Coordinate, char> map,
                                                             params Func<Coordinate, string?>[] colorSelectors)
    {
        return map.Select(x =>
                              new DrawableCoordinate
                              {
                                  X = x.Key.X,
                                  Y = x.Key.Y,
                                  Text = x.Value.ToString(),
                                  Color = colorSelectors.Select(sel => sel(x.Key))
                                      .FirstOrDefault(clr => clr != null)
                              })
            .ToArray();
    }

    #endregion
}