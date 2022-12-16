using System.Diagnostics;
using System.Text.RegularExpressions;

using Helpers.FileReaders;
using Helpers.Heaps;
using Helpers.Structure;

namespace PuzzleDays
{
    public class Day16 : ProblemBase
    {
        protected override async Task<string> SolvePartOneInternal()
        {
            valvesToOpen = valves.Values.Count(x => x.FlowRate > 0);

            // Dijkstra to find the length between valves we could actually open
            foreach (var (_, valve) in valves)
            {
                var distance = valves.Values.ToDictionary(x => x.Name,
                                                          _ => int.MaxValue);

                distance[valve.Name] = 0;

                var queue = MinHeap.CreateMinHeap<string>();

                foreach (var (value, prio) in distance)
                {
                    queue.Enqueue(value,
                                  prio);
                }
                

                while (queue.Any())
                {
                    var current = valves[queue.Dequeue()];

                    foreach (var neighbor in current.Neighbors)
                    {
                        var alternateDistance = distance[current.Name] + 1;

                        if (alternateDistance < distance[neighbor.Name])
                        {
                            distance[neighbor.Name] = alternateDistance;
                            queue.UpdatePriority(neighbor.Name,
                                                 alternateDistance);
                        }
                    }
                }

                // Save the distance from this node to all the other valves that have a flow rate
                valve.OptimizedDistanceMap = distance.Where(x => valves[x.Key]
                                                                     .FlowRate
                                                                 > 0
                                                                 && x.Value != 0)
                                                     .ToDictionary(x => x.Key,
                                                                   x => x.Value);
            }

            var result = GetMaxPressureReleased(new List<Valve>(),
                                                string.Empty,
                                                valves["AA"],
                                                30,
                                                0);

            return result.ToString();
        }

        private Dictionary<string, int> memo = new();

        private int valvesToOpen;

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

            int AddPressureReleased(int minutes)
            {
                return currentPressureReleased + (openValves.Sum(x => x.FlowRate) * minutes);
            }

            var maxPressurePossibleFromHere = 0;

            // Can skip a lot of empty steps once we get all valves open
            if (openValves.Count == valvesToOpen)
            {
                var bestPressureFromHere = openValves.Sum(x => x.FlowRate) * minutesRemaining;

                memo[scenarioCode] = currentPressureReleased + bestPressureFromHere;
                return bestPressureFromHere;
            }

            // Options:
            // Open current valve if not open
            // Skip if it wouldn't release any more pressure
            if (!currentValve.IsOpen && currentValve.FlowRate > 0)
            {
                var minutesAfterOpening = minutesRemaining - 1;
                var updatedRelease = AddPressureReleased(1);
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

            // Move to neighbor without opening, doing calculations here to go directly to a valve that we could open
            foreach (var (neighborName, distance) in currentValve.OptimizedDistanceMap)
            {
                var neighbor = valves[neighborName];

                // No reason to move back to an open valve
                if (openValves.Contains(neighbor))
                {
                    continue;
                }

                var minutesAfterMoving = minutesRemaining - distance;
                var updatedRelease = AddPressureReleased(distance);

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

    public Dictionary<string, int> OptimizedDistanceMap { get; set; }
}