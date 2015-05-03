using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;

namespace JustObjectsPrototype.UI.Editors
{
	public class ReferenceTypeListPropertyViewModel : IPropertyViewModel
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
				var refs = Enumerable.Concat
				(
					Enumerable.Concat
					(
						new[] { NullEntry },
						((IEnumerable)Property.GetValue(Instance.ProxiedObject)).OfType<object>()
					),
					Objects
				).Distinct();
				return refs;
			}
		}

		public class ReferenceTypeWrapper
		{
			public Action ValueChanged { get; set; }
			object _value;
			public object Value
			{
				get { return _value; }
				set
				{
					_value = value;
					if (ValueChanged != null) ValueChanged();
				}
			}

			public object Value2
			{
				get { return Value.ToString(); }
				set { string s = value.ToString(); }
			}

			public object References { get; set; }
		}

		ObservableCollection<ReferenceTypeWrapper> collection;
		Type collectionItemType;

		public object Value
		{
			get
			{
				if (collection == null)
				{
					collectionItemType = Property.PropertyType.IsGenericType ? Property.PropertyType.GenericTypeArguments[0] : null;
					var values = (IEnumerable)Property.GetValue(Instance.ProxiedObject);
					var wrapper = Enumerable.Empty<ReferenceTypeWrapper>();
					if (values != null)
					{
						wrapper = values.OfType<object>().Select(s => new ReferenceTypeWrapper
						{
							Value = s,
							References = References,
							ValueChanged = Assign
						});
					}
					collection = new ObservableCollection<ReferenceTypeWrapper>(wrapper);
					collection.CollectionChanged += collection_CollectionChanged;
				}
				return collection;
			}
		}

		void collection_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			Assign();
		}

		private void Assign()
		{
			try
			{
				var canBeNull = !collectionItemType.IsValueType || (Nullable.GetUnderlyingType(collectionItemType) != null);
				var listType = typeof(List<>);
				var constructedListType = listType.MakeGenericType(collectionItemType);
				var list = (IList)Activator.CreateInstance(constructedListType);
				foreach (var item in collection)
				{
					if (item.ValueChanged == null) item.ValueChanged = Assign;
					if (item.References == null) item.References = References;
					if (!canBeNull && item.Value == null) continue;
					list.Add(Convert.ChangeType(item.Value, collectionItemType ?? typeof(object)));
				}

				Property.SetValue(Instance.ProxiedObject, list);
				Instance.RaisePropertyChanged(Property.Name);
				if (ChangeCallback != null) ChangeCallback();
			}
			catch (Exception ex)
			{
				System.Windows.MessageBox.Show("Assignment error: " + ex.Message);
			}
		}
	}
}
