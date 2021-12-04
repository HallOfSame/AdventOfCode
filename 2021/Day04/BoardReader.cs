namespace Day04
{
    internal class BoardReader
    {
        public async Task<(List<int> numberSequence, List<BingoBoard> boards)> ReadInputFromFile()
        {
            await using var file = File.OpenRead("PuzzleInput.txt");
            using var reader = new StreamReader(file);

            var firstLine = true;

            List<int> numberSequence = null;
            var boards = new List<BingoBoard>();

            const int BoardWidth = 5;

            var boardLineCounter = 0;

            var currentBoard = new int[BoardWidth, BoardWidth];

            while (!reader.EndOfStream)
            {
                var nextLine = await reader.ReadLineAsync();

                if (string.IsNullOrEmpty(nextLine))
                {
                    continue;
                }

                if (firstLine)
                {
                    numberSequence = nextLine.Split(',').Select(int.Parse).ToList();
                    firstLine = false;
                    continue;
                }

                var nextBoardLine = nextLine.Split(' ').Where(x => !string.IsNullOrWhiteSpace(x)).Select(int.Parse).ToArray();

                for (var i = 0; i < nextBoardLine.Length; i++)
                {
                    currentBoard[boardLineCounter, i] = nextBoardLine[i];
                }

                boardLineCounter++;

                if (boardLineCounter == BoardWidth)
                {
                    boards.Add(new BingoBoard((int[,])currentBoard.Clone()));
                    boardLineCounter = 0;
                }
            }

            return (numberSequence, boards);
        }
    }
}
