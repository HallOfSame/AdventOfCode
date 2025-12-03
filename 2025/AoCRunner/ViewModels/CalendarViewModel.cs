using System.Collections.Generic;
using System.Linq;
using AoCRunner.Models;
using AoCRunner.Services;
using Helpers.Interfaces;

namespace AoCRunner.ViewModels;

public class CalendarViewModel : ViewModelBase
{
    private readonly NavigationService navigationService;

    public CalendarViewModel(IEnumerable<IPuzzle> puzzles, NavigationService navigationService)
    {
        this.navigationService = navigationService;
        var puzzleDictionary = puzzles.ToDictionary(x => x.Info.Day, x => x.Info.Name);
        var currentRow = 0;
        var currentColumn = 1;

        PuzzleInfos = [.. Enumerable.Range(1, 31)
            .Select(x =>
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
            })];
    }

    public List<PuzzleInfoUiModel> PuzzleInfos { get; }

    public void SelectPuzzle(int day)
    {
        navigationService.NavigateToPuzzlePage(day);
    }
}