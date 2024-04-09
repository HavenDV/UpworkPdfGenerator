using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Globalization;
using UpworkPdfGenerator.Core;

namespace UpworkPdfGenerator.Apps;

public partial class MainViewModel(
    ILauncher launcher,
    IFilePicker filePicker,
    IPreferences preferences)
    : ObservableObject
{
    public DateTime SelectedDate
    {
        get => preferences.Get(nameof(SelectedDate), DateTime.UtcNow);
        set
        {
            preferences.Set(nameof(SelectedDate), value);
            OnPropertyChanged();
        }
    }

    public string Sign
    {
        get => preferences.Get(nameof(Sign), string.Empty);
        set
        {
            preferences.Set(nameof(Sign), value);
            OnPropertyChanged();
        }
    }

    public string ContractorRus
    {
        get => preferences.Get(nameof(ContractorRus), string.Empty);
        set
        {
            preferences.Set(nameof(ContractorRus), value);
            OnPropertyChanged();
        }
    }

    public string ContractorEng
    {
        get => preferences.Get(nameof(ContractorEng), string.Empty);
        set
        {
            preferences.Set(nameof(ContractorEng), value);
            OnPropertyChanged();
        }
    }

    public double Amount
    {
        get => preferences.Get(nameof(Amount), 0.0);
        set
        {
            preferences.Set(nameof(Amount), value);
            OnPropertyChanged();
        }
    }

    [RelayCommand]
    public void Generate()
    {
        using var destinationStream = new MemoryStream();
        var signPngBytes = File.Exists(Sign)
            ? File.ReadAllBytes(Sign)
            : Array.Empty<byte>();

        var date = SelectedDate;
        PdfGenerator.GenerateConfirmationOfServicesForm(
            destinationStream,
            signPngBytes,
            ContractorRus,
            ContractorEng,
            Amount,
            date);

        var bytes = destinationStream.ToArray();
        var path = Path.Combine(
            Path.GetTempPath(),
            $"Confirmation of Services Form - {date.ToString("MMMM dd, yyyy", CultureInfo.InvariantCulture)}.pdf");

        File.WriteAllBytes(path, bytes);

        _ = launcher.OpenAsync(new Uri(path));
    }

    [RelayCommand]
    public async Task BrowseSign(CancellationToken cancellationToken = default)
    {
        var file = await filePicker.PickAsync(new PickOptions
        {
            PickerTitle = "Select your signature image",
        });
        if (file == null)
        {
            return;
        }

        Sign = file.FullPath;
    }
}