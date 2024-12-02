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
            return InitialState.Reports.Count(IsSafeReportWithDampener)
                               .ToString();
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

        private static bool IsSafeReport(IList<int> report)
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

        private static bool IsSafeReportWithDampener(List<int> report)
        {
            var size = report.Count;

            if (IsSafeReport(report))
            {
                // It just is valid
                return true;
            }

            // Check variations skipping one index
            for(var skipIndex = 0; skipIndex < size; skipIndex++)
            {
                var updatedReport = new int[report.Count - 1];
                report.CopyTo(0, updatedReport, 0, skipIndex);
                report.CopyTo(skipIndex + 1, updatedReport, skipIndex, size - 1 - skipIndex);

                if (IsSafeReport(updatedReport))
                {
                    return true;
                }
            }

            return false;
        }

        #endregion

        #region Nested type: ExecState

        public record ExecState(List<List<int>> Reports);

        #endregion
    }
}