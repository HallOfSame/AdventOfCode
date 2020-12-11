using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day11
{
    internal class Program
    {
        #region Fields

        private static readonly Dictionary<(int, int), List<Space>> neighborSpaces = new Dictionary<(int, int), List<Space>>();

        #endregion

        #region Class Methods

        public static void Main(string[] args)
        {
            var fileText = File.ReadAllText("PuzzleInput.txt");

            var lines = fileText.Split(Environment.NewLine);

            var rows = lines.Select(x => x.ToCharArray())
                            .ToList();

            var model = new Space[lines.Length][];

            for (var i = 0; i < rows.Count; i++)
            {
                var nextRow = rows[i];

                var modelRows = new List<Space>(nextRow.Length);

                for (var c = 0; c < nextRow.Length; c++)
                {
                    var spot = nextRow[c];
                    modelRows.Add(new Space(spot == '.',
                                            c,
                                            i));
                }

                model[i] = modelRows.ToArray();
            }

            // PT 1
            var occupiedSeats = new HashSet<Space>();

            while (true)
            {
                //Drawing slows it down by a lot
                //Console.WriteLine("");
                //Console.WriteLine("RUN");

                //foreach (var row in model)
                //{
                //    Console.WriteLine(string.Join("",
                //                                  row.Select(x => occupiedSeats.Contains(x) ? '#' : x.IsFloor ? '.' : 'L')));
                //}

                //Console.WriteLine("");
                //Console.WriteLine("");

                var (occupied, _) = RunModelOnce(model,
                                                 occupiedSeats,
                                                 4,
                                                 GetNeighboringNonFloorSpaces);

                if (occupied.SetEquals(occupiedSeats))
                {
                    break;
                }

                occupiedSeats = occupied;
            }

            Console.WriteLine($"PT 1 There are {occupiedSeats.Count} seats occupied.");

            // PT 2
            occupiedSeats = new HashSet<Space>();

            while (true)
            {
                var (occupied, _) = RunModelOnce(model,
                                                 occupiedSeats,
                                                 5,
                                                 GetNeighboringVisibleSeats);

                if (occupied.SetEquals(occupiedSeats))
                {
                    break;
                }

                occupiedSeats = occupied;
            }

            Console.WriteLine($"PT 2 There are {occupiedSeats.Count} seats occupied.");
        }

        private static List<Space> GetNeighboringNonFloorSpaces(Space[][] model,
                                                                int currentRow,
                                                                int currentColumn)
        {
            return new List<Space>
                   {
                       SafeGetByIndex(model,
                                      currentRow - 1,
                                      currentColumn - 1),
                       SafeGetByIndex(model,
                                      currentRow - 1,
                                      currentColumn),
                       SafeGetByIndex(model,
                                      currentRow - 1,
                                      currentColumn + 1),
                       SafeGetByIndex(model,
                                      currentRow,
                                      currentColumn - 1),

                       SafeGetByIndex(model,
                                      currentRow,
                                      currentColumn + 1),
                       SafeGetByIndex(model,
                                      currentRow + 1,
                                      currentColumn - 1),
                       SafeGetByIndex(model,
                                      currentRow + 1,
                                      currentColumn),
                       SafeGetByIndex(model,
                                      currentRow + 1,
                                      currentColumn + 1),
                   }.Where(x => x != null && !x.IsFloor)
                    .ToList();
        }

        private static List<Space> GetNeighboringVisibleSeats(Space[][] model,
                                                              int currentRow,
                                                              int currentColumn)
        {
            if (neighborSpaces.TryGetValue((currentRow, currentColumn),
                                           out var neighbors))
            {
                return neighbors;
            }

            // TODO calculate
            // 8 directions to check
            // Keep moving in one until we get null or a non floor space

            // UP
            var row = currentRow;
            var column = currentColumn;

            Space visibleSeatAbove;

            do
            {
                row += 1;

                visibleSeatAbove = SafeGetByIndex(model,
                                                  row,
                                                  column);
            }
            while (visibleSeatAbove != null
                   && visibleSeatAbove.IsFloor);

            // UP + RIGHT
            row = currentRow;
            column = currentColumn;

            Space visibleSeatUpRight;

            do
            {
                row += 1;
                column += 1;

                visibleSeatUpRight = SafeGetByIndex(model,
                                                    row,
                                                    column);
            }
            while (visibleSeatUpRight != null
                   && visibleSeatUpRight.IsFloor);

            // RIGHT
            row = currentRow;
            column = currentColumn;

            Space visibleSeatRight;

            do
            {
                column += 1;

                visibleSeatRight = SafeGetByIndex(model,
                                                  row,
                                                  column);
            }
            while (visibleSeatRight != null
                   && visibleSeatRight.IsFloor);

            // DOWN + RIGHT
            row = currentRow;
            column = currentColumn;

            Space visibleSeatDownRight;

            do
            {
                row -= 1;
                column += 1;

                visibleSeatDownRight = SafeGetByIndex(model,
                                                      row,
                                                      column);
            }
            while (visibleSeatDownRight != null
                   && visibleSeatDownRight.IsFloor);

            // DOWN
            row = currentRow;
            column = currentColumn;

            Space visibleSeatDown;

            do
            {
                row -= 1;

                visibleSeatDown = SafeGetByIndex(model,
                                                 row,
                                                 column);
            }
            while (visibleSeatDown != null
                   && visibleSeatDown.IsFloor);

            // DOWN + LEFT
            row = currentRow;
            column = currentColumn;

            Space visibleSeatDownLeft;

            do
            {
                row -= 1;
                column -= 1;

                visibleSeatDownLeft = SafeGetByIndex(model,
                                                     row,
                                                     column);
            }
            while (visibleSeatDownLeft != null
                   && visibleSeatDownLeft.IsFloor);

            // LEFT
            row = currentRow;
            column = currentColumn;

            Space visibleSeatLeft;

            do
            {
                column -= 1;

                visibleSeatLeft = SafeGetByIndex(model,
                                                 row,
                                                 column);
            }
            while (visibleSeatLeft != null
                   && visibleSeatLeft.IsFloor);

            // UP + LEFT
            row = currentRow;
            column = currentColumn;

            Space visibleSeatUpLeft;

            do
            {
                row += 1;
                column -= 1;

                visibleSeatUpLeft = SafeGetByIndex(model,
                                                   row,
                                                   column);
            }
            while (visibleSeatUpLeft != null
                   && visibleSeatUpLeft.IsFloor);

            neighbors = new List<Space>
                        {
                            visibleSeatAbove,
                            visibleSeatUpRight,
                            visibleSeatRight,
                            visibleSeatDownRight,
                            visibleSeatDown,
                            visibleSeatDownLeft,
                            visibleSeatLeft,
                            visibleSeatUpLeft
                        }.Where(x => x != null)
                         .ToList();

            neighborSpaces[(currentRow, currentColumn)] = neighbors;

            return neighbors;
        }

        private static (HashSet<Space> occupied, HashSet<Space> unoccupied) RunModelOnce(Space[][] model,
                                                                                         HashSet<Space> occupiedSeats,
                                                                                         int seatsToFlip,
                                                                                         Func<Space[][], int, int, List<Space>> getNeighbors)
        {
            var unoccupiedAfterThisRun = new HashSet<Space>();
            var occupiedAfterThisRun = new HashSet<Space>();

            for (var row = 0; row < model.Length; row++)
            {
                var currentRow = model[row];

                for (var column = 0; column < currentRow.Length; column++)
                {
                    var currentSeat = model[row][column];

                    if (currentSeat.IsFloor)
                    {
                        continue;
                    }

                    var currentSeatOccupied = occupiedSeats.Contains(currentSeat);

                    var occupiedSeatsByThisSeat = getNeighbors(model,
                                                               row,
                                                               column)
                        .Count(x => occupiedSeats.Contains(x));

                    if (currentSeatOccupied)
                    {
                        if (occupiedSeatsByThisSeat >= seatsToFlip)
                        {
                            unoccupiedAfterThisRun.Add(currentSeat);
                        }
                        else
                        {
                            occupiedAfterThisRun.Add(currentSeat);
                        }
                    }
                    else
                    {
                        if (occupiedSeatsByThisSeat == 0)
                        {
                            occupiedAfterThisRun.Add(currentSeat);
                        }
                        else
                        {
                            unoccupiedAfterThisRun.Add(currentSeat);
                        }
                    }
                }
            }

            return (occupiedAfterThisRun, unoccupiedAfterThisRun);
        }

        private static Space SafeGetByIndex(Space[][] model,
                                            int row,
                                            int column)
        {
            try
            {
                if (row < 0
                    || row >= model.Length)
                {
                    return null;
                }

                if (column < 0
                    || column
                    >= model[0]
                        .Length)
                {
                    return null;
                }

                return model[row][column];
            }
            catch (IndexOutOfRangeException)
            {
                return null;
            }
        }

        #endregion

        #region Nested type: Space

        public class Space
        {
            #region Constructors

            public Space(bool isFloor,
                         int column,
                         int row)
            {
                IsFloor = isFloor;
                Column = column;
                Row = row;
            }

            #endregion

            #region Instance Properties

            public int Column { get; }

            public bool IsFloor { get; }

            public int Row { get; }

            #endregion

            #region Instance Methods

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null,
                                    obj))
                {
                    return false;
                }

                if (ReferenceEquals(this,
                                    obj))
                {
                    return true;
                }

                if (obj.GetType() != this.GetType())
                {
                    return false;
                }

                return Equals((Space)obj);
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(Column,
                                        Row);
            }

            protected bool Equals(Space other)
            {
                return Column == other.Column && Row == other.Row;
            }

            #endregion
        }

        #endregion
    }
}