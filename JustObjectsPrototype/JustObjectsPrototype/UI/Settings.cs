using System;
using System.Collections.Generic;

namespace JustObjectsPrototype.UI
{
	public class Settings
	{
		public static Settings New(Action<Settings> setup)
		{
			var settings = new Settings();
			setup(settings);
			return settings;
		}

		public static Settings Default { get { return new Settings(); } }
		private Settings()
		{
			DisplayedTypes = new List<Type>();
			AllowNew = new Dictionary<Type, bool>();
			AllowDelete = new Dictionary<Type, bool>();
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
	}
}
