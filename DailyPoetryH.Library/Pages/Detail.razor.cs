using Microsoft.AspNetCore.Components;

namespace DailyPoetryH.Library.Pages;

public partial class Detail
{
    [Parameter]  // ID的值从参数中传入
    public string Id { get; set; }
}
