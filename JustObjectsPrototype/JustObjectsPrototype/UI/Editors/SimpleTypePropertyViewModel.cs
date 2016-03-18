using System;
using System.Reflection;

namespace JustObjectsPrototype.UI.Editors
{
	public class DateTimePropertyViewModel : SimpleTypePropertyViewModel { }

	public class BooleanPropertyViewModel : SimpleTypePropertyViewModel { }

	public class SimpleTypePropertyViewModel : ViewModel, IPropertyViewModel
	{
		ObjectProxy _Instance;
		public ObjectProxy Instance
		{
			private get { return _Instance; }
			set
			{
				_Instance = value;
				_Instance.PropertyChanged += (s, e) =>
				{
					if (e.PropertyName == Property.Name || e.PropertyName == string.Empty)
					{
						Changed(() => Value);
					}
				};
			}
		}
		public PropertyInfo Property { private get; set; }
		public Action ChangeCallback { private get; set; }

		public bool CanWrite
		{
			get { return Property.CanWrite && Property.SetMethod.IsPublic; }
		}

		public string Label { get { return ObjectDisplay.Nicely(Property); } }

		public Type ValueType { get { return Property.PropertyType; } }

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
					Instance.RaisePropertyChanged(string.Empty);
					if (ChangeCallback != null) ChangeCallback();
				}
				catch (Exception e)
				{
					System.Windows.MessageBox.Show("Assignment error: " + e.Message);
				}
			}
		}

		public void Refresh()
		{
			Changed(() => Value);
		}
	}
}
