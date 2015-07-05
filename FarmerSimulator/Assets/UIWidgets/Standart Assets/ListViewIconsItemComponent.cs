using UnityEngine;
using UnityEngine.UI;
using System;

namespace UIWidgets {
	/// <summary>
	/// ListViewIcons item component.
	/// </summary>
	public class ListViewIconsItemComponent : ListViewItem {
		/// <summary>
		/// The icon.
		/// </summary>
		[SerializeField]
		public Image Icon;

		/// <summary>
		/// The text.
		/// </summary>
		[SerializeField]
		public Text Text;

		/// <summary>
		/// Sets component data with specified item.
		/// </summary>
		/// <param name="item">Item.</param>
		public void SetData(ListViewIconsItemDescription item)
		{
			if (item==null)
			{
				Icon.sprite = null;
				Text.text = string.Empty;
			}
			else
			{
				Icon.sprite = item.Icon;
				Text.text = item.Name;
			}

			Icon.SetNativeSize();
			//set transparent color if no icon
			Icon.color = (Icon.sprite==null) ? new Color(0, 0, 0, 0) : Color.white;
		}
	}
}