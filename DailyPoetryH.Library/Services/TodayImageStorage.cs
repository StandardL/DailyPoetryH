using DailyPoetryH.Library.Models;

namespace DailyPoetryH.Library.Services;

public class TodayImageStorage : ITodayImageStorage
{
    private readonly IPreferenceStorage _preferenceStorage;

    public TodayImageStorage(IPreferenceStorage preferenceStorage)
    {
        _preferenceStorage = preferenceStorage;
    }

    public static readonly string FullStartDateKey = nameof(TodayImageStorage) + "." + nameof(TodayImage.FullStartDate);
    public static readonly string ExpiresAtKey = nameof(TodayImageStorage) + "." + nameof(TodayImage.ExpiresAt);
    public static readonly string CopyrightKey = nameof(TodayImageStorage) + "." + nameof(TodayImage.Copyright);
    public static readonly string CopyrightLinkKey = nameof(TodayImageStorage) + "." + nameof(TodayImage.CopyrightLink);
    public static readonly string ImageBytesKey = nameof(TodayImageStorage) + "." + nameof(TodayImage.ImageBytes);

    public const string FullStartDateDefault = "202408221530";
    public static readonly DateTime ExpiresAtDefault = new(2024, 8, 23, 15, 30, 00);
    public const string CopyrightDefault = "Salt field province vietnam work (© Quangpraha/Pixabay)";
    public const string CopyrightLinkDefault = "https://pixabay.com/photos/salt-field-province-vietnam-work-3344508/";
    public const string FileName = "todayImage.bin";

    public static readonly string TodayImagePath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), FileName);

    public async Task<TodayImage> GetTodayImageAsync(bool includingImageStream)
    {
        var todayImage = new TodayImage
        {
            FullStartDate = _preferenceStorage.Get(FullStartDateKey, FullStartDateDefault),
            ExpiresAt = _preferenceStorage.Get(ExpiresAtKey, ExpiresAtDefault),
            Copyright = _preferenceStorage.Get(CopyrightKey, CopyrightDefault),
            CopyrightLink = _preferenceStorage.Get(CopyrightLinkKey, CopyrightLinkDefault),
        };

        if (!File.Exists(TodayImagePath))
        {
            // ??运算符，如果左边为null，则返回右边的值，否则返回左边的值
            await using var imageAssetFileStream = new FileStream(TodayImagePath, FileMode.Create) ??
                throw new NullReferenceException("Null file stream.");
            await using var imageAssetStream = typeof(TodayImageStorage).Assembly.GetManifestResourceStream(FileName) ??
                throw new NullReferenceException("Null manifest resource stream.");
            await imageAssetStream.CopyToAsync(imageAssetFileStream);
        }

        if (!includingImageStream)
        {
            return todayImage;
        }
        // 先用流读入内存，再转换为二进制数据
        var imageMemoryStream = new MemoryStream();
        await using var imageFileStream = new FileStream(TodayImagePath, FileMode.Open);
        await imageFileStream.CopyToAsync(imageMemoryStream);
        todayImage.ImageBytes = imageMemoryStream.ToArray();

        return todayImage;
    }

    public async Task SaveTodayImageAsync(TodayImage todayImage, bool savingExpiresAtOnly)
    {
        _preferenceStorage.Set(ExpiresAtKey, todayImage.ExpiresAt);
        if (savingExpiresAtOnly)
            return;

        if (todayImage.ImageBytes == null)
        {
            throw new ArgumentNullException("Null image bytes.", nameof(todayImage));
        }

        _preferenceStorage.Set(FullStartDateKey, todayImage.FullStartDate);
        _preferenceStorage.Set(CopyrightKey, todayImage.Copyright);
        _preferenceStorage.Set(CopyrightLinkKey, todayImage.CopyrightLink);

        await using var imageFileStream = new FileStream(TodayImagePath, FileMode.Create);
        await imageFileStream.WriteAsync(todayImage.ImageBytes, 0, todayImage.ImageBytes.Length);
    }
}
