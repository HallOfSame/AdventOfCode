using System.Diagnostics;
using System.Text.RegularExpressions;

using Helpers.Extensions;
using Helpers.FileReaders;
using Helpers.Heaps;
using Helpers.Structure;

namespace PuzzleDays
{
    public class Day16 : ProblemBase
    {
        protected override async Task<string> SolvePartOneInternal()
        {
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

                if (valve.Name == "AA")
                {
                    distanceFromStart = distance.ToDictionary(x => x.Key,
                                                              x => x.Value);
                }

                // Save the distance from this node to all the other valves that have a flow rate
                valve.OptimizedDistanceMap = distance.Where(x => valves[x.Key]
                                                                     .FlowRate
                                                                 > 0
                                                                 && x.Value != 0)
                                                     .ToDictionary(x => x.Key,
                                                                   x => x.Value);
            }

            nonZeroValves = valves.Values.Where(x => x.FlowRate > 0)
                                  .ToList();

            // X == closed
            // O == open
            var valveStates = new char[valves.Count];

            foreach (var valve in valves.Values)
            {
                valveStates[valve.Index] = valve.FlowRate == 0
                                               ? 'O'
                                               : 'X';
            }

            maxFlow = valves.Values.Sum(x => x.FlowRate);

            var result = GetMaxPressureReleased(valveStates,
                                                valves["AA"],
                                                30,
                                                0,
                                                0);

            return result.ToString();
        }

        private Dictionary<string, int> distanceFromStart;

        private int maxFlow;

        private List<Valve> nonZeroValves;

        private int GetMaxPressureReleased(char[] valveStates,
                                           Valve currentValve,
                                           int minutesRemaining,
                                           int currentFlow,
                                           int currentPressureReleased)
        {
            // Return if out of time
            if (minutesRemaining == 0)
            {
                return currentPressureReleased;
            }

            int AddPressureReleased(int minutes)
            {
                return currentPressureReleased + (currentFlow * minutes);
            }

            var maxPressurePossibleFromHere = 0;

            // Can skip a lot of empty steps once we get all valves open
            if (currentFlow == maxFlow)
            {
                var bestPressureFromHere = AddPressureReleased(minutesRemaining);

                return bestPressureFromHere;
            }

            // Move to neighbor without opening, doing calculations here to go directly to a valve that we could open
            foreach (var (neighborName, distance) in currentValve.OptimizedDistanceMap)
            {
                var neighbor = valves[neighborName];

                // No reason to move to an open valve
                if (valveStates[neighbor.Index] == 'O')
                {
                    continue;
                }

                var minutesAfterMoveAndOpen = minutesRemaining - (distance + 1);

                // Invalid to move to negative time remaining
                if (minutesAfterMoveAndOpen < 0)
                {
                    continue;
                }

                // Add pressure for moving and while opening
                var updatedRelease = AddPressureReleased(distance + 1);

                // Then open and add that flow
                valveStates[neighbor.Index] = 'O';
                currentFlow += neighbor.FlowRate;

                maxPressurePossibleFromHere = Math.Max(maxPressurePossibleFromHere,
                                                       GetMaxPressureReleased(valveStates,
                                                                              neighbor,
                                                                              minutesAfterMoveAndOpen,
                                                                              currentFlow,
                                                                              updatedRelease));

                // Undo our changes
                valveStates[neighbor.Index] = 'X';
                currentFlow -= neighbor.FlowRate;
            }

            return maxPressurePossibleFromHere;
        }

        private IEnumerable<List<Valve>> GenerateOpeningPaths(Valve currentPosition, List<Valve> openValves, int timeRemaining)
        {
            var options = new List<List<Valve>>();

            foreach (var nextValve in nonZeroValves)
            {
                if (currentPosition == nextValve
                    || openValves.Contains(nextValve))
                {
                    continue;
                }

                var timeRequired = currentPosition.OptimizedDistanceMap[nextValve.Name] + 1;

                if (timeRequired < timeRemaining)
                {
                    openValves.Add(nextValve);

                    options.Add(openValves.ToList());

                    options.AddRange(GenerateOpeningPaths(nextValve,
                                                          openValves,
                                                          timeRemaining - timeRequired));
                    openValves.Remove(nextValve);
                }
            }

            return options;
        }

