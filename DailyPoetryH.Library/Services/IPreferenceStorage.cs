namespace DailyPoetryH.Library.Services;
// 定义一个接口读写数据
public interface IPreferenceStorage
{
    void Set(string key, int value);
    int Get(string key, int defaultValue);

    void Set(string key, string value);
    string Get(string key, string defaultValue);
}
