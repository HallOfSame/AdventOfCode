using Spectre.Console;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Helpers.Structure
{
    [Obsolete("Replaced when adding the Blazor solver")]
    public class Solver
    {
        private readonly ProblemBase problem;

        private const int NotImplementedTime = -1;

        private const int ExceptionTime = -2;

        private const string ExceptionText = "InputLoadException";

        private const string NotImplementedText = "Not Implemented";

        private const string SuccessText = "Success";

        public Solver(ProblemBase problem)
        {
            this.problem = problem;
        }

        public async Task Solve()
        {
            try
            {
                var stopwatch = new Stopwatch();

                stopwatch.Start();

                ResultInfo readInput;

                AnsiConsole.MarkupLine("[green]Reading input...[/]");

                try
                {
                    await problem.ReadInput();

                    stopwatch.Stop();

                    readInput = new ResultInfo
                                {
                                    ElapsedMs = stopwatch.ElapsedMilliseconds,
                                    Result = SuccessText
                                };
                }
                catch (Exception ex)
                {
                    LogException(ex);
                    readInput = new ResultInfo
                                {
                                    ElapsedMs = ExceptionTime,
                                    Result = ExceptionText
                                };
                }

                AnsiConsole.MarkupLine("[green]Running Part One...[/]");

                stopwatch.Restart();

                ResultInfo partOne;

                try
                {
                    var partOneResult = await problem.SolvePartOne();

                    stopwatch.Stop();

                    partOne = new ResultInfo
                              {
                                  ElapsedMs = stopwatch.ElapsedMilliseconds,
                                  Result = partOneResult
                              };
                }
                catch (NotImplementedException)
                {
                    AnsiConsole.MarkupLine("[bold yellow]Part One Not Implemented.[/]");

                    partOne = new ResultInfo
                              {
                                  ElapsedMs = NotImplementedTime,
                                  Result = NotImplementedText
                              };
                }
                catch (Exception ex)
                {
                    LogException(ex);

                    partOne = new ResultInfo
                              {
                                  ElapsedMs = ExceptionTime,
                                  Result = ExceptionText
                              };
                }

                AnsiConsole.MarkupLine("[green]Running Part Two...[/]");

                stopwatch.Restart();

                ResultInfo partTwo;

                try
                {
                    var partTwoResult = await problem.SolvePartTwo();

                    stopwatch.Stop();

                    partTwo = new ResultInfo
                              {
                                  ElapsedMs = stopwatch.ElapsedMilliseconds,
                                  Result = partTwoResult
                              };
                }
                catch (NotImplementedException)
                {
                    AnsiConsole.MarkupLine("[bold yellow]Part Two Not Implemented.[/]");
                    partTwo = new ResultInfo
                              {
                                  ElapsedMs = NotImplementedTime,
                                  Result = NotImplementedText
                              };
                }
                catch (Exception ex)
                {
                    LogException(ex);
                    partTwo = new ResultInfo
                              {
                                  ElapsedMs = ExceptionTime,
                                  Result = ExceptionText
                              };
                }

                DisplayResults(readInput,
                               partOne,
                               partTwo);
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine("[red]InputLoadException during core solve method.[/]");
                LogException(ex);
            }
        }

        private void LogException(Exception ex)
        {
            AnsiConsole.WriteException(ex,
                                       ExceptionFormats.ShortenPaths | ExceptionFormats.ShortenTypes | ExceptionFormats.ShortenMethods | ExceptionFormats.ShowLinks);
        }

        private void DisplayResults(ResultInfo readInput,
                                    ResultInfo partOne,
                                    ResultInfo partTwo)
        {
            AnsiConsole.WriteLine();
            AnsiConsole.WriteLine();

            var table = new Table();
            table.Title = new TableTitle("Results");

            table.AddColumn("Operation");
            table.AddColumn("Result");
            table.AddColumn("Elapsed Time {ms}");

            string FormatResult(string result,
                                string defaultColor)
            {
                var color = defaultColor;

                if (result.Equals(ExceptionText))
                {
                    color = "red";
                }

                if (result.Equals(NotImplementedText))
                {
                    color = "yellow";
                }

                return $"[{color}]{result}[/]";
            }

            string FormatTime(long time)
            {
                if (time == NotImplementedTime)
                {
                    return $"[yellow]{time} (Not Implemented)[/]";
                }

                if (time == ExceptionTime)
                {
                    return $"[red]{time} (InputLoadException)[/]";
                }

                return $"[blue]{time}[/]";
            }

            string GetAggregateResult()
            {
                if (!readInput.Result.Equals(SuccessText))
                {
                    return "[red]Incomplete[/]";
                }

                if (partOne.Result.Equals(ExceptionText)
                    || partOne.Result.Equals(NotImplementedText))
                {
                    return "[red]Incomplete[/]";
                }

                if (partTwo.Result.Equals(ExceptionText)
                    || partTwo.Result.Equals(NotImplementedText))
                {
                    return "[yellow]Partially Complete[/]";
                }

                return "[green]Complete[/]";
            }

            table.AddRow("Read Input",
                         FormatResult(readInput.Result,
                                      "blue"),
                         FormatTime(readInput.ElapsedMs));
            table.AddRow("Part One",
                         FormatResult(partOne.Result,
                                      "green"),
                         FormatTime(partOne.ElapsedMs));
            table.AddRow("Part Two",
                         FormatResult(partTwo.Result,
                                      "green"),
                         FormatTime(partTwo.ElapsedMs));
            table.AddRow("Total",
                         GetAggregateResult(),
                         FormatTime(readInput.ElapsedMs + partOne.ElapsedMs + partTwo.ElapsedMs));

            AnsiConsole.Write(table);
        }

        struct ResultInfo
        {
            public long ElapsedMs;

            public string Result;
        }
    }
}
