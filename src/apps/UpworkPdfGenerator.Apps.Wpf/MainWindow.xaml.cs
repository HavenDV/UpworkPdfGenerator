using Mvvm.CommonInteractions;

namespace UpworkPdfGenerator.Apps;

public partial class MainWindow
{
    public MainViewModel ViewModel { get; } = new(new FileInteractions(), new WebInteractions());

    public MainWindow()
    {
        InitializeComponent();
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
