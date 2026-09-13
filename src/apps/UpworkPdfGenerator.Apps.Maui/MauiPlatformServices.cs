namespace UpworkPdfGenerator.Apps;

internal sealed class MauiLauncher : ILauncher
{
    public Task<bool> OpenAsync(Uri uri)
    {
        return Microsoft.Maui.ApplicationModel.Launcher.Default.OpenAsync(uri);
    }
}

internal sealed class MauiFilePicker : IFilePicker
{
    public async Task<string?> PickAsync(
        string title,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var file = await Microsoft.Maui.Storage.FilePicker.Default.PickAsync(new()
        {
            PickerTitle = title,
            FileTypes = Microsoft.Maui.Storage.FilePickerFileType.Png,
        }).ConfigureAwait(true);

        return file?.FullPath;
    }
}

internal sealed class MauiPreferences : IPreferences
{
    public T Get<T>(string key, T defaultValue)
    {
        return Microsoft.Maui.Storage.Preferences.Default.Get(key, defaultValue);
    }

    public void Set<T>(string key, T value)
    {
        Microsoft.Maui.Storage.Preferences.Default.Set(key, value);
    }
}
