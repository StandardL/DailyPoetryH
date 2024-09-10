using System.Linq.Expressions;
using DailyPoetryH.Library.Models;
using SQLite;

namespace DailyPoetryH.Library.Services;
public class PoetryStorage : IPoetryStorage
{
    public const int NumberPoetry = 30;
    public const string DbName = "poetrydb.sqlite3";
    public static readonly string PoetryDbPath =
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), DbName);
    
    // 数据库连接
    private SQLiteAsyncConnection _connection;
    private SQLiteAsyncConnection Connection => _connection ??= new SQLiteAsyncConnection(PoetryDbPath);

    private readonly IPreferenceStorage _preferenceStorage;
    public PoetryStorage(IPreferenceStorage preferenceStorage)
    {
           _preferenceStorage = preferenceStorage;
    } // 通过构造函数，使用依赖注入实现接口

    public bool IsInitialized => 
        _preferenceStorage.Get(PoetryStorageConstant.DbVersionKey, 0) == PoetryStorageConstant.Version;

    public async Task InitializeAsync()
    {
        // 先打开写的文件，然后打开读的文件，copy，关闭读的文件，关闭写的文件
        // 使用Using语句，在作用域结束后可以自动释放资源
        // await表示可以异步关闭文件
        await using var dbFileStream = 
            new FileStream(PoetryDbPath, FileMode.OpenOrCreate);
        // typeof是反射, Assembly是读取编译后的dll文件，从而打开了资源流
        await using var dbAssetStream = 
            typeof(PoetryStorage).Assembly.GetManifestResourceStream(DbName);
        await dbAssetStream.CopyToAsync(dbFileStream);

        _preferenceStorage.Set(PoetryStorageConstant.DbVersionKey, PoetryStorageConstant.Version);
    }

    public Task<Poetry> GetPoetryAsync(int id) =>
        Connection.Table<Poetry>().FirstOrDefaultAsync(p => p.Id == id);
    public async Task<IEnumerable<Poetry>> GetPoetriesAsync(
        Expression<Func<Poetry, bool>> where, int skip, int take) => 
        await Connection.Table<Poetry>().Where(where).Skip(skip).Take(take)// AsyncTalbeQuery
            .ToListAsync();// Task<List<...>>

    public async Task CloseAsync() => await _connection.CloseAsync();

}

public static class PoetryStorageConstant
{
    public const String DbVersionKey = 
        nameof(PoetryStorageConstant) + "." + nameof(DbVersionKey);
    // "PoetryStorageConstant.DbVersionKey"

    public const int Version = 1;
}