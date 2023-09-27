using System.Linq.Expressions;
using DailyPoetryH.Library.Models;
namespace DailyPoetryH.Library.Services;
public interface IPoetryStorage
{
    bool IsInitialized { get; }

    Task InitializeAsync();  // 文件操作一定是异步的

    Task<Poetry> GetPoetryAsync(int id);  // 根据id获取诗词
    
    // 展示一种可能性
    Task<IEnumerable<Poetry>> GetPoetriesAsync(
        Expression<Func<Poetry, bool>> where, int skip, int take);
    // skip跳过多少行，take取多少行
    // Connection.Table<Poetry>().Where(p => p.Name.Contains("H")).ToListAsync();
}