using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace JustObjectsPrototype.UI.Editors
{
	public class ReferenceTypePropertyViewModel : ViewModel, IPropertyViewModel
	{
		public static object NullEntry = " ";

		public ObjectProxy Instance { private get; set; }
		public PropertyInfo Property { private get; set; }
		public IEnumerable<object> Objects { private get; set; }
		public Action ChangeCallback { private get; set; }

		public bool CanWrite
		{
			get { return Property.CanWrite; }
		}

		public string Label { get { return Property.Name; } }

		public IEnumerable<object> References
		{
			get
			{
				return Enumerable.Concat(new[] { NullEntry, Value ?? NullEntry }, Objects).Distinct();
			}
		}

		public Type ValueType { get { return Property.PropertyType; } }

		public object Value
		{
			get
			{
				return Property.GetValue(Instance.ProxiedObject);
			}
			set
			{
				if (value == NullEntry) value = null;

				Property.SetValue(Instance.ProxiedObject, value);
				Instance.RaisePropertyChanged(Property.Name);
			}
		}

		public void RaiseChanged()
		{
			Changed(() => Value);
		}
	}
}
