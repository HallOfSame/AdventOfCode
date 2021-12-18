using Helpers;
using Helpers.Structure;

var solver = new Solver(new Day18Problem());

await solver.Solve();

class Day18Problem : ProblemBase
{
    private List<Digit> numbersFromInput;

    protected override async Task<string> SolvePartOneInternal()
    {
        var currentSum = numbersFromInput.First();

        foreach (var digit in numbersFromInput.Skip(1))
        {
            var newSum = new Pair
                         {
                             Left = currentSum,
                             Right = digit,
                         };

            currentSum.Parent = newSum;
            digit.Parent = newSum;

            currentSum = Reduce(newSum);
        }

        return currentSum.GetMagnitude()
                         .ToString();
    }

    protected override async Task<string> SolvePartTwoInternal()
    {
        throw new NotImplementedException();
    }

    private RegularNumber? GetNearestRegularNumber(Digit input,
                                                   bool checkLeft,
                                                   Digit? prev)
    {
        Func<Pair, Digit> checkDigitFunc = checkLeft
                                               ? p => p.Left
                                               : p => p.Right;

        Func<Pair, Digit> inverseCheck = checkLeft
                                               ? p => p.Right
                                               : p => p.Left;

        if (input.Parent is null
            || input.Parent is not Pair p)
        {
            return null;
        }

        // This logic sure became a mess, but it works
        if (checkDigitFunc(p) is not RegularNumber reg)
        {
            if (checkDigitFunc(p) is Pair pairToSide && pairToSide != input)
            {
                while (inverseCheck(pairToSide) is not RegularNumber)
                {
                    pairToSide = (Pair)inverseCheck(pairToSide);
                }

                return (RegularNumber)inverseCheck(pairToSide);
            }

            if (p.Parent is null)
            {
                // At the top level, the other side was a pair so go down that tree
                if (checkDigitFunc(p) is Pair pairToCheck && pairToCheck != input)
                {
                    while (inverseCheck(pairToCheck) is not RegularNumber)
                    {
                        pairToCheck = (Pair)inverseCheck(pairToCheck);
                    }

                    return (RegularNumber)inverseCheck(pairToCheck);
                }
            }

            return GetNearestRegularNumber(p,
                                           checkLeft,
                                           input);
        }

        return reg;
    }

    private Digit Reduce(Digit inputDigit)
    {
        var updatedDigit = inputDigit.Clone();

        while (true)
        {
            var firstDigitToExplode = (Pair?)updatedDigit.ToList()
                                                         .FirstOrDefault(x => x is Pair { NestLevel: >= 4 });

            if (firstDigitToExplode != null)
            {
                var leftReg = GetNearestRegularNumber(firstDigitToExplode,
                                                      true,
                                                      null);

                var rightReg = GetNearestRegularNumber(firstDigitToExplode,
                                                       false,
                                                       null);

                // Add left to first left regular number
                if (leftReg != null)
                {
                    leftReg.Value += ((RegularNumber)firstDigitToExplode.Left).Value;
                }

                // Add right to first right regular number
                if (rightReg != null)
                {
                    rightReg.Value += ((RegularNumber)firstDigitToExplode.Right).Value;
                }

                // Replace this pair with regular number w/ value 0
                var parent = (Pair)firstDigitToExplode.Parent!;

                var newValue = new RegularNumber
                               {
                                   Value = 0L,
                                   Parent = parent
                               };

                if (parent.Left == firstDigitToExplode)
                {
                    parent.Left = newValue;
                }
                else
                {
                    parent.Right = newValue;
                }

                continue;
            }

            var firstDigitToSplit = (RegularNumber?)updatedDigit.ToList()
                                                                .FirstOrDefault(x => x is RegularNumber { Value: >= 10 });

            if (firstDigitToSplit != null)
            {
                if (firstDigitToSplit.Parent is not Pair p)
                {
                    throw new ApplicationException("Didn't anticipate this scenario.");
                }

                var leftValue = (long)Math.Floor(firstDigitToSplit.Value / 2m);
                var rightValue = (long)Math.Ceiling(firstDigitToSplit.Value / 2m);

                var newLeft = new RegularNumber
                              {
                                  Value = leftValue
                              };

                var newRight = new RegularNumber
                               {
                                   Value = rightValue
                               };

                var newValue = new Pair
                               {
                                   Left = newLeft,
                                   Right = newRight,
                                   Parent = p
                               };

                newLeft.Parent = newValue;
                newRight.Parent = newValue;

                if (p.Left == firstDigitToSplit)
                {
                    p.Left = newValue;
                }
                else
                {
                    p.Right = newValue;
                }

                continue;
            }

            break;
        }

        return updatedDigit;
    }

