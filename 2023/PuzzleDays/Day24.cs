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

            var validIntersectsForPart1 = intersects.Where(x => x.intersect.x >= testAreaSize.min
                                                                && x.intersect.x <= testAreaSize.max &&
                                                                x.intersect.y >= testAreaSize.min &&
                                                                x.intersect.y <= testAreaSize.max)
                .Where(x => !WasIntersectInPast(x.intersect, x.hailOne) && !WasIntersectInPast(x.intersect, x.hailTwo))
                .ToList();

            return validIntersectsForPart1.Count.ToString();
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
            var maxVel = 250;
            var minVel = -1 * maxVel;

            // Probably don't need to check the full input to find a solution
            hail = hail.OrderBy(x => x.StartPosition.X)
                .ThenBy(x => x.StartPosition.Y)
                .Take(20)
                .ToList();

            // Checking just x and y at first
            for (var x = minVel; x <= maxVel; x++)
            {
                for (var y = minVel; y <= maxVel; y++)
                {
                    // Pick a velocity for the rock
                    var rockVelocity = new Coordinate3d(x, y, 0);

                    // Update all the hail stones to be moving relative to the rock
                    var updatedHailStones = hail.Select(x => new HailStone
                        {
                            StartPosition = x.StartPosition,
                            Velocity = new Coordinate3d(x.Velocity.X - rockVelocity.X,
                                                        x.Velocity.Y - rockVelocity.Y,
                                                        x.Velocity.Z)
                        })
                        .ToList();
                    
                    // And then check where those new updated lines would intersect
                    var linesFromHail = updatedHailStones.Where(x => x.Velocity.X != 0)
                        .Select(h =>
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

                    var intersects = new List<((decimal x, decimal y) intersect, HailStone hailOne, HailStone hailTwo)>();

                    foreach (var pair in pairs)
                    {
                        var h1 = pair.First();
                        var h2 = pair.Last();

                        var intersect = Calculate2dIntersect(h1
                                                                 .line,
                                                             h2
                                                                 .line);

                        if (intersect is null)
                        {
                            continue;
                        }

                        if (WasIntersectInPast(intersect.Value, h1.hail))
                        {
                            continue;
                        }

                        if (WasIntersectInPast(intersect.Value, h2.hail))
                        {
                            continue;
                        }

                        intersects.Add((intersect.Value, h1.hail, h2.hail));
                    }

                    // If they all intersect at the same point, then this solution in 2d is correct
                    if (intersects.Select(x => x.intersect)
                            .Distinct(new ApproximateComparer())
                            .Count() != 1)
                    {
                        continue;
                    }

                    // Now move to checking the Z works out
                    for (var z = minVel; z < maxVel; z++)
                    {
                        var zAtIntersect = intersects.SelectMany(x => new[]
                        {
                            GetZAtIntersect(x.intersect, x.hailOne, rockVelocity.Z + z),
                            GetZAtIntersect(x.intersect, x.hailTwo, rockVelocity.Z + z)
                        });

                        // If they all also intersect at the same z then we found a solution
                        if (zAtIntersect
                                .Distinct(new ApproximateComparer2())
                                .Count() == 1)
                        {
                            // And the coordinates for the solution are just the intersect point
                            var ansZ = zAtIntersect.First();

                            var xy = intersects.First()
                                .intersect;

                            return Math.Round(ansZ + xy.x + xy.y).ToString();
                        }
                    }
                }
            }

            throw new Exception("Increase bounds");
        }

        private decimal GetZAtIntersect((decimal x, decimal y) intersect, HailStone h, decimal zOffset)
        {
            var xDiff = intersect.x - h.StartPosition.X;

            var time = xDiff / h.Velocity.X;

            return h.StartPosition.Z + (time * (h.Velocity.Z - zOffset));
        }

        private List<HailStone> hail;

        public override async Task ReadInput()
        {
            hail = await new HailFileReader().ReadInputFromFile();
        }

        private class ApproximateComparer2 : IEqualityComparer<decimal>
        {
            private const decimal delta = 0.0001m;

            public bool Equals(decimal x, decimal y)
            {
                if (Math.Abs(x - y) < delta)
                {
                    return true;
                }

                return x == y;
            }

            public int GetHashCode(decimal obj)
            {
                return 6;
            }
        }

        private class ApproximateComparer : IEqualityComparer<(decimal, decimal)>
        {
            private const decimal delta = 0.0001m;

            public bool Equals((decimal, decimal) x, (decimal, decimal) y)
            {
                if (Math.Abs(x.Item1 - y.Item1) < delta && Math.Abs(x.Item2 - y.Item2) < delta)
                {
                    return true;
                }

                return x.Item1 == y.Item1 && x.Item2 == y.Item2;
            }

            public int GetHashCode((decimal, decimal) obj)
            {
                return 6;
            }
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
