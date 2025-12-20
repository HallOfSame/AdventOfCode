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
                // Uses a cool algorithm somebody described on Reddit
                // It takes a bit of time (~10s) to run but that's probably more to do with how I set it up
                // Much more interesting than "turn it into a sequence of equations and pass it to a python solver" that most people seemed to do
                var min = GetMinPressesForJoltage(machine.GoodJoltageState, machine, [], 0);
                logger.LogProgress($"Min was {min}");

                result += min;
            }

            return result.ToString();
        }

        private static int GetMinPressesForJoltage(JoltageCounter expectedCounter, Machine machine, Dictionary<(JoltageCounter, Machine), int> memo, int depth)
        {
            if (expectedCounter.Current.ToArray()
                .All(x => x == 0))
            {
                // If we have nothing to add, return 0
                return 0;
            }

            if (memo.TryGetValue((expectedCounter, machine), out var existingCalc))
            {
                return existingCalc;
            }

            // So first we figure out which buttons need to be pressed an odd number of times
            // If you think of it like part 1, it's the same as making sure these lights end up on
            // So first figure out what light setting this counter corresponds to
            // I.e. # when counter value is odd, . when it is even
            var targetLights = GetLightPolarity(expectedCounter);

            // Find every combination of buttons that ends up with the desired light state
            // We either do or don't hit a button since hitting it twice does nothing
            // So we have 2^x possible combinations where x is the number of buttons
            var allWaysToGetPolarity =
                RecursiveGetPressesForState([], 0, machine, new IndicatorLights(targetLights.Size), targetLights);

            var minPresses = int.MaxValue;

            // For each way we can do the odd presses
            foreach (var startPoint in allWaysToGetPolarity)
            {
                // First actually press all of them
                var presses = startPoint.Count;
                var counterWithThesePresses =
                    startPoint.Aggregate(new JoltageCounter(expectedCounter.Size), (x, btn) => x.Press(btn, 1));

                // And make sure that we haven't passed the counter in any position
                if (counterWithThesePresses.Matches(expectedCounter) == 1)
                {
                    // Something got too high
                    continue;
                }

                // Okay now every counter should be an even number
                // We can calculate the necessary even number of button presses to get the remaining joltage
                // But since they're all even, it can be simplified as 2x the number of presses to get 1/2 that counter value
                // And we can recursively call this function to calculate that
                var remaining = new JoltageCounter(Enumerable.Range(0, expectedCounter.Size)
                                                       .Select(idx => (expectedCounter.Current[idx] -
                                                                       counterWithThesePresses.Current[idx]) / 2)
                                                       .ToArray());

                var pressesForRemaining = GetMinPressesForJoltage(remaining, machine, memo, depth + 1);

                // Guard against running into a case where we couldn't actually reach that remaining value
                if (pressesForRemaining == int.MaxValue)
                {
                    continue;
                }

                presses += pressesForRemaining * 2;

                minPresses = Math.Min(presses, minPresses);
            }

            memo[(expectedCounter, machine)] = minPresses;

            return minPresses;
        }

        private static List<List<Button>> RecursiveGetPressesForState(List<Button> pressedButtons,
                                                                      int currentIndex,
                                                                      Machine machine,
                                                                      IndicatorLights currentState,
                                                                      IndicatorLights targetState)
        {
            // We've gone through all the buttons
            if (currentIndex == machine.Buttons.Count)
            {
                // If it matches, our list is a valid way to get what we want
                if (currentState.Matches(targetState))
                {
                    return [pressedButtons.ToList()];
                }

                // Otherwise, nothing to return
                return [];
            }

            var withButtonNotPressed =
                RecursiveGetPressesForState(pressedButtons, currentIndex + 1, machine, currentState, targetState);

            var withPressState = currentState.Press(machine.Buttons[currentIndex]);
            pressedButtons.Add(machine.Buttons[currentIndex]);
            var withButtonPressed =
                RecursiveGetPressesForState(pressedButtons, currentIndex + 1, machine, withPressState, targetState);

            pressedButtons.Remove(machine.Buttons[currentIndex]);

            return [..withButtonNotPressed, ..withButtonPressed];
        }

        private static IndicatorLights GetLightPolarity(JoltageCounter counter)
        {
            // Return a light where 1 is odd joltage and 0 is even
            return new IndicatorLights(counter.Current.ToArray()
                                           .Select(x => x % 2)
                                           .ToArray());
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

            public JoltageCounter Press(Button button, int times)
            {
                var newStatus = counterStatus.ToArray();

                foreach (var counterIndex in button.Indices)
                {
                    newStatus[counterIndex] += times;
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
