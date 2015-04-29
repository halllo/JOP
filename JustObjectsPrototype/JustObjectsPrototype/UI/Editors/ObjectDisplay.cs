using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;

namespace JustObjectsPrototype.UI.Editors
{
	public class ObjectDisplay : TextBlock
	{
		public object DisplayObject
		{
			get { return (object)GetValue(DisplayObjectProperty); }
			set { SetValue(DisplayObjectProperty, value); }
		}

		public static readonly DependencyProperty DisplayObjectProperty = DependencyProperty.Register("DisplayObject", typeof(object), typeof(ObjectDisplay), new PropertyMetadata(null, DisplayObjectChanged));

		private static void DisplayObjectChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) 
		{
			var od = sender as ObjectDisplay;
			if (e.NewValue == null || (e.NewValue != null && e.NewValue == ReferenceTypePropertyViewModel.NullEntry))
				return;

			od.Text = ToStringOrJson(e.NewValue);
		}

		public static string ToStringOrJson(object value)
		{
			var type = value.GetType();
			var toString = type.GetMethod("ToString", BindingFlags.DeclaredOnly);
			if (toString != null)
			{
				return toString.Invoke(value, new object[0]).ToString();
			}

			var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(p => p.GetIndexParameters().Length == 0);
			return "{" + string.Join(", ", properties.Select(p => p.Name + ": \"" + p.GetValue(value) + "\"")) + "}";
		}
	}
}
