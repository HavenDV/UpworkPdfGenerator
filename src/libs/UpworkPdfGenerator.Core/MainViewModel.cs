using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Globalization;
using Mvvm.CommonInteractions;
using UpworkPdfGenerator.App.Properties;
using UpworkPdfGenerator.Core;

namespace UpworkPdfGenerator.Apps;

public partial class MainViewModel : ObservableObject
{
    private IFileInteractions FileInteractions { get; }
    private IWebInteractions WebInteractions { get; }
    
    [ObservableProperty]
    private DateTime? _selectedDate;

    [ObservableProperty]
    private string _sign = string.Empty;

    [ObservableProperty]
    private string _contractorRus = string.Empty;

    [ObservableProperty]
    private string _contractorEng = string.Empty;

    [ObservableProperty]
    private double? _value;

    public MainViewModel(
        IFileInteractions fileInteractions,
        IWebInteractions webInteractions)
    {
        FileInteractions = fileInteractions ?? throw new ArgumentNullException(nameof(fileInteractions));
        WebInteractions = webInteractions ?? throw new ArgumentNullException(nameof(webInteractions));
        
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

        _ = WebInteractions.OpenUrlAsync(new Uri(path));
    }

    [RelayCommand]
    public async Task BrowseSign(CancellationToken cancellationToken = default)
    {
        var file = await FileInteractions.OpenFileAsync(new OpenFileArguments
        {
            SuggestedFileName = "sign.png",
            Extensions = new[] { ".png" },
            FilterName = "PNG Files",
        }, cancellationToken).ConfigureAwait(true);
        if (file == null)
        {
            return;
        }

        Sign = file.FullPath;

        var settings = Settings.Default;
        settings.Sign = Sign;
        settings.Save();
    }
}