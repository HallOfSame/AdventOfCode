using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Helpers;
using Helpers.Maps._3D;
using Helpers.Structure;

namespace PuzzleDays
{
    public class Day22 : ProblemBase
    {
        protected override async Task<string> SolvePartOneInternal()
        {
            var result = LetBricksFall();

            return result.ToString();
        }

        private int LetBricksFall()
        {
            var brickSupporterMap = new Dictionary<Brick, List<Brick>>();

            // For this to work, start with the lowest bricks first
            bricks = bricks.OrderBy(x => x.Start.Z)
                .ToList();

            foreach (var brick in bricks)
            {
                bool BrickIsOnGround()
                {
                    return brick.Start.Z == 1 || brick.End.Z == 1;
                }

                var bricksBelowThisOne = bricks
                    .Any(x => brick.IsOnTopOf(x));

                while (!bricksBelowThisOne && !BrickIsOnGround())
                {
                    // Console.WriteLine($"Move {brick} down one");
                    brick.MoveDownOne();

                    bricksBelowThisOne = bricks
                        .Any(x => brick.IsOnTopOf(x));
                }
            }

            foreach (var brick in bricks)
            {
                // Create a dictionary where value = bricks that the key is on top of
                brickSupporterMap[brick] = bricks
                    .Where(x => brick.IsOnTopOf(x))
                    .ToList();
            }

            var bricksWeCanDisintegrate = bricks.Where(b =>
                {
                    // If there are any bricks where this is the only brick on top of it we cannot delete it
                    return !brickSupporterMap.Any(x => x.Value.Count == 1 && x.Value[0] == b);
                })
                .ToList();

            return bricksWeCanDisintegrate.Count;
        }

        protected override async Task<string> SolvePartTwoInternal()
        {
            throw new NotImplementedException();
        }

        public override async Task ReadInput()
        {
            bricks = await new BrickFileReader().ReadInputFromFile();
        }

        private List<Brick> bricks;

        class Brick
        {
            public string Name { get;}

            public Coordinate3d Start { get; set; }

            public Coordinate3d End { get; set; }

            public Brick(string name, Coordinate3d start, Coordinate3d end)
            {
                Name = name;
                Start = start;
                End = end;
                XRange = (Math.Max(Start.X, End.X), Math.Min(Start.X, End.X));
                YRange = (Math.Max(Start.Y, End.Y), Math.Min(Start.Y, End.Y));
                Top = Math.Max(Start.Z, End.Z);
                Bottom = Math.Min(Start.Z, End.Z);
            }

            public (int max, int min) XRange { get; }

            public (int max, int min) YRange { get; }

            public int Top { get; set; }

            public int Bottom { get; set; }

            public void MoveDownOne()
            {
                Start -= new Coordinate3d(0, 0, 1);
                End -=  new Coordinate3d(0, 0, 1);
                Top -= 1;
                Bottom -= 1;
            }

            public bool IsOnTopOf(Brick brickToCheck)
            {
                // If the top of the other brick isn't 1 below ours, it can't be sitting on top of it
                if (brickToCheck.Top != Bottom - 1)
                {
                    return false;
                }

                // Check if X & Y intersect
                var xOverlap = XRange.min <= brickToCheck.XRange.max && brickToCheck.XRange.min <= XRange.max;
                var yOverlap = YRange.min <= brickToCheck.YRange.max && brickToCheck.YRange.min <= YRange.max;

                return xOverlap && yOverlap;
            }

            public override string ToString()
            {
                return $"{Name}({Start} - {End})";
            }
        }

        class BrickFileReader : FileReader<Brick>
        {
            private int currName;

            protected override Brick ProcessLineOfFile(string line)
            {
                var split = line.Split('~');

                var c1 = split[0]
                    .Split(',');

                var c2 = split[1]
                    .Split(',');

                var name = currName.ToString();

                currName++;

                return new Brick(name,
                                 new Coordinate3d(int.Parse(c1[0]), int.Parse(c1[1]), int.Parse(c1[2])),
                                 new Coordinate3d(int.Parse(c2[0]), int.Parse(c2[1]), int.Parse(c2[2])));
            }
        }
    }
}
