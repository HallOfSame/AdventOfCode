using System.Text.RegularExpressions;
using Helpers.Drawing;
using Helpers.Interfaces;
using Helpers.Maps;
using Helpers.Structure;

namespace PuzzleDays
{
    public class Day14 : StepExecutionPuzzle<Day14.ExecState>, IVisualize2d
    {
        public record ExecState(List<Robot> Robots, int Height, int Width, int SecondsPassed);
        
        public record Robot(Coordinate Position, Coordinate Velocity);

        public override PuzzleInfo Info => new(2024, 14, "Restroom Redoubt");
        public override bool ResetOnNewPart => false;
        protected override async Task<ExecState> LoadInitialState(string puzzleInput)
        {
            var robotRegex = new Regex(@"p=(.*),(.*) v=(.*),(.*)");

            var lines = puzzleInput.Trim()
                .Split('\n');
            var robots = new List<Robot>();
            foreach (var line in lines)
            {
                var match = robotRegex.Match(line);
                var p1 = int.Parse(match.Groups[1].Value);
                var p2 = int.Parse(match.Groups[2].Value);
                var v1 = int.Parse(match.Groups[3].Value);
                var v2 = int.Parse(match.Groups[4].Value);
                robots.Add(new Robot(new Coordinate(p1, p2), new Coordinate(v1, v2)));
            }

            var height = 103;
            var width = 101;

            if (robots.Count <= 12)
            {
                // This is an example
                height = 7;
                width = 11;
            }

            // Update the coordinates to the expects origin
            foreach (var robot in robots)
            {
                // Y is number of tiles from the top so we need to adjust
                robot.Position.Y = height - robot.Position.Y - 1;
            }

            return new ExecState(robots, height, width, 0);
        }

        protected override async Task<(bool isComplete, string? result)> ExecutePuzzleStepPartOne()
        {
            var updatedRobots = new List<Robot>(CurrentState.Robots.Count);
            foreach (var robot in CurrentState.Robots)
            {
                updatedRobots.Add(Move(robot, CurrentState.Height, CurrentState.Width));
            }

            var newSeconds = CurrentState.SecondsPassed + 1;

            CurrentState = CurrentState with { Robots = updatedRobots, SecondsPassed = newSeconds };

            var halfWidth = CurrentState.Width / 2;
            var halfHeight = CurrentState.Height / 2;

            var q1 = GetRobotsInBounds(updatedRobots, -1, halfWidth, -1, halfHeight);
            var q2 = GetRobotsInBounds(updatedRobots, halfWidth, CurrentState.Width, -1, halfHeight);
            var q3 = GetRobotsInBounds(updatedRobots, -1, halfWidth, halfHeight, CurrentState.Height);
            var q4 = GetRobotsInBounds(updatedRobots, halfWidth, CurrentState.Width, halfHeight, CurrentState.Height);

            return (newSeconds == 100, (q1 * q2 * q3 * q4).ToString());
        }

        protected override async Task<(bool isComplete, string? result)> ExecutePuzzleStepPartTwo()
        {
            throw new NotImplementedException();
        }

        private static Robot Move(Robot robot, int height, int width)
        {
            var currentPosition = robot.Position;

            // Positive X means moving right
            // Positive Y means moving down
            var updatedPosition =
                new Coordinate(currentPosition.X + robot.Velocity.X, currentPosition.Y - robot.Velocity.Y);

            if (updatedPosition.X >= width)
            {
                updatedPosition.X -= width;
            }
            else if (updatedPosition.X < 0)
            {
                updatedPosition.X += width;
            }

            if (updatedPosition.Y >= height)
            {
                updatedPosition.Y -= height;
            }
            else if (updatedPosition.Y < 0)
            {
                updatedPosition.Y += height;
            }

            return robot with { Position = updatedPosition };
        }

        private static int GetRobotsInBounds(List<Robot> robots, int minX, int maxX, int minY, int maxY)
        {
            return robots.Count(x => x.Position.X > minX && x.Position.X < maxX && x.Position.Y > minY &&
                                     x.Position.Y < maxY);
        }

        public DrawableCoordinate[] GetCoordinates()
        {
            var robotCounts = CurrentState.Robots.GroupBy(x => x.Position)
                .ToDictionary(x => x.Key, x => x.Count().ToString());
            var coordinateList = new List<DrawableCoordinate>(CurrentState.Height * CurrentState.Width);

            for (var x = 0; x < CurrentState.Width; x++)
            {
                for (var y = 0; y < CurrentState.Height; y++)
                {
                    coordinateList.Add(new DrawableCoordinate
                    {
                        X = x,
                        Y = y,
                        Text = robotCounts.GetValueOrDefault(new Coordinate(x, y), ".")
                    });
                }
            }

            return coordinateList.ToArray();
        }
    }
}
