using System;
using System.Reflection;

namespace JustObjectsPrototype.UI
{
	public interface IPropertyViewModel
	{
		string Label { get; }
	}

	public class TextPropertyViewModel : IPropertyViewModel
	{
		object _Instance;
		PropertyInfo _Property;
		Action _ChangeCallback;

		public TextPropertyViewModel(object instance, PropertyInfo property, Action changeCallback = null)
		{
			_Instance = instance;
			_Property = property;
		}

		public string Label { get { return _Property.Name; } }

		public string Value
		{
			get { return (_Property.GetValue(_Instance) ?? "").ToString(); }
			set
			{
				_Property.SetValue(_Instance, value);
				if (_ChangeCallback != null) _ChangeCallback();
			}
		}
	}

	public class ReferencePropertyViewModel : IPropertyViewModel
	{
		public string Label { get; set; }
		public object Reference { get; set; }
	}
}
