using System.Diagnostics;
using System.Globalization;
using System.IO;
using UpworkPdfGenerator.App.Properties;
using UpworkPdfGenerator.Core;

namespace UpworkPdfGenerator.Apps;

public partial class MainWindow
{
    public MainWindow()
    {
        InitializeComponent();

        var settings = Settings.Default;
        if (settings.UpgradeRequired)
        {
            settings.Upgrade();
            settings.UpgradeRequired = false;
            settings.Save();
        }

        SignTextBox.Text = settings.Sign;
        ContractorRusTextBox.Text = settings.ContractorRus;
        ContractorEngTextBox.Text = settings.ContractorEng;
        ValueTextBox.Text = $"{settings.Value}";
    }

    private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
    {
        var settings = Settings.Default;
        settings.Sign = SignTextBox.Text;
        settings.ContractorRus = ContractorRusTextBox.Text;
        settings.ContractorEng = ContractorEngTextBox.Text;
        settings.Value = Convert.ToDouble(ValueTextBox.Text);
        settings.Save();

        using var destinationStream = new MemoryStream();
        var signPngBytes = File.ReadAllBytes(settings.Sign);

        var date = DateTime.UtcNow;
        PdfGenerator.GenerateConfirmationOfServicesForm(
            destinationStream,
            signPngBytes,
            settings.ContractorRus,
            settings.ContractorEng,
            settings.Value,
            date);

        var bytes = destinationStream.ToArray();
        var path = Path.Combine(
            Path.GetTempPath(),
            $"Confirmation of Services Form - {date.ToString("MMMM dd, yyyy", CultureInfo.InvariantCulture)}.pdf");

        File.WriteAllBytes(path, bytes);

        Process.Start(new ProcessStartInfo("chrome.exe", $"\"{path}\"")
        {
            UseShellExecute = true,
        });
    }

    private void CloseButton_Click(object sender, System.Windows.RoutedEventArgs e)
    {
        Close();
    }

    private void Title_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        DragMove();
    }
}
