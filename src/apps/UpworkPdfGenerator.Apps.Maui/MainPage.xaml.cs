namespace UpworkPdfGenerator.Apps;

public partial class MainPage
{
	public MainPage(MainViewModel viewModel)
	{
		InitializeComponent();

		BindingContext = viewModel;
	}
}

