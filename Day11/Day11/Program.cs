using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day11
{
    internal class Program
    {
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
            var unoccupiedSeats = model.SelectMany(r => r.Where(s => !s.IsFloor))
                                       .ToHashSet();

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
                                                 4);

                if (occupied.SetEquals(occupiedSeats))
                {
                    break;
                }

                occupiedSeats = occupied;
            }


            Console.WriteLine($"There are {occupiedSeats.Count} seats occupied.");
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

        private static (HashSet<Space> occupied, HashSet<Space> unoccupied) RunModelOnce(Space[][] model,
                                                                                         HashSet<Space> occupiedSeats,
                                                                                         int seatsToFlip)
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

                    var occupiedSeatsByThisSeat = GetNeighboringNonFloorSpaces(model,
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