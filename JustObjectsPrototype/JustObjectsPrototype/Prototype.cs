using System.Collections.Generic;

namespace JustObjectsPrototype
{
	public class Prototype
	{
		internal Prototype()
		{
		}

		public ICollection<object> Repository { get; internal set; }
	}
}
