using System.Text.Json;
using DailyPoetryH.Library.Models;

namespace DailyPoetryH.Library.Services;

public class JinrishiciService : ITodayPoetryService
{
    private readonly IAlertService _alertService;
    public JinrishiciService(IAlertService alertService)
    {
        _alertService = alertService;
    }

    private const string Server = "今日诗词服务器";

    public async Task<TodayPoetry> GetTodayPoetryAsync() => throw new NotImplementedException();

    public async Task<string> GetTokenAsync()
    {
        // TODO: 有问题，需要解决
        using var httpClient = new HttpClient();  // 连接工具
        HttpResponseMessage response;  // 响应信息
        try
        {
            response = await httpClient.GetAsync("https://v2.jinrishici.com/token");  // get请求
            response.EnsureSuccessStatusCode();  // 确保响应成功
        }
        catch (Exception e)
        {
            await _alertService.AlertAsync(ErrorMessages.HttpClientErrorTitle,
                ErrorMessages.GetHttpClientError(Server, e.Message),
                ErrorMessages.HttpClientErrorButton);
            throw;
        }

        var json = await response.Content.ReadAsStringAsync();  // 读取响应内容字符串

        try
        {
            var jinrishiciToken = JsonSerializer
                .Deserialize<JinrishiciToken>(json,
                  new JsonSerializerOptions
                  {
                      PropertyNameCaseInsensitive = true
                  });  // 反序列化，并且控制大小写不敏感
            var token = jinrishiciToken.Data;  // 获取token
        }
        catch (Exception e)
        {
            // TODO: 异常处理，给出友好提示，返回空token
            await _alertService.AlertAsync(ErrorMessages.JsonDeserializationErrorTitle,
                ErrorMessages.GetJsonDeserializationError(Server, e.Message),
                ErrorMessages.JsonDeserializationErrorButton);
            throw;
        }

    }
}

public class JinrishiciToken
{
    // { "status": "success","data": "cv0FRKSPnv1I/Zc1k4RrjIXqvUC/RWGf"}
    public string Status
    {
        get; set;
    }
    public string Data
    {
        get; set;
    }
}
