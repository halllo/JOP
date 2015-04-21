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
		Dictionary<Type, ObservableCollection<object>> _ObjectsByTypes;

		public Objects(ICollection<object> objects)
		{
			_Objects = objects;
			if (_Objects is ObservableCollection<object>)
			{
				((ObservableCollection<object>)_Objects).CollectionChanged += objectlist_CollectionChanged;
			}

			var objectsByType = objects.ToLookup(o => o.GetType());
			var typesAndObjects = objectsByType.Select(t => t.Key).ToDictionary(t => t, t =>
			{
				var observableCollection = new ObservableCollection<object>(objectsByType[t]);
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

		public ObservableCollection<object> OfType(Type type)
		{
			if (_ObjectsByTypes.ContainsKey(type) == false)
			{
				var observableCollection = new ObservableCollection<object>();
				observableCollection.CollectionChanged += typedictionaryobjectlist_CollectionChanged;

				_ObjectsByTypes.Add(type, observableCollection);
				_Types.Add(type);
			}
			return _ObjectsByTypes[type];
		}

		void objectlist_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			var observableCollection = sender as ObservableCollection<object>;
			if (e.Action == NotifyCollectionChangedAction.Add)
			{
				foreach (var newItem in e.NewItems)
				{
					var type = newItem.GetType();
					if (_ObjectsByTypes.ContainsKey(type) == false)
					{
						_ObjectsByTypes.Add(type, new ObservableCollection<object>());
						_Types.Add(type);
					}
					if (_ObjectsByTypes[type].Contains(newItem) == false)
					{
						_ObjectsByTypes[type].Add(newItem);
					}
				}
			}
			if (e.Action == NotifyCollectionChangedAction.Remove)
			{
				foreach (var oldItem in e.OldItems)
				{
					var type = oldItem.GetType();
					if (_ObjectsByTypes.ContainsKey(type) == true && _ObjectsByTypes[type].Contains(oldItem) == true)
					{
						_ObjectsByTypes[type].Remove(oldItem);
					}
				}
			}
		}

		void typedictionaryobjectlist_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			var observableCollection = sender as ObservableCollection<object>;
			if (e.Action == NotifyCollectionChangedAction.Add)
			{
				foreach (var newItem in e.NewItems)
				{
					if (_Objects.Contains(newItem) == false)
					{
						_Objects.Add(newItem);
					}
				}
			}
			if (e.Action == NotifyCollectionChangedAction.Remove)
			{
				foreach (var oldItem in e.OldItems)
				{
					if (_Objects.Contains(oldItem) == true)
					{
						_Objects.Remove(oldItem);
					}
				}
			}
		}
	}
}
