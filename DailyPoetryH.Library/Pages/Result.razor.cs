using System.Linq.Expressions;
using DailyPoetryH.Library.Models;

namespace DailyPoetryH.Library.Pages;

public partial class Result
{
    private Expression<Func<Poetry, bool>> _where = p => true;

    public const string Loading = "正在加载...";
    public const string NoResult = "没有满足条件的结果";
    public const string NoMoreResult = "没有更多结果";

    private string _status = string.Empty; // 用于显示当前状态
    public const int PageSize = 20;  // 每页显示20条数据

    private List<Poetry> _poetries = new();

    protected async override Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender)
            return;

        // TODO 测试代码 记得删除
        await _poetryStorage.InitializeAsync();
        await LoadMoreAsync();
    }

    private async Task LoadMoreAsync()  // 无限滚动
    {
        _status = Loading;
        // 条件，跳过已经读取的数，读取PageSize条数据
        var poetries = 
            await _poetryStorage.GetPoetriesAsync(_where, _poetries.Count, PageSize);
        await Task.Delay(1000); // 模拟1s的网络延迟
        _status = string.Empty;
        _poetries.AddRange(poetries);

        if (poetries.Count() < PageSize)
            _status = NoMoreResult;

        if (!poetries.Any() && _poetries.Count() == 0)
            _status = NoResult;
    }
}
