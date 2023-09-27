using Microsoft.AspNetCore.Components;

namespace DailyPoetryH.Library.Services;
public class NavigationServices : INavigationServices
{
    private readonly NavigationManager _navigationManager;

    public NavigationServices(NavigationManager navigationManager)
    {
           _navigationManager = navigationManager;
    }

    public void Navigate(string uri, object parameter) => throw new NotImplementedException();
    public void NavigateTo(string uri) => _navigationManager.NavigateTo(uri);
}