    public override async Task ReadInput()
    {
        numbersFromInput = await new DigitReader().ReadInputFromFile();
    }
}

class DigitReader : FileReader<Digit>
{
    protected override Digit ProcessLineOfFile(string line)
    {
        return GetDigit(line,
                        0);
    }

    private Digit GetDigit(string line,
                           int nestLevel)
    {
        // If a pair
        if (line.StartsWith('['))
        {
            var output = new Pair();

            var stringStack = new Stack<char>();

            var rightStartIdx = 1;

            var leftString = new string(line.Skip(1)
                                            .TakeWhile(x =>
                                                       {
                                                           // If opening a new pair push to the stack
                                                           if (x == '[')
                                                           {
                                                               stringStack.Push(x);
                                                           }
                                                           // Pop when one closes
                                                           else if (x == ']' && stringStack.Count > 0)
                                                           {
                                                               stringStack.Pop();
                                                           }

                                                           rightStartIdx++;

                                                           // If the stack is empty and we hit a comma, it's the split for the current pair
                                                           return !(x == ',' && stringStack.Count == 0);
                                                       })
                                            .ToArray());

            var rightString = new string(line.Substring(rightStartIdx)
                                             .TakeWhile(x =>
                                                        {
                                                            if (x == '[')
                                                            {
                                                                stringStack.Push(x);
                                                            }
                                                            else if (x == ']' && stringStack.Count > 0)
                                                            {
                                                                stringStack.Pop();
                                                            }

                                                            return !(x == ']' && stringStack.Count == 0);
                                                        })
                                             .ToArray());

            output.Left = GetDigit(leftString,
                                   nestLevel + 1);

            output.Left.Parent = output;

            output.Right = GetDigit(rightString,
                                    nestLevel + 1);

            output.Right.Parent = output;

            return output;
        }

        // Normal number
        var lineSplit = line.Split(',');

        return new RegularNumber
               {
                   Value = long.Parse(lineSplit[0])
               };
    }
}

abstract class Digit
{
    public Digit? Parent { get; set; }

    public abstract long GetMagnitude();

    public abstract List<Digit> ToList();

    public abstract Digit Clone();
}

class RegularNumber : Digit
{
    public long Value { get; set; }

    public override long GetMagnitude()
    {
        return Value;
    }

    public override string ToString()
    {
        return Value.ToString();
    }

    public override List<Digit> ToList()
    {
        return new List<Digit>
               {
                   this
               };
    }

    public override Digit Clone()
    {
        return new RegularNumber
        {
            Value = this.Value
        };
    }
}

class Pair : Digit
{
    public int NestLevel
    {
        get
        {
            return this.Parent == null
                       ? 0
                       : ((Pair)this.Parent).NestLevel + 1;
        }
    }

    public Digit Left { get; set; }

    public Digit Right { get; set; }

    public override long GetMagnitude()
    {
        return (3L * Left.GetMagnitude()) + (2L * Right.GetMagnitude());
    }

    public override List<Digit> ToList()
    {
        var left = Left.ToList();
        var right = Right.ToList();

        var totalCap = left.Count + right.Count + 1;

        var list = new List<Digit>(totalCap)
                   {
                       this
                   };

        list.AddRange(left);
        list.AddRange(right);

        return list;
    }

    public override string ToString()
    {
        return $"[{Left}, {Right}]";
    }

    public override Digit Clone()
    {
        var leftClone = Left.Clone();
        var rightClone = Right.Clone();

        var thisClone = new Pair
                        {
                            Left = leftClone,
                            Right = rightClone
                        };

        leftClone.Parent = thisClone;
        rightClone.Parent = thisClone;

        return thisClone;
    }
}