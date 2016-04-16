using System.Collections.ObjectModel;

namespace JustObjectsPrototype
{
	public class Prototype
	{
		internal Prototype()
		{
		}

		public ObservableCollection<object> Repository { get; internal set; }
	}
}
