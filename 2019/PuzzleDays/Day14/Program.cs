using Helpers;

var reactions = await new ReactionReader().ReadInputFromFile();

// Part 1

var oreRequired = new ReactionVessel().GetOreRequiredForSingleOutput("FUEL",
                                                      reactions);

Console.WriteLine($"Ore required for 1 fuel: {oreRequired}.");

class ReactionVessel
{
    public const string Ore = "ORE";

    private Dictionary<string, long> stockpile = new()
                                                {
                                                    {
                                                        Ore, long.MaxValue
                                                    }
                                                };

    private long GenerateOutput(string symbol,
                                long amount,
                                List<Intermediate> reactions)
    {
        var inputsNeeded = reactions.First(x => x.Output.Symbol == symbol)
                                    .GetScaledInput(amount,
                                                    out var totalOutput);

        var leftoverAmount = totalOutput - amount;

        var oreUsedThisTransaction = 0L;

        foreach (var input in inputsNeeded)
        {
            var amountNeeded = input.Amount;

            if (input.Symbol == Ore)
            {
                Console.WriteLine($"Using {amountNeeded} ORE to generate {amount} of {symbol}.");

                oreUsedThisTransaction += amountNeeded;
                continue;
            }

            if (stockpile.TryGetValue(input.Symbol,
                                      out var existingAmount))
            {
                if (amountNeeded < existingAmount)
                {
                    Console.WriteLine($"Stockpile covered required {amountNeeded} for {symbol}.");

                    amountNeeded = 0;
                    stockpile[input.Symbol] -= input.Amount;
                }
                else
                {
                    Console.WriteLine($"Emptied stockpile of {input.Symbol} while generating {symbol}.");

                    stockpile[input.Symbol] = 0;
                    amountNeeded -= existingAmount;
                }
            }

            if (amountNeeded > 0)
            {
                Console.WriteLine($"Still need {amountNeeded} of {input.Symbol} as part of producing {amount} {symbol}.");

                oreUsedThisTransaction += GenerateOutput(input.Symbol,
                                                         amountNeeded,
                                                         reactions);
            }
        }

        if (leftoverAmount > 0)
        {
            if (stockpile.ContainsKey(symbol))
            {
                Console.WriteLine($"Adding an additional {leftoverAmount} of {symbol} to the stockpile.");

                stockpile[symbol] += leftoverAmount;
            }
            else
            {
                Console.WriteLine($"Adding an new quantity {leftoverAmount} of {symbol} to the stockpile.");

                stockpile[symbol] = leftoverAmount;
            }
        }

        return oreUsedThisTransaction;
    }

    public long GetOreRequiredForSingleOutput(string outputSymbol,
                                              List<Intermediate> reactions)
    {
        return GenerateOutput(outputSymbol,
                              1,
                              reactions);
    }
}

class Intermediate
{
    public List<Ingredient> Input { get; set; }

    public Ingredient Output { get; set; }

    public List<Ingredient> GetScaledInput(long outputAmount,
                                           out long totalOutput)
    {
        var scaleFactor = (long)Math.Ceiling(outputAmount / (decimal)Output.Amount);

        totalOutput = Output.Amount * scaleFactor;

        return Input.Select(x => new Ingredient
                                 {
                                     Symbol = x.Symbol,
                                     Amount = x.Amount * scaleFactor
                                 })
                    .ToList();
    }
}

class Ingredient
{
    public string Symbol { get; set; }

    public long Amount { get; set; }

    public override string ToString()
    {
        return $"{Symbol} - ({Amount})";
    }
}

class ReactionReader : FileReader<Intermediate>
{
    protected override Intermediate ProcessLineOfFile(string line)
    {
        var split = line.Split(" => ");

        var inputs = split[0]
                     .Split(", ")
                     .Select(x =>
                             {
                                 var symVal = x.Split(" ");

                                 return new Ingredient
                                        {
                                            Symbol = symVal[1],
                                            Amount = long.Parse(symVal[0])
                                        };
                             })
                     .ToList();

        var output = split[1]
                     .Split(", ")
                     .Select(x =>
                             {
                                 var symVal = x.Split(" ");

                                 return (symVal[1], long.Parse(symVal[0]));
                             })
                     .First();

        return new Intermediate
               {
                   Input = inputs,
                   Output = new Ingredient
                            {
                                Symbol = output.Item1,
                                Amount = output.Item2
                            }
               };
    }
}