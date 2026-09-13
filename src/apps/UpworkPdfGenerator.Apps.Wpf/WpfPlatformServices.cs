using System.Diagnostics;
using System.IO;
using System.Text.Json;
using Microsoft.Win32;

namespace UpworkPdfGenerator.Apps;

internal sealed class WpfLauncher : ILauncher
{
    public Task<bool> OpenAsync(Uri uri)
    {
        ArgumentNullException.ThrowIfNull(uri);

        var target = uri.IsFile ? uri.LocalPath : uri.AbsoluteUri;
        var process = Process.Start(new ProcessStartInfo(target)
        {
            UseShellExecute = true,
        });

        return Task.FromResult(process != null);
    }
}

internal sealed class WpfFilePicker : IFilePicker
{
    public Task<string?> PickAsync(
        string title,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var dialog = new OpenFileDialog
        {
            CheckFileExists = true,
            CheckPathExists = true,
            Filter = "PNG images (*.png)|*.png",
            Title = title,
        };

        var result = dialog.ShowDialog() == true
            ? dialog.FileName
            : null;

        return Task.FromResult(result);
    }
}

internal sealed class WpfPreferences : IPreferences
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
    };

    private readonly object syncRoot = new();
    private readonly string path = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "UpworkPdfGenerator",
        "preferences.json");
    private Dictionary<string, JsonElement>? values;

    public T Get<T>(string key, T defaultValue)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);

        lock (syncRoot)
        {
            var preferences = GetValues();
            if (!preferences.TryGetValue(key, out var value))
            {
                return defaultValue;
            }

            try
            {
                return value.Deserialize<T>(JsonOptions) ?? defaultValue;
            }
            catch (JsonException)
            {
                return defaultValue;
            }
        }
    }

    public void Set<T>(string key, T value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);

        lock (syncRoot)
        {
            var preferences = GetValues();
            preferences[key] = JsonSerializer.SerializeToElement(value, JsonOptions);

            var directory = Path.GetDirectoryName(path)!;
            _ = Directory.CreateDirectory(directory);
            File.WriteAllText(path, JsonSerializer.Serialize(preferences, JsonOptions));
        }
    }

    private Dictionary<string, JsonElement> GetValues()
    {
        if (values != null)
        {
            return values;
        }

        if (!File.Exists(path))
        {
            return values = new Dictionary<string, JsonElement>();
        }

        try
        {
            return values = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(
                    File.ReadAllText(path),
                    JsonOptions) ??
                new Dictionary<string, JsonElement>();
        }
        catch (JsonException)
        {
            return values = new Dictionary<string, JsonElement>();
        }
    }
}
