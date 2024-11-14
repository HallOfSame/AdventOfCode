using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Helpers.FileReaders;
using Helpers.Structure;

namespace PuzzleDays
{
    public class Day25 : ProblemBase
    {
        protected override async Task<string> SolvePartOneInternal()
        {
            var written = new HashSet<string>();

            foreach (var c in connections)
            {
                Console.WriteLine($"{c.Key} -> {string.Join(", ", c.Value)}");
            }

            return "wait";
        }

        private int GetSubGraphCount(string current, HashSet<string> visited)
        {
            var neighbors = connections[current];

            foreach (var neighbor in neighbors)
            {
                if (visited.Contains(neighbor))
                {
                    continue;
                }

                visited.Add(neighbor);

                GetSubGraphCount(neighbor, visited);
            }

            return visited.Count;
        }

        protected override async Task<string> SolvePartTwoInternal()
        {
            throw new NotImplementedException();
        }

        private readonly Dictionary<string, HashSet<string>> connections = new();

        public override async Task ReadInput()
        {
            var strings = await new StringFileReader().ReadInputFromFile();

            foreach (var s in strings)
            {
                var split = s.Split(": ");

                var name = split[0];

                var connected = split[1]
                    .Split(" ")
                    .ToHashSet();

                if (connections.TryGetValue(name, out var existingHashSet))
                {
                    existingHashSet.UnionWith(connected);
                }
                else
                {
                    connections[name] = connected;
                }

                foreach (var c in connected)
                {
                    if (!connections.TryGetValue(c, out var cHash))
                    {
                        connections[c] = new HashSet<string>
                        {
                            name
                        };
                    }
                    else
                    {
                        cHash.Add(name);
                    }
                }
            }
        }
    }
}
