using Helpers;
using Helpers.Structure;

var solver = new Solver(new Day10Problem());

await solver.Solve();

class Day10Problem : ProblemBase
{
    List<char[]> puzzleInput;

    public override async Task ReadInput()
    {
        puzzleInput = await new CharArrayFileReader().ReadInputFromFile();
    }

    protected override Task<string> SolvePartOneInternal()
    {
        var validator = new LineValidator();

        var result = puzzleInput.Select(x => validator.GetLineSyntaxScore(x))
            .Sum()
            .ToString();

        return Task.FromResult(result);
    }

    protected override Task<string> SolvePartTwoInternal()
    {
        throw new NotImplementedException();
    }
}

class LineValidator
{
    public int GetLineSyntaxScore(char[] line)
    {
        var firstInvalid = GetFirstInvalidLineChar(line);

        if (firstInvalid == null)
        {
            return 0;
        }

        switch(firstInvalid)
        {
            case ')':
                return 3;
            case '}':
              return 1197;
            case ']':
                return 57;
            case '>':
                return 25137;
        }

        throw new InvalidOperationException($"No score exists for invalid character: {firstInvalid}.");
    }

    private char? GetFirstInvalidLineChar(char[] line)
    {
        var runningStack = new Stack<char>();

        foreach(var nextCharacterInLine in line)
        {
            if (nextCharacterInLine.IsStartCharacter())
            {
                runningStack.Push(nextCharacterInLine);
            }
            else
            {
                var chunkOpenChar = runningStack.Pop();

                var expectedEndChar = chunkOpenChar.GetEndChar();

                if (nextCharacterInLine != expectedEndChar)
                {
                    return nextCharacterInLine;
                }
            }
        }

        return null;
    }
}

static class Extensions
{
    public static bool IsStartCharacter(this char c)
    {
        return c == '(' || c == '[' || c == '<' || c == '{';
    }

    public static char GetEndChar(this char startChar)
    {
        switch (startChar)
        {
            case '(':
                return ')';
            case '[':
                return ']';
            case '<':
                return '>';
            case '{':
                return '}';
        }

        throw new InvalidOperationException($"No valid end character exists for {startChar}.");
    }
}