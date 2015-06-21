using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.ObjectModel;

namespace JustObjectsPrototype.Tests
{
	[TestClass]
	public class ObjectsTests
	{
		public class DomainObject1 { public int Id { get; set; } }

		[TestMethod]
		public void Add_To_ObjectsList_Also_Adds_To_TypeObjectsList()
		{
			var list = new ObservableCollection<object>();
			var objects = new Objects(list);

			var domainObject1s = objects.OfType(typeof(DomainObject1));

			list.Add(new DomainObject1 { Id = 1 });

			var o = (DomainObject1)(dynamic)domainObject1s[0];
			Assert.IsTrue(o.Id == 1);
		}

		[TestMethod]
		public void Add_To_TypeObjectsList_Also_Adds_To_ObjectsList()
		{
			var list = new ObservableCollection<object>();
			var objects = new Objects(list);

			var domainObject1s = objects.OfType(typeof(DomainObject1));

			domainObject1s.Add(new ObjectProxy(new DomainObject1 { Id = 1 }));

			Assert.IsTrue(((DomainObject1)list[0]).Id == 1);
		}

		[TestMethod]
		public void Add_To_ObjectsList_DoesNot_Also_Add_To_UnproxiedTypeObjectsList()
		{
			var list = new ObservableCollection<object>();
			var objects = new Objects(list);

			var domainObject1s = objects.OfType_OneWayToSourceChangePropagation<DomainObject1>();

			list.Add(new DomainObject1 { Id = 1 });

			Assert.IsTrue(domainObject1s.Count == 0);
		}

		[TestMethod]
		public void Add_To_UnproxiedTypeObjectsList_Also_Adds_To_ObjectsList()
		{
			var list = new ObservableCollection<object>();
			var objects = new Objects(list);

			var domainObject1s = objects.OfType_OneWayToSourceChangePropagation<DomainObject1>();

			domainObject1s.Add(new DomainObject1 { Id = 1 });

			Assert.IsTrue(((DomainObject1)list[0]).Id == 1);
		}

		[TestMethod]
		public void Add_To_ObjectsList_DoesNot_Also_Add_To_UnproxiedTypeObjectsList2()
		{
			var list = new ObservableCollection<object>();
			var objects = new Objects(list);

			var domainObject1s = (ObservableCollection<DomainObject1>)objects.OfType_OneWayToSourceChangePropagation(typeof(DomainObject1));

			list.Add(new DomainObject1 { Id = 1 });

			Assert.IsTrue(domainObject1s.Count == 0);
		}

		[TestMethod]
		public void Add_To_UnproxiedTypeObjectsList_Also_Adds_To_ObjectsList2()
		{
			var list = new ObservableCollection<object>();
			var objects = new Objects(list);

			var domainObject1s = (ObservableCollection<DomainObject1>)objects.OfType_OneWayToSourceChangePropagation(typeof(DomainObject1));

			domainObject1s.Add(new DomainObject1 { Id = 1 });

			Assert.IsTrue(((DomainObject1)list[0]).Id == 1);
		}

		[TestMethod]
		public void Clear_To_UnproxiedTypeObjectsList_Also_Clears_To_ObjectsList()
		{
			var list = new ObservableCollection<object>();
			list.Add(new DomainObject1 { Id = 1 });

			var objects = new Objects(list);

			var domainObject1s = objects.OfType_OneWayToSourceChangePropagation<DomainObject1>();
			domainObject1s.Clear();
			
			Assert.IsTrue(list.Count == 0);
		}
	}
}
