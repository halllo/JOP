using JustObjectsPrototype.UI;
using System;
using System.Collections.Generic;

namespace JustObjectsPrototype
{
	public static class Show
	{
		public static void With(ICollection<object> objects, List<Type> types = null, Settings settings = null)
		{
			var windowModel = new MainWindowModel(objects, types, settings ?? Settings.Default)
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
