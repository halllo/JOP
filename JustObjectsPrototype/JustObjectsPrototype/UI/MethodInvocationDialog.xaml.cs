using System.Windows;

namespace JustObjectsPrototype.UI
{
	public partial class MethodInvocationDialog : Window
	{
		public MethodInvocationDialog()
		{
			InitializeComponent();
		}

		private void Invoke_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = true;
		}
	}
}
