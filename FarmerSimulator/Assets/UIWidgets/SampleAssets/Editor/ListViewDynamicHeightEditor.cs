using UnityEditor;
using UIWidgets;

namespace UIWidgetsSamples
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(ListViewDynamicHeight), true)]
	public class ListViewDynamicHeightEditor : ListViewCustomEditor
	{
	}
}