using Helpers.Extensions;
using Helpers.Maps;
using Helpers.Structure;
using InputStorageDatabase;

namespace PuzzleDays
{
    public class Day21 : SingleExecutionPuzzle<Day21.ExecState>
    {
        public record ExecState(List<string> Sequences);

        public override PuzzleInfo Info => new(2024, 21, "Keypad Conundrum");
        protected override async Task<ExecState> LoadInputState(string puzzleInput, PuzzleInputType inputType)
        {
            return new ExecState(puzzleInput.Trim()
                                     .Split('\n')
                                     .ToList());
        }

        protected override async Task<string> ExecutePuzzlePartOne()
        {
            var robotThreeKeypadMoves = GetAllNumericKeypadMoves();
            var directionalKeypadMoves = GetAllDirectionalKeypadMoves();

            var result = 0;

            foreach (var sequence in InitialState.Sequences)
            {
                var minMoves = GetMinMovesForSequence(sequence, robotThreeKeypadMoves, directionalKeypadMoves);
                var numericPartOfSequence = int.Parse(sequence.Replace("A", string.Empty));
                result += minMoves * numericPartOfSequence;
            }

            return result.ToString();
        }

        private int GetMinMovesForSequence(string sequence, Dictionary<(char start, char end), List<string>> robotThreeKeypadMoves, Dictionary<(char start, char end), List<string>> directionalKeypadMoves)
        {
            // At each index, it's the options robot 2 has
            var robotTwoMoveOptions = new List<List<string>>();

            var robotThreePointer = 'A';

            foreach (var sequenceChar in sequence)
            {
                robotTwoMoveOptions.Add(robotThreeKeypadMoves[(robotThreePointer, sequenceChar)]);
                robotThreePointer = sequenceChar;
            }

            // Consolidate the lists to all combinations, so it's just a flat list of moves we could make for the numeric keypad result
            var robotTwoConsolidated = new List<string>();
            ConsolidateOptions(robotTwoMoveOptions, 0, string.Empty, robotTwoConsolidated);

            var robotOneConsolidated = new List<string>();
            var robotTwoPointer = 'A';

            // Foreach move sequence robot two could make
            foreach (var robotTwoMoveList in robotTwoConsolidated)
            {
                var robotOneMoveOptions = new List<List<string>>();

                // Figure out the ways robot one could command that sequence
                foreach (var moveChar in robotTwoMoveList)
                {
                    if (moveChar == robotTwoPointer)
                    {
                        robotOneMoveOptions.Add(["A"]);
                        continue;
                    }

                    robotOneMoveOptions.Add(directionalKeypadMoves[(robotTwoPointer, moveChar)]);
                    robotTwoPointer = moveChar;
                }

                // And flatten as we go
                ConsolidateOptions(robotOneMoveOptions, 0, string.Empty, robotOneConsolidated);
            }

            // Make sure no duplicates (probably could've used a hashset)
            robotOneConsolidated = robotOneConsolidated.Distinct()
                .ToList();

            // Now we have the moves for robot one
            // Repeat the process but for moves we could make
            var youConsolidated = new List<string>();
            var robotOnePointer = 'A';

            foreach (var robotOneMoveList in robotOneConsolidated)
            {
                var youMoveOptions = new List<List<string>>();

                foreach (var moveChar in robotOneMoveList)
                {
                    if (moveChar == robotOnePointer)
                    {
                        youMoveOptions.Add(["A"]);
                        continue;
                    }

                    youMoveOptions.Add(directionalKeypadMoves[(robotOnePointer, moveChar)]);
                    robotOnePointer = moveChar;
                }

                ConsolidateOptions(youMoveOptions, 0, string.Empty, youConsolidated);
            }

            youConsolidated = youConsolidated.Distinct()
                .ToList();

            return youConsolidated.Min(x => x.Length);
        }

        protected override async Task<string> ExecutePuzzlePartTwo()
        {
            partTwoMemo.Clear();
            var numberOfCenterRobots = 24;
            var finalRobotKeypadMoves = GetAllNumericKeypadMoves();
            var directionalKeypadMoves = GetAllDirectionalKeypadMoves();

            var finalRobotPointer = 'A';
            var result = 0L;
            foreach (var sequence in InitialState.Sequences)
            {
                var minMoves = 0L;

                // The way this works is:
                // Functionally at each level, the sequence to get the next robot to do something, i.e. <<^A does not change
                // And it even works deeper down then that, because we have to hit A at the end. So the << of that pattern is the same sequence twice at the next level
                // So we figure out the directional sequence needed by the robot directly controlling the final one (pretty easy)
                // And so if we memoize as we go, we quickly get the best way at each level to run a small subsequence of moves
                foreach (var sequenceChar in sequence)
                {
                    var firstKeypadOptions = finalRobotKeypadMoves[(finalRobotPointer, sequenceChar)];

                    var optionToUse = firstKeypadOptions.Select(x => FindMinMovesForDirectionSequence(x,
                                                                     0,
                                                                     numberOfCenterRobots,
                                                                     directionalKeypadMoves))
                        .Min();

                    minMoves += optionToUse;

                    finalRobotPointer = sequenceChar;
                }

                var numericPartOfSequence = int.Parse(sequence.Replace("A", string.Empty));
                result += minMoves * numericPartOfSequence;
            }

            return result.ToString();
        }

        private Dictionary<(string sequence, int recursionDepth), long> partTwoMemo = [];

        private long FindMinMovesForDirectionSequence(string directionSequence,
                                                      int recursionDepth,
                                                      int maxRecursionDepth,
                                                      Dictionary<(char start, char end), List<string>> directionalKeypadMoves)
        {
            if (partTwoMemo.TryGetValue((directionSequence, recursionDepth), out var existingResult))
            {
                return existingResult;
            }

            var result = 0L;
            var nextPointer = 'A';
            foreach (var directionButton in directionSequence)
            {
                // If the button is the same, we just hit A again
                if (directionButton == nextPointer)
                {
                    result += 1;
                    continue;
                }

                // Figure out options we could use to direct the robot below to hit this button
                var moveOptionsForThisButton = directionalKeypadMoves[(nextPointer, directionButton)];

                if (recursionDepth == maxRecursionDepth)
                {
                    // If we're all the way at the end, then we just need the length of the strings
                    var optionToUse = moveOptionsForThisButton.MinBy(x => x.Length)!;

                    result += optionToUse.Length;
                }
                else
                {
                    // Otherwise, recurse to figure out the min at the next level
                    var optionToUse = moveOptionsForThisButton.Select(x => FindMinMovesForDirectionSequence(x,
                                                                           recursionDepth + 1,
                                                                           maxRecursionDepth,
                                                                           directionalKeypadMoves))
                        .Min();

                    result += optionToUse;
                }

                nextPointer = directionButton;
            }

            partTwoMemo[(directionSequence, recursionDepth)] = result;
            return result;
        }

        private static void ConsolidateOptions(List<List<string>> robotTwoMoveOptions, int index, string current, List<string> results)
        {
            if (index == robotTwoMoveOptions.Count)
            {
                results.Add(current);
                return;
            }

            foreach (var optionAtThisIndex in robotTwoMoveOptions[index])
            {
                ConsolidateOptions(robotTwoMoveOptions, index + 1, current + optionAtThisIndex, results);
            }
        }

        private Dictionary<(char start, char end), List<string>> GetAllDirectionalKeypadMoves()
        {
            var result = new Dictionary<(char start, char end), List<string>>();
            var keypadValues = directionalKeypad.Keys.ToList();

            var keypadGap = new Coordinate(0, 1);

            foreach (var keypadMove in keypadValues.Permutations(2))
            {
                var start = keypadMove[0];
                var end = keypadMove[1];
                List<string> possibleMoves;

                var startCoordinate = directionalKeypad[start];
                var endCoordinate = directionalKeypad[end];

                var xDiff = startCoordinate.X - endCoordinate.X;
                var yDiff = startCoordinate.Y - endCoordinate.Y;
                var xSpaces = (int)Math.Abs(xDiff);
                var ySpaces = (int)Math.Abs(yDiff);
                var horizontalDirection = xDiff > 0 ? Direction.West : Direction.East;
                var verticalDirection = yDiff > 0 ? Direction.South : Direction.North;

                var horizontalLandsInGap = startCoordinate.Move(horizontalDirection, xSpaces) == keypadGap;
                var verticalLandsInGap = startCoordinate.Move(verticalDirection, ySpaces) == keypadGap;

                var horizontalFirstString =
                    $"{DirectionRepeatToString(horizontalDirection, xSpaces)}{DirectionRepeatToString(verticalDirection, ySpaces)}A";
                var vertFirstString = $"{DirectionRepeatToString(verticalDirection, ySpaces)}{DirectionRepeatToString(horizontalDirection, xSpaces)}A";

                if (verticalLandsInGap || horizontalLandsInGap)
                {
                    // Has a sequence it needs to happen in to avoid the gap
                    possibleMoves = verticalLandsInGap ? [horizontalFirstString] : [vertFirstString];
                }
                else if (horizontalFirstString == vertFirstString)
                {
                    // Only moves in one direction, doesn't matter which we pick here, but we only need one
                    possibleMoves = [horizontalFirstString];
                }
                else
                {
                    // Order does not matter, and we can use either
                    // This can affect robots controlling us so add both
                    possibleMoves = [horizontalFirstString, vertFirstString];
                }

                result[(start, end)] = possibleMoves;
            }

            return result;
        }

        private Dictionary<(char start, char end), List<string>> GetAllNumericKeypadMoves()
        {
            var result = new Dictionary<(char start, char end), List<string>>();
            var keypadValues = numericalKeypad.Keys.ToList();

            var keypadGap = new Coordinate(0, 0);

            foreach (var keypadMove in keypadValues.Permutations(2))
            {
                var start = keypadMove[0];
                var end = keypadMove[1];
                List<string> possibleMoves;

                var startCoordinate = numericalKeypad[start];
                var endCoordinate = numericalKeypad[end];

                var xDiff = startCoordinate.X - endCoordinate.X;
                var yDiff = startCoordinate.Y - endCoordinate.Y;
                var xSpaces = (int)Math.Abs(xDiff);
                var ySpaces = (int)Math.Abs(yDiff);
                var horizontalDirection = xDiff > 0 ? Direction.West : Direction.East;
                var verticalDirection = yDiff > 0 ? Direction.South : Direction.North;

                var horizontalLandsInGap = startCoordinate.Move(horizontalDirection, xSpaces) == keypadGap;
                var verticalLandsInGap = startCoordinate.Move(verticalDirection, ySpaces) == keypadGap;

                var horizontalFirstString =
                    $"{DirectionRepeatToString(horizontalDirection, xSpaces)}{DirectionRepeatToString(verticalDirection, ySpaces)}A";
                var vertFirstString = $"{DirectionRepeatToString(verticalDirection, ySpaces)}{DirectionRepeatToString(horizontalDirection, xSpaces)}A";

                if (verticalLandsInGap || horizontalLandsInGap)
                {
                    possibleMoves = verticalLandsInGap ? [horizontalFirstString] : [vertFirstString];
                }
                else if (horizontalFirstString == vertFirstString)
                {
                    // Only moves in one direction, doesn't matter which we pick here
                    possibleMoves = [horizontalFirstString];
                }
                else
                {
                    // Order does not matter
                    possibleMoves = [horizontalFirstString, vertFirstString];
                }

                result[(start, end)] = possibleMoves;
            }

            return result;
        }

        private static string DirectionRepeatToString(Direction direction, int repeat)
        {
            return CharRepeatToString(direction.ToChar(), repeat);
        }

        private static string CharRepeatToString(char c, int repeat)
        {
            if (repeat == 0)
            {
                return string.Empty;
            }

            return new string(Enumerable.Repeat(c, repeat)
                                  .ToArray());
        }

        private readonly IReadOnlyDictionary<char, Coordinate> numericalKeypad =
            new Dictionary<char, Coordinate>()
            {
                { '0', new Coordinate(1, 0) },
                { 'A', new Coordinate(2, 0) },
                { '1', new Coordinate(0, 1) },
                { '2', new Coordinate(1, 1) },
                { '3', new Coordinate(2, 1) },
                { '4', new Coordinate(0, 2) },
                { '5', new Coordinate(1, 2) },
                { '6', new Coordinate(2, 2) },
                { '7', new Coordinate(0, 3) },
                { '8', new Coordinate(1, 3) },
                { '9', new Coordinate(2, 3) },
            };

        private readonly IReadOnlyDictionary<char, Coordinate> directionalKeypad = new Dictionary<char, Coordinate>
        {
            { '<', new Coordinate(0, 0) },
            { 'v', new Coordinate(1, 0) },
            { '>', new Coordinate(2, 0) },
            { '^', new Coordinate(1, 1) },
            { 'A', new Coordinate(2, 1) },
        };
    }
}
