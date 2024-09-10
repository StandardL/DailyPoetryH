using DailyPoetryH.Library.Models;

namespace DailyPoetryH.Library.Pages;

public partial class Today
{
    private TodayPoetry _todayPoery = new();

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender)
            return;
        
        _todayPoery = await _todayPoetryService.GetTodayPoetryAsync();

        StateHasChanged();
    }
}

