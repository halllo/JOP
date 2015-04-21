using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;

namespace JustObjectsPrototype.UI
{
	public class MainWindowModel : ViewModel
	{
		Objects _Objects;

		public MainWindowModel(ICollection<object> objects, List<Type> types = null)
		{
			_Objects = new Objects(objects);

			Types = types != null ? new ObservableCollection<Type>(types) : _Objects.Types;
			Functions = new List<string> { "do1", "do2" };

			Diagnose = new Command(
				execute: () => MessageBox.Show("all objects: \n\n\t" + string.Join("\n\t", _Objects.All)));
			New = new Command(
				execute: () =>
				{
					var newObject = Activator.CreateInstance(SelectedType);
					Objects.Add(newObject);
					SelectedObject = newObject;
				},
				canExecute: () => SelectedType != null);
			Delete = new Command(
				execute: () =>
				{
					if (MessageBoxResult.Yes == MessageBox.Show("Are you sure?", "Delete object", MessageBoxButton.YesNo))
						Objects.Remove(SelectedObject);
				},
				canExecute: () => SelectedType != null && SelectedObject != null);
		}


		public ObservableCollection<Type> Types { get; set; }
		Type selectedType;
		public Type SelectedType
		{
			get
			{
				return selectedType;
			}
			set
			{
				selectedType = value;
				Objects = _Objects.OfType(selectedType);

				Changed(() => Objects);
				New.RaiseCanExecuteChanged();
				Delete.RaiseCanExecuteChanged();
			}
		}

		public ObservableCollection<object> Objects { get; set; }
		object selectedObject;
		public object SelectedObject
		{
			get
			{
				return selectedObject;
			}
			set
			{
				selectedObject = value;
				if (selectedObject != null)
				{
					var type = selectedObject.GetType();
					var properties = type.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
					var propertiesViewModels = from property in properties
											   select new TextPropertyViewModel(selectedObject, property);

					Properties = propertiesViewModels.ToList<IPropertyViewModel>();
				}
				else
				{
					Properties = new List<IPropertyViewModel>();
				}

				Changed(() => Properties);
				Delete.RaiseCanExecuteChanged();
			}
		}


		public List<string> Functions { get; set; }
		public List<IPropertyViewModel> Properties { get; set; }

		public Command Diagnose { get; set; }
		public Command New { get; set; }
		public Command Delete { get; set; }
	}
}
