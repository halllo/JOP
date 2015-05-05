using JustObjectsPrototype.UI;
using System;
using System.Collections.Generic;

namespace JustObjectsPrototype
{
	public static class Show
	{
		public static void With(ICollection<object> objects, List<Type> types = null)
		{
			var window = new MainWindow
			{
				DataContext = new MainWindowModel(objects, types)
			};
			window.ShowDialog();
		}
	}
}
