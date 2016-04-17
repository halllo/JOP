using JustObjectsPrototype.UI.Editors;
using System;
using System.Collections.Generic;

namespace JustObjectsPrototype.UI
{
	public class Settings
	{
		public Settings()
		{
			DisplayedTypes = new List<Type>();

			AllowNew = new Dictionary<Type, bool>();
			AllowDelete = new Dictionary<Type, bool>();

			NewEvents = new Dictionary<Type, object>();
			DeleteEvents = new Dictionary<Type, object>();
			ChangeEvents = new Dictionary<Type, Action<ObjectChangedEventArgs>>();
		}

		public List<Type> DisplayedTypes { get; set; }

		public Dictionary<Type, bool> AllowNew { get; set; }
		internal bool IsAllowNew(Type type)
		{
			return AllowNew.ContainsKey(type) == false
				|| (AllowNew.ContainsKey(type) && AllowNew[type]);
		}

		public Dictionary<Type, bool> AllowDelete { get; set; }
		internal bool IsAllowDelete(Type type)
		{
			return AllowDelete.ContainsKey(type) == false
				|| (AllowDelete.ContainsKey(type) && AllowDelete[type]);
		}


		public Dictionary<Type, object> NewEvents { get; set; }
		internal void InvokeNewEvents(object obj)
		{
			InvokeEventsIfExist(NewEvents, obj);
		}

		public Dictionary<Type, object> DeleteEvents { get; set; }
		internal void InvokeDeleteEvents(object obj)
		{
			InvokeEventsIfExist(DeleteEvents, obj);
		}

		public Dictionary<Type, Action<ObjectChangedEventArgs>> ChangeEvents { get; set; }
		internal void InvokeChangeEvents(ObjectChangedEventArgs obj)
		{
			var type = obj.Object.GetType();
			if (ChangeEvents.ContainsKey(type))
			{
				try
				{
					ChangeEvents[type].Invoke(obj);
				}
				catch (Exception)
				{
				}
			}
		}

		private static void InvokeEventsIfExist(Dictionary<Type, object> events, object o)
		{
			var type = o.GetType();
			if (events.ContainsKey(type))
			{
				try
				{
					(events[type] as Action<object>).Invoke(o);
				}
				catch (Exception)
				{
				}
			}
		}
	}
}
