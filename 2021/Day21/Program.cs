using Helpers.Structure;

var solver = new Solver(new Day21Problem());

await solver.Solve();

class Day21Problem : ProblemBase
{
    private int playerOneStart;

    private int playerTwoStart;

    protected override Task<string> SolvePartOneInternal()
    {
        var currentDiceValue = 1;

        var numberOfRolls = 0;

        int GetDiceRoll()
        {
            var nextRoll = currentDiceValue;

            currentDiceValue++;

            if (currentDiceValue > 100)
            {
                currentDiceValue = 1;
            }

            numberOfRolls++;

            return nextRoll;
        }

        int GetForwardMove()
        {
            var movement = GetDiceRoll() + GetDiceRoll() + GetDiceRoll();

            return movement;
        }

        void PlayTurn(ref int score,
                      ref int currentPosition)
        {
            var move = GetForwardMove();

            var newPosition = (currentPosition + move) % 10;

            if (newPosition == 0)
            {
                newPosition = 10;
            }

            currentPosition = newPosition;

            score += newPosition;
        }

        var playerOneScore = 0;
        var playerTwoScore = 0;

        var playerOnePos = playerOneStart;
        var playerTwoPos = playerTwoStart;

        const int WinningScore = 1000;

        while (playerTwoScore < WinningScore)
        {
            PlayTurn(ref playerOneScore,
                     ref playerOnePos);

            if (playerOneScore >= WinningScore)
            {
                break;
            }

            PlayTurn(ref playerTwoScore,
                     ref playerTwoPos);
        }

        var losingScore = playerOneScore >= WinningScore
                              ? playerTwoScore
                              : playerOneScore;

        return Task.FromResult((losingScore * numberOfRolls).ToString());
    }

    protected override Task<string> SolvePartTwoInternal()
    {
        throw new NotImplementedException();
    }

    public override Task ReadInput()
    {
        playerOneStart = 6;
        playerTwoStart = 7;

        return Task.CompletedTask;
    }
}