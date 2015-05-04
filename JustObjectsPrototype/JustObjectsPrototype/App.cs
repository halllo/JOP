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
				new Domain.Akte { Name = "Akte 1", Datum = DateTime.Now },
				new Domain.Akte { Name = "Akte 2" },
				new Domain.Akte { Name = "Akte 3" },
				new Domain.Kunde { 
					Vorname = "Manuel", 
					Nachname = "Naujoks", 
					Freunde = new List<Domain.Kunde> {
						new Domain.Kunde { Vorname = "Tester1", Nachname ="Test1"},
						new Domain.Kunde { Vorname = "Tester2", Nachname ="Test2"},
					}
				},
				new Domain.Rechnung { Betrag = 20.00m },
				new Domain.Rechnung { 
					Betrag = 150.00m, 
					Empfänger = new Domain.Kunde { Nachname = "Müller"}, 
					Strings = new List<string> { "Hallo", "Welt" },
					Decimals = new List<decimal> { 3.5m, 3.3333m },
				},
			};

			var window = new MainWindow
			{
				DataContext = new MainWindowModel(objects)
			};
			window.ShowDialog();
		}
	}
}
