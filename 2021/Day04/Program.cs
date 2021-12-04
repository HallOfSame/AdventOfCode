using Day04;

int.Parse(" 5");

var (sequence, boards) = await new BoardReader().ReadInputFromFile();

var currentDrawnNumbers = new HashSet<int>();

// Add first four, can't possibly have a winner yet
sequence.Take(4).ToList().ForEach(x => currentDrawnNumbers.Add(x));

var foundFirstWinner = false;

for(var i = 4; i < sequence.Count; i++)
{
    var nextNumber = sequence[i];

    currentDrawnNumbers.Add(nextNumber);

    if (!foundFirstWinner)
    {
        // Part 1
        var winningBoard = boards.FirstOrDefault(x => x.IsWinner(currentDrawnNumbers));

        if (winningBoard != null)
        {
            Console.WriteLine($"Part 1 Winner: {winningBoard.GetFinalScore(nextNumber, currentDrawnNumbers)}.");

            Console.WriteLine(winningBoard.ToString());

            foundFirstWinner = true;
        }
    }
    else
    {
        // Keep processing for part 2        

        if (boards.Count == 1 && boards[0].IsWinner(currentDrawnNumbers))
        {
            var losingBoard = boards[0];

            Console.WriteLine($"Part 2 Losing Board: {losingBoard.GetFinalScore(nextNumber, currentDrawnNumbers)}.");

            Console.WriteLine(losingBoard.ToString());

            break;
        }
        else
        {
            // Don't waste time processing boards that already won
            boards.RemoveAll(x => x.IsWinner(currentDrawnNumbers));
        }
    }
}