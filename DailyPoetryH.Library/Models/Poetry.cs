namespace DailyPoetryH.Library.Models;

// SQLite时，类名和表名不一致，需要加上Table特性来映射
[SQLite.Table("works")]
public class Poetry
{
    // 数据库有的用列名做映射
    [SQLite.Column("id")]
    public int Id
    {
        get; set;
    }

    [SQLite.Column("name")]
    public string Name { get; set; } = string.Empty;

    [SQLite.Column("author_name")]
    public string AuthorName { get; set; } = string.Empty;

    [SQLite.Column("dynasty")]
    public string Dynasty { get; set; } = string.Empty;

    [SQLite.Column("content")]
    public string Content { get; set; } = string.Empty;

    private string _snippet;  // 预览，正文的第一句话（动态生成）

    // 数据库没有的用Ignore特性来忽略
    [SQLite.Ignore]
    public string Snippet =>
        _snippet ??= Content.Split('。')[0].Replace("\r\n", " ");
}
