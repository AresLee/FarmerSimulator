using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System;
using System.Linq;

namespace UIWidgets
{
	/// <summary>
	/// Base class for custom ListView with dynamic items heights.
	/// </summary>
	public class ListViewCustomHeight<TComponent,TItem> : ListViewCustom<TComponent,TItem>
		where TComponent : ListViewItem, IListViewItemHeight
		where TItem: IItemHeight
	{
		TComponent defaultItemCopy;
		RectTransform defaultItemCopyRect;

		protected TComponent DefaultItemCopy {
			get {
				if (defaultItemCopy==null)
				{
					defaultItemCopy = Instantiate(DefaultItem) as TComponent;
					Utilites.FixInstantiated (DefaultItem, defaultItemCopy);
					defaultItemCopy.transform.SetParent(DefaultItem.transform.parent);
					defaultItemCopy.gameObject.name = "copy";
					defaultItemCopy.gameObject.SetActive(false);
				}
				return defaultItemCopy;
			}
		}

		protected RectTransform DefaultItemCopyRect {
			get {
				if (defaultItemCopyRect==null)
				{
					defaultItemCopyRect = defaultItemCopy.GetComponent<RectTransform>();
				}
				return defaultItemCopyRect;
			}
		}

		protected override void Awake()
		{
			Start();
		}

		protected override void CalculateMaxVisibleItems()
		{
			SetItemsHeight(customItems);

			maxVisibleItems = GetLastVisibleIndex() - GetFirstVisibleIndex();
		}
		
		/// <summary>
		/// Scrolls to item with specifid index.
		/// </summary>
		/// <param name="index">Index.</param>
		protected override void ScrollTo(int index)
		{
			if (!CanOptimize())
			{
				return ;
			}
			
			var first_visible = GetFirstVisibleIndex(true);
			var last_visible = GetLastVisibleIndex(true);

			if (first_visible > index)
			{
				var item_starts = ItemStartAt(index);
				var movement = 1 - scrollRect.verticalScrollbar.size - (item_starts / FullHeight());
				var value_top = movement / (1 - scrollRect.verticalScrollbar.size);
				
				scrollRect.verticalScrollbar.value = value_top;
			}
			else if (last_visible < index)
			{
				var item_ends = ItemEndAt(index) + layout.GetMarginTop();
				var movement = (FullHeight() - item_ends) / FullHeight();
				var value_bottom = movement / (1 - scrollRect.verticalScrollbar.size);
				
				scrollRect.verticalScrollbar.value = value_bottom;
			}
		}

		protected override float CalculateBottomFillerHeight()
		{
			float height = 0;
			customItems.GetRange(topHiddenItems + visibleItems, bottomHiddenItems).ForEach(x => {
				height += x.Height + layout.Spacing.y;
			});
			return height - layout.Spacing.y;
		}
		
		protected override float CalculateTopFillerHeight()
		{
			float height = 0;
			customItems.GetRange(0, topHiddenItems).ForEach(x => {
				height += x.Height + layout.Spacing.y;
			});
			return height - layout.Spacing.y;
		}

		float ItemStartAt(int index)
		{
			if (index==0)
			{
				return 0;
			}
			float height = 0;
			customItems.TakeWhile((item, i) => {
				if (i >= index)
				{
					return false;
				}

				height += item.Height;
				if (index > 0)
				{
					height += layout.Spacing.y;
				}
				return true;
			});

			return height;
		}

		float ItemEndAt(int index)
		{
			float height = 0;
			customItems.TakeWhile((item, i) => {
				height += item.Height;
				if (index > 0)
				{
					height += layout.Spacing.y;
				}
				return i <= index;
			});
			
			return height;
		}

		/// <summary>
		/// Add the specified item.
		/// </summary>
		/// <param name="item">Item.</param>
		/// <returns>Index of added item.</returns>
		public override int Add(TItem item)
		{
			if (item==null)
			{
				throw new ArgumentNullException("Item is null.");
			}
			if (item.Height==0)
			{
				item.Height = GetItemHeight(item);
			}

			return base.Add(item);
		}

		void SetItemsHeight(List<TItem> items)
		{
			items.ForEach(x => {
				if (x.Height==0)
				{
					x.Height = GetItemHeight(x);
				}
			});
		}

		protected override void UpdateItems(List<TItem> newItems)
		{
			SetItemsHeight(newItems);

			base.UpdateItems(newItems);
		}

		protected override void OnScroll(float value)
		{
			CalculateMaxVisibleItems();

			base.OnScroll(value);
		}

		int GetIndexByHeight(float height)
		{
			return customItems.TakeWhile((item, index) => {
				height -= item.Height;
				if (index > 0)
				{
					height -= layout.Spacing.y;
				}
				return height > 0;
			}).Count();
		}

		protected override int GetLastVisibleIndex(bool strict=false)
		{
			var last_visible_index = GetIndexByHeight(GetScrollMargin() + scrollHeight);
			
			return (strict) ? last_visible_index : last_visible_index + 2;
		}
		
		protected override int GetFirstVisibleIndex(bool strict=false)
		{
			var first_visible_index = GetIndexByHeight(GetScrollMargin());

			if (strict)
			{
				first_visible_index = (first_visible_index>0) ? first_visible_index - 1 : first_visible_index;
			}
			return Math.Min(first_visible_index, Math.Max(0, customItems.Count - visibleItems));
		}

		float GetItemHeight(TItem item)
		{
			//it's impossible to get correct height of item with VerticalLayoutGroup
			//may be it is possible using Text.preferredHeight() and calculate it with own function
			//DefaultItemCopy.gameObject.SetActive(true);

			SetData(DefaultItemCopy, item);
			var height = DefaultItemCopy.Height;

			//DefaultItemCopy.gameObject.SetActive(false);

			return height;
		}
	}
}