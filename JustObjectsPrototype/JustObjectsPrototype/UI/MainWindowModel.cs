using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;

namespace JustObjectsPrototype.UI
{
	public class MainWindowModel : ViewModel
	{
		Objects _Objects;

		public MainWindowModel(List<object> objects, List<Type> types = null)
		{
			_Objects = new Objects(objects);

			Types = types != null ? new ObservableCollection<Type>(types) : _Objects.Types;
			Functions = new List<string> { "do1", "do2" };

			Diagnose = new Command(
				execute: () => MessageBox.Show("all objects: \n\n\t" + string.Join("\n\t", _Objects.All)));
			New = new Command(
				execute: () => _Objects.Add(Activator.CreateInstance(SelectedType)),
				canExecute: () => SelectedType != null);
			Delete = new Command(
				execute: () =>
				{
					if (MessageBoxResult.Yes == MessageBox.Show("Are you sure?", "Delete object", MessageBoxButton.YesNo))
						_Objects.Remove(SelectedObject);
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
				Properties = new List<IPropertyViewModel>{ 
					new TextPropertyViewModel { Label = "test1", Value = "tjdskjf"}, 
					new TextPropertyViewModel { Label = "test at " + DateTime.Now.Ticks, Value = "tjdskjf"},
					new ReferencePropertyViewModel { Label = "test3", Reference = "tjdskjf"}
				};

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


	public interface IPropertyViewModel
	{
		string Label { get; }
	}

	public class TextPropertyViewModel : IPropertyViewModel
	{
		public string Label { get; set; }
		public string Value { get; set; }
	}

	public class ReferencePropertyViewModel : IPropertyViewModel
	{
		public string Label { get; set; }
		public object Reference { get; set; }
	}
}
