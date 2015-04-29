using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Domain
{
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
	}
}
