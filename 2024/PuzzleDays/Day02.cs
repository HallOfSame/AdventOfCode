using Helpers.Structure;

namespace PuzzleDays
{
    public class Day02 : SingleExecutionPuzzle<Day02.ExecState>
    {
        #region Instance Properties

        public override PuzzleInfo Info =>
            new(2024,
                02,
                "Red-Nosed Reports");

        #endregion

        #region Instance Methods

        protected override async Task<string> ExecutePuzzlePartOne()
        {
            return InitialState.Reports.Count(IsSafeReport)
                               .ToString();
        }

        protected override async Task<string> ExecutePuzzlePartTwo()
        {
            throw new NotImplementedException();
        }

        protected override async Task<ExecState> LoadInputState(string puzzleInput)
        {
            var lines = puzzleInput.Trim().Split('\n');

            var allReports = lines.Select(line => line.Split(" ")
                                                      .Select(int.Parse)
                                                      .ToList())
                                  .ToList();

            return new ExecState(allReports);
        }

        private static bool IsSafeReport(List<int> report)
        {
            const int MaxChange = 3;
            const int MinChange = 1;

            var decreasing = report[0] > report[1];

            for (var i = 0; i < report.Count - 1; i++)
            {
                var first = report[i];
                var second = report[i + 1];
                var check = decreasing ? first - second : second - first;

                if (check is < MinChange or > MaxChange)
                {
                    return false;
                }
            }

            return true;
        }

        #endregion

        #region Nested type: ExecState

        public record ExecState(List<List<int>> Reports);

        #endregion
    }
}