using JustObjectsPrototype.UI;
using JustObjectsPrototype.UI.Editors;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace JustObjectsPrototype
{
	public class Prototype
	{
		internal Prototype()
		{
		}

		public ICollection<object> Repository { get; internal set; }


		public static Prototype Show(params IEnumerable<object>[] with)
		{
			return JustObjectsPrototype.Show.Prototype(With.Objects(with));
		}
		public static Prototype Show(ICollection<object> with)
		{
			return JustObjectsPrototype.Show.Prototype(With.Objects(with));
		}
		public static Prototype Show(PrototypeBuilder with)
		{
			return JustObjectsPrototype.Show.Prototype(with);
		}
	}

	public static class Show
	{
		public static Prototype Prototype(params IEnumerable<object>[] with)
		{
			return Prototype(With.Objects(with));
		}
		public static Prototype Prototype(ICollection<object> with)
		{
			return Prototype(With.Objects(with));
		}
		public static Prototype Prototype(PrototypeBuilder with)
		{
			var objects = with.Repository;
			if (objects.Any(o => o == null)) throw new ArgumentNullException();

			var windowModel = new MainWindowModel(objects, with.Settings)
			{
				ShowMethodInvocationDialog = dataContext =>
				{
					var dialog = new MethodInvocationDialog { DataContext = dataContext };
					return dialog.ShowDialog();
				}
			};
			var window = new MainWindow
			{
				DataContext = windowModel
			};
			window.ShowDialog();

			return new Prototype
			{
				Repository = objects
			};
		}
	}

	public static class With
	{
		public static PrototypeBuilder<TNext> SettingsFor<TNext>()
		{
			var builder = new PrototypeBuilder();
			return builder.AndSettingsFor<TNext>();
		}
		public static PrototypeBuilder<TNext> ViewOf<TNext>()
		{
			var builder = new PrototypeBuilder();
			return builder.AndViewOf<TNext>();
		}
		public static PrototypeBuilder Objects(params IEnumerable<object>[] objects)
		{
			var builder = new PrototypeBuilder();
			builder.AndObjects(objects);
			return builder;
		}
		public static PrototypeBuilder These(params IEnumerable<object>[] objects)
		{
			var builder = new PrototypeBuilder();
			builder.AndThese(objects);
			return builder;
		}
		public static PrototypeBuilder Objects(ICollection<object> objects)
		{
			var builder = new PrototypeBuilder();
			builder.AndObjects(objects);
			return builder;
		}
		public static PrototypeBuilder These(ICollection<object> objects)
		{
			var builder = new PrototypeBuilder();
			builder.AndThese(objects);
			return builder;
		}
	}
	public class PrototypeBuilder
	{
		internal PrototypeBuilder()
		{
			Settings = new Settings();
			Repository = new List<object>();
		}
		internal Settings Settings { get; set; }
		internal ICollection<object> Repository { get; set; }

		public PrototypeBuilder<TNext> AndSettingsFor<TNext>()
		{
			return new PrototypeBuilder<TNext>
			{
				Settings = Settings,
				Repository = Repository,
			};
		}
		public PrototypeBuilder<TNext> AndViewOf<TNext>()
		{
			Settings.DisplayedTypes.Add(typeof(TNext));

			return new PrototypeBuilder<TNext>
			{
				Settings = Settings,
				Repository = Repository,
			};
		}
		public PrototypeBuilder AndObjects(params IEnumerable<object>[] objects)
		{
			Repository = new ObservableCollection<object>(objects.SelectMany(l => l));
			return this;
		}
		public PrototypeBuilder AndThese(params IEnumerable<object>[] objects)
		{
			Repository = new ObservableCollection<object>(objects.SelectMany(l => l));
			return this;
		}
		public PrototypeBuilder AndObjects(ICollection<object> objects)
		{
			Repository = objects;
			return this;
		}
		public PrototypeBuilder AndThese(ICollection<object> objects)
		{
			Repository = objects;
			return this;
		}
	}
	public class PrototypeBuilder<T> : PrototypeBuilder
	{
		internal PrototypeBuilder()
		{
		}

		/// <summary>
		/// Setup to not support object creation via ribbon.
		/// </summary>
		/// <returns></returns>
		public PrototypeBuilder<T> DisableNew()
		{
			Settings.AllowNew[typeof(T)] = false;
			return this;
		}
		/// <summary>
		/// Setup to support object creation via ribbon.
		/// </summary>
		/// <param name="afterNew">Callback to extend object creation.</param>
		/// <returns></returns>
		public PrototypeBuilder<T> EnableNew(Action<T> afterNew = null)
		{
			Settings.AllowNew[typeof(T)] = true;
			if (afterNew != null)
			{
				Settings.NewEvents[typeof(T)] = new Action<object>(o => afterNew((T)o));
			}
			return this;
		}
		/// <summary>
		/// Setup to not support object deletion via ribbon.
		/// </summary>
		/// <returns></returns>
		public PrototypeBuilder<T> DisableDelete()
		{
			Settings.AllowDelete[typeof(T)] = false;
			return this;
		}
		/// <summary>
		/// Setup to support object deletion via ribbon.
		/// </summary>
		/// <param name="afterDelete">Callback to extend object deletion.</param>
		/// <returns></returns>
		public PrototypeBuilder<T> EnableDelete(Action<T> afterDelete = null)
		{
			Settings.AllowDelete[typeof(T)] = true;
			Settings.DeleteEvents[typeof(T)] = new Action<object>(o => afterDelete((T)o));
			return this;
		}
		/// <summary>
		/// Extend object change via detail view.
		/// </summary>
		/// <param name="afterValueChanged">Callback to extend object change.</param>
		/// <returns></returns>
		public PrototypeBuilder<T> OnValueChanged(Action<T> afterValueChanged)
		{
			Settings.ChangeEvents[typeof(T)] = new Action<ObjectChangedEventArgs>(o =>
			{
				afterValueChanged((T)o.Object);
			});
			return this;
		}
	}
}
