using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AoCRunner.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using Helpers.Interfaces;
using InputStorageDatabase;
using PuzzleDays;

namespace AoCRunner.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    public MainWindowViewModel(IEnumerable<IPuzzle> puzzles)
    {
        var puzzleDictionary = puzzles.ToDictionary(x => x.Info.Day, x => x.Info.Name);
        var currentRow = 0;
        var currentColumn = 1;

        PuzzleInfos = Enumerable.Range(1, 31).Select(x =>
            {
                var newPuzzle = new PuzzleInfoUiModel
                {
                    PuzzleName = puzzleDictionary.GetValueOrDefault(x, string.Empty),
                    DayNumber = x,
                    GridRow = currentRow,
                    GridColumn = currentColumn
                };

                currentColumn++;

                if (currentColumn >= 7)
                {
                    currentRow++;
                    currentColumn = 0;
                }

                return newPuzzle;
            })
            .ToList();
    }

    public List<PuzzleInfoUiModel> PuzzleInfos { get; }
    public string Input { get; set; } = string.Empty;

    [ObservableProperty]
    private string _result = string.Empty;

    public Day01 Puzzle = new();

    public async Task RunPartOne()
    {
        await Puzzle.LoadInput(Input, PuzzleInputType.Example);
        var result = await Puzzle.ExecutePartOne();
            
        Result = $"Got result {result.Result} in {result.ElapsedTime}";
    }

    public async Task RunPartTwo()
    {
        await Puzzle.LoadInput(Input, PuzzleInputType.Example);
        var result = await Puzzle.ExecutePartTwo();

        Result = $"Got result {result.Result} in {result.ElapsedTime}";
    }
}
