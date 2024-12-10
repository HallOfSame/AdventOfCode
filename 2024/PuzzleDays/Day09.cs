using System.Globalization;
using Helpers.Structure;

namespace PuzzleDays
{
    public class Day09 : SingleExecutionPuzzle<Day09.ExecState>
    {
        /// <summary>
        /// The mempackage copied hates linked lists. It just crashes outright.
        /// </summary>
        public record ExecState(string Input);

        public override PuzzleInfo Info => new(2024, 9, "Drive Defragmenter");
        protected override async Task<ExecState> LoadInputState(string puzzleInput)
        {
            return new ExecState(puzzleInput.Trim());
        }

        protected override async Task<string> ExecutePuzzlePartOne()
        {
            var isFile = true;
            var fileIndex = 0;
            DriveRecord? lastRecord = null;
            var records = new List<DriveRecord>();
            for (var i = 0; i < InitialState.Input.Length; i++)
            {
                var newRecordSize = int.Parse(InitialState.Input[i]
                                                  .ToString());

                if (newRecordSize == 0)
                {
                    isFile = !isFile;
                    continue;
                }

                var newRecord = new DriveRecord
                {
                    FileIndex = isFile ? fileIndex : 0,
                    IsFile = isFile,
                    Size = newRecordSize,
                    Prev = lastRecord
                };

                if (isFile)
                {
                    fileIndex++;
                }

                isFile = !isFile;
                lastRecord = newRecord;
                records.Add(newRecord);
            }

            for (var i = records.Count - 2; i >= 0; i--)
            {
                records[i].Next = records[i + 1];
            }

            var drive = new HardDrive
            {
                FirstRecord = records[0]
            };

            var compactor = DriveCompactor.Initialize(drive);

            compactor.Compact();

            var result = drive.GetChecksum();

            return result.ToString(CultureInfo.InvariantCulture);
        }

        protected override async Task<string> ExecutePuzzlePartTwo()
        {
            throw new NotImplementedException();
        }

        public class DriveCompactor
        {
            public static DriveCompactor Initialize(HardDrive drive)
            {
                var currentRecord = drive.FirstRecord;
                DriveRecord? earliestFreeRecord = null;
                DriveRecord? latestFileRecord = null;

                // Iterate the drive to find our starting points
                while (true)
                {
                    switch (currentRecord.IsFile)
                    {
                        case false when earliestFreeRecord is null:
                            earliestFreeRecord = currentRecord;
                            break;
                        case true:
                            latestFileRecord = currentRecord;
                            break;
                    }

                    currentRecord = currentRecord.Next;

                    if (currentRecord is null)
                    {
                        break;
                    }
                }

                if (earliestFreeRecord is null)
                {
                    throw new InvalidOperationException("Found no free space");
                }

                if (latestFileRecord is null)
                {
                    throw new InvalidOperationException("Found no files");
                }

                return new DriveCompactor(earliestFreeRecord, latestFileRecord);
            }

            private DriveCompactor(DriveRecord earliestFree, DriveRecord latestFile)
            {
                earliestFreeRecord = earliestFree;
                latestFileRecord = latestFile;
            }

            private DriveRecord earliestFreeRecord;
            private DriveRecord latestFileRecord;

