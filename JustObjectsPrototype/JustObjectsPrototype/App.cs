using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;

namespace JustObjectsPrototype
{
	public class App : Application
	{
		[STAThreadAttribute()]
		public static void Main()
		{
			var objects = new List<object> 
			{
				new Domain.Akte { Name = "Akte 1" },
				new Domain.Akte { Name = "Akte 2" },
				new Domain.Akte { Name = "Akte 3" },
				new Domain.Kunde { Vorname = "Manuel", Nachname = "Naujoks"},
				new Domain.Rechnung { Betrag = 20.00m },
				new Domain.Rechnung { Betrag = 150.00m, Empfänger = new Domain.Kunde { Nachname = "Müller"} },
			};

			var viewModel = new MainWindowModel(objects);

			var window = new MainWindow
			{
				DataContext = viewModel
			};
			window.ShowDialog();
		}
	}
}
