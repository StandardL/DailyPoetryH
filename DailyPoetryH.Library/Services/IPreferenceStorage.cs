namespace DailyPoetryH.Library.Services;
// 定义一个接口读写整型数据
public interface IPreferenceStorage
{
    void Set(string key, int value);
    int Get(string key, int defaultValue);
}
