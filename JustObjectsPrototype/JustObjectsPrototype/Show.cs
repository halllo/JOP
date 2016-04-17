using JustObjectsPrototype.UI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace JustObjectsPrototype
{
	public static class Show
	{
		public static Prototype With(params IEnumerable<object>[] objects)
		{
			return new PrototypeBuilder().With(objects);
		}
		public static Prototype With(ICollection<object> objects)
		{
			return new PrototypeBuilder().With(objects);
		}
		public static PrototypeBuilder<T> ViewOf<T>()
		{
			return new PrototypeBuilder<T>
			{
				Settings = new Settings
				{
					DisplayedTypes = new List<Type> { typeof(T) }
				}
			};
		}
		public static Prototype EmptyViewOf<T>()
		{
			return new PrototypeBuilder<T>
			{
				Settings = new Settings
				{
					DisplayedTypes = new List<Type> { typeof(T) }
				}
			}.With(new List<object>());
		}


		public class PrototypeBuilder
		{
			internal PrototypeBuilder()
			{
				Settings = new Settings();
			}

			internal Settings Settings { get; set; }

			public Prototype With(params IEnumerable<object>[] objects)
			{
				if (objects.Length == 0 || objects.Any(o => o == null)) throw new ArgumentNullException();

				var collection = new ObservableCollection<object>(objects.Aggregate((o1, o2) => o1.Concat(o2)));
				return With(collection); ;
			}
			public Prototype With(ICollection<object> objects)
			{
				if (objects.Any(o => o == null)) throw new ArgumentNullException();

				var windowModel = new MainWindowModel(objects, Settings)
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
					Repository = objects is ObservableCollection<object> ? objects as ObservableCollection<object> : new ObservableCollection<object>(objects)
				};
			}
		}

		public class PrototypeBuilder<T> : PrototypeBuilder
		{
			internal PrototypeBuilder()
			{
			}

			public PrototypeBuilder<TNext> ViewOf<TNext>()
			{
				Settings.DisplayedTypes.Add(typeof(TNext));

				return new PrototypeBuilder<TNext>
				{
					Settings = Settings
				};
			}
			public PrototypeBuilder<T> DisableNew()
			{
				Settings.AllowNew[typeof(T)] = false;
				return this;
			}
			public PrototypeBuilder<T> EnableNew(Action<T> afterNew)
			{
				Settings.AllowNew[typeof(T)] = true;
				Settings.NewEvents[typeof(T)] = new Action<object>(o => afterNew((T)o));
				return this;
			}
			public PrototypeBuilder<T> DisableDelete()
			{
				Settings.AllowDelete[typeof(T)] = false;
				return this;
			}
			public PrototypeBuilder<T> EnableDelete(Action<T> afterDelete)
			{
				Settings.AllowDelete[typeof(T)] = true;
				Settings.DeleteEvents[typeof(T)] = new Action<object>(o => afterDelete((T)o));
				return this;
			}
			public PrototypeBuilder<T> OnValueChanged(Action<T> afterValueChanged)
			{
				Settings.ChangeEvents[typeof(T)] = new Action<object>(o => afterValueChanged((T)o));
				return this;
			}
		}
	}
}
