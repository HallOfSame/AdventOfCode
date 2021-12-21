using Helpers.Extensions;
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
        var (p1Win, p2Win) = DiceGame.CountWins(new GameModel
                                                {
                                                    PlayerOnePos = playerOneStart,
                                                    PlayerTwoPos = playerTwoStart,
                                                    PlayerOneScore = 0,
                                                    PlayerTwoScore = 0,
                                                    IsPlayerOnesTurn = true,
                                                    CurrentTurn = 0
                                                });

        var higherWin = p1Win > p2Win
                            ? p1Win
                            : p2Win;

        return Task.FromResult(higherWin.ToString());
    }

    public override Task ReadInput()
    {
        playerOneStart = 6;
        playerTwoStart = 7;

        return Task.CompletedTask;
    }
}

class DiceGame
{
    private static Dictionary<GameModel, (long playerOne, long playerTwo)> knownWins = new();

    public static (long playerOne, long playerTwo) CountWins(GameModel gameModel)
    {
        if (knownWins.TryGetValue(gameModel,
                                  out var knownWin))
        {
            return knownWin;
        }

        if (gameModel.IsOver)
        {
            if (gameModel.PlayerOneWon)
            {
                knownWins[gameModel] = (1, 0);
                return (1, 0);
            }

            knownWins[gameModel] = (0, 1);
            return (0, 1);
        }

        var winCount = (playerOne: 0L, playerTwo: 0L);

        void AddWins(int rollVal)
        {
            var newModel = gameModel.Roll(rollVal);

            var (p1, p2) = CountWins(newModel);

            winCount.playerOne += p1;
            winCount.playerTwo += p2;
        }

        for (var firstRoll = 1; firstRoll <= 3; firstRoll++)
        {
            for (var secondRoll = 1; secondRoll <= 3; secondRoll++)
            {
                for (var thirdRoll = 1; thirdRoll <= 3; thirdRoll++)
                {
                    AddWins(firstRoll + secondRoll + thirdRoll);
                }
            }
        }

        knownWins[gameModel] = winCount;

        return winCount;
    }
}

class GameModel
{
    public int PlayerOnePos { get; set; }

    public int PlayerTwoPos { get; set; }

    public int PlayerOneScore { get; set; }

    public int PlayerTwoScore { get; set; }

    public bool IsPlayerOnesTurn { get; set; }

    public int CurrentTurn { get; set; }

    public bool IsOver
    {
        get
        {
            return PlayerOneScore >= 21 || PlayerTwoScore >= 21;
        }
    }

    public bool PlayerOneWon
    {
        get
        {
            return PlayerOneScore > PlayerTwoScore;
        }
    }

    public GameModel Roll(int rollValue)
    {
        var clone = new GameModel
                    {
                        PlayerOnePos = this.PlayerOnePos,
                        PlayerTwoPos = this.PlayerTwoPos,
                        PlayerOneScore = this.PlayerOneScore,
                        PlayerTwoScore = this.PlayerTwoScore,
                        IsPlayerOnesTurn = this.IsPlayerOnesTurn,
                        CurrentTurn = this.CurrentTurn + 1
                    };

        var currentPos = clone.IsPlayerOnesTurn
                             ? clone.PlayerOnePos
                             : clone.PlayerTwoPos;

        currentPos = (currentPos + rollValue) % 10;

        if (currentPos == 0)
        {
            currentPos = 10;
        }

        if (clone.IsPlayerOnesTurn)
        {
            clone.PlayerOnePos = currentPos;
            clone.PlayerOneScore += clone.PlayerOnePos;
        }
        else
        {
            clone.PlayerTwoPos = currentPos;
            clone.PlayerTwoScore += clone.PlayerTwoPos;
        }

        clone.IsPlayerOnesTurn = !clone.IsPlayerOnesTurn;

        return clone;
    }

    protected bool Equals(GameModel other)
    {
        return CurrentTurn == other.CurrentTurn && IsPlayerOnesTurn == other.IsPlayerOnesTurn && PlayerOnePos == other.PlayerOnePos && PlayerTwoPos == other.PlayerTwoPos && PlayerOneScore == other.PlayerOneScore && PlayerTwoScore == other.PlayerTwoScore;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null,
                            obj))
        {
            return false;
        }

        if (ReferenceEquals(this,
                            obj))
        {
            return true;
        }

        if (obj.GetType() != this.GetType())
        {
            return false;
        }

        return Equals((GameModel)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(PlayerOnePos,
                                PlayerTwoPos,
                                PlayerOneScore,
                                PlayerTwoScore,
                                IsPlayerOnesTurn,
                                CurrentTurn);
    }
}