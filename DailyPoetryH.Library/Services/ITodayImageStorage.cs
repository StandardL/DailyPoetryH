using DailyPoetryH.Library.Models;

namespace DailyPoetryH.Library.Services;

public interface ITodayImageStorage
{
    Task<TodayImage> GetTodayImageAsync(bool includingImageStream);

    Task SaveTodayImageAsync(TodayImage todayImage, bool savingExpiresAtOnly);
}
