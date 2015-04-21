using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.ObjectModel;

namespace JustObjectsPrototype.Tests
{
	[TestClass]
	public class ObjectsTests
	{
		[TestMethod]
		public void TestMethod1()
		{
			var objects = new Objects(new ObservableCollection<object>());
		}
	}
}
