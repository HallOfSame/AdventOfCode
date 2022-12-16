using System.Diagnostics;
using System.Text.RegularExpressions;

using Helpers.FileReaders;
using Helpers.Structure;

namespace PuzzleDays
{
    public class Day16 : ProblemBase
    {
        protected override async Task<string> SolvePartOneInternal()
        {
            var result = GetMaxPressureReleased(new List<Valve>(),
                                                string.Empty,
                                                valves["AA"],
                                                30,
                                                0);

            return result.ToString();
        }

        private Dictionary<string, int> memo = new();

        private int GetMaxPressureReleased(List<Valve> openValves,
                                           string openValvesString,
                                           Valve currentValve,
                                           int minutesRemaining,
                                           int currentPressureReleased)
        {
            var scenarioCode = GetScenarioCode(openValvesString,
                                               currentValve,
                                               minutesRemaining);

            if (memo.TryGetValue(scenarioCode,
                                 out var result))
            {
                return result;
            }

            // Base case, out of time
            if (minutesRemaining <= 0)
            {
                memo[scenarioCode] = currentPressureReleased;
                return currentPressureReleased;
            }

            int AddPressureReleased()
            {
                return currentPressureReleased + openValves.Sum(x => x.FlowRate);
            }

            var maxPressurePossibleFromHere = 0;

            // Options:
            // Open current valve if not open
            // Skip if it wouldn't release any more pressure
            if (!currentValve.IsOpen && currentValve.FlowRate > 0)
            {
                var minutesAfterOpening = minutesRemaining - 1;
                var updatedRelease = AddPressureReleased();
                currentValve.IsOpen = true;
                openValves.Add(currentValve);
                var addToCode = $"{currentValve.Name}{minutesAfterOpening}";
                openValvesString += addToCode;
                maxPressurePossibleFromHere = GetMaxPressureReleased(openValves,
                                                                     openValvesString,
                                                                     currentValve,
                                                                     minutesAfterOpening,
                                                                     updatedRelease);
                openValves.RemoveAt(openValves.Count - 1);
                openValvesString = openValvesString[..^addToCode.Length];
                currentValve.IsOpen = false;
            }

            // Move to neighbor without opening
            foreach (var neighbor in currentValve.Neighbors)
            {
                var minutesAfterMoving = minutesRemaining - 1;
                var updatedRelease = AddPressureReleased();

                maxPressurePossibleFromHere = Math.Max(maxPressurePossibleFromHere,
                                                       GetMaxPressureReleased(openValves,
                                                                              openValvesString,
                                                                              neighbor,
                                                                              minutesAfterMoving,
                                                                              updatedRelease));
            }

            memo[scenarioCode] = maxPressurePossibleFromHere;
            return maxPressurePossibleFromHere;
        }

        private string GetScenarioCode(string openValvesString,
                                       Valve currentValve,
                                       int minutesRemaining)
        {
            // var openValvesCode = $"O{string.Join("|", openValves.Select(x => $"{x.Item1.Name}-{x.Item2}"))}";

            return $"{openValvesString}C{currentValve.Name}M{minutesRemaining}";
        }

        protected override async Task<string> SolvePartTwoInternal()
        {
            throw new NotImplementedException();
        }

        public override async Task ReadInput()
        {
            var lines = await new StringFileReader().ReadInputFromFile();

            valves = new Dictionary<string, Valve>();

            var neighbors = new Dictionary<string, string[]>();

            var valveRegex = new Regex("Valve (.*) has flow rate=(\\d+); tunnels? leads? to valves? (.*)");

            foreach (var line in lines)
            {
                var valveMatch = valveRegex.Match(line);

                var neighborNames = valveMatch.Groups[3]
                                              .Value.Split(", ");

                var newValve = new Valve
                               {
                                   Name = valveMatch.Groups[1]
                                                    .Value,
                                   FlowRate = int.Parse(valveMatch.Groups[2]
                                                                  .Value),
                                   IsOpen = false,
                                   Neighbors = new Valve[neighborNames.Length]
                               };

                neighbors[newValve.Name] = neighborNames;
                valves[newValve.Name] = newValve;

            }

            foreach (var (name, valve) in valves)
            {
                var i = 0;

                foreach (var neighbor in neighbors[name])
                {
                    valve.Neighbors[i++] = valves[neighbor];
                }
            }
        }

        private Dictionary<string, Valve> valves;
    }
}

[DebuggerDisplay("{Name} ({FlowRate}) ({IsOpen})")]
class Valve
{
    public string Name { get; init; }

    public bool IsOpen { get; set; }

    public int FlowRate { get; init; }

    public Valve[] Neighbors { get; init; }
}