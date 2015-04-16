using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace JustObjectsPrototype
{
	public class MainWindowModel : ViewModel
	{
		Dictionary<Type, ObservableCollection<object>> _TypesAndObjects;

		public MainWindowModel(List<object> objects, List<Type> types = null)
		{
			var objectsByType = objects.ToLookup(o => o.GetType());
			var typesAndObjects = (types ?? objectsByType.Select(t => t.Key)).ToDictionary(t => t, t => new ObservableCollection<object>(objectsByType[t]));

			_TypesAndObjects = typesAndObjects;

			Types = new List<Type>(_TypesAndObjects.Keys);
			Functions = new List<string> { "do1", "do2" };

			Diagnose = new Command(
				execute: () => MessageBox.Show("all objects: \n\n\t" + string.Join("\n\t", _TypesAndObjects.Values.SelectMany(v => v))));
			New = new Command(
				execute: () => _TypesAndObjects[SelectedType].Add(Activator.CreateInstance(SelectedType)));
			Delete = new Command(
				execute: () =>
				{
					if (MessageBoxResult.Yes == MessageBox.Show("Are you sure?", "Delete object", MessageBoxButton.YesNo))
						_TypesAndObjects[SelectedType].Remove(SelectedObject);
				},
				canExecute: () => SelectedType != null && SelectedObject != null);
		}


		public List<Type> Types { get; set; }
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
				Objects = _TypesAndObjects[selectedType];
				Changed(() => Objects);
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
				Delete.RaiseCanExecuteChanged();
			}
		}


		public List<string> Functions { get; set; }
		public List<Tuple<string, string>> Properties { get; set; }

		public Command Diagnose { get; set; }
		public Command New { get; set; }
		public Command Delete { get; set; }
	}
}
