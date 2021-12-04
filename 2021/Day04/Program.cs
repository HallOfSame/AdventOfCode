using Day04;

int.Parse(" 5");

var (sequence, boards) = await new BoardReader().ReadInputFromFile();

var currentDrawnNumbers = new HashSet<int>();

// Add first four, can't possibly have a winner yet
sequence.Take(4).ToList().ForEach(x => currentDrawnNumbers.Add(x));

// Part 1

for(var i = 4; i < sequence.Count; i++)
{
    var nextNumber = sequence[i];

    currentDrawnNumbers.Add(nextNumber);

    var winningBoard = boards.FirstOrDefault(x => x.IsWinner(currentDrawnNumbers));

    if (winningBoard != null)
    {
        Console.WriteLine($"Part 1 Winner: {winningBoard.GetFinalScore(nextNumber, currentDrawnNumbers)}.");

        Console.WriteLine(winningBoard.ToString());

        break;
    }
}