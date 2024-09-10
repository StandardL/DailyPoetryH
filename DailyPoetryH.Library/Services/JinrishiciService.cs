using System.Linq.Expressions;
using System.Text.Json;
using DailyPoetryH.Library.Models;

namespace DailyPoetryH.Library.Services;

public class JinrishiciService : ITodayPoetryService
{
    private readonly IAlertService _alertService;
    private readonly IPreferenceStorage _preferenceStorage;
    private readonly IPoetryStorage _poetryStorage;

    public static readonly string JinrishiciTokenKey = $"{nameof(JinrishiciService)}.Token";

    public JinrishiciService(IAlertService alertService, IPreferenceStorage preferenceStorage, IPoetryStorage poetryStorage)
    {
        _alertService = alertService;
        _preferenceStorage = preferenceStorage;
        _poetryStorage = poetryStorage;
    }

    private const string Server = "今日诗词服务器";
    private string _token = string.Empty;  // cache

    public async Task<TodayPoetry> GetRandomPoetryAsync()
    {
        // p => true
        var poetries = await _poetryStorage.GetPoetriesAsync(
            Expression.Lambda<Func<Poetry, bool>>(Expression.Constant(true), Expression.Parameter(typeof(Poetry), "p")),
            new Random().Next(PoetryStorage.NumberPoetry), 1);
        var poetry = poetries.First();
        return new TodayPoetry
        {
            Snippet = poetry.Snippet,
            Author = poetry.AuthorName,
            Dynasty = poetry.Dynasty,
            Name = poetry.Name,
            Content = poetry.Content,
            Source = TodayPoetrySources.Local
        };
    }

    public async Task<TodayPoetry> GetTodayPoetryAsync()
    {
        var token = await GetTokenAsync();
        // TODO 无token的情况
        using var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Add("X-User-Token", token);

        HttpResponseMessage response;
        try
        {
            response = await httpClient.GetAsync("https://v2.jinrishici.com/sentence");
            response.EnsureSuccessStatusCode();
        }
        catch (Exception e)
        {
            await _alertService.AlertAsync(ErrorMessages.HttpClientErrorTitle,
                ErrorMessages.GetHttpClientError(Server, e.Message),
                ErrorMessages.HttpClientErrorButton);
            return await GetRandomPoetryAsync();
        }

        var json = await response.Content.ReadAsStringAsync();  // 读取响应内容字符串
        JinrishiciSentence jinrishiciSentence;
        try
        {
            jinrishiciSentence = JsonSerializer.Deserialize<JinrishiciSentence>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });  // 反序列化，并且控制大小写不敏感
        }
        catch (Exception e)
        {
            await _alertService.AlertAsync(
                ErrorMessages.JsonDeserializationErrorTitle,
                ErrorMessages.GetJsonDeserializationError(Server, e.Message),
                ErrorMessages.JsonDeserializationErrorButton);
            return await GetRandomPoetryAsync();
        }

        return new TodayPoetry
        {
            Snippet = jinrishiciSentence.Data.Content,
            Author = jinrishiciSentence.Data.Origin.Author,
            Dynasty = jinrishiciSentence.Data.Origin.Dynasty,
            Name = jinrishiciSentence.Data.Origin.Title,
            Content = string.Join(Environment.NewLine, jinrishiciSentence.Data.Origin.Content),
            Source = TodayPoetrySources.Jinrishici
        };
    }

    public async Task<string> GetTokenAsync()
    {
        // 1.从存储中获取token
        // 2.如果没有，则从服务器获取
        // 3.然后保存
        if (!string.IsNullOrWhiteSpace(_token))
        {
            return _token;  // 内存Cache
        }

        _token = _preferenceStorage.Get(JinrishiciTokenKey, string.Empty);
        if (!string.IsNullOrWhiteSpace(_token))
        {
            return _token;
        }

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
            return _token;
        }

        var json = await response.Content.ReadAsStringAsync();  // 读取响应内容字符串
        JinrishiciToken jinrishiciToken;
        try
        {
            jinrishiciToken = JsonSerializer.Deserialize<JinrishiciToken>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });  // 反序列化，并且控制大小写不敏感
        }
        catch (Exception e)
        {
            // TODO: 异常处理，给出友好提示，返回空token
            await _alertService.AlertAsync(
                ErrorMessages.JsonDeserializationErrorTitle,
                ErrorMessages.GetJsonDeserializationError(Server, e.Message),
                ErrorMessages.JsonDeserializationErrorButton);
            return _token;
        }

        _token = jinrishiciToken.Data;  // 获取token
        _preferenceStorage.Set(JinrishiciTokenKey, _token);
        return _token;
    }
}

public class JinrishiciToken
{
    // { "status": "success","data": "cv0FRKSPnv1I/Zc1k4RrjIXqvUC/RWGf" }
    public string Status
    {
        get; set;
    }
    public string Data
    {
        get; set;
    }
}


public class JinrishiciSentence
{
    public JinrishiciData Data { get; set; }
}

public class JinrishiciData
{
    public string Content { get; set; }
    public JinrishiciOrigin Origin { get; set; }
}

public class JinrishiciOrigin
{
    public string Title { get; set; }
    public string Dynasty { get; set; }
    public string Author { get; set; }
    public string[] Content { get; set; }
}
