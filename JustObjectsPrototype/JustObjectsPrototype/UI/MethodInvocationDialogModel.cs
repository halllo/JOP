using JustObjectsPrototype.UI.Editors;
using System.Collections.Generic;

namespace JustObjectsPrototype.UI
{
	public class MethodInvocationDialogModel : ViewModel
	{
		public string MethodName { get; set; }
		public List<IPropertyViewModel> Properties { get; set; }
	}
}
