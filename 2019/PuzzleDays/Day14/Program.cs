using Helpers;

var reactions = await new ReactionReader().ReadInputFromFile();

// Part 1

var oreRequired = new ReactionVessel
                  {
                      IncludeDebugOutput = false
                  }.GetOreRequiredForSingleOutput("FUEL",
                                                  reactions);

Console.WriteLine($"Ore required for 1 fuel: {oreRequired}.");

// Part 2

var fuelPossible = new ReactionVessel
                   {
                       IncludeDebugOutput = false
                   }.DetermineAmountOfFuelPossible(reactions);

Console.WriteLine($"Possible fuel we can create: {fuelPossible}.");

class ReactionVessel
{
    public const string Ore = "ORE";

    private Dictionary<string, long> stockpile = new()
                                                 {
                                                     {
                                                         Ore, 1000000000000L
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
                if (stockpile[Ore] < amountNeeded)
                {
                    throw new ApplicationException("Out of ORE");
                }

                WriteMessage($"Using {amountNeeded} ORE to generate {amount} of {symbol}.");

                oreUsedThisTransaction += amountNeeded;
                stockpile[Ore] -= oreUsedThisTransaction;
                continue;
            }

            if (stockpile.TryGetValue(input.Symbol,
                                      out var existingAmount))
            {
                if (amountNeeded < existingAmount)
                {
                    WriteMessage($"Stockpile covered required {amountNeeded} for {symbol}.");

                    amountNeeded = 0;
                    stockpile[input.Symbol] -= input.Amount;
                }
                else
                {
                    WriteMessage($"Emptied stockpile of {input.Symbol} while generating {symbol}.");

                    stockpile[input.Symbol] = 0;
                    amountNeeded -= existingAmount;
                }
            }

            if (amountNeeded > 0)
            {
                WriteMessage($"Still need {amountNeeded} of {input.Symbol} as part of producing {amount} {symbol}.");

                oreUsedThisTransaction += GenerateOutput(input.Symbol,
                                                         amountNeeded,
                                                         reactions);
            }
        }

        if (leftoverAmount > 0)
        {
            if (stockpile.ContainsKey(symbol))
            {
                WriteMessage($"Adding an additional {leftoverAmount} of {symbol} to the stockpile.");

                stockpile[symbol] += leftoverAmount;
            }
            else
            {
                WriteMessage($"Adding an new quantity {leftoverAmount} of {symbol} to the stockpile.");

                stockpile[symbol] = leftoverAmount;
            }
        }

        return oreUsedThisTransaction;
    }

    public bool IncludeDebugOutput { get; set; }

    private void WriteMessage(string message)
    {
        if (!IncludeDebugOutput)
        {
            return;
        }

        Console.WriteLine(message);
    }

    public long GetOreRequiredForSingleOutput(string outputSymbol,
                                              List<Intermediate> reactions)
    {
        return GenerateOutput(outputSymbol,
                              1,
                              reactions);
    }

    public long DetermineAmountOfFuelPossible(List<Intermediate> reactions)
    {
        var fuelGenerated = 0L;

        var fuelFactor = 10_000_000; // Arbitrary start at 10 Mil. Ran fast enough for me.

        Dictionary<string, long> savedStockpile = null;

        while (true)
        {
            try
            {
                // We need to be able to roll back if this fails, so copy our current stockpile
                savedStockpile = stockpile.ToDictionary(x => x.Key,
                                                        x => x.Value);

                GenerateOutput("FUEL",
                               fuelFactor,
                               reactions);

                fuelGenerated += fuelFactor;
            }
            catch (ApplicationException) // Thrown if we run out of over
            {
                // Reset the stockpile to before we tried to over-gen fuel
                stockpile = savedStockpile!;

                // If we were only creating a single unit, we're done here
                if (fuelFactor == 1)
                {
                    break;
                }

                // Otherwise back down the factor by half and keep running
                fuelFactor /= 2;
            }
        }

        return fuelGenerated;
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