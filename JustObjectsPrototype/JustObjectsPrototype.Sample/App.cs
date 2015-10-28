using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

		public void RechnungenLöschen(ObservableCollection<Rechnung> rechnungen)
		{
			MessageBox.Show("Löschen: " + rechnungen.Count + " Rechnungen");
			rechnungen.Clear();
			MessageBox.Show("Gelöscht: " + rechnungen.Count + " Rechnungen");
		}

		public void BriefeSchreiben(ObservableCollection<Brief> briefe)
		{
			MessageBox.Show(briefe.Count + " Briefe?");
			briefe.Add(new Brief { Inhalt = "Hallo Welt" });
		}

		public static void AlleLöschen(ObservableCollection<Akte> akten)
		{
			akten.Clear();
		}

		public static void NeuErzeugen(ObservableCollection<Akte> akten)
		{
			akten.Add(new Akte { Name = "Neue Akte 1", Datum = DateTime.Today });
			akten.Add(new Akte { Name = "Neue Akte 2", Datum = DateTime.Today });
		}
	}

	public class Rechnung
	{
		public Kunde Empfänger { get; set; }
		public decimal Betrag { get; set; }
		bool _Bezahlt;
		public bool Bezahlt
		{
			get { return _Bezahlt; }
			set { _Bezahlt = value; }
		}
		public IEnumerable<string> Strings { get; set; }
		public IEnumerable<decimal> Decimals { get; set; }

		public void Erhöhen()
		{
			Betrag += 1;
		}
	}

	public class Kunde
	{
		public string Vorname { get; set; }
		public string Nachname { get; set; }
		public Kunde Vertreter { get; set; }
		public List<Kunde> Freunde { get; set; }

		public List<Kunde> NeuerFreund()
		{
			var kunde = new Kunde { Vorname = "Neuer" + DateTime.Now.Ticks, Nachname = "Freund" + DateTime.Now.Ticks };
			Freunde.Add(kunde);

			return new List<Kunde> { kunde };
		}

		public Kunde Clonen()
		{
			var kunde = new Kunde { Vorname = Vorname + DateTime.Now.Ticks, Nachname = Nachname + DateTime.Now.Ticks };

			return kunde;
		}

		public Kunde Ich()
		{
			return this;
		}

		public void Zahlen(List<int> zahlen)
		{
			MessageBox.Show("Zahlen: " + string.Join(", ", zahlen));
		}

		public void HalloSagen(List<Kunde> kunden)
		{
			MessageBox.Show("Hallo " + string.Join(", ", kunden.ConvertAll(k => k.Vorname + " " + k.Nachname)));
		}

		public Brief Schreiben(string inhalt, bool wirklich)
		{
			return new Brief { Inhalt = inhalt };
		}
	}

	public class Brief
	{
		public string Inhalt { get; set; }

		public int Fünf()
		{
			return 5;
		}

		public string HalloWeltString()
		{
			return "hallo welt";
		}

		public ContentElement ContentElement()
		{
			return new ContentElement();
		}
	}
}
