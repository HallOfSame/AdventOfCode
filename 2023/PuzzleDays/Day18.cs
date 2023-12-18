using Helpers;
using Helpers.Extensions;
using Helpers.Maps;
using Helpers.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuzzleDays
{
    public class Day18 : ProblemBase
    {
        protected override async Task<string> SolvePartOneInternal()
        {
            var trenchLocations = new HashSet<Coordinate>();

            var currentLocation = new Coordinate(0, 0);

            trenchLocations.Add(currentLocation);

            foreach (var instruction in digInstructions)
            {
                for (var step = 0; step < instruction.DigSteps; step++)
                {
                    currentLocation = currentLocation.GetDirection(instruction.DigDirection);
                    trenchLocations.Add(currentLocation);
                }
            }

            var floodFillStart = new Coordinate(1,
                                                -1);

            trenchLocations.Concat(new [] { floodFillStart}).Draw(x => x.X == floodFillStart.X && x.Y == floodFillStart.Y ? "F" : "#", forceOrigin: true);

            var trenchSize = trenchLocations.Count;
            var fillSize = FloodFillCount(floodFillStart,
                                          trenchLocations);

            var result = trenchSize + fillSize;

            return result.ToString();
        }

        private int FloodFillCount(Coordinate currentLocation, HashSet<Coordinate> edgeCoordinates)
        {
            var queue = new Queue<Coordinate>();

            queue.Enqueue(currentLocation);

            var visited = new HashSet<Coordinate>();

            while (queue.Count > 0)
            {
                var n = queue.Dequeue();

                if (edgeCoordinates.Contains(n) || visited.Contains(n))
                {
                    continue;
                }

                visited.Add(n);

                n.GetNeighbors()
                 .ForEach(queue.Enqueue);
            }

            return visited.Count;
        }

        protected override async Task<string> SolvePartTwoInternal()
        {
            throw new NotImplementedException();
        }

        private List<DigInstruction> digInstructions;

        public override async Task ReadInput()
        {
            digInstructions = await new DigInstructionReader().ReadInputFromFile();
        }

        class DigInstructionReader : FileReader<DigInstruction>
        {
            protected override DigInstruction ProcessLineOfFile(string line)
            {
                var splits = line.Split(' ');

                var direction = splits[0] switch
                {
                    "U" => Direction.North,
                    "D" => Direction.South,
                    "L" => Direction.West,
                    "R" => Direction.East,
                    _ => throw new NotImplementedException(),
                };

                var steps = int.Parse(splits[1]);

                var colorCode = splits[2][2..^1];

                return new DigInstruction
                       {
                           DigDirection = direction,
                           DigSteps = steps,
                           HexColorCode = colorCode,
                       };
            }
        }

        class DigInstruction
        {
            public string HexColorCode { get; set; }

            public Direction DigDirection { get; set; }

            public int DigSteps { get; set; }
        }
    }
}
