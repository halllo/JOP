using JustObjectsPrototype.UI;
using JustObjectsPrototype.UI.Editors;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace JustObjectsPrototype
{
	/// <summary>
	/// Setup the prototype with .For() or .ViewOf().
	/// Invoke the prototype with .With(objects).
	/// </summary>
	public static class Show
	{
		/// <summary>
		/// Start setup for type without limiting displayed types.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public static PrototypeBuilder<T> For<T>()
		{
			return new PrototypeBuilder<T>
			{
				Settings = new Settings
				{
					DisplayedTypes = new List<Type> { }
				}
			};
		}
		/// <summary>
		/// Start setup for type and limit displayed types to T.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
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
		/// <summary>
		/// Show prototype with no objects and limit displayed types to T.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
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
		/// <summary>
		/// Show prototype with objects from all passed in object collections without limiting displayed types.
		/// </summary>
		/// <param name="objects"></param>
		/// <returns></returns>
		public static Prototype With(params IEnumerable<object>[] objects)
		{
			return new PrototypeBuilder().With(objects);
		}
		/// <summary>
		/// Show prototype with objects from the passed in object collection without limiting displayed types.
		/// </summary>
		/// <param name="objects"></param>
		/// <returns></returns>
		public static Prototype With(ICollection<object> objects)
		{
			return new PrototypeBuilder().With(objects);
		}

		public class PrototypeBuilder
		{
			internal PrototypeBuilder()
			{
				Settings = new Settings();
			}

			internal Settings Settings { get; set; }

			/// <summary>
			/// Complete setup and show prototype with objects from all passed in object collections.
			/// </summary>
			/// <param name="objects"></param>
			/// <returns></returns>
			public Prototype With(params IEnumerable<object>[] objects)
			{
				if (objects.Length == 0 || objects.Any(o => o == null)) throw new ArgumentNullException();

				var collection = new ObservableCollection<object>(objects.Aggregate((o1, o2) => o1.Concat(o2)));
				return With(collection); ;
			}
			/// <summary>
			/// Complete setup and show prototype with objects from the passed in object collection.
			/// </summary>
			/// <param name="objects"></param>
			/// <returns></returns>
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

			/// <summary>
			/// Continue setup for new type without limiting displayed types.
			/// </summary>
			/// <typeparam name="TNext"></typeparam>
			/// <returns></returns>
			public PrototypeBuilder<TNext> For<TNext>()
			{
				return new PrototypeBuilder<TNext>
				{
					Settings = Settings
				};
			}
			/// <summary>
			/// Continue setup for new type and limit displayed types to TNext as well.
			/// </summary>
			/// <typeparam name="TNext"></typeparam>
			/// <returns></returns>
			public PrototypeBuilder<TNext> ViewOf<TNext>()
			{
				Settings.DisplayedTypes.Add(typeof(TNext));

				return new PrototypeBuilder<TNext>
				{
					Settings = Settings
				};
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
}
