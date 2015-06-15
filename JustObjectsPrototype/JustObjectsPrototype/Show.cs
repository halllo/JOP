using JustObjectsPrototype.UI;
using System;
using System.Collections.Generic;

namespace JustObjectsPrototype
{
	public static class Show
	{
		public static void With(ICollection<object> objects, List<Type> types = null)
		{
			var windowModel = new MainWindowModel(objects, types)
			{
				ShowMethodInvocationDialog = dataContext =>
				{
					var dialog = new MethodInvocationDialog { Title = dataContext.ToString() };
					dialog.ShowDialog();
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
