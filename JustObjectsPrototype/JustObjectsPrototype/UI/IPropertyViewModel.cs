using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace JustObjectsPrototype.UI
{
	public interface IPropertyViewModel
	{
		string Label { get; }
	}

	public class DisplayPropertyViewModel : IPropertyViewModel
	{
		ObjectProxy _Instance;
		PropertyInfo _Property;
		
		public DisplayPropertyViewModel(ObjectProxy instance, PropertyInfo property)
		{
			_Instance = instance;
			_Property = property;
		}

		public string Label { get { return _Property.Name; } }

		public string Value
		{
			get
			{
				return (_Property.GetValue(_Instance.ProxiedObject) ?? "").ToString();
			}
		}
	}

	public class TextPropertyViewModel : IPropertyViewModel
	{
		ObjectProxy _Instance;
		PropertyInfo _Property;
		Action _ChangeCallback;

		public TextPropertyViewModel(ObjectProxy instance, PropertyInfo property, Action changeCallback = null)
		{
			_Instance = instance;
			_Property = property;
			_ChangeCallback = changeCallback;
		}

		public string Label { get { return _Property.Name; } }

		public string Value
		{
			get
			{
				return (_Property.GetValue(_Instance.ProxiedObject) ?? "").ToString();
			}
			set
			{
				_Property.SetValue(_Instance.ProxiedObject, value);
				_Instance.RaisePropertyChanged(_Property.Name);
				if (_ChangeCallback != null) _ChangeCallback();
			}
		}
	}

	public class ReferencePropertyViewModel : IPropertyViewModel
	{
		ObjectProxy _Instance;
		PropertyInfo _Property;
		
		IEnumerable<object> _Objects;

		public static object NullEntry = " ";

		public ReferencePropertyViewModel(ObjectProxy instance, PropertyInfo property, IEnumerable<object> objects)
		{
			_Instance = instance;
			_Property = property;

			_Objects = objects;
		}

		public string Label { get { return _Property.Name; } }

		public IEnumerable<object> References
		{
			get
			{
				return Enumerable.Concat(new[] { NullEntry }, _Objects);
			}
		}

		public object Reference
		{
			get
			{
				return _Property.GetValue(_Instance.ProxiedObject);
			}
			set
			{
				if (value == NullEntry) value = null;

				_Property.SetValue(_Instance.ProxiedObject, value);
				_Instance.RaisePropertyChanged(_Property.Name);
			}
		}
	}
}
