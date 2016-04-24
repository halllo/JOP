
namespace JustObjectsPrototype.UI
{
	public partial class MainWindow
	{
		public MainWindow()
		{
			InitializeComponent();
		}

		public MainWindowModel ViewModel
		{
			get { return DataContext as MainWindowModel; }
		}
	}
}
