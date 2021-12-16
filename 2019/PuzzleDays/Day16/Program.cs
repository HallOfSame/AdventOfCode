
var originalPattern = new long[]
{
    0, 1, 0, -1
};

var patternDictionary = new Dictionary<int, long[]>
                        {
                            {
                                1, originalPattern
                            }
                        };

long[] GetPatternForDigit(int digit)
{
    if (!patternDictionary.TryGetValue(digit,
                                       out var pattern))
    {
        pattern = originalPattern.SelectMany(x => Enumerable.Repeat(x,
                                                                    digit))
                                 .ToArray();

        //LeftShiftArray(pattern,
        //               digit - 1);

        patternDictionary.Add(digit,
                              pattern);
    }

    return pattern;
}

List<long> GetDigitsAfterPhases(List<long> startValue,
                                long phases)
{
    var currentValue = startValue;

    for (var phase = 0; phase < phases; phase++)
    {
        var newValue = new List<long>(currentValue.Count);

        for (var digit = 1; digit <= currentValue.Count; digit++)
        {
            var patternForDigit = GetPatternForDigit(digit);

            newValue.Add(Math.Abs(currentValue.Select((x,
                                                       idx) =>
                                                      {
                                                          var patternValue = patternForDigit[(idx + 1) % patternForDigit.Length];

                                                          //Console.WriteLine($"Doing {x} * {patternValue} while getting {digit}...");

                                                          // Get the 1s digit of the result
                                                          return (x * patternValue) % 10;
                                                      })
                                              .Sum())
                         % 10);

            //Console.WriteLine($"New value of digit {digit} after {phase} phases: {newValue[digit - 1]}.");
        }

        currentValue = newValue;
    }

    return currentValue;
}

List<long> GetDigitsAfterPhasesPart2(List<long> startValue,
                                     long phases,
                                     int offset)
{
    var currentValue = startValue.ToArray();

    for (var phase = 0; phase < phases; phase++)
    {
        var newValue = currentValue.ToArray();

        var runningSum = 0L;

        for (var digit = currentValue.Length - 1; digit >= offset; digit--)
        {
            runningSum += currentValue[digit];

            var updatedDigit = runningSum % 10;

            newValue[digit] = updatedDigit;
            //Console.WriteLine($"New value of digit {digit} after {phase} phases: {newValue[digit - 1]}.");
        }

        currentValue = newValue;
    }

    return currentValue.ToList();
}

var inputString = "03036732577212944063491565474664";
var numberOfPhases = 100;

var inputSignal = inputString.ToCharArray()
                             .Select(x => long.Parse(x.ToString()))
                             .ToList();

// Part 1

var newValue = GetDigitsAfterPhases(inputSignal,
                                    numberOfPhases);

Console.WriteLine($"Output signal: {string.Join(string.Empty, newValue).Substring(0, 8)}.");

// Part 2

var numberOfRepeats = 10000;

var messageOffset = long.Parse(string.Join(string.Empty, inputString.Take(7)));

var bigInput = Enumerable.Range(0,
                                numberOfRepeats)
                         .SelectMany(x => inputSignal)
                         .ToList();

var newValuePart2 = GetDigitsAfterPhasesPart2(bigInput,
                                              numberOfPhases,
                                              (int)messageOffset);

Console.WriteLine($"Output signal: {string.Join(string.Empty, newValuePart2.Skip(Convert.ToInt32(messageOffset)).Take(8)).Substring(0, 8)}.");