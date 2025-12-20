using System.Text.RegularExpressions;
using Helpers.Logging;
using Helpers.Structure;

namespace PuzzleDays
{
    public partial class Day10(ProgressLogger logger) : SingleExecutionPuzzle<Day10.State>
    {
        public override PuzzleInfo Info => new(2025, 10, "Factory");

        protected override async Task<string> ExecutePuzzlePartOne()
        {
            var result = 0;

            foreach (var machine in InitialState.Machines)
            {
                var min = GetMinButtonPressesToStart(machine);
                logger.LogProgress($"Min was {min}");

                result += min;
            }

            return result.ToString();
        }

        private int GetMinButtonPressesToStart(Machine machine)
        {
            var queue = new Queue<(Button btn, IndicatorLights lightState, int pressCount)>();
            var visited = new HashSet<(Button, IndicatorLights)>();

            foreach (var button in machine.Buttons)
            {
                var emptyLights = new IndicatorLights(machine.OnLightState.Size);
                queue.Enqueue((button, emptyLights, 1));
                visited.Add((button, emptyLights));
            }

            while (true)
            {
                var next = queue.Dequeue();

                var newState = next.lightState.Press(next.btn);

                if (newState.Matches(machine.OnLightState))
                {
                    return next.pressCount;
                }

                foreach (var button in machine.Buttons)
                {
                    if (visited.Add((button, newState)))
                    {
                        queue.Enqueue((button, newState, next.pressCount + 1));
                    }
                }
            }
        }

        protected override async Task<string> ExecutePuzzlePartTwo()
        {
            throw new NotImplementedException();
        }

        public class State
        {
            public required List<Machine> Machines { get; init; }
        }

        public class Machine(IndicatorLights desiredLights)
        {
            public IndicatorLights OnLightState { get; } = desiredLights;
            public required List<Button> Buttons { get; init; }
        }

        public class Button(int[] lights)
        {
            public ReadOnlySpan<int> Lights => lights.AsSpan();
        }

        public class IndicatorLights(int[] state)
        {
            public IndicatorLights(int size) : this(new int[size])
            {
            }

            private readonly int[] lightStatus = state;

            public int Size { get; } = state.Length;

            public ReadOnlySpan<int> Current => lightStatus.AsSpan();

            public bool Matches(IndicatorLights other)
            {
                var otherStatus = other.Current;

                if (otherStatus.Length != lightStatus.Length)
                {
                    throw new InvalidOperationException("Something broke, lights are not the same size");
                }

                for (var i = 0; i < lightStatus.Length; i++)
                {
                    if (otherStatus[i] != lightStatus[i])
                    {
                        return false;
                    }
                }

                return true;
            }

            public IndicatorLights Press(Button button)
            {
                var newStatus = lightStatus.ToArray();

                foreach (var lightIndex in button.Lights)
                {
                    newStatus[lightIndex] ^= 1;
                }

                return new IndicatorLights(newStatus);
            }

            public static IndicatorLights Parse(string status)
            {
                var size = status.Length;

                var res = new IndicatorLights(size);

                for (var i = 0; i < status.Length; i++)
                {
                    if (status[i] == '#')
                    {
                        res.lightStatus[i] = 1;
                    }
                }

                return res;
            }

            public override int GetHashCode()
            {
                return lightStatus.Aggregate(string.Empty, (curr, next) => curr + next)
                    .GetHashCode();
            }

            public override bool Equals(object? obj)
            {
                if (obj is IndicatorLights other)
                {
                    return other.Matches(this);
                }

                return false;
            }
        }

        protected override async Task<State> LoadInputState(string puzzleInput, PuzzleInputType inputType)
        {
            var machines = new List<Machine>();

            foreach (var line in puzzleInput.Split('\n'))
            {
                var match = MachineRegex()
                    .Match(line);

                var indicator = match.Groups[1].Value;
                var buttons = match.Groups[2].Value;
                // TODO probably in part 2
                // var joltage = match.Groups[3].Value;
                var buttonSplit = buttons.Split(' ')
                    .Select(x =>
                    {
                        var eachIndex = x.Replace('(', ' ')
                            .Replace(')', ' ')
                            .Split(',')
                            .Select(int.Parse)
                            .ToArray();

                        return eachIndex;
                    })
                    .ToList();

                var machine = new Machine(IndicatorLights.Parse(indicator))
                {
                    Buttons = buttonSplit.Select(x => new Button(x))
                        .ToList()
                };

                machines.Add(machine);
            }

            return new State
            {
                Machines = machines
            };
        }

        [GeneratedRegex(@"\[(.*)\] (.*) \{(.*)\}")]
        private static partial Regex MachineRegex();
    }
}
