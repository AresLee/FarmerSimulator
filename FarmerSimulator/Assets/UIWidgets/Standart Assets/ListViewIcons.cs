using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace UIWidgets {

	[System.Serializable]
	/// <summary>
	/// ListViewIcons item description.
	/// </summary>
	public class ListViewIconsItemDescription {
		/// <summary>
		/// The icon.
		/// </summary>
		[SerializeField]
		public Sprite Icon;

		/// <summary>
		/// The name.
		/// </summary>
		[SerializeField]
		public string Name;

		/// <summary>
		/// Serves as a hash function for a particular type.
		/// </summary>
		/// <returns>A hash code for this instance that is suitable for use in hashing algorithms and data structures such as a hash table.</returns>
		public override int GetHashCode()
		{
			return Icon.GetHashCode() ^ Name.GetHashCode();
		}

		/// <summary>
		/// Determines whether the specified <see cref="System.Object"/> is equal to the current <see cref="UIWidgets.ListViewIconsItemDescription"/>.
		/// </summary>
		/// <param name="obj">The <see cref="System.Object"/> to compare with the current <see cref="UIWidgets.ListViewIconsItemDescription"/>.</param>
		/// <returns><c>true</c> if the specified <see cref="System.Object"/> is equal to the current
		/// <see cref="UIWidgets.ListViewIconsItemDescription"/>; otherwise, <c>false</c>.</returns>
		public override bool Equals(System.Object obj)
		{
			ListViewIconsItemDescription descObj = obj as ListViewIconsItemDescription; 

			if (descObj == null)
			{
				return false;
			}
			if (((descObj.Icon==null) && (Icon!=null)) || ((descObj.Icon!=null) && (Icon==null)))
			{
				return false;
			}

			return Name==descObj.Name && ((Icon==null && descObj.Icon==null) || Icon.Equals(descObj.Icon));
		}
	}

	/// <summary>
	/// ListViewIcons.
	/// </summary>
	[AddComponentMenu("UI/ListViewIcons", 252)]
	public class ListViewIcons : ListViewCustom<ListViewIconsItemComponent,ListViewIconsItemDescription> {
		protected override void Awake()
		{
			Start();
		}

		[System.NonSerialized]
		bool isStartedListViewIcons = false;

		/// <summary>
		/// Start this instance.
		/// </summary>
		public override void Start()
		{
			if (isStartedListViewIcons)
			{
				return ;
			}
			isStartedListViewIcons = true;

			SortFunc = (x) => x.OrderBy(y => y.Name).ToList();
			base.Start();
		}

		/// <summary>
		/// Sets component data with specified item.
		/// </summary>
		/// <param name="component">Component.</param>
		/// <param name="item">Item.</param>
		protected override void SetData(ListViewIconsItemComponent component, ListViewIconsItemDescription item)
		{
			component.SetData(item);
		}

		/// <summary>
		/// Set highlights colors of specified component.
		/// </summary>
		/// <param name="component">Component.</param>
		protected override void HighlightColoring(ListViewIconsItemComponent component)
		{
			base.HighlightColoring(component);
			component.Text.color = HighlightedColor;
		}

		/// <summary>
		/// Set select colors of specified component.
		/// </summary>
		/// <param name="component">Component.</param>
		protected override void SelectColoring(ListViewIconsItemComponent component)
		{
			base.SelectColoring(component);
			component.Text.color = SelectedColor;
		}

		/// <summary>
		/// Set default colors of specified component.
		/// </summary>
		/// <param name="component">Component.</param>
		protected override void DefaultColoring(ListViewIconsItemComponent component)
		{
			base.DefaultColoring(component);
			component.Text.color = DefaultColor;
		}

		#if UNITY_EDITOR
		[UnityEditor.MenuItem("GameObject/UI/ListViewIcons", false, 1080)]
		static void CreateObject()
		{
			Utilites.CreateWidgetFromAsset("ListViewIcons");
		}
		#endif
	}
}