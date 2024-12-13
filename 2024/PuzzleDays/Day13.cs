using System.Text.RegularExpressions;
using Helpers.Maps;
using Helpers.Structure;

namespace PuzzleDays
{
    public class Day13 : SingleExecutionPuzzle<Day13.ExecState>
    {
        public record ExecState(List<MachineSettings> Machines);

        public override PuzzleInfo Info => new(2024, 13, "Claw Contraption");
        protected override async Task<ExecState> LoadInputState(string puzzleInput)
        {
            puzzleInput = puzzleInput.Trim();

            var lines = puzzleInput.Split('\n');
            var machines = new List<MachineSettings>();
            var buttonRegex = new Regex(@"X\+(\d+), Y\+(\d+)");
            var prizeRegex = new Regex(@"X=(\d+), Y=(\d+)"); 

            foreach (var line in lines.Chunk(4))
            {
                /*
                 *Button A: X+94, Y+34
                   Button B: X+22, Y+67
                   Prize: X=8400, Y=5400
                 */
                var aButton = buttonRegex.Match(line[0]
                                                    .Replace("Button A: ", string.Empty))
                    .Groups;
                var bButton = buttonRegex.Match(line[1]
                                                    .Replace("Button A: ", string.Empty))
                    .Groups;
                var prize = prizeRegex.Match(line[2]
                                                 .Replace("Prize: ", string.Empty))
                    .Groups;

                var aCoord = new Coordinate(int.Parse(aButton[1].Value), int.Parse(aButton[2].Value));
                var bCoord = new Coordinate(int.Parse(bButton[1].Value), int.Parse(bButton[2].Value));
                var prizeCoord = new Coordinate(int.Parse(prize[1].Value), int.Parse(prize[2].Value));
                machines.Add(new MachineSettings(aCoord, bCoord, prizeCoord));
            }

            return new ExecState(machines);
        }

        protected override async Task<string> ExecutePuzzlePartOne()
        {
            var result = 0;

            foreach (var machine in InitialState.Machines)
            {
                invalidBranches.Clear();
                result += FindCostToWin(machine, new Coordinate(0, 0), (0, 0));
            }

            return result.ToString();
        }

        protected override async Task<string> ExecutePuzzlePartTwo()
        {
            throw new NotImplementedException();
        }

        private readonly HashSet<Coordinate> invalidBranches = [];

        private int FindCostToWin(MachineSettings machine, Coordinate currentLocation, (int aPresses, int bPresses) pressCounter)
        {
            if (invalidBranches.Contains(currentLocation))
            {
                return 0;
            }

            if (pressCounter.aPresses > 100 || pressCounter.bPresses > 100)
            {
                invalidBranches.Add(currentLocation);
                return 0;
            }

            if (currentLocation.X == machine.PrizeLocation.X && currentLocation.Y == machine.PrizeLocation.Y)
            {
                return (pressCounter.aPresses * 3) + pressCounter.bPresses;
            }

            if (currentLocation.X > machine.PrizeLocation.X || currentLocation.Y > machine.PrizeLocation.Y)
            {
                invalidBranches.Add(currentLocation);
                return 0;
            }

            var locationAfterPressingB = new Coordinate(currentLocation.X + machine.BButtonMove.X,
                                                        currentLocation.Y + machine.BButtonMove.Y);
            var updatedCounter = (pressCounter.aPresses, pressCounter.bPresses + 1);

            var resultPressingB = FindCostToWin(machine, locationAfterPressingB, updatedCounter);

            if (resultPressingB != 0)
            {
                return resultPressingB;
            }

            var locationAfterPressingA = new Coordinate(currentLocation.X + machine.AButtonMove.X,
                                                        currentLocation.Y + machine.AButtonMove.Y);
            updatedCounter = (pressCounter.aPresses + 1, pressCounter.bPresses);

            var resultPressingA = FindCostToWin(machine, locationAfterPressingA, updatedCounter);

            if (resultPressingA != 0)
            {
                return resultPressingA;
            }

            invalidBranches.Add(currentLocation);
            return 0;
        }

        public record MachineSettings(Coordinate AButtonMove, Coordinate BButtonMove, Coordinate PrizeLocation);
    }
}
