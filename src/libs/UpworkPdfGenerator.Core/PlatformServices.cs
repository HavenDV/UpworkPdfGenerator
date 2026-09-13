namespace UpworkPdfGenerator.Apps;

public interface ILauncher
{
    Task<bool> OpenAsync(Uri uri);
}

public interface IFilePicker
{
    Task<string?> PickAsync(
        string title,
        CancellationToken cancellationToken = default);
}

public interface IPreferences
{
    T Get<T>(string key, T defaultValue);

    void Set<T>(string key, T value);
}
