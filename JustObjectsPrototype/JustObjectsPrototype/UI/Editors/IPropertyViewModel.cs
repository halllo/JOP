
namespace JustObjectsPrototype.UI.Editors
{
	public interface IPropertyViewModel
	{
		string Label { get; }
		object Value { get; }
		void RaiseChanged();
	}
}
