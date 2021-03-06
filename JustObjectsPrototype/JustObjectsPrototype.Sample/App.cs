﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;

namespace JustObjectsPrototype.Sample
{
	public class App : Application
	{
		[STAThread()]
		public static void Main()
		{
			var objects = new ObservableCollection<object>
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
				new Kunde { Vorname = "Max", Nachname = "Musterman" },
				new Kunde { Vorname = "King", Nachname = "Kong" },
				new Rechnung { Betrag = 20.00m },
				new Rechnung {
					Betrag = 150.00m,
					Empfänger = new Kunde { Nachname = "Maier"},
					Strings = new List<string> { "Hallo", "Welt" },
					Decimals = new List<decimal> { 3.5m, 3.3333m },
				},
			};

			/*
			var akten = objects.OfType<Akte>().ToList();
			var kunden = objects.OfType<Kunde>().ToList();
			Show.Prototype(akten, kunden);
			Show.Prototype(with: objects);
			Show.Prototype();
			Show.Prototype(With.ViewOf<Akte>());
			Show.Prototype(With.Window(w => w.Title = "Akten").AndViewOf<Akte>());
			Show.Prototype(With.ViewOf<Akte>().AndWindow(w => w.Title = "Akten").AndObjects());
			Show.Prototype(With.These(akten));
			Show.Prototype(With
				.SettingsFor<Akte>().DisableNew().DisableDelete()
				.AndSettingsFor<Kunde>().DisableNew().DisableDelete()
				.AndObjects(objects)
			);
			Show.Prototype(With.These(akten).AndSettingsFor<Akte>().DisableDelete().DisableNew());
			Show.Prototype(With.These(objects)
				.AndViewOf<Akte>()
					.EnableNew(newed => { newed.Name = "new"; MessageBox.Show("neue Akte: " + newed.Name); })
					.EnableDelete(deleted => { MessageBox.Show("Akte gelöscht: " + deleted.Name); })
					.OnValueChanged(changed => { MessageBox.Show("Akte geändert: " + changed.Name); })
				.AndViewOf<Kunde>()
					.EnableNew()
					.DisableDelete()
					.OnValueChanged(changed => { changed.Geändert = true; })
			);
			*/



			var customers = new List<Customer>
			{
				new Customer { Name = "Max Musterman" }
			};

			Show.Prototype(With.These(objects).AndWindow(w =>
			{
				w.Title = "Customer Manager";
				//w.ViewModel.SelectedType = w.ViewModel.Types[0];
			}));
		}
	}



	public class Invoice
	{
		public Customer Receiver { get; set; }
		public decimal Amount { get; set; }

		public void Increase()
		{
			Amount += 1;
		}
	}
	public class Customer
	{
		public string Name { get; set; }
		public override string ToString()
		{
			return Name;
		}

		public Invoice Bill(decimal amount)
		{
			return new Invoice
			{
				Receiver = this,
				Amount = amount,
			};
		}
	}

	public class Akte
	{
		public Akte()
		{
		}
		public int ID { get; set; }
		public string Name { get; set; }
		Kunde mandant = new Kunde { Vorname = "Hans", Nachname = "Müller" };
		public Kunde Mandant { get { return mandant; } }
		public DateTime Datum { get; set; }

		public void Rechnungen_Löschen(ObservableCollection<Rechnung> rechnungen)
		{
			MessageBox.Show("Löschen: " + rechnungen.Count + " Rechnungen");
			rechnungen.Clear();
			MessageBox.Show("Gelöscht: " + rechnungen.Count + " Rechnungen");
		}

		public void Briefe_Schreiben(ObservableCollection<Brief> briefe, Kunde autor)
		{
			MessageBox.Show(briefe.Count + " Briefe?");
			briefe.Add(new Brief(autor) { Inhalt = "Hallo Welt" });
		}

		public void Info()
		{
			MessageBox.Show(Name);
		}

		public static void Alle_Löschen(ObservableCollection<Akte> akten)
		{
			akten.Clear();
		}

		public static void Neu_Erzeugen(ObservableCollection<Akte> akten, int wieviele)
		{
			for (int i = 0; i < wieviele; i++)
			{
				akten.Add(new Akte { Name = "Neue Akte " + (i + 1), Datum = DateTime.Now });
			}
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
		public bool BezahltSchreibgeschützt { get { return Bezahlt; } }
		public IEnumerable<string> Strings { get; set; }
		public IEnumerable<decimal> Decimals { get; set; }

		public void Erhöhen()
		{
			Betrag += 1;
		}
	}

	public class Kunde
	{
		public bool Geändert { get; set; }
		public string Vorname { get; set; }
		public string Nachname { get; set; }
		public Kunde Vertreter { get; set; }
		public List<Kunde> Freunde { get; set; }

		public override string ToString()
		{
			return (Vorname + " " + Nachname).Trim();
		}

		public static void Alle_Ungeändert(ObservableCollection<Kunde> kunden)
		{
			foreach (var kunde in kunden)
			{
				kunde.Geändert = false;
			}
		}

		public List<Kunde> Neuer_Freund()
		{
			var kunde = new Kunde { Vorname = "Neuer" + DateTime.Now.Ticks, Nachname = "Freund" + DateTime.Now.Ticks };
			Freunde.Add(kunde);

			return new List<Kunde> { kunde };
		}

		public void Neuer_Freund(Kunde freund)
		{
			Freunde.Add(freund);
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

		public void Hallo_Sagen(List<Kunde> kunden)
		{
			MessageBox.Show("Hallo " + string.Join(", ", kunden.ConvertAll(k => k.Vorname + " " + k.Nachname)));
		}

		public Brief Schreiben(string inhalts_text, bool wirklich)
		{
			return new Brief(this) { Inhalt = inhalts_text };
		}
	}

	public class Brief
	{
		public Brief(Kunde autor)
		{
			Geschrieben_von = autor;
		}

		public string Inhalt { get; set; }

		public Kunde Geschrieben_von { get; private set; }

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
