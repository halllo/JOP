using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

			var objectsByType = objects.ToLookup(o => o.GetType());
			var typesAndObjects = objectsByType.Select(t => t.Key).ToDictionary(t => t, t => new ObservableCollection<object>(objectsByType[t]));

			_ObjectsByTypes = typesAndObjects;
			_Types = new ObservableCollection<Type>(typesAndObjects.Keys);
		}

		public ObservableCollection<Type> Types
		{
			get { return _Types; }
		}

		public void Add(object added)
		{
			_Objects.Add(added);

			var type = added.GetType();
			if (_ObjectsByTypes.ContainsKey(type) == false)
			{
				_ObjectsByTypes.Add(type, new ObservableCollection<object>());
				_Types.Add(type);
			}
			_ObjectsByTypes[type].Add(added);
		}

		public void Remove(object removed)
		{
			_Objects.Remove(removed);

			var type = removed.GetType();
			if (_ObjectsByTypes.ContainsKey(type))
			{
				_ObjectsByTypes[type].Remove(removed);
			}
		}

		public IEnumerable<object> All
		{
			get { return _Objects; }
		}

		public ObservableCollection<object> OfType(Type type)
		{
			if (_ObjectsByTypes.ContainsKey(type))
			{
				return _ObjectsByTypes[type];
			}
			else
			{
				return new ObservableCollection<object>();
			}
		}
	}
}
