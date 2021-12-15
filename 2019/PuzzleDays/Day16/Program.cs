
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

void LeftShiftArray(long[] array,
                    int shift)
{
    shift %= array.Length;
    var buffer = new long[shift];
    Array.Copy(array,
               buffer,
               shift);
    Array.Copy(array,
               shift,
               array,
               0,
               array.Length - shift);
    Array.Copy(buffer,
               0,
               array,
               array.Length - shift,
               shift);
}

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

var inputString = "59719811742386712072322509550573967421647565332667367184388997335292349852954113343804787102604664096288440135472284308373326245877593956199225516071210882728614292871131765110416999817460140955856338830118060988497097324334962543389288979535054141495171461720836525090700092901849537843081841755954360811618153200442803197286399570023355821961989595705705045742262477597293974158696594795118783767300148414702347570064139665680516053143032825288231685962359393267461932384683218413483205671636464298057303588424278653449749781937014234119757220011471950196190313903906218080178644004164122665292870495547666700781057929319060171363468213087408071790";
var numberOfPhases = 100;

var inputSignal = inputString.ToCharArray()
                             .Select(x => long.Parse(x.ToString()))
                             .ToList();

var newValue = GetDigitsAfterPhases(inputSignal, numberOfPhases);

Console.WriteLine($"Output signal: {string.Join(string.Empty, newValue).Substring(0, 8)}.");