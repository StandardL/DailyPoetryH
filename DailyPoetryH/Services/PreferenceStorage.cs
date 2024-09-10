using DailyPoetryH.Library.Services;
namespace DailyPoetryH.Services;
public class PreferenceStorage : IPreferenceStorage
{
    public int Get(string key, int defaultValue) => 
        Preferences.Get(key, defaultValue);
    public void Set(string key, int value) => 
        Preferences.Set(key, value);

    public string Get(string key, string defaultValue) =>
        Preferences.Get(key, defaultValue);
    public void Set(string key, string value) =>
        Preferences.Set(key, value);
}