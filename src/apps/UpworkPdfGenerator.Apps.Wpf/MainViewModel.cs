using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using UpworkPdfGenerator.App.Properties;
using UpworkPdfGenerator.Core;

namespace UpworkPdfGenerator.Apps.Wpf;

public partial class MainViewModel : ObservableObject
{
    [ObservableProperty]
    public DateTime? _selectedDate;

    [ObservableProperty]
    public string? _sign;

    [ObservableProperty]
    public string? _contractorRus;

    [ObservableProperty]
    public string? _contractorEng;

    [ObservableProperty]
    public double? _value;

    public MainViewModel()
    {
        var settings = Settings.Default;
        if (settings.UpgradeRequired)
        {
            settings.Upgrade();
            settings.UpgradeRequired = false;
            settings.Save();
        }

        SelectedDate = DateTime.UtcNow;
        Sign = settings.Sign;
        ContractorRus = settings.ContractorRus;
        ContractorEng = settings.ContractorEng;
        Value = settings.Value;
    }

    [RelayCommand]
    public void Generate()
    {
        var settings = Settings.Default;
        settings.Sign = Sign;
        settings.ContractorRus = ContractorRus;
        settings.ContractorEng = ContractorEng;
        settings.Value = Convert.ToDouble(Value);
        settings.Save();

        using var destinationStream = new MemoryStream();
        var signPngBytes = File.Exists(settings.Sign)
            ? File.ReadAllBytes(settings.Sign)
            : Array.Empty<byte>();

        var date = SelectedDate;
        PdfGenerator.GenerateConfirmationOfServicesForm(
            destinationStream,
            signPngBytes,
            settings.ContractorRus ?? string.Empty,
            settings.ContractorEng ?? string.Empty,
            settings.Value,
            date);

        var bytes = destinationStream.ToArray();
        var path = Path.Combine(
            Path.GetTempPath(),
            $"Confirmation of Services Form - {date?.ToString("MMMM dd, yyyy", CultureInfo.InvariantCulture)}.pdf");

        File.WriteAllBytes(path, bytes);

        Process.Start(new ProcessStartInfo("chrome.exe", $"\"{path}\"")
        {
            UseShellExecute = true,
        });
    }

    [RelayCommand]
    public void BrowseSign()
    {
        var wildcards = new[] { ".png" }
            .Select(static extension => $"*{extension}")
            .ToArray();
        var filter = $@"PNG Files ({string.Join(", ", wildcards)})|{string.Join(";", wildcards)}";

        var dialog = new OpenFileDialog
        {
            CheckFileExists = true,
            CheckPathExists = true,
            Filter = filter,
        };
        if (dialog.ShowDialog() != true)
        {
            return;
        }

        var path = dialog.FileName;

        Sign = path;

        var settings = Settings.Default;
        settings.Sign = Sign;
        settings.Save();
    }
}