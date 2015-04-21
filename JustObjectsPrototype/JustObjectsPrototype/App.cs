using JustObjectsPrototype.UI;
using System;
using System.Collections.Generic;
using System.Windows;

namespace JustObjectsPrototype
{
	public class App : Application
	{
		[STAThreadAttribute()]
		public static void Main()
		{
			var objects = new System.Collections.ObjectModel.ObservableCollection<object> 
			{
				new Domain.Akte { Name = "Akte 1" },
				new Domain.Akte { Name = "Akte 2" },
				new Domain.Akte { Name = "Akte 3" },
				new Domain.Kunde { Vorname = "Manuel", Nachname = "Naujoks"},
				new Domain.Rechnung { Betrag = 20.00m },
				new Domain.Rechnung { Betrag = 150.00m, Empfänger = new Domain.Kunde { Nachname = "Müller"} },
			};

			var window = new MainWindow
			{
				DataContext = new MainWindowModel(objects)
			};
			window.ShowDialog();
		}
	}
}
