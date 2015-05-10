
namespace JustObjectsPrototype.UI.Editors
{
	public interface IPropertyViewModel
	{
		string Label { get; }
		void RaiseChanged();
	}
}
