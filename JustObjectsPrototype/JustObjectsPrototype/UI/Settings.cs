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
			ChangedEvents = new Dictionary<Type, object>();
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
		public Dictionary<Type, object> DeleteEvents { get; set; }
		public Dictionary<Type, object> ChangedEvents { get; set; }
	}
}
