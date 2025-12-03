using AoCRunner.Services;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AoCRunner.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private readonly CalendarViewModel calendarViewModel;

    public MainWindowViewModel(CalendarViewModel calendarViewModel, NavigationService navigationService)
    {
        this.calendarViewModel = calendarViewModel;
        navigationService.OnNavigation += NavigationServiceOnOnNavigation;
        CurrentPageViewModel = calendarViewModel;
    }

    private void NavigationServiceOnOnNavigation(object? sender, NavigationEventArgs e)
    {
        CurrentPageViewModel = e.ToCalendarView ? calendarViewModel : e.ViewModel!;
    }

    [ObservableProperty]
    private ViewModelBase currentPageViewModel;
}