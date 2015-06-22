using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

namespace JustObjectsPrototype
{
	public class Objects
	{
		ICollection<object> _Objects;

		ObservableCollection<Type> _Types;
		Dictionary<Type, ObservableCollection<ObjectProxy>> _ObjectsByTypes;
		Dictionary<object, ObjectProxy> _ObjectToProxy;

		public Objects(ICollection<object> objects)
		{
			_Objects = objects;
			if (_Objects is ObservableCollection<object>)
			{
				((ObservableCollection<object>)_Objects).CollectionChanged += objectlist_CollectionChanged;
			}

			_ObjectToProxy = objects.ToDictionary(o => o, o => new ObjectProxy(o));

			var objectsByType = objects.ToLookup(o => o.GetType());
			var typesAndObjects = objectsByType.Select(t => t.Key).ToDictionary(t => t, t =>
			{
				var observableCollection = new ObservableCollection<ObjectProxy>(objectsByType[t].Select(o => _ObjectToProxy[o]));
				observableCollection.CollectionChanged += typedictionaryobjectlist_CollectionChanged;
				return observableCollection;
			});

			_ObjectsByTypes = typesAndObjects;
			_Types = new ObservableCollection<Type>(typesAndObjects.Keys);
		}

		public ObservableCollection<Type> Types
		{
			get { return _Types; }
		}

		public IEnumerable<object> All
		{
			get { return _Objects; }
		}

		public ObservableCollection<ObjectProxy> OfType(Type type)
		{
			if (_ObjectsByTypes.ContainsKey(type) == false)
			{
				var observableCollection = new ObservableCollection<ObjectProxy>();
				observableCollection.CollectionChanged += typedictionaryobjectlist_CollectionChanged;

				_ObjectsByTypes.Add(type, observableCollection);
				_Types.Add(type);
			}
			return _ObjectsByTypes[type];
		}

		public object OfType_OneWayToSourceChangePropagation(Type type)
		{
			return typeof(Objects)
				.GetMethods()
					.First(m => m.IsGenericMethod && m.Name == "OfType_OneWayToSourceChangePropagation")
				.MakeGenericMethod(type)
				.Invoke(this, null);
		}

		public ObservableCollection<T> OfType_OneWayToSourceChangePropagation<T>()
		{
			var observableCollection = new ObservableCollection<T>(OfType(typeof(T)).Select(o => (T)o.ProxiedObject));
			observableCollection.CollectionChanged += objectlist_CollectionChanged;

			return observableCollection;
		}

		void objectlist_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (e.Action == NotifyCollectionChangedAction.Add)
			{
				foreach (var newItem in e.NewItems)
				{
					var type = newItem.GetType();
					if (_ObjectsByTypes.ContainsKey(type) == false)
					{
						var newTypeObjectsList = new ObservableCollection<ObjectProxy>();
						newTypeObjectsList.CollectionChanged += typedictionaryobjectlist_CollectionChanged;
						_ObjectsByTypes.Add(type, newTypeObjectsList);
						_Types.Add(type);
					}
					if (_ObjectToProxy.ContainsKey(newItem) == false)
					{
						var newProxy = new ObjectProxy(newItem);
						_ObjectToProxy.Add(newItem, newProxy);

						_ObjectsByTypes[type].Add(newProxy);
					}
				}
			}
			if (e.Action == NotifyCollectionChangedAction.Remove)
			{
				foreach (var oldItem in e.OldItems)
				{
					var type = oldItem.GetType();
					if (_ObjectsByTypes.ContainsKey(type) == true && _ObjectToProxy.ContainsKey(oldItem) == true)
					{
						var oldProxy = _ObjectToProxy[oldItem];
						_ObjectToProxy.Remove(oldItem);

						_ObjectsByTypes[type].Remove(oldProxy);
					}
				}
			}
			if (e.Action == NotifyCollectionChangedAction.Reset)//clear
			{
				if (_Objects == sender)
				{
					foreach (var type in Types.ToList())
					{
						if (_ObjectsByTypes[type].Any()) _ObjectsByTypes[type].Clear();
					}
				}
				else
				{
					var type = sender.GetType().GetGenericArguments().First();
					if (_ObjectsByTypes[type].Any()) _ObjectsByTypes[type].Clear();
				}
			}
		}

		void typedictionaryobjectlist_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (e.Action == NotifyCollectionChangedAction.Add)
			{
				foreach (var newItem in e.NewItems.Cast<ObjectProxy>())
				{
					if (_ObjectToProxy.ContainsKey(newItem.ProxiedObject) == false)
					{
						_ObjectToProxy.Add(newItem.ProxiedObject, newItem);
					}
					if (_Objects.Contains(newItem.ProxiedObject) == false)
					{
						_Objects.Add(newItem.ProxiedObject);
					}
				}
			}
			if (e.Action == NotifyCollectionChangedAction.Remove)
			{
				foreach (var oldItem in e.OldItems.Cast<ObjectProxy>())
				{
					if (_ObjectToProxy.ContainsKey(oldItem.ProxiedObject) == true)
					{
						_ObjectToProxy.Remove(oldItem.ProxiedObject);
					}
					if (_Objects.Contains(oldItem.ProxiedObject) == true)
					{
						_Objects.Remove(oldItem.ProxiedObject);
					}
				}
			}
			if (e.Action == NotifyCollectionChangedAction.Reset)//clear
			{
				var type = _ObjectsByTypes.First(kvp => kvp.Value == sender).Key;

				foreach (var kvp in _ObjectToProxy.Where(p => p.Key.GetType() == type).ToList())
				{
					_ObjectToProxy.Remove(kvp.Key);
				}
				foreach (var o in _Objects.Where(o => o.GetType() == type).ToList())
				{
					_Objects.Remove(o);
				}
			}
		}
	}
}
