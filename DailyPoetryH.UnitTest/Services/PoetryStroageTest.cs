using System.Linq.Expressions;
using DailyPoetryH.Library.Models;
using DailyPoetryH.Library.Services;
using DailyPoetryH.UnitTest.Helpers;
using Moq;

namespace DailyPoetryH.UnitTest.Services;
public class PoetryStroageTest : IDisposable
{
    public PoetryStroageTest()
    {
        // xUnit通过在构造函数中调用RemoveDatabaseFile方法，确保每次测试都是干净的
        // 注意，xUnit是每次在运行测试类之前，都会先跑这里的方法。
        PoetryStorageHelper.RemoveDatabaseFile();
    }

    public void Dispose()
    {
        // 通过继承IDisposable接口，制造一个相当于析构函数的东西，
        // 利用garbage collactor确保每次测试都是干净的
        PoetryStorageHelper.RemoveDatabaseFile();
    }

    // MS命名规则：被测函数名_测试情况
    [Fact]
    public void IsInitialized_Initialized()
    {
        // Mock技术（模拟技术）：模拟一个对象，用于测试
        var preferenceStorageMock = new Mock<IPreferenceStorage>();
        // 在调用了Mock对象的Get方法后，并传入了DbVersionKey和0两个参数后，
        // 返回PoetryStorageConstant.Version
        preferenceStorageMock
            .Setup(p => p.Get(PoetryStorageConstant.DbVersionKey, 0))
            .Returns(PoetryStorageConstant.Version);
        var mockPreferenceStorage = preferenceStorageMock.Object;

        var poetryStorage = new PoetryStorage(mockPreferenceStorage);
        Assert.True(poetryStorage.IsInitialized);  // 断言
    }

    [Fact]
    public void IsInitialized_NotInitialized()
    {
        // Mock技术（模拟技术）：模拟一个对象，用于测试
        var preferenceStorageMock = new Mock<IPreferenceStorage>();
        // 在调用了Mock对象的Get方法后，并传入了DbVersionKey和0两个参数后，
        // 返回PoetryStorageConstant.Version
        preferenceStorageMock
            .Setup(p => p.Get(PoetryStorageConstant.DbVersionKey, 0))
            .Returns(0);
        var mockPreferenceStorage = preferenceStorageMock.Object;

        var poetryStorage = new PoetryStorage(mockPreferenceStorage);
        Assert.False(poetryStorage.IsInitialized);  // 断言
    }

    [Fact]
    public async Task InitializeAsync_Default()
    {
        var preferenceStorageMock = new Mock<IPreferenceStorage>();
        // 没有返回值，就不需要SETUP
        var mockPreferenceStorage = preferenceStorageMock.Object;
        
        var poetryStorage = new PoetryStorage(mockPreferenceStorage);

        // 单元测试，假设数据库文件不存在
        Assert.False(File.Exists(PoetryStorage.PoetryDbPath));
        await poetryStorage.InitializeAsync();
        Assert.True(File.Exists(PoetryStorage.PoetryDbPath));

        // 验证调用了一次Set方法
        preferenceStorageMock.Verify(p => 
            p.Set(PoetryStorageConstant.DbVersionKey, PoetryStorageConstant.Version)
            , Times.Once);
    }

    [Fact]
    public async Task GetPoetryAsync_Default()
    {
        var poetryStorage = await PoetryStorageHelper.GetInitializedPoetryStorage();
        var poetry = await poetryStorage.GetPoetryAsync(10001);
        Assert.Equal("临江仙 · 夜归临皋", poetry.Name);
        await poetryStorage.CloseAsync();
    }

    [Fact]
    public async Task GetPoetriesAsync_Default()
    {
        // p => true (让所有的都返回)
        // 下面的Lambda表达式，只要取出来的是变量p，且p是Poetry类型，就返回true
        var poetryStorage = await PoetryStorageHelper.GetInitializedPoetryStorage();
        var poetries = await poetryStorage.GetPoetriesAsync(
            Expression.Lambda<Func<Poetry, bool>>(Expression.Constant(true),
                Expression.Parameter(typeof(Poetry), "p")), 0, 30);
        Assert.Equal(30, poetries.Count());
        await poetryStorage.CloseAsync();
    }
}