        private int GetPressureReleasedForOrder(List<Valve> openingOrder, int timeLeft)
        {
            var distanceMap = distanceFromStart;

            var score = 0;

            foreach (var valve in openingOrder)
            {
                timeLeft -= (distanceMap[valve.Name] + 1);
                score += valve.FlowRate * timeLeft;
                distanceMap = valve.OptimizedDistanceMap;
            }

            return score;
        }

        protected override async Task<string> SolvePartTwoInternal()
        {
            // Get every permutation of orders a single actor could open the valves in, starting at AA with 26 min left
            // From opening none to opening all (if there is enough time)
            var openOrders = GenerateOpeningPaths(valves["AA"],
                                                  new List<Valve>(),
                                                  26)
                .ToList();

            // For each set of valves that we open with a given order (i.e. opening 4 of X)
            var bestOrderForSubsetOfValves = new Dictionary<string, (HashSet<Valve>, int)>();

            // Find and store the best order to open in for that set
            foreach (var order in openOrders)
            {
                var hash = order.ToHashSet();

                var score = GetPressureReleasedForOrder(order,
                                                        26);

                var valves = string.Join(",",
                                         order.Select(x => x.Name).OrderBy(x => x));

                bestOrderForSubsetOfValves[valves] = (hash, Math.Max(bestOrderForSubsetOfValves.GetValueOrDefault(valves,
                                                                                                                  (hash, 0))
                                                                                               .Item2,
                                                                     score));
            }

            var result = 0;

            var openingOptions = bestOrderForSubsetOfValves.Values.OrderByDescending(x => x.Item2).ToList();

            // Iterate through options of valves you open
            for (var yourIndex = 0; yourIndex < openingOptions.Count - 1; yourIndex++)
            {
                var (yourValves, yourScore) = openingOptions[yourIndex];

                if (yourScore + openingOptions[yourIndex + 1].Item2 < result)
                {
                    // Fast exit if we can't beat our best score so far with any remaining options
                    continue;
                }

                // And ones the elephant opens
                for (var elephantIndex = yourIndex + 1; elephantIndex < openingOptions.Count; elephantIndex++)
                {
                    var (elephantValves, elephantScore) = openingOptions[elephantIndex];

                    // If the two sets have no overlap this is a possible solution
                    if (!elephantValves.Intersect(yourValves).Any())
                    {
                        // Might not actually need math.max here since we have the other guard above
                        result = Math.Max(result,
                                          yourScore + elephantScore);
                    }
                }
            }

            return result.ToString();
        }

        public override async Task ReadInput()
        {
            var lines = await new StringFileReader().ReadInputFromFile();

            valves = new Dictionary<string, Valve>();

            var neighbors = new Dictionary<string, string[]>();

            var valveRegex = new Regex("Valve (.*) has flow rate=(\\d+); tunnels? leads? to valves? (.*)");

            var valveIndex = 0;

            foreach (var line in lines)
            {
                var valveMatch = valveRegex.Match(line);

                var neighborNames = valveMatch.Groups[3]
                                              .Value.Split(", ");

                var newValve = new Valve
                               {
                                   Index = valveIndex++,
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

class Target
{
    public Valve Valve { get; set; }

    public int Distance { get; set; }
}

[DebuggerDisplay("{Name} ({FlowRate}) ({IsOpen})")]
class Valve
{
    public int Index {get; init; }

    public string Name { get; init; }

    public bool IsOpen { get; set; }

    public int FlowRate { get; init; }

    public Valve[] Neighbors { get; init; }

    public Dictionary<string, int> OptimizedDistanceMap { get; set; }
}