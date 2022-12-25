using Helpers.FileReaders;
using Helpers.Structure;

namespace PuzzleDays
{
    public class Day25 : ProblemBase
    {
        protected override async Task<string> SolvePartOneInternal()
        {
            var sum = fuelRequirements.Select(x => SnafuConverter.ToDecimal(x))
                                      .Sum();

            return SnafuConverter.ToSnafu(sum);
        }

        protected override async Task<string> SolvePartTwoInternal()
        {
            return "Christmas";
        }

        public override async Task ReadInput()
        {
            fuelRequirements = await new StringFileReader().ReadInputFromFile();
        }

        private List<string> fuelRequirements;
    }
}

static class SnafuConverter
{
    public static long ToDecimal(string snafu)
    {
        // Checked to watch for overflow issues
        checked
        {
            var result = 0L;

            for (var digit = snafu.Length - 1; digit >= 0; digit--)
            {
                var ch = snafu[snafu.Length - 1 - digit];

                var digitValue = ch switch
                {
                    '2' => 2,
                    '1' => 1,
                    '0' => 0,
                    '-' => -1,
                    '=' => -2
                };

                var digitMult = (long)Math.Pow(5L,
                                              digit);

                result += digitValue * digitMult;
            }

            return result;
        }
    }

    public static string ToSnafu(long decimalValue)
    {
        // Kind of pieced this algorithm together by hand just playing with the values
        checked
        {
            // Holds the values for each digit in the result
            var snafuValue = new List<char>();

            var currentValue = decimalValue;

            var currentDigit = 0;

            while (true)
            {
                // Get the multiplier of our current place
                var digitMultiplier = (long)Math.Pow(5L,
                                                    currentDigit);

                // Check what our basic value would be
                // If it is 0, 1 or 2 we can just add it
                var digitAmount = currentValue / digitMultiplier;

                if (digitAmount > 2)
                {
                    // We may need some kind of negation for this number to work correctly
                    // Get the base of the next digit
                    var nextDigitMult = (long)Math.Pow(5L,
                                                      currentDigit + 1);

                    // And see if it has a remainder with the current value, we need to go in to each digit with no remainder
                    var nextDigitRemainder = currentValue % nextDigitMult;

                    // If the next digit base amount works cleanly with the current value, add 0 here
                    // Example we are in the 5s place and the value is 50, which cleanly divides by 25, so add a '0' here
                    if (nextDigitRemainder == 0)
                    {
                        snafuValue.Add('0');
                    }
                    else
                    {
                        // If the next digit needs a 1 or 2 here to work cleanly, then add it as well
                        // For example we are in the 5s place and the value is 55
                        // Next digit remainder would be 5, so we need to add '1' here for the next digit to work
                        var thisDigit = nextDigitRemainder / digitMultiplier;

                        if (thisDigit is 1 or 2)
                        {
                            snafuValue.Add(thisDigit switch
                            {
                                1 => '1',
                                2 => '2'
                            });

                            currentValue -= thisDigit * digitMultiplier;
                        }
                        else
                        {
                            // Getting here means we need a negation
                            // Which is the next digit remainder - the full value of a 1 in the next digit
                            // Divided by our current base
                            // Example: we are in the 5s place and the current value is 45
                            // The below becomes (20 - 25) / 5 == -5 / 5 == -1
                            // Adding a negative 5 makes the current value 50 which will then fix it for the 25s place digit
                            var negationNeeded = (nextDigitRemainder - nextDigitMult) / digitMultiplier;

                            // TIL this throws if a match is not found so we don't need an extra check
                            snafuValue.Add(negationNeeded switch
                            {
                                -1 => '-',
                                -2 => '='
                            });

                            currentValue -= digitMultiplier * negationNeeded;
                        }
                    }
                }
                else
                {
                    // Our number just needs 1 or 2 of the current value to work
                    // i.e. we're in the 5s place and the number is 60, just add 2, which makes the remaining amount 50, which works for 25 just fine
                    snafuValue.Add(digitAmount == 1
                                       ? '1'
                                       : '2');
                    currentValue -= digitAmount * digitMultiplier;
                }

                if (currentValue == 0)
                {
                    break;
                }

                currentDigit++;
            }

            // We added the digits starting at the 1s place so flip that around
            snafuValue.Reverse();

            return new string(snafuValue.ToArray());
        }
    }
}