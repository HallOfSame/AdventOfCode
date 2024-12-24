using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Helpers.Structure;
using InputStorageDatabase;

namespace PuzzleDays
{
    public class Day23 : SingleExecutionPuzzle<Day23.ExecState>
    {
        public record ExecState(List<string> Connections);

        public override PuzzleInfo Info => new(2024, 23, "LAN Party");
        protected override async Task<ExecState> LoadInputState(string puzzleInput, PuzzleInputType inputType)
        {
            return new ExecState(puzzleInput.Trim()
                                     .Split('\n')
                                     .ToList());
        }

        protected override async Task<string> ExecutePuzzlePartOne()
        {
            var computers = new Dictionary<string, HashSet<string>>();

            foreach (var connection in InitialState.Connections)
            {
                var names = connection.Split('-');
                var nameOne = names[0].Trim();
                var nameTwo = names[1].Trim();

                if (!computers.TryGetValue(nameOne, out var nameOneConnections))
                {
                    computers[nameOne] = [nameTwo];
                }
                else
                {
                    nameOneConnections.Add(nameTwo);
                }

                if (!computers.TryGetValue(nameTwo, out var nameTwoConnections))
                {
                    computers[nameTwo] = [nameOne];
                }
                else
                {
                    nameTwoConnections.Add(nameOne);
                }
            }

            var allGroups = new List<HashSet<string>>();
            FindComputerGroupsSizeThree([], computers.Keys.ToHashSet(), [], allGroups, computers);

            return allGroups
                .Count(x => x.Any(pc => pc.StartsWith('t')))
                .ToString();
        }

        protected override async Task<string> ExecutePuzzlePartTwo()
        {
            var computers = new Dictionary<string, HashSet<string>>();

            foreach (var connection in InitialState.Connections)
            {
                var names = connection.Split('-');
                var nameOne = names[0].Trim();
                var nameTwo = names[1].Trim();

                if (!computers.TryGetValue(nameOne, out var nameOneConnections))
                {
                    computers[nameOne] = [nameTwo];
                }
                else
                {
                    nameOneConnections.Add(nameTwo);
                }

                if (!computers.TryGetValue(nameTwo, out var nameTwoConnections))
                {
                    computers[nameTwo] = [nameOne];
                }
                else
                {
                    nameTwoConnections.Add(nameOne);
                }
            }

            var allGroups = new List<HashSet<string>>();
            FindComputerGroups([], computers.Keys.ToHashSet(), [], allGroups, computers);

            return string.Join(",",
                               allGroups
                                   .MaxBy(x => x.Count)
                                   .OrderBy(x => x));
        }

        private void FindComputerGroupsSizeThree(HashSet<string> currentGroup,
                                                 HashSet<string> computersToCheck,
                                                 HashSet<string> alreadyChecked,
                                                 List<HashSet<string>> allGroups,
                                                 Dictionary<string, HashSet<string>> computers)
        {
            // This is a modified version of the Bron–Kerbosch algorithm
            // It finds every length 3 clique
            if (currentGroup.Count == 3)
            {
                allGroups.Add(currentGroup.ToHashSet());
                return;
            }

            foreach (var computer in computersToCheck)
            {
                var checkHere = computers[computer]
                    .Where(computersToCheck.Contains)
                    .ToHashSet();
                var checkedHere = computers[computer]
                    .Where(alreadyChecked.Contains)
                    .ToHashSet();

                FindComputerGroups(new HashSet<string>(currentGroup) { computer },
                                   checkHere,
                                   checkedHere,
                                   allGroups,
                                   computers);
                computersToCheck.Remove(computer);
                alreadyChecked.Add(computer);
            }
        }

        private void FindComputerGroups(HashSet<string> currentGroup, HashSet<string> computersToCheck, HashSet<string> alreadyChecked,
                                        List<HashSet<string>> allGroups, Dictionary<string, HashSet<string>> computers)
        {
            // This is the more standard Bron–Kerbosch algorithm
            // It finds maximal (can't be added to) cliques
            if (computersToCheck.Count == 0 && alreadyChecked.Count == 0)
            {
                allGroups.Add(currentGroup.ToHashSet());
                return;
            }

            foreach (var computer in computersToCheck)
            {
                var checkHere = computers[computer]
                    .Where(computersToCheck.Contains)
                    .ToHashSet();
                var checkedHere = computers[computer]
                    .Where(alreadyChecked.Contains)
                    .ToHashSet();

                FindComputerGroups([..currentGroup, computer], checkHere, checkedHere, allGroups, computers);
                computersToCheck.Remove(computer);
                alreadyChecked.Add(computer);
            }
        }
    }
}
