using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UIWidgets;

namespace UIWidgetsSamples {
	
	[System.Serializable]
	public class ListViewDynamicHeightItemDescription : IItemHeight {
		//specify data fields
		[SerializeField]
		public string Name;
		[SerializeField]
		public string Text;

		[SerializeField]
		float height;

		public float Height {
			get {
				return height;
			}
			set {
				height = value;
			}
		}
	}
	
	//public class ListViewDynamicHeight : ListViewCustom<ListViewDynamicHeightComponent,ListViewDynamicHeightItemDescription> {
	public class ListViewDynamicHeight : ListViewCustomHeight<ListViewDynamicHeightComponent,ListViewDynamicHeightItemDescription> {
		bool isStartedListViewDynamic = false;
		
		protected override void Awake()
		{
			Start();
		}
		
		public override void Start()
		{
			if (isStartedListViewDynamic)
			{
				return ;
			}
			isStartedListViewDynamic = true;
			
			SortFunc = (x) => x.OrderBy(y => y.Name).ToList();
			base.Start();
		}
		
		protected override void SetData(ListViewDynamicHeightComponent component, ListViewDynamicHeightItemDescription item)
		{
			component.SetData(item);
		}
		
		protected override void HighlightColoring(ListViewDynamicHeightComponent component)
		{
			base.HighlightColoring(component);
			component.Text.color = HighlightedColor;
		}
		
		protected override void SelectColoring(ListViewDynamicHeightComponent component)
		{
			base.SelectColoring(component);
			component.Text.color = SelectedColor;
		}
		
		protected override void DefaultColoring(ListViewDynamicHeightComponent component)
		{
			base.DefaultColoring(component);
			component.Text.color = DefaultColor;
		}
	}
}