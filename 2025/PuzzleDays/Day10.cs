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
            var result = 0;

            foreach (var machine in InitialState.Machines)
            {
                var min = GetMinButtonPressesForJoltage(machine);
                logger.LogProgress($"Min was {min}");

                result += min;
            }

            return result.ToString();
        }

        private int GetMinButtonPressesForJoltage(Machine machine)
        {
            // TODO this takes too long to run
            // My current guess is something like:
            // There's some number of presses that hits a lower joltage state where you repeat it enough times for the answer
            // So you really have to find this magic sequence / state and then multiply how many times it takes to get there
            // I'm not sure if it's like an LCM or something else though
            // Maybe when we check a state we should be doing something like check if each index divides into it cleanly
            // I.e. if expected[i] / curr[i] = x for every index, then result = currentPresses * x

            // This didn't work
            // Maybe it's like, find which buttons can get counter idx 0 to the right value?
            // Then see where everything else is from there or something?
            // Or somehow isolate hitting a single button over and over
            // Order doesn't matter anymore
            // So maybe it's like when you press a button you might as well press it until a counter is correct
            // So our queue becomes press 15 (e.g.) times instead of once

            var goodState = machine.GoodJoltageState.Current;
            var queue = new Queue<(Button btn, JoltageCounter joltState, int pressCount)>();
            var visited = new HashSet<(Button, JoltageCounter)>();

            foreach (var button in machine.Buttons)
            {
                var emptyCounters = new JoltageCounter(machine.GoodJoltageState.Size);
                queue.Enqueue((button, emptyCounters, 1));
                visited.Add((button, emptyCounters));
            }

            while (true)
            {
                var next = queue.Dequeue();

                var newState = next.joltState.Press(next.btn);
                var comparison = newState.Matches(machine.GoodJoltageState);

                if (comparison == 0)
                {
                    return next.pressCount;
                }

                if (comparison == 1)
                {
                    // Something is too high so stop looking down this line
                    continue;
                }

                var current = newState.Current;
                if (current[0] != 0)
                {
                    var mod = Math.DivRem(machine.GoodJoltageState.Current[0], current[0]);
                    var i = 1;
                    var found = mod.Remainder == 0;

                    while (mod.Remainder == 0 && i < current.Length)
                    {
                        if (current[i] == 0)
                        {
                            found = false;
                            break;
                        }

                        var nextMod = Math.DivRem(machine.GoodJoltageState.Current[i], newState.Current[i]);

                        if (nextMod.Quotient != mod.Quotient)
                        {
                            found = false;
                            break;
                        }

                        mod = nextMod;
                    }

                    if (found)
                    {
                        var z = 4;
                    }
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

        public class State
        {
            public required List<Machine> Machines { get; init; }
        }

        public class Machine(IndicatorLights desiredLights, JoltageCounter desiredJoltage)
        {
            public IndicatorLights OnLightState { get; } = desiredLights;
            public JoltageCounter GoodJoltageState { get; } = desiredJoltage;
            public required List<Button> Buttons { get; init; }
        }

        public class Button(int[] lights)
        {
            public ReadOnlySpan<int> Indices => lights.AsSpan();
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

                foreach (var lightIndex in button.Indices)
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

        public class JoltageCounter(int[] state)
        {
            public JoltageCounter(int size) : this(new int[size])
            {
            }

            private readonly int[] counterStatus = state;

            public int Size { get; } = state.Length;

            public ReadOnlySpan<int> Current => counterStatus.AsSpan();

            public int Matches(JoltageCounter other)
            {
                var otherStatus = other.Current;

                if (otherStatus.Length != counterStatus.Length)
                {
                    throw new InvalidOperationException("Something broke, counters are not the same size");
                }

                var under = false;

                for (var i = 0; i < counterStatus.Length; i++)
                {
                    var comparison = otherStatus[i]
                        .CompareTo(counterStatus[i]);

                    switch (comparison)
                    {
                        case < 0:
                            return 1;
                        case > 0:
                            under = true;
                            break;
                    }
                }

                return under ? -1 : 0;
            }

            public JoltageCounter Press(Button button)
            {
                var newStatus = counterStatus.ToArray();

                foreach (var counterIndex in button.Indices)
                {
                    newStatus[counterIndex] += 1;
                }

                return new JoltageCounter(newStatus);
            }

            public override int GetHashCode()
            {
                return counterStatus.Aggregate(string.Empty, (curr, next) => curr + next)
                    .GetHashCode();
            }

            public override bool Equals(object? obj)
            {
                if (obj is JoltageCounter other)
                {
                    return other.Matches(this) == 0;
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
                var joltage = match.Groups[3]
                    .Value.Split(',')
                    .Select(int.Parse)
                    .ToArray();
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

                var machine = new Machine(IndicatorLights.Parse(indicator), new JoltageCounter(joltage))
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
