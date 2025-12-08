using System;
using System.Collections.Generic;
using System.Linq;
using AoCRunner.ViewModels;
using Helpers.Interfaces;
using Helpers.Logging;

namespace AoCRunner.Services;

public class NavigationService(IEnumerable<IPuzzle> puzzles, ProgressLogger progress)
{
    public event EventHandler<NavigationEventArgs>? OnNavigation;

    public void NavigateToCalendarPage()
    {
        OnNavigation?.Invoke(this, new NavigationEventArgs
        {
            ToCalendarView = true
        });
    }

    public void NavigateToPuzzlePage(int day)
    {
        var puzzle = puzzles.First(x => x.Info.Day == day);

        // TODO handle other types
        var viewModel = new SingleExecutionPuzzleViewModel((ISingleExecutionPuzzle)puzzle, progress);

        OnNavigation?.Invoke(this, new NavigationEventArgs
        {
            ViewModel = viewModel
        });
    }
}

public class NavigationEventArgs
{
    public bool ToCalendarView { get; set; }
    public ViewModelBase? ViewModel { get; set; }
}