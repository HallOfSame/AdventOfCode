using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace Helpers.Structure
{
    public class Solver
    {
        private readonly ProblemBase problem;

        private const int NotImplementedTime = -1;

        private const int ExceptionTime = -2;

        public Solver(ProblemBase problem)
        {
            this.problem = problem;
        }

        public async Task Solve()
        {
            await AnsiConsole.Status()
            .StartAsync("Running Puzzle Solver...", async ctx =>
            {

                var stopwatch = new Stopwatch();

                stopwatch.Start();

                long readInputTime;

                AnsiConsole.MarkupLine("[green]Reading input...[/]");

                try
                {
                    await problem.ReadInput();

                    stopwatch.Stop();

                    readInputTime = stopwatch.ElapsedMilliseconds;
                }
                catch (Exception ex)
                {
                    LogException(ex);
                    readInputTime = ExceptionTime;
                }

                AnsiConsole.MarkupLine("[green]Running Part One...[/]");

                stopwatch.Restart();

                long partOneTime;

                try
                {
                    await problem.SolvePartOne();

                    stopwatch.Stop();

                    partOneTime = stopwatch.ElapsedMilliseconds;
                }
                catch (NotImplementedException)
                {
                    AnsiConsole.MarkupLine("[bold yellow]Part One Not Implemented.[/]");
                    partOneTime = NotImplementedTime;
                }
                catch (Exception ex)
                {
                    LogException(ex);
                    partOneTime = ExceptionTime;
                }

                AnsiConsole.MarkupLine("[green]Running Part Two...[/]");

                stopwatch.Restart();

                long partTwoTime;

                try
                {
                    await problem.SolvePartTwo();

                    stopwatch.Stop();

                    partTwoTime = stopwatch.ElapsedMilliseconds;
                }
                catch (NotImplementedException)
                {
                    AnsiConsole.MarkupLine("[bold yellow]Part Two Not Implemented.[/]");
                    partTwoTime = NotImplementedTime;
                }
                catch (Exception ex)
                {
                    LogException(ex);
                    partTwoTime = ExceptionTime;
                }

                DisplayResults(readInputTime, partOneTime, partTwoTime);
            });               
        }

        private void LogException(Exception ex)
        {
            AnsiConsole.WriteException(ex,
                                        ExceptionFormats.ShortenPaths | ExceptionFormats.ShortenTypes |
                                        ExceptionFormats.ShortenMethods | ExceptionFormats.ShowLinks);
        }

        private void DisplayResults(long readInputTime, long partOneTime, long partTwoTime)
        {
            var table = new Table();

            table.AddColumn("Operation");
            table.AddColumn("Elapsed Time {ms}");

            string FormatTime(long time)
            {
                if (time == NotImplementedTime)
                {
                    return $"[yellow]{time} (Not Implemented)[/]";
                }

                if (time == ExceptionTime)
                {
                    return $"[red]{time} (Exception)[/]";
                }

                return $"[green]{time}[/]";
            }

            table.AddRow("Read Input", FormatTime(readInputTime));
            table.AddRow("Part One", FormatTime(partOneTime));
            table.AddRow("Part Two", FormatTime(partTwoTime));

            AnsiConsole.Write(table);
        }
    }
}
