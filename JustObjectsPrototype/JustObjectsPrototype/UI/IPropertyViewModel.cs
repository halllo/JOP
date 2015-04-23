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
		ObjectProxy _Instance;
		PropertyInfo _Property;

		public TextPropertyViewModel(ObjectProxy instance, PropertyInfo property)
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
			set
			{
				_Property.SetValue(_Instance.ProxiedObject, value);
				_Instance.RaisePropertyChanged(_Property.Name);
			}
		}
	}

	public class ReferencePropertyViewModel : IPropertyViewModel
	{
		ObjectProxy _Instance;
		PropertyInfo _Property;

		public ReferencePropertyViewModel(ObjectProxy instance, PropertyInfo property)
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
			set
			{
				_Property.SetValue(_Instance.ProxiedObject, value);
				_Instance.RaisePropertyChanged(_Property.Name);
			}
		}
	}
}
