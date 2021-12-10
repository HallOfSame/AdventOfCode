using Helpers;
using Helpers.Structure;

var solver = new Solver(new Day10Problem());

await solver.Solve();

class Day10Problem : ProblemBase
{
    List<char[]> puzzleInput;

    List<char[]> incompleteLines;

    public override async Task ReadInput()
    {
        puzzleInput = await new CharArrayFileReader().ReadInputFromFile();
    }

    protected override Task<string> SolvePartOneInternal()
    {
        var validator = new LineValidator();

        var scoredLines = puzzleInput.Select(x => new {
            Score = validator.GetLineSyntaxScore(x),
            Line = x
        });

        incompleteLines = scoredLines.Where(x => x.Score == 0)
            .Select(x => x.Line)
            .ToList();

        var result = scoredLines.Where(x => x.Score > 0)
            .Select(x => x.Score)
            .Sum()
            .ToString();

        return Task.FromResult(result);
    }

    protected override Task<string> SolvePartTwoInternal()
    {
        var validator = new LineValidator();

        var completionScores = incompleteLines.Select(x => validator.GetLineCompletionScore(x))
            .OrderBy(x => x).ToArray();

        var middleIndex = completionScores[completionScores.Length / 2];

        return Task.FromResult(middleIndex.ToString());
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

    public long GetLineCompletionScore(char[] line)
    {
        var completionString = GetCompletionForLine(line);

        var completionScore = 0l;

        foreach(var completionChar in completionString)
        {
            completionScore *= 5;

            switch(completionChar)
            {
                case ')':
                    completionScore += 1;
                    break;
                case '}':
                    completionScore += 3;
                    break;
                case ']':
                    completionScore += 2;
                    break;
                case '>':
                    completionScore += 4;
                    break;
            }
        }

        return completionScore;
    }

    private char[] GetCompletionForLine(char[] line)
    {
        var runningStack = new Stack<char>();

        foreach (var nextCharacterInLine in line)
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
                    throw new InvalidOperationException($"Line {line} is corrupted.");
                }
            }
        }

        var completionString = new List<char>(runningStack.Count);

        while(runningStack.Count > 0)
        {
            completionString.Add(runningStack.Pop().GetEndChar());
        }

        return completionString.ToArray();
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