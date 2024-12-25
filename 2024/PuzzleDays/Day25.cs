using Helpers.Structure;
using InputStorageDatabase;

namespace PuzzleDays
{
    public class Day25 : SingleExecutionPuzzle<Day25.ExecState>
    {
        public record ExecState(List<Lock> Locks, List<Key> Keys);

        public override PuzzleInfo Info => new(2024, 25, "Code Chronicle");
        protected override async Task<ExecState> LoadInputState(string puzzleInput, PuzzleInputType inputType)
        {
            var linesOfFile = puzzleInput.Trim()
                .Split('\n');
            var locks = new List<Lock>();
            var keys = new List<Key>();

            for(var i = 0; i < linesOfFile.Length; i++)
            {
                var line = linesOfFile[i];

                var isLock = line.All(x => x == '#');

                if (isLock)
                {
                    var lockLines = linesOfFile.Skip(i + 1)
                        .Take(6);

                    var lockHeights = Enumerable.Repeat(-1, 5)
                        .ToList();
                    var currentHeight = 0;

                    foreach (var lockLine in lockLines)
                    {
                        for (var lockIndex = 0; lockIndex < lockLine.Length; lockIndex++)
                        {
                            if (lockLine[lockIndex] == '.' && lockHeights[lockIndex] == -1)
                            {
                                lockHeights[lockIndex] = currentHeight;
                            }
                        }

                        currentHeight++;
                    }

                    locks.Add(new Lock(lockHeights));
                }
                else
                {
                    var keyLines = linesOfFile.Skip(i)
                        .Take(7)
                        .ToList();

                    keyLines.Reverse();

                    var keyHeights = Enumerable.Repeat(-1, 5)
                        .ToList();
                    var currentHeight = 0;

                    foreach (var keyLine in keyLines.Skip(1))
                    {
                        for (var keyIndex = 0; keyIndex < keyLine.Length; keyIndex++)
                        {
                            if (keyLine[keyIndex] == '.' && keyHeights[keyIndex] == -1)
                            {
                                keyHeights[keyIndex] = currentHeight;
                            }
                        }

                        currentHeight++;
                    }

                    keys.Add(new Key(keyHeights));
                }

                // 6 of the def + 1 to skip the newline
                i += 7;
            }

            return new ExecState(locks, keys);
        }

        protected override async Task<string> ExecutePuzzlePartOne()
        {
            return InitialState.Locks.Select(l => InitialState.Keys.Count(k => CanFit(l, k)))
                .Sum()
                .ToString();
        }

        public static bool CanFit(Lock l, Key key)
        {
            for (var i = 0; i < 5; i++)
            {
                var lockHeight = l.Heights[i];
                var keyHeight = key.Heights[i];

                if (lockHeight + keyHeight > 5)
                {
                    return false;
                }
            }

            return true;
        }

        public record Key(List<int> Heights);

        public record Lock(List<int> Heights);

        protected override async Task<string> ExecutePuzzlePartTwo()
        {
            return "50 Stars!";
        }
    }
}