            public void Compact()
            {
                while (true)
                {
                    // First copy the file data we have space for to the spot before the free space record
                    var sizeToMove = Math.Min(latestFileRecord.Size, earliestFreeRecord.Size);
                    var movedFile = new DriveRecord
                    {
                        IsFile = true,
                        FileIndex = latestFileRecord.FileIndex,
                        Size = sizeToMove
                    };

                    // Place it before the earliest free record
                    InsertBetween(earliestFreeRecord.Prev, earliestFreeRecord, movedFile);
                    // Update the sizes (one should be going to 0, maybe even both will)
                    earliestFreeRecord.Size -= movedFile.Size;
                    latestFileRecord.Size -= movedFile.Size;

                    // We also created free space after the current file
                    if (latestFileRecord.Next is null)
                    {
                        // If it was null, add a record
                        var newFreeRecord = new DriveRecord
                        {
                            IsFile = false,
                            Size = movedFile.Size
                        };

                        InsertBetween(latestFileRecord, latestFileRecord.Next, newFreeRecord);
                    }
                    else
                    {
                        if (latestFileRecord.Next.IsFile)
                        {
                            throw new
                                InvalidOperationException("Latest file should always be followed only by free space");
                        }

                        // Otherwise, increase the size of the existing one
                        latestFileRecord.Next.Size += movedFile.Size;
                    }

                    if (earliestFreeRecord.Size == 0)
                    {
                        // We completely used this space and need to remove if from the linked list
                        RemoveRecord(earliestFreeRecord);

                        var nextFreeRecord = FindNextFreeRecord();

                        if (nextFreeRecord is null)
                        {
                            break;
                        }

                        earliestFreeRecord = nextFreeRecord;
                    }

                    if (latestFileRecord.Size == 0)
                    {
                        // We moved the rest of this file, delete the empty record
                        RemoveRecord(latestFileRecord);

                        var nextFileRecord = FindNextFileToMove();

                        if (nextFileRecord is null)
                        {
                            break;
                        }

                        latestFileRecord = nextFileRecord;
                    }
                }
            }

            private DriveRecord? FindNextFreeRecord()
            {
                var current = earliestFreeRecord.Next;

                while (true)
                {
                    if (current is null)
                    {
                        return null;
                    }

                    if (current == latestFileRecord)
                    {
                        // No free space exists before what we'd want to move now
                        return null;
                    }

                    if (!current.IsFile)
                    {
                        return current;
                    }

                    current = current.Next;
                }
            }

            private DriveRecord? FindNextFileToMove()
            {
                var current = latestFileRecord.Prev;

                while (true)
                {
                    if (current is null)
                    {
                        return null;
                    }

                    if (current == earliestFreeRecord)
                    {
                        // We moved everything before this
                        return null;
                    }

                    if (current.IsFile)
                    {
                        return current;
                    }

                    current = current.Prev;
                }
            }

            private static void InsertBetween(DriveRecord? previousRecord, DriveRecord? nextRecord, DriveRecord recordToInsert)
            {
                if (previousRecord == nextRecord)
                {
                    throw new InvalidOperationException("Not designed to be called with the same record twice");
                }

                if (recordToInsert.Next is not null || recordToInsert.Prev is not null)
                {
                    throw new InvalidOperationException("Shouldn't set these properties outside of this method");
                }

                if (previousRecord != null)
                {
                    previousRecord.Next = recordToInsert;
                }

                if (nextRecord != null)
                {
                    nextRecord.Prev = recordToInsert;
                }

                recordToInsert.Prev = previousRecord;
                recordToInsert.Next = nextRecord;
            }

            private static void RemoveRecord(DriveRecord recordToRemove)
            {
                if (recordToRemove.Size != 0)
                {
                    throw new InvalidOperationException("Should only remove 0 size records");
                }

                var prev = recordToRemove.Prev;
                var next = recordToRemove.Next;

                if (prev != null)
                {
                    prev.Next = next;
                }

                if (next != null)
                {
                    next.Prev = prev;
                }
            }
        }

        public class HardDrive
        {
            public DriveRecord FirstRecord { get; set; }

            public decimal GetChecksum()
            {
                var checksum = 0m;
                var driveIndex = 0;
                var current = FirstRecord;

                while (current != null)
                {
                    var size = current.Size;

                    for (var i = 0; i < size; i++)
                    {
                        checksum += current.FileIndex * driveIndex;
                        driveIndex++;
                    }

                    current = current.Next;
                }

                return checksum;
            }
        }

        public class DriveRecord
        {
            public DriveRecord? Prev { get; set; }
            public DriveRecord? Next { get; set; }

            public int FileIndex { get; set; }
            public bool IsFile { get; set; }
            public int Size { get; set; }

            public override string ToString()
            {
                return IsFile ? $"FILE-{FileIndex}-{Size}" : $"FREE-{Size}";
            }
        }
    }
}
