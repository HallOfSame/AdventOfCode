using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Helpers;
using Helpers.Extensions;
using Helpers.Maps._3D;
using Helpers.Structure;

namespace PuzzleDays
{
    public class Day24 : ProblemBase
    {
        protected override async Task<string> SolvePartOneInternal()
        {
            var linesFromHail = hail.Select(h =>
                {
                    var m = (decimal)h.Velocity.Y / h.Velocity.X;

                    var mx1Neg = -m * h.StartPosition.X;

                    var rightSide = mx1Neg + h.StartPosition.Y;

                    return (line: new _2DLine
                    {
                        A = m * -1,
                        B = 1,
                        C = rightSide * -1,
                    }, hail: h);
                })
                .ToList();

            var pairs = linesFromHail.Combinations(2);

            var intersects = pairs.Select(x =>
                {

                    return (intersect: Calculate2dIntersect(x.First()
                                                                .line,
                                                            x.Last()
                                                                .line),
                        hailOne: x.First()
                            .hail, hailTwo: x.Last()
                            .hail);
                })
                .Where(x => x.intersect is not null)
                .Select(x => (intersect: x.intersect.Value, x.hailOne, x.hailTwo))
                .ToList();

            var testAreaSize = (min: 200000000000000, max: 400000000000000);

            var validIntersects = intersects.Where(x => x.intersect.x >= testAreaSize.min
                                                        && x.intersect.x <= testAreaSize.max &&
                                                        x.intersect.y >= testAreaSize.min &&
                                                        x.intersect.y <= testAreaSize.max)
                .Where(x => !WasIntersectInPast(x.intersect, x.hailOne) && !WasIntersectInPast(x.intersect, x.hailTwo))
                .ToList();

            return validIntersects.Count.ToString();
        }

        private bool WasIntersectInPast((decimal x, decimal y) intersect, HailStone h)
        {
            if (h.Velocity.X > 0 && h.StartPosition.X > intersect.x)
            {
                return true;
            }

            if (h.Velocity.X < 0 && h.StartPosition.X < intersect.x)
            {
                return true;
            }

            if (h.Velocity.Y > 0 && h.StartPosition.Y > intersect.y)
            {
                return true;
            }

            if (h.Velocity.Y < 0 && h.StartPosition.Y < intersect.y)
            {
                return true;
            }

            return false;
        }

        private (decimal x, decimal y)? Calculate2dIntersect(_2DLine lineOne, _2DLine lineTwo)
        {
            if (lineOne.A == lineTwo.A && lineOne.B == lineTwo.B)
            {
                // Parallel
                return null;
            }

            var tl = lineOne.B * lineTwo.C;
            var tr = lineTwo.B * lineOne.C;
            var bl = lineOne.A * lineTwo.B;
            var br = lineTwo.A * lineOne.B;

            var x = (decimal)(tl - tr) / (bl - br);

            tl = lineOne.C * lineTwo.A;
            tr = lineTwo.C * lineOne.A;
            bl = lineOne.A * lineTwo.B;
            br = lineTwo.A * lineOne.B;

            var y = (decimal)(tl - tr) / (bl - br);

            return (x, y);
        }

        protected override async Task<string> SolvePartTwoInternal()
        {
            throw new NotImplementedException();
        }

        private List<HailStone> hail;

        public override async Task ReadInput()
        {
            hail = await new HailFileReader().ReadInputFromFile();
        }

        private class HailFileReader : FileReader<HailStone>
        {
            protected override HailStone ProcessLineOfFile(string line)
            {
                var split = line.Split(" @ ");

                var posCoords = split[0]
                    .Split(", ");

                var velCoords = split[1]
                    .Split(", ");

                return new HailStone
                {
                    StartPosition = new Coordinate3d(decimal.Parse(posCoords[0]),
                                                     decimal.Parse(posCoords[1]),
                                                     decimal.Parse(posCoords[2])),
                    Velocity = new Coordinate3d(int.Parse(velCoords[0]),
                                                int.Parse(velCoords[1]),
                                                int.Parse(velCoords[2])),
                };
            }
        }

        public class _2DLine
        {
            public decimal A { get; set; }

            public decimal B { get; set; }

            public decimal C { get; set; }
        }

        [DebuggerDisplay("{StartPosition} @ {Velocity}")]
        class HailStone
        {
            public Coordinate3d StartPosition { get; set; }

            public Coordinate3d Velocity { get; set; }
        }
    }
}
