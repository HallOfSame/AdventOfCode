using Helpers;
using Helpers.Structure;

namespace PuzzleDays
{
    public class Day02Problem : ProblemBase
    {
        protected override async Task<string> SolvePartOneInternal()
        {
            return rounds.Sum(x => x.CalculateRoundScore())
                         .ToString();
        }

        protected override async Task<string> SolvePartTwoInternal()
        {
            return rounds.Sum(x => x.CalculateRoundScorePartTwo())
                         .ToString();
        }

        public override async Task ReadInput()
        {
            rounds = await new RoundReader().ReadInputFromFile();
        }

        private List<Round> rounds;
    }

    class Round
    {
        public string OpponentPlay { get; }

        public string SecondColumn { get; }

        public Round(string opponentPlay,
                     string secondColumn)
        {
            OpponentPlay = opponentPlay;
            SecondColumn = secondColumn;
        }

        private int GetScoreForPlay(string play)
        {
            return play switch
            {
                "X" => 1,
                "Y" => 2,
                "Z" => 3,
                _ => throw new ArgumentOutOfRangeException(nameof(SecondColumn)),
            };
        }

        public int CalculateRoundScore()
        {
            var scoreForYourPlay = GetScoreForPlay(SecondColumn);

            var roundId = $"{OpponentPlay}{SecondColumn}";

            var resultScore = roundId switch
            {
                "AX" or "BY" or "CZ" => 3, // Draw
                "AY" or "BZ" or "CX" => 6, // Win
                _ => 0 // Loss
            };

            return scoreForYourPlay + resultScore;
        }

        public int CalculateRoundScorePartTwo()
        {
            // X = lose
            // Y = draw
            // Z = win
            var scoreForOutcome = SecondColumn switch
            {
                "X" => 0,
                "Y" => 3,
                "Z" => 6,
                _ => throw new ArgumentOutOfRangeException(nameof(SecondColumn)),
            };

            string GetHandToPlay()
            {
                // Draw
                if (SecondColumn == "Y")
                {
                    return OpponentPlay switch
                    {
                        "A" => "X",
                        "B" => "Y",
                        "C" => "Z",
                        _ => throw new ArgumentOutOfRangeException(nameof(OpponentPlay))
                    };
                }

                // Lose
                if (SecondColumn == "X")
                {
                    return OpponentPlay switch
                    {
                        "A" => "Z",
                        "B" => "X",
                        "C" => "Y",
                        _ => throw new ArgumentOutOfRangeException(nameof(OpponentPlay))
                    };
                }

                // Win
                if (SecondColumn == "Z")
                {
                    return OpponentPlay switch
                    {
                        "A" => "Y",
                        "B" => "Z",
                        "C" => "X",
                        _ => throw new ArgumentOutOfRangeException(nameof(OpponentPlay))
                    };
                }

                throw new ArgumentOutOfRangeException(nameof(SecondColumn));
            }

            var scoreForYourPlay = GetScoreForPlay(GetHandToPlay());

            return scoreForYourPlay + scoreForOutcome;
        }
    }

    class RoundReader : FileReader<Round>
    {
        protected override Round ProcessLineOfFile(string line)
        {
            var split = line.Split(' ');

            var round = new Round(split[0],
                                  split[1]);

            return round;
        }
    }
}