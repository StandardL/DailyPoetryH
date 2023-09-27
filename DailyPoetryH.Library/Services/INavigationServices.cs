namespace DailyPoetryH.Library.Services;

public interface INavigationServices
{
    void NavigateTo(string uri);
    // 传Query String导航
    // /detail/1     /detail/?id=1

    void Navigate(string uri, object parameter);
    // /showNews new NewObject{ Title="", Content="" }
}

public static class NavigationServiceConstants
{
    public const string TodayPage = "/today";
    public const string InitializationPage = "/initialization";
    public const string ResultPage = "/result";
    public const string DetailPage = "/detail";
}