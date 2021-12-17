using Helpers.Structure;

var solver = new Solver(new Day17Problem());

await solver.Solve();

class Day17Problem : ProblemBase
{
    protected override Task<string> SolvePartOneInternal()
    {
        // If we had x > maxX we would just go straight past the target
        var xRange = Enumerable.Range(1,
                                      maxX - 1);

        var invalidTurns = 0;
        int? bestY = null;

        // Can't have yVel lower than minY or we shoot right below the target
        var yVel = minY;

        do
        {
            var bestYForThisYVel = xRange.Select(x => TestVelocity(x,
                                                                   yVel,
                                                                   out var bestY)
                                                          ? bestY
                                                          : default(int?))
                                         .Where(bestY => bestY != null)
                                         .OrderByDescending(bestY => bestY)
                                         .FirstOrDefault();

            if (bestYForThisYVel.HasValue)
            {
                invalidTurns = 0;
            }
            else
            {
                invalidTurns++;
            }

            if (bestYForThisYVel.HasValue
                && (!bestY.HasValue || bestYForThisYVel > bestY.Value))
            {
                bestY = bestYForThisYVel;
            }

            yVel++;
        }
        // Don't stop until we get some value and then have gone 100 turns without a valid solution
        while (!bestY.HasValue || invalidTurns < 100);

        return Task.FromResult(bestY.Value.ToString());
    }

    private bool TestVelocity(int x,
                              int y,
                              out int maxYPos)
    {
        var xPos = 0;
        var yPos = 0;

        bool InTargetArea()
        {
            return xPos >= minX && xPos <= maxX && yPos >= minY && yPos <= maxY;
        }

        bool PastTargetArea()
        {
            return xPos > maxX || yPos < minY;
        }

        var step = 0;
        maxYPos = int.MinValue;

        while (!PastTargetArea())
        {
            step++;

            xPos += x;
            yPos += y;

            if (yPos > maxYPos)
            {
                maxYPos = yPos;
            }

            x = x > 0
                    ? x - 1
                    : 0;

            y -= 1;

            if (InTargetArea())
            {
                return true;
            }
        }

        return false;
    }

    protected override async Task<string> SolvePartTwoInternal()
    {
        throw new NotImplementedException();
    }

    public override async Task ReadInput()
    {
        //minX = 20;
        //maxX = 30;

        //minY = -10;
        //maxY = -5;
        minX = 244;
        maxX = 303;

        minY = -91;
        maxY = -54;
    }

    private int minX;

    private int minY;

    private int maxX;

    private int maxY;
}