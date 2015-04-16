using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Domain
{
	public class Akte
	{
		public string Name { get; set; }
	}

	public class Rechnung
	{
		public Kunde Empfänger { get; set; }
		public decimal Betrag { get; set; }
	}

	public class Kunde
	{
		public string Vorname { get; set; }
		public string Nachname { get; set; }
	}
}
