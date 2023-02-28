using Mvvm.CommonInteractions;

namespace UpworkPdfGenerator.Apps;

public partial class MainPage
{
	public MainPage()
	{
		InitializeComponent();

		BindingContext = new MainViewModel(new FileInteractions(), new WebInteractions());
	}
}

