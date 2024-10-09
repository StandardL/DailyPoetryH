using DailyPoetryH.Library.Models;
using Microsoft.JSInterop;

namespace DailyPoetryH.Library.Pages;

public partial class Today
{
    private TodayPoetry _todayPoery = new();
    private TodayImage _todayImage = new();
    private bool _isLoading = true;
    private bool _isShowDetail = false;

    protected override void OnAfterRender(bool firstRender)
    {
        if (!firstRender)
            return;

        Task.Run(async () =>
        {
            _isLoading = true;
            await InvokeAsync(StateHasChanged);

            _todayPoery = await _todayPoetryService.GetTodayPoetryAsync();
            _isLoading = false;
            await InvokeAsync(StateHasChanged);
        });

        Task.Run(async () =>
        {
            _todayImage = await _todayImageService.GetTodayImageAsync();
            await _jsRuntime.InvokeVoidAsync("setImage",
                new DotNetStreamReference(new MemoryStream(_todayImage.ImageBytes)),
                "image");
        });
    }
}

