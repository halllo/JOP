﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace JustObjectsPrototype.UI.Editors
{
	public class ReferenceTypePropertyViewModel : IPropertyViewModel
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
				return Enumerable.Concat(new[] { NullEntry, Reference ?? NullEntry }, Objects).Distinct();
			}
		}

		public object Reference
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
	}
}
