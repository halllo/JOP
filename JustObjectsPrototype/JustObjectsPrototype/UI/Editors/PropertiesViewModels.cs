using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace JustObjectsPrototype.UI.Editors
{
	public static class PropertiesViewModels
	{
		public static List<IPropertyViewModel> Of(Type type, Objects objects, ObjectProxy selectedObject)
		{
			var properties = type
				.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance)
				.Where(p => p.GetIndexParameters().Length == 0);

			var propertiesViewModels =
				from property in properties
				select property.CanRead && property.PropertyType == typeof(DateTime) ? (IPropertyViewModel)new DateTimePropertyViewModel { Instance = selectedObject, Property = property }

					 : property.CanRead && property.PropertyType == typeof(string) ? (IPropertyViewModel)new SimpleTypePropertyViewModel { Instance = selectedObject, Property = property }

					 : property.CanRead
							&& property.PropertyType.IsGenericType
							&& (property.PropertyType.GetGenericTypeDefinition() == typeof(IEnumerable<>)
								||
								property.PropertyType.GetGenericTypeDefinition().GetInterfaces().Contains(typeof(IEnumerable)))
							&& objects.Types.Contains(property.PropertyType.GetGenericArguments().FirstOrDefault()) ? (IPropertyViewModel)new ReferenceTypeListPropertyViewModel { Instance = selectedObject, Property = property, Objects = objects.OfType(property.PropertyType.GetGenericArguments().FirstOrDefault()).Select(o => o.ProxiedObject) }

					: property.CanRead
							&& property.PropertyType.IsGenericType
							&& (property.PropertyType.GetGenericTypeDefinition() == typeof(IEnumerable<>)
								||
								property.PropertyType.GetGenericTypeDefinition().GetInterfaces().Contains(typeof(IEnumerable))) ?
																														(IPropertyViewModel)new SimpleTypeListPropertyViewModel { Instance = selectedObject, Property = property }

					 : property.CanRead && objects.Types.Contains(property.PropertyType) ? (IPropertyViewModel)new ReferenceTypePropertyViewModel { Instance = selectedObject, Property = property, Objects = objects.OfType(property.PropertyType).Select(o => o.ProxiedObject) }

					 : property.CanRead ? (IPropertyViewModel)new SimpleTypePropertyViewModel { Instance = selectedObject, Property = property }
					 : null;

			return propertiesViewModels.Where(p => p != null).ToList<IPropertyViewModel>();
		}
	}
}
