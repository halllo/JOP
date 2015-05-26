﻿using JustObjectsPrototype.UI.Editors;
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
			//1c object parameters
			//1d object list parameters (to add and remove objects from the ambient objects)


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

					var properties = type.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
					var propertiesViewModels =
						from property in properties
						select property.CanRead && property.PropertyType == typeof(DateTime) ? (IPropertyViewModel)new DateTimePropertyViewModel { Instance = selectedObject, Property = property }

							 : property.CanRead && property.PropertyType == typeof(string) ? (IPropertyViewModel)new SimpleTypePropertyViewModel { Instance = selectedObject, Property = property }

							 : property.CanRead
									&& property.PropertyType.IsGenericType
									&& (property.PropertyType.GetGenericTypeDefinition() == typeof(IEnumerable<>)
										||
										property.PropertyType.GetGenericTypeDefinition().GetInterfaces().Contains(typeof(IEnumerable)))
									&& _Objects.Types.Contains(property.PropertyType.GetGenericArguments().FirstOrDefault()) ? (IPropertyViewModel)new ReferenceTypeListPropertyViewModel { Instance = selectedObject, Property = property, Objects = _Objects.OfType(property.PropertyType.GetGenericArguments().FirstOrDefault()).Select(o => o.ProxiedObject) }

							: property.CanRead
									&& property.PropertyType.IsGenericType
									&& (property.PropertyType.GetGenericTypeDefinition() == typeof(IEnumerable<>)
										||
										property.PropertyType.GetGenericTypeDefinition().GetInterfaces().Contains(typeof(IEnumerable))) ?
																																(IPropertyViewModel)new SimpleTypeListPropertyViewModel { Instance = selectedObject, Property = property }

							 : property.CanRead && _Objects.Types.Contains(property.PropertyType) ? (IPropertyViewModel)new ReferenceTypePropertyViewModel { Instance = selectedObject, Property = property, Objects = _Objects.OfType(property.PropertyType).Select(o => o.ProxiedObject) }

							 : property.CanRead ? (IPropertyViewModel)new SimpleTypePropertyViewModel { Instance = selectedObject, Property = property }
							 : null;

					Properties = propertiesViewModels.Where(p => p != null).ToList<IPropertyViewModel>();


					var methods = type.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
					var functions = from m in methods
									where m.DeclaringType != typeof(object)
									where m.IsSpecialName == false
									select Tuple.Create(m.Name, new Command(() =>
									{
										var result = m.Invoke(selectedObject.ProxiedObject, new object[0]);
										var resultType = result.GetType();

										if (result != null)
										{
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

		public List<IPropertyViewModel> Properties { get; set; }
		public List<Tuple<string, Command>> Functions { get; set; }

		public Command Diagnose { get; set; }
		public Command New { get; set; }
		public Command Delete { get; set; }
	}
}
