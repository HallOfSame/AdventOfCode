using Helpers.Coordinates;
using Helpers.Extensions;

using IntCodeInterpreter;
using IntCodeInterpreter.Input;

using Spectre.Console;

var program = await new FileInputParser().ReadOperationsFromFile("PuzzleInput.txt");

var manager = new VacuumManager();

manager.Run(program);

class VacuumManager
{
    private readonly Interpreter interpreter = new();

    public void Run(List<long> instructions)
    {
        var output = new List<List<Tile>>();

        var currentLine = new List<Tile>();

        var currentX = 0;
        var currentY = 0;

        interpreter.ProcessOperations(instructions,
                                      () =>
                                      {
                                          var userInput = AnsiConsole.Ask<long>("Input");

                                          return userInput;
                                      },
                                      x =>
                                      {
                                          string? text = null;

                                          switch (x)
                                          {
                                              case 35:
                                                  text = "#";
                                                  break;
                                              case 46:
                                                  text = ".";
                                                  break;
                                              case 10:
                                                  // I think we get a second 10 at the end of the output
                                                  if (currentLine.Count == 0)
                                                  {
                                                      return;
                                                  }

                                                  output.Add(currentLine.ToList());
                                                  currentLine = new List<Tile>();
                                                  currentY++;
                                                  currentX = 0;
                                                  return;
                                          }

                                          if (!string.IsNullOrEmpty(text))
                                          {
                                              var newTile = new Tile
                                                            {
                                                                X = currentX,
                                                                Y = currentY,
                                                                Text = text
                                                            };

                                              currentLine.Add(newTile);
                                          }

                                          currentX++;
                                      });

        var alignmentParamSum = 0;

        for (var y = 0; y < output.Count; y++)
        {
            var drawLine = string.Empty;

            for (var x = 0; x < output[y].Count; x++)
            {
                var currentChar = output[y][x];

                var text = currentChar.Text;

                try
                {
                    if (text == "#")
                    {
                        var neighbors = currentChar.GetNeighborCoordinates()
                                                   .Select(x => output[x.Y][x.X])
                                                   .ToList();

                        if (neighbors.Count == 4
                            && neighbors.All(x => x.Text == "#"))
                        {
                            alignmentParamSum += (x * y);
                            text = "O";
                        }
                    }
                }
                // Not elegant but I don't feel like adding all the checks :)
                catch(ArgumentOutOfRangeException)
                { }

                drawLine += text;
            }

            AnsiConsole.MarkupLine($"[yellow]{drawLine}[/]");
        }

        AnsiConsole.MarkupLine($"[green]Alignment: {alignmentParamSum}.[/]");
    }

    class Tile : Coordinate2D
    {
        public string Text { get; init; }
    }
}