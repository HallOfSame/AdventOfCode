using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Helpers.Extensions;
using Helpers.FileReaders;
using Helpers.Maps;
using Helpers.Structure;
using Priority_Queue;

namespace PuzzleDays
{
    public class Day17 : ProblemBase
    {
        protected override async Task<string> SolvePartOneInternal()
        {
            var result = FindHeatLoss(1, 3);

            if (result != 953)
            {
                throw new Exception("Wrong");
            }

            return result.ToString();
        }

        protected override async Task<string> SolvePartTwoInternal()
        {
            var result = FindHeatLoss(4, 10);

            if (result != 1180)
            {
                throw new Exception("Wrong");
            }

            return result.ToString();
        }

        private int FindHeatLoss(int minBeforeTurn, int maxBeforeTurn)
        {
            var toCheck = new SimplePriorityQueue<(Coordinate, Direction), int>();

            var yMax = coordinates.Max(x => x.Coordinate.Y);
            var xMax = coordinates.Max(x => x.Coordinate.X);

            var coordinateMap = coordinates.ToDictionary(x => x.Coordinate, x => x.Value);
            var costDictionary = new Dictionary<(Coordinate, Direction), int>();
            // var prevDictionary = new Dictionary<(Coordinate, Direction), (Coordinate, Direction)?>();

            foreach (var dir in new[] { Direction.North, Direction.East, Direction.South, Direction.West })
            {
                foreach (var coordinate in coordinateMap.Keys)
                {
                    costDictionary[(coordinate, dir)] = int.MaxValue;
                    // prevDictionary[(coordinate, dir)] = null;
                }
            }

            var visited = new HashSet<(Coordinate, Direction)>();

            var startPoint = new Coordinate(0, yMax);

            foreach (var direction in new[] { Direction.East, Direction.South })
            {
                var runningCost = 0;

                for (var i = 1; i <= maxBeforeTurn; i++)
                {
                    var coordinate = startPoint.GetDirection(direction, i);

                    if (!coordinateMap.TryGetValue(coordinate, out var neighborCost))
                    {
                        continue;
                    }

                    runningCost += neighborCost;

                    if (i < minBeforeTurn)
                    {
                        continue;
                    }

                    costDictionary[(coordinate, direction)] = runningCost;
                    // prevDictionary[(coordinate, direction)] = (startPoint, direction);

                    toCheck.Enqueue((coordinate, direction), runningCost);
                }
            }

            var endDirection = Direction.North;

            while (toCheck.Count > 0)
            {
                var (currentCoordinate, currentDirection) = toCheck.Dequeue();

                visited.Add((currentCoordinate, currentDirection));

                if (currentCoordinate.Y == 0 && currentCoordinate.X == xMax)
                {
                    endDirection = currentDirection;
                    break;
                }

                var newDirections = currentDirection switch
                {
                    Direction.North => (Direction.East, Direction.West),
                    Direction.East => (Direction.North, Direction.South),
                    Direction.South => (Direction.East, Direction.West),
                    Direction.West => (Direction.North, Direction.South),
                    _ => throw new Exception()
                };

                foreach (var direction in new[] { newDirections.Item1, newDirections.Item2 })
                {
                    var neighborCost = 0;

                    for (var i = 1; i <= maxBeforeTurn; i++)
                    {
                        var neighbor = currentCoordinate.GetDirection(direction, i);

                        if (!coordinateMap.TryGetValue(neighbor, out var addedCost))
                        {
                            // If we broke the bounds, we won't find anything else in this direction
                            break;
                        }

                        neighborCost += addedCost;

                        if (i < minBeforeTurn)
                        {
                            continue;
                        }

                        if (visited.Contains((neighbor, direction)))
                        {
                            continue;
                        }

                        var costSoFar = costDictionary[(currentCoordinate, currentDirection)];
                        var newDistance = neighborCost + costSoFar;

                        if (newDistance < costDictionary[(neighbor, direction)])
                        {
                            toCheck.Enqueue((neighbor, direction), newDistance);
                            costDictionary[(neighbor, direction)] = newDistance;
                            // prevDictionary[(neighbor, direction)] = (currentCoordinate, currentDirection);
                        }
                    }
                }
            }

            //var path = new Dictionary<Coordinate, Direction>();

            var end = new Coordinate(xMax, 0);

            var result = costDictionary[(end, endDirection)];

            //while (end != startPoint)
            //{
            //    path.Add(end, endDirection);
            //    var z = prevDictionary[(end, endDirection)];

            //    var next = z.Value.Item1;
            //    var nextDir = z.Value.Item2;

            //    if (CoordinateHelper.ManhattanDistance(next, end) != 1)
            //    {
            //        if (next.X == end.X)
            //        {
            //            var yDiff = Math.Abs(next.Y - end.Y);

            //            var higherY = next.Y > end.Y ? next.Y : end.Y;

            //            for (var sub = yDiff - 1; sub > 0; sub--)
            //            {
            //                path.Add(new Coordinate(next.X, higherY - sub), endDirection);
            //            }
            //        }

            //        if (next.Y == end.Y)
            //        {
            //            var xDiff = Math.Abs(next.X - end.X);

            //            var higherX = next.X > end.X ? next.X : end.X;

            //            for (var sub = xDiff - 1; sub > 0; sub--)
            //            {
            //                path.Add(new Coordinate(higherX - sub, end.Y), endDirection);
            //            }
            //        }
            //    }

            //    end = next;
            //    endDirection = nextDir;
            //}

            //coordinateMap.Keys.Draw(x =>
            //{
            //    if (path.TryGetValue(x, out var dir))
            //    {
            //        return dir switch
            //        {
            //            Direction.East => ">",
            //            Direction.North => "^",
            //            Direction.South => "v",
            //            Direction.West => "<",
            //            _ => throw new Exception()
            //        };
            //    }

            //    return coordinateMap[x].ToString();
            //});

            return result;
        }

        private List<CoordinateWithValue<int>> coordinates;

        public override async Task ReadInput()
        {
            coordinates = (await new GridFileReader().ReadInputFromFile())
                .Select(x => new CoordinateWithValue<int>(x.Coordinate)
                {
                    Value = int.Parse(x.Value.ToString()),
                })
                .ToList();
        }
    }
}
