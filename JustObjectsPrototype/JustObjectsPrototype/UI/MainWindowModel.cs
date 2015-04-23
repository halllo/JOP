using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace JustObjectsPrototype.UI
{
	public class MainWindowModel : ViewModel
	{
		Objects _Objects;

		public MainWindowModel(ICollection<object> objects, List<Type> types = null)
		{
			//TODO: 
			//1. object reference property changer with no ToString
			//2. number & datetime property changer
			//3. object functionality ribbon

			_Objects = new Objects(objects);

			Columns = new ObservableCollection<DataGridColumn>();
			Types = types != null ? new ObservableCollection<Type>(types) : _Objects.Types;
			Functions = new List<string> { "do1", "do2" };

			Diagnose = new Command(
				execute: () => MessageBox.Show("all objects: \n\n\t" + string.Join("\n\t", _Objects.All)));
			New = new Command(
				execute: () =>
				{
					var newObject = Activator.CreateInstance(SelectedType);
					var newProxy = new ObjectProxy(newObject);
					Objects.Add(newProxy);
					SelectedObject = newProxy;
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

				var properties = selectedType.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
				var columns = properties.Select(p => new DataGridTextColumn { Header = p.Name, Binding = new Binding(p.Name) }).ToList();
				Columns.Clear();
				foreach (var column in columns) Columns.Add(column);

				Changed(() => Objects);
				New.RaiseCanExecuteChanged();
				Delete.RaiseCanExecuteChanged();
			}
		}

		public ObservableCollection<DataGridColumn> Columns { get; private set; }

		public ObservableCollection<ObjectProxy> Objects { get; set; }
		ObjectProxy selectedObject;
		public ObjectProxy SelectedObject
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
					var type = selectedObject.ProxiedObject.GetType();
					var properties = type.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
					var propertiesViewModels = from property in properties
											   select property.PropertyType == typeof(string) ? (IPropertyViewModel)new TextPropertyViewModel(selectedObject, property)
													: _Objects.Types.Contains(property.PropertyType) ? (IPropertyViewModel)new ReferencePropertyViewModel(selectedObject, property, _Objects.OfType(property.PropertyType).Select(o => o.ProxiedObject))
													: (IPropertyViewModel)new DisplayPropertyViewModel(selectedObject, property);

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
