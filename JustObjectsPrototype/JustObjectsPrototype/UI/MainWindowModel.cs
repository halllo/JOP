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
		Settings _Settings;

		public MainWindowModel(ICollection<object> objects, Settings settings)
		{
			_Objects = new Objects(objects);
			_Settings = settings;

			Columns = new ObservableCollection<DataGridColumn>();
			Types = _Settings.DisplayedTypes.Any() ? new ObservableCollection<Type>(_Settings.DisplayedTypes) : _Objects.Types;

			Diagnose = new Command(
				execute: () => MessageBox.Show("all objects: \n\n\t" + string.Join("\n\t", _Objects.All)));
			New = new Command(
				execute: () =>
				{
					try
					{
						var newObject = Activator.CreateInstance(SelectedType);
						var newProxy = new ObjectProxy(newObject);
						Objects.Add(newProxy);
						SelectedObject = newProxy;
						Changed(() => SelectedObject);
					}
					catch (Exception)
					{
						MessageBox.Show("Object creation with this constructor is not supported yet.", "missing feature", MessageBoxButton.OK, MessageBoxImage.Information);
					}
				},
				canExecute: () => SelectedType != null && _Settings.IsAllowNew(SelectedType));
			Delete = new Command(
				execute: () =>
				{
					if (MessageBoxResult.Yes == MessageBox.Show("Are you sure?", "Delete object", MessageBoxButton.YesNo))
						Objects.Remove(SelectedObject);
				},
				canExecute: () => SelectedType != null && SelectedObject != null && _Settings.IsAllowDelete(SelectedType));
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

				var properties = selectedType
					.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance)
					.Where(p => p.GetIndexParameters().Length == 0);
				var columns = properties.Select(p =>
					p.PropertyType == typeof(bool) ? (DataGridColumn)new DataGridCheckBoxColumn { Header = ObjectDisplay.Nicely(p), Binding = new Binding(p.Name) }
													: (DataGridColumn)new DataGridTextColumn { Header = ObjectDisplay.Nicely(p), Binding = new Binding(p.Name) }
				).ToList();
				Columns.Clear();
				foreach (var column in columns) Columns.Add(column);

				var staticMethods = selectedType.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
				var staticFunctions = GetFunctions(null, staticMethods);
				Functions = staticFunctions.ToList();

				Changed(() => Functions);
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

					var staticMethods = selectedType.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
					var staticFunctions = GetFunctions(null, staticMethods);
					var methods = type.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
					var functions = GetFunctions(selectedObject, methods);
					Functions = staticFunctions.Union(functions).ToList();
				}
				else
				{
					Properties = new List<IPropertyViewModel>();
				}

				Changed(() => Properties);
				Changed(() => Functions);
				Delete.RaiseCanExecuteChanged();
			}
		}

		IEnumerable<Tuple<string, Command>> GetFunctions(ObjectProxy instance, MethodInfo[] methods)
		{
			return from m in methods
				   where m.DeclaringType != typeof(object)
				   where m.IsSpecialName == false
				   where m.Name != "ToString"
				   select Tuple.Create(ObjectDisplay.Nicely(m), new Command(() =>
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
						   result = m.Invoke(instance != null ? instance.ProxiedObject : null, parameterInstances.ToArray());
					   }
					   catch (TargetInvocationException tie)
					   {
						   MessageBox.Show("An Exception occured in " + m.Name + ".\n\n" + tie.InnerException.ToString(), "Exception", MessageBoxButton.OK, MessageBoxImage.Error);
						   return;
					   }

					   if (result != null)
					   {
						   var resultType = result.GetType();

						   if (resultType.IsGenericType
							   && (resultType.GetGenericTypeDefinition() == typeof(IEnumerable<>)
								   ||
								   resultType.GetGenericTypeDefinition().GetInterfaces().Contains(typeof(IEnumerable)))
							   && resultType.GetGenericArguments().Any()
							   && resultType.GetGenericArguments().First().IsValueType == false
							   && IsMicrosoftType(resultType.GetGenericArguments().First()) == false)
						   {
							   var resultItemType = resultType.GetGenericArguments().First();
							   var objectsOfType = _Objects.OfType(resultItemType);
							   foreach (var resultItem in (IEnumerable)result)
							   {
								   if (resultItem != null && objectsOfType.All(o => !o.ProxiedObject.Equals(resultItem)))
								   {
									   objectsOfType.Add(new ObjectProxy(resultItem));
								   }
							   }
						   }
						   if (resultType.IsValueType == false && IsMicrosoftType(resultType) == false)
						   {
							   var objectsOfType = _Objects.OfType(resultType);
							   if (objectsOfType.All(o => !o.ProxiedObject.Equals(result)))
							   {
								   objectsOfType.Add(new ObjectProxy(result));
							   }
						   }
					   }
					   if (instance != null) instance.RaisePropertyChanged(string.Empty);
					   if (Properties != null) Properties.ForEach(p => p.Refresh());
				   }));
		}

		static bool IsObservableCollection(Type type)
		{
			return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(ObservableCollection<>);
		}

		static bool IsMicrosoftType(Type type)
		{
			object[] attrs = type.Assembly.GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
			return attrs.OfType<AssemblyCompanyAttribute>().Any(attr => attr.Company == "Microsoft Corporation");
		}

		public List<IPropertyViewModel> Properties { get; set; }
		public List<Tuple<string, Command>> Functions { get; set; }

		public Command Diagnose { get; set; }
		public Command New { get; set; }
		public Command Delete { get; set; }

		public Func<object, bool?> ShowMethodInvocationDialog { get; set; }
	}
}
