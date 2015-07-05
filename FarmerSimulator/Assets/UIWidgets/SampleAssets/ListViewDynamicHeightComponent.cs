using UnityEngine;
using UnityEngine.UI;
using UIWidgets;

namespace UIWidgetsSamples {
	public class ListViewDynamicHeightComponent : ListViewItem, IListViewItemHeight {
		[SerializeField]
		public Text Name;

		[SerializeField]
		public Text Text;

		public float Height {
			get {
				return CalculateHeight();
			}
		}

		// Displaying item data
		public void SetData(ListViewDynamicHeightItemDescription item)
		{
			Name.text = item.Name;
			Text.text = item.Text.Replace("\\n", "\n");
		}

		float CalculateHeight()
		{
			float default_total_height = 57;
			float default_name_height = 21;
			float default_text_height = 21;

			return default_total_height - default_name_height - default_text_height + Name.preferredHeight + Text.preferredHeight;
		}
	}
}