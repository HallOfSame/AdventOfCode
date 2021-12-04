using System.Text;

namespace Day04
{
    internal class BingoBoard
    {
        private int[,] board;

        public BingoBoard(int[,] board)
        {
            this.board = board;
        }

        public int[] GetColumn(int columnNumber)
        {
            return Enumerable.Range(0, board.GetLength(0))
                    .Select(x => board[x, columnNumber])
                    .ToArray();
        }

        public int[] GetRow(int rowNumber)
        {
            return Enumerable.Range(0, board.GetLength(1))
                    .Select(x => board[rowNumber, x])
                    .ToArray();
        }

        public bool IsWinner(HashSet<int> currentDrawnNumbers)
        {
            var winningRowsOrColumns = Enumerable.Range(0, board.GetLength(0)).SelectMany(x => new int[][]
            {
                GetRow(x),
                GetColumn(x)
            }).Where(arr => arr.All(val => currentDrawnNumbers.Contains(val))).ToList();

            var isWinner = winningRowsOrColumns.Any();

            return isWinner;
        }

        public int GetFinalScore(int finalNumberCalled, HashSet<int> calledNumbers)
        {
            var allValues = board.Cast<int>().ToList();

            var unmarkedNumberSum = allValues.Where(x => !calledNumbers.Contains(x)).Sum();

            return unmarkedNumberSum * finalNumberCalled;
        }

        public override string ToString()
        {
            var rowSize = board.GetLength(1);

            var stringBuilder = new StringBuilder();

            void AppendBorder()
            {
                stringBuilder.AppendLine(new string(Enumerable.Repeat('-', (rowSize * 2) + (rowSize - 1)).ToArray()));
            }

            AppendBorder();

            for(var i = 0; i < rowSize; i++)
            {
                stringBuilder.AppendLine(string.Join(' ', GetRow(i).Select(x => x.ToString().PadLeft(2, ' '))));
            }

            AppendBorder();

            return stringBuilder.ToString();
        }
    }
}
