﻿using System;
using System.Collections.Generic;
using System.Windows;

namespace JustObjectsPrototype.Sample
{
	public class App : Application
	{
		[STAThreadAttribute()]
		public static void Main()
		{
			var objects = new System.Collections.ObjectModel.ObservableCollection<object> 
			{
				new Akte { Name = "Akte 1", Datum = DateTime.Now },
				new Akte { Name = "Akte 2" },
				new Akte { Name = "Akte 3" },
				new Kunde { 
					Vorname = "Manuel", 
					Nachname = "Naujoks", 
					Freunde = new List<Kunde> {
						new Kunde { Vorname = "Tester1", Nachname ="Test1"},
						new Kunde { Vorname = "Tester2", Nachname ="Test2"},
					}
				},
				new Rechnung { Betrag = 20.00m },
				new Rechnung { 
					Betrag = 150.00m, 
					Empfänger = new Kunde { Nachname = "Maier"}, 
					Strings = new List<string> { "Hallo", "Welt" },
					Decimals = new List<decimal> { 3.5m, 3.3333m },
				},
			};

			Show.With(objects);
		}
	}

	public class Akte
	{
		public Akte()
		{
		}
		public string Name { get; set; }
		Kunde mandant = new Kunde { Vorname = "Hans", Nachname = "Müller" };
		public Kunde Mandant { get { return mandant; } }
		public DateTime Datum { get; set; }
	}

	public class Rechnung
	{
		public Kunde Empfänger { get; set; }
		public decimal Betrag { get; set; }
		public IEnumerable<string> Strings { get; set; }
		public IEnumerable<decimal> Decimals { get; set; }
	}

	public class Kunde
	{
		public string Vorname { get; set; }
		public string Nachname { get; set; }
		public Kunde Vertreter { get; set; }
		public IEnumerable<Kunde> Freunde { get; set; }

		//public override string ToString()
		//{
		//	return Vorname + " " + Nachname;
		//}
	}
}
