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

        private int maxFlow;

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