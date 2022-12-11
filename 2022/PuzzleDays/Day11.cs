using Helpers.FileReaders;
using Helpers.Structure;

namespace PuzzleDays
{
    public class Day11 : ProblemBase
    {
        protected override async Task<string> SolvePartOneInternal()
        {
            keepAway.Play(20, true);

            return keepAway.Monkeys.Select(x => x.InspectionCounter)
                           .OrderByDescending(x => x)
                           .Take(2)
                           .Aggregate(1m, (x, y) => x * y)
                           .ToString();
        }

        protected override async Task<string> SolvePartTwoInternal()
        {
            partTwo.Play(10_000,
                          false);

            var answer = partTwo.Monkeys.Select(x => x.InspectionCounter)
                                .OrderByDescending(x => x)
                                .Take(2)
                                .Aggregate(1m,
                                           (x,
                                            y) => x * y);

            return answer.ToString();
        }

        public override async Task ReadInput()
        {
            var lines = await new StringFileReader().ReadInputFromFile();

            var monkeys = new List<Monkey>();
            var p2Monkeys = new List<Monkey>();

            foreach (var monkeyLines in lines.Chunk(7))
            {
                var startingItemsLine = monkeyLines[1];
                var operationLine = monkeyLines[2];
                var testLine = monkeyLines[3];
                var trueIdxLine = monkeyLines[4];
                var falseIdxLine = monkeyLines[5];

                var startingItems = startingItemsLine.Split(":")[1]
                                                     .Split(",")
                                                     .Where(x => !string.IsNullOrWhiteSpace(x))
                                                     .Select(x => decimal.Parse(x.Trim()))
                                                     .ToList();

                var operationSplit = operationLine.Split(":")[1]
                                                  .Trim()
                                                  .Split(" ");

                var operationType = operationSplit[3] == "+" ? WorryActionType.Add : operationSplit[4] == "old" ? WorryActionType.Square : WorryActionType.Multiply;

                var operationValue = operationType != WorryActionType.Square
                                         ? int.Parse(operationSplit[4])
                                         : 0;

                var worryAction = new WorryAction
                                  {
                                      ActionType = operationType,
                                      Value = operationValue
                                  };

                var test = new DecisionTest
                           {
                               DivisibleBy = int.Parse(testLine.Trim().Split(" ")[3]),
                               TrueMonkeyIndex = int.Parse(trueIdxLine.Trim().Split(" ")[5]),
                               FalseMonkeyIndex = int.Parse(falseIdxLine.Trim().Split(" ")[5]),
                           };

                monkeys.Add(new Monkey(startingItems,
                                       worryAction,
                                       test));
                p2Monkeys.Add(new Monkey(startingItems,
                                         worryAction,
                                         test));
            }

            var productOfTests = monkeys.Select(x => x.DecisionTest.DivisibleBy)
                                        .Aggregate((x,
                                                    y) => x * y);

            keepAway = new KeepAway(monkeys,
                                    productOfTests);
            partTwo = new KeepAway(p2Monkeys,
                                   productOfTests);
        }

        private KeepAway keepAway;

        private KeepAway partTwo;
    }
}

class KeepAway
{
    private readonly int productOfTests;

    public Monkey[] Monkeys { get; }

    public KeepAway(List<Monkey> monkeys,
                    int productOfTests)
    {
        this.productOfTests = productOfTests;
        Monkeys = monkeys.ToArray();
    }

    public void Play(int roundCount,
                     bool useRelief)
    {
        for (var i = 0; i < roundCount; i++)
        {
            PlayRound(useRelief);
        }
    }

    private void PlayRound(bool useRelief)
    {
        foreach (var monkey in Monkeys)
        {
            monkey.PlayTurn(Monkeys,
                            useRelief,
                            productOfTests);
        }
    }
}

class Monkey
{
    public List<decimal> HeldItems { get; }

    public WorryAction WorryAction { get; }

    public DecisionTest DecisionTest { get; }

    public decimal InspectionCounter { get; private set; }

    public Monkey(List<decimal> startingItems,
                  WorryAction worryAction,
                  DecisionTest decisionTest)
    {
        HeldItems = startingItems.ToList();
        InspectionCounter = 0;
        WorryAction = worryAction;
        DecisionTest = decisionTest;
    }

    public void PlayTurn(Monkey[] allMonkeys,
                         bool useRelief,
                         int productOfTests)
    {
        foreach (var item in HeldItems)
        {
            // Inspect
            InspectionCounter++;

            var newItemValue = WorryAction.AlterValue(item) % productOfTests;

            // Relief
            if (useRelief)
            {
                newItemValue = Math.Floor(newItemValue / 3m);
            }

            // Test
            var targetIndex = newItemValue % DecisionTest.DivisibleBy == 0
                                  ? DecisionTest.TrueMonkeyIndex
                                  : DecisionTest.FalseMonkeyIndex;

            // Transfer
            allMonkeys[targetIndex]
                .HeldItems.Add(newItemValue);
        }

        // Empty the list of items since we threw them all
        HeldItems.Clear();
    }
}

class WorryAction
{
    public WorryActionType ActionType { get; set; }

    public int Value { get; set; }

    public decimal AlterValue(decimal startingValue)
    {
        return ActionType switch
        {
            WorryActionType.Add => startingValue + Value,
            WorryActionType.Multiply => startingValue * Value,
            WorryActionType.Square => startingValue * startingValue,
            _ => throw new Exception("Invalid action type")
        };
    }
}

enum WorryActionType
{
    Add,
    Multiply,
    Square
}

class DecisionTest
{
    public int DivisibleBy { get; set; }

    public int TrueMonkeyIndex { get; set; }

    public int FalseMonkeyIndex { get; set; }
}