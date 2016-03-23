using JustObjectsPrototype.UI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace JustObjectsPrototype
{
	public static class Show
	{
		public static ObservableCollection<object> With(params IEnumerable<object>[] objects)
		{
			var collection = new ObservableCollection<object>(objects.Aggregate((o1, o2) => o1.Concat(o2)));
			With(collection, null);
			return collection;
		}

		public static void With(ICollection<object> objects, Settings settings = null)
		{
			if (objects.Any(o => o == null)) throw new ArgumentNullException();

			var windowModel = new MainWindowModel(objects, settings ?? Settings.Default)
			{
				ShowMethodInvocationDialog = dataContext =>
				{
					var dialog = new MethodInvocationDialog { DataContext = dataContext };
					return dialog.ShowDialog();
				}
			};
			var window = new MainWindow
			{
				DataContext = windowModel
			};
			window.ShowDialog();
		}
	}
}
