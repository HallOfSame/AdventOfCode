using Helpers.FileReaders;
using Helpers.Maps;
using Helpers.Structure;

using Spectre.Console;

var solver = new Solver(new Day13Problem());

await solver.Solve();

class Day13Problem : ProblemBase
{
    protected override Task<string> SolvePartOneInternal()
    {
        var firstFold = folds.First();

        ProcessFold(firstFold);

        return Task.FromResult(dotLocations.Count.ToString());
    }

    private void ProcessFold(FoldInstruction instruction)
    {
        if (instruction.IsVertical)
        {
            foreach (var dotLocation in dotLocations.Where(x => x.X > instruction.FoldLine)
                                                    .ToList())
            {
                var newX = (-1 * (dotLocation.X - instruction.FoldLine)) + instruction.FoldLine;

                var newCoordinate = new Coordinate(newX, dotLocation.Y);

                dotLocations.Add(newCoordinate);
                dotLocations.Remove(dotLocation);
            }
        }
        else
        {
            foreach (var dotLocation in dotLocations.Where(x => x.Y > instruction.FoldLine)
                                                    .ToList())
            {
                var newY = (-1 * (dotLocation.Y - instruction.FoldLine)) + instruction.FoldLine;

                var newCoordinate = new Coordinate(dotLocation.X, newY);

                dotLocations.Add(newCoordinate);
                dotLocations.Remove(dotLocation);
            }
        }
    }

    protected override Task<string> SolvePartTwoInternal()
    {
        folds.Skip(1)
             .ToList()
             .ForEach(ProcessFold);

        var maxX = dotLocations.Max(x => x.X);
        var maxY = dotLocations.Max(x => x.Y);

        var canvas = new Canvas(maxX + 1,
                                maxY + 1);

        dotLocations.ToList()
                    .ForEach(x => canvas.SetPixel(x.X,
                                                  x.Y,
                                                  Color.Aqua));

        var panel = new Panel(canvas)
                    {
                        Header = new PanelHeader("Part Two Answer")
                    };

        AnsiConsole.Write(panel);

        return Task.FromResult("See above picture.");
    }

    public override async Task ReadInput()
    {
        var fileStrings = await new StringFileReader().ReadInputFromFile();

        dotLocations = new HashSet<Coordinate>();
        folds = new List<FoldInstruction>();

        foreach (var line in fileStrings)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                continue;
            }

            if (!line.StartsWith("fold"))
            {
                var intValues = line.Split(',')
                                    .Select(int.Parse)
                                    .ToArray();

                dotLocations.Add(new Coordinate(intValues[0], intValues[1]));
            }
            else
            {
                var foldInfo = line.Split('=');

                var isVertical = foldInfo[0][foldInfo[0]
                                                 .Length
                                             - 1]
                                 == 'x';

                var foldLine = int.Parse(foldInfo[1]);

                folds.Add(new FoldInstruction
                          {
                              IsVertical = isVertical,
                              FoldLine = foldLine
                          });
            }
        }
    }

    private HashSet<Coordinate> dotLocations;

    private List<FoldInstruction> folds;
}

class FoldInstruction
{
    public bool IsVertical { get; set; }

    public int FoldLine { get; set; }
}