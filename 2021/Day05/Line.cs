using Helpers;
using System.Text;

namespace Day05
{
    internal class Field
    {
        public int Width { get; }

        public int Height { get; }

        public int[,] FloorData { get; }

        public Field(int width, int height)
        {
            Width = width;
            Height = height;
            FloorData = new int[Height, Width];
        }

        public int[] GetRow(int rowNumber)
        {
            return Enumerable.Range(0, Width)
                    .Select(x => FloorData[x, rowNumber])
                    .ToArray();
        }

        public override string ToString()
        {
            var stringBuilder = new StringBuilder();

            void AppendBorder()
            {
                stringBuilder.AppendLine(new string(Enumerable.Repeat('-', Width).ToArray()));
            }

            AppendBorder();

            for(var i = 0; i < Height; i++)
            {
                var row = GetRow(i);
                stringBuilder.AppendLine(string.Join(string.Empty, row));
            }

            AppendBorder();

            return stringBuilder.ToString();
        }
    }

    internal class Coordinate
    {
        public int X { get; set; }

        public int Y { get; set; }
    }

    internal class Line
    {
        public Coordinate Start { get; set; }

        public Coordinate End { get; set; }
    }

    internal class LineReader : FileReader<Line>
    {
        protected override Line ProcessLineOfFile(string line)
        {
            var lineSplits = line.Split(" -> ").Select(s => s.Split(',')).ToArray();

            var start = new Coordinate
            {
                X = int.Parse(lineSplits[0][0]),
                Y = int.Parse(lineSplits[0][1])
            };

            var end = new Coordinate
            {
                X = int.Parse(lineSplits[1][0]),
                Y = int.Parse(lineSplits[1][1])
            };

            if (start.X > end.X || start.Y > end.Y)
            {
                // Coordinates in the data are not in order
                return new Line
                {
                    Start = end,
                    End = start
                };
            }

            return new Line
            {
                Start = start,
                End = end
            };
        }
    }
}
