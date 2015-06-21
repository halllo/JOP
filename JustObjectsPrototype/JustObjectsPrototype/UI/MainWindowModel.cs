using JustObjectsPrototype.UI.Editors;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace JustObjectsPrototype.UI
{
	public class MainWindowModel : ViewModel
	{
		Objects _Objects;

		public MainWindowModel(ICollection<object> objects, List<Type> types)
		{
			//TODO: 
			//1. object functionality ribbon
			//1d object list parameters (clear objects from the ambient objects)
			//1e object list parameters (add objects of previously untracked types should make the type appear)
			

			_Objects = new Objects(objects);

			Columns = new ObservableCollection<DataGridColumn>();
			Types = types != null ? new ObservableCollection<Type>(types) : _Objects.Types;

			Diagnose = new Command(
				execute: () => MessageBox.Show("all objects: \n\n\t" + string.Join("\n\t", _Objects.All)));
			New = new Command(
				execute: () =>
				{
					var newObject = Activator.CreateInstance(SelectedType);
					var newProxy = new ObjectProxy(newObject);
					Objects.Add(newProxy);
					SelectedObject = newProxy;
					Changed(() => SelectedObject);
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

					Properties = PropertiesViewModels.Of(type, _Objects, selectedObject);

					var methods = type.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
					var functions = from m in methods
									where m.DeclaringType != typeof(object)
									where m.IsSpecialName == false
									select Tuple.Create(m.Name, new Command(() =>
									{
										var parameters = m.GetParameters();
										var runtimeTypeForParameters = TypeCreator.New(m.Name, parameters.ToDictionary(p => p.Name, p => p.ParameterType));
										var runtimeTypeForParametersInstance = Activator.CreateInstance(runtimeTypeForParameters);
										var propertiesViewModels = PropertiesViewModels
											.Of(runtimeTypeForParameters, _Objects, new ObjectProxy(runtimeTypeForParametersInstance))
											.Where(p => IsObservableCollection(p.ValueType) == false).ToList();
										if (parameters.Any(p => IsObservableCollection(p.ParameterType) == false))
										{
											var dialogResult = ShowMethodInvocationDialog(new MethodInvocationDialogModel
											{
												MethodName = m.Name,
												Properties = propertiesViewModels
											});
											if (dialogResult != true) return;

										}
										var runtimeTypeForParametersProperties = runtimeTypeForParameters.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
										var parameterInstances = runtimeTypeForParametersProperties.Select(p => IsObservableCollection(p.PropertyType) 
											? _Objects.OfType_OneWayToSourceChangePropagation(p.PropertyType.GetGenericArguments().First()) 
											: p.GetValue(runtimeTypeForParametersInstance)).ToList();

										object result = null;
										try
										{
											result = m.Invoke(selectedObject.ProxiedObject, parameterInstances.ToArray());
										}
										catch (TargetInvocationException tie)
										{
											MessageBox.Show("An Exception occured in " + m.Name + ".\n\n" + tie.InnerException.ToString(), "Exception", MessageBoxButton.OK, MessageBoxImage.Error);
											return;
										}

										if (result != null)
										{
											var resultType = result.GetType();
											if (_Objects.Types.Contains(resultType))
											{
												var objectsOfType = _Objects.OfType(resultType);
												if (objectsOfType.All(o => !o.ProxiedObject.Equals(result)))
												{
													objectsOfType.Add(new ObjectProxy(result));
												}
											}
											if (resultType.IsGenericType
												&& (resultType.GetGenericTypeDefinition() == typeof(IEnumerable<>)
													||
													resultType.GetGenericTypeDefinition().GetInterfaces().Contains(typeof(IEnumerable)))
												&& _Objects.Types.Contains(resultType.GetGenericArguments().FirstOrDefault()))
											{
												var resultItemType = resultType.GetGenericArguments().FirstOrDefault();
												var objectsOfType = _Objects.OfType(resultItemType);
												foreach (var resultItem in (IEnumerable)result)
												{
													if (resultItem != null && objectsOfType.All(o => !o.ProxiedObject.Equals(resultItem)))
													{
														objectsOfType.Add(new ObjectProxy(resultItem));
													}
												}
											}
										}
										selectedObject.RaisePropertyChanged(string.Empty);
										Properties.ForEach(p => p.RaiseChanged());
									}));

					Functions = functions.ToList();
				}
				else
				{
					Properties = new List<IPropertyViewModel>();
					Functions = new List<Tuple<string, Command>>();
				}

				Changed(() => Properties);
				Changed(() => Functions);
				Delete.RaiseCanExecuteChanged();
			}
		}

		private static bool IsObservableCollection(Type type)
		{
			return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(ObservableCollection<>);
		}

		public List<IPropertyViewModel> Properties { get; set; }
		public List<Tuple<string, Command>> Functions { get; set; }

		public Command Diagnose { get; set; }
		public Command New { get; set; }
		public Command Delete { get; set; }

		public Func<object, bool?> ShowMethodInvocationDialog { get; set; }
	}
}
