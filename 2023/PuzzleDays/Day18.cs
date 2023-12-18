using Helpers;
using Helpers.Extensions;
using Helpers.Maps;
using Helpers.Structure;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuzzleDays
{
    public class Day18 : ProblemBase
    {
        protected override async Task<string> SolvePartOneInternal()
        {
            var totalArea = CalculateArea(digInstructions);

            return totalArea.ToString();
        }

        private decimal CalculateArea(List<DigInstruction> instructions)
        {
            var edges = new List<(Coordinate, Coordinate)>();

            var currentLocation = new Coordinate(0, 0);

            foreach (var instruction in instructions)
            {
                var edgeStart = (Coordinate)currentLocation.Clone();

                var edgeEnd = currentLocation.GetDirection(instruction.DigDirection,
                                                           instruction.DigSteps);

                edges.Add((edgeStart, edgeEnd));

                currentLocation = edgeEnd;
            }

            // Push everything up to the positive quadrant
            var minY = edges.Min(x => Math.Min(x.Item1.Y, x.Item2.Y));
            var minX = edges.Min(x => Math.Min(x.Item1.X,
                                               x.Item2.X));

            minY = minY > 0
                       ? 0
                       : Math.Abs(minY);
            minX = minX > 0
                       ? 0
                       : Math.Abs(minX);


            edges.ForEach(e =>
                          {
                              e.Item1.X += minX;
                              e.Item1.Y += minY;
                              e.Item2.X += minX;
                              e.Item2.Y += minY;
                          });

            // Shouldn't need a final edge, instructions get us back to the start
            var innerArea = 0m;

            foreach (var edge in edges)
            {
                var avgHeight = (edge.Item1.Y + edge.Item2.Y) / 2m;
                var width = edge.Item2.X - edge.Item1.X;

                var edgeArea = avgHeight * width;

                innerArea += edgeArea;
            }

            var edgeLengths = edges.Sum(x => CoordinateHelper.ManhattanDistance(x.Item1,
                                                                                x.Item2));

            return innerArea + (edgeLengths / 2m) + 1;
        }

        protected override async Task<string> SolvePartTwoInternal()
        {
            var updatedInstructions = digInstructions.Select(x =>
                                                             {
                                                                 var newDirection = x.HexColorCode[^1] switch
                                                                 {
                                                                     '0' => Direction.East,
                                                                     '1' => Direction.South,
                                                                     '2' => Direction.West,
                                                                     '3' => Direction.North,
                                                                     _ => throw new NotImplementedException()
                                                                 };

                                                                 var hexStepCount = x.HexColorCode[..^1];

                                                                 var stepCount = Convert.ToInt32(hexStepCount,
                                                                                                 16);

                                                                 return new DigInstruction
                                                                        {
                                                                            DigDirection = newDirection,
                                                                            DigSteps = stepCount,
                                                                            HexColorCode = x.HexColorCode
                                                                        };
                                                             })
                                                     .ToList();

            var result = CalculateArea(updatedInstructions);

            return result.ToString();
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

        [DebuggerDisplay("#{HexColorCode} = {DirectionToChar} {DigSteps}")]
        class DigInstruction
        {
            public string HexColorCode { get; set; }

            public Direction DigDirection { get; set; }

            public int DigSteps { get; set; }

            public string DirectionToChar
            {
                get
                {
                    return DigDirection switch
                    {
                        Direction.North => "U",
                        Direction.South => "D",
                        Direction.West => "L",
                        Direction.East => "R",
                        _ => throw new NotImplementedException(),
                    };
                }
            }
        }
    }
}
