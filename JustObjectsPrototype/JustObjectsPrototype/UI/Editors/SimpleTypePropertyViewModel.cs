using System;
using System.Reflection;

namespace JustObjectsPrototype.UI.Editors
{
	public class DateTimePropertyViewModel : SimpleTypePropertyViewModel { }

	public class SimpleTypePropertyViewModel : IPropertyViewModel
	{
		public ObjectProxy Instance { private get; set; }
		public PropertyInfo Property { private get; set; }
		public Action ChangeCallback { private get; set; }

		public bool CanWrite
		{
			get { return Property.CanWrite; }
		}

		public string Label { get { return Property.Name; } }

		public object Value
		{
			get
			{
				return Property.GetValue(Instance.ProxiedObject);
			}
			set
			{
				try
				{
					var convertedValue = Convert.ChangeType(value, Property.PropertyType);
					Property.SetValue(Instance.ProxiedObject, convertedValue);
					Instance.RaisePropertyChanged(Property.Name);
					if (ChangeCallback != null) ChangeCallback();
				}
				catch (Exception e)
				{
					System.Windows.MessageBox.Show("Assignment error: " + e.Message);
				}
			}
		}
	}
}
