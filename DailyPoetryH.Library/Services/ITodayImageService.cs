using DailyPoetryH.Library.Models;

namespace DailyPoetryH.Library.Services;

public interface ITodayImageService
{
    Task<TodayImage> GetTodayImageAsync();
    Task<TodayImageServiceCheckUpdateResult> CheckUpdateAsync();
}

public class TodayImageServiceCheckUpdateResult
{
    public bool HasUpdate { get; set; }
    public TodayImage todayImage { get; set; } = new();
}
