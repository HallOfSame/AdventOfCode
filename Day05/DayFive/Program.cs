using System;
using System.Linq;

namespace DayFive
{
    internal class Program
    {
        #region Constants

        private const int MaxColumn = 7;

        private const int MaxRow = 127;

        #endregion

        #region Class Methods

        public static void Main(string[] args)
        {
            var seatsOnFlight = Input.PuzzleInput.Select(GetSeatFromInputString)
                                     .ToList();

            var maxSeatId = seatsOnFlight.Max(s => s.SeatId);

            // Part one
            Console.WriteLine($"Max seat ID in input: {maxSeatId}.");

            // Part two
            var allRows = Enumerable.Range(0,
                                           MaxRow + 1);
            var allColumns = Enumerable.Range(0,
                                              MaxColumn + 1);

            var allSeats = allRows.Where(r => r != 0 && r != MaxRow)
                                  .SelectMany(r => allColumns.Select(c => new SeatModel(r,
                                                                                        c)))
                                  .ToList();

            var seatIdsOnFlight = seatsOnFlight.Select(x => x.SeatId)
                                               .ToHashSet();

            var missingSeats = allSeats.Except(seatsOnFlight)
                                       .Where(s => seatIdsOnFlight.Contains(s.SeatId + 1) && seatIdsOnFlight.Contains(s.SeatId - 1))
                                       .ToList();

            if (missingSeats.Count != 1)
            {
                throw new InvalidOperationException("Should only be one seat left :(");
            }

            var missingSeat = missingSeats.First();

            Console.WriteLine($"Your seat is row {missingSeat.Row} column {missingSeat.Column} ID {missingSeat.SeatId}.");

            Console.ReadKey();
        }

        private static SeatModel GetSeatFromInputString(string boardingPassString)
        {
            var rowString = boardingPassString.Substring(0,
                                                         7);

            var columnString = boardingPassString.Substring(7,
                                                            3);

            var row = GetValueFromBinaryPartitionString(MaxRow,
                                                        0,
                                                        'B',
                                                        'F',
                                                        rowString);

            var column = GetValueFromBinaryPartitionString(MaxColumn,
                                                           0,
                                                           'R',
                                                           'L',
                                                           columnString);

            return new SeatModel(row,
                                 column);
        }

        private static int GetValueFromBinaryPartitionString(int upperBound,
                                                             int lowerBound,
                                                             char upperChar,
                                                             char lowerChar,
                                                             string binaryString)
        {
            var currentRangeTop = upperBound;
            var currentRangeBottom = lowerBound;

            foreach (var charValue in binaryString)
            {
                if (charValue == upperChar)
                {
                    currentRangeBottom = (int)Math.Floor((currentRangeTop + currentRangeBottom) / 2m) + 1;
                }
                else if (charValue == lowerChar)
                {
                    currentRangeTop = (int)Math.Ceiling((currentRangeBottom + currentRangeTop) / 2m) - 1;
                }
                else
                {
                    throw new ArgumentException($"Invalid char in binary string {binaryString} {charValue}.");
                }
            }

            // Sanity check
            if (currentRangeTop != currentRangeBottom)
            {
                throw new InvalidOperationException($"{binaryString} resulted in different end ranges {currentRangeTop} - {currentRangeBottom}.");
            }

            return currentRangeTop;
        }

        #endregion
    }
}