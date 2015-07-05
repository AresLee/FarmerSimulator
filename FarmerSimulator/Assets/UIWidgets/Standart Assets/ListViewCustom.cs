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
	/// Custom ListView event.
	/// </summary>
	public class ListViewCustomEvent : UnityEvent<int> {
		
	}

	/// <summary>
	/// Base class for custom ListView.
	/// </summary>
	public class ListViewCustom<TComponent,TItem> : ListViewBase where TComponent : ListViewItem {

		[SerializeField]
		protected List<TItem> customItems = new List<TItem>();

		/// <summary>
		/// Gets or sets the items.
		/// </summary>
		/// <value>Items.</value>
		new public List<TItem> Items {
			get {
				return new List<TItem>(customItems);
			}
			set {
				UpdateItems(new List<TItem>(value));
				if (scrollRect!=null)
				{
					scrollRect.verticalScrollbar.value = 1f;
				}
			}
		}

		/// <summary>
		/// The default item.
		/// </summary>
		[SerializeField]
		public TComponent DefaultItem;

		List<TComponent> components = new List<TComponent>();

		List<TComponent> componentsCache = new List<TComponent>();

		List<UnityAction<PointerEventData>> callbacksEnter = new List<UnityAction<PointerEventData>>();

		List<UnityAction<PointerEventData>> callbacksExit = new List<UnityAction<PointerEventData>>();

		/// <summary>
		/// Gets the selected item.
		/// </summary>
		/// <value>The selected item.</value>
		public TItem SelectedItem {
			get {
				if (SelectedIndex==-1)
				{
					return default(TItem);
				}
				return customItems[SelectedIndex];
			}
		}

		/// <summary>
		/// Gets the selected items.
		/// </summary>
		/// <value>The selected items.</value>
		public List<TItem> SelectedItems {
			get {
				if (SelectedIndex==-1)
				{
					return null;
				}
				return SelectedIndicies.ConvertAll(x => customItems[x]);
			}
		}

		/// <summary>
		/// Gets the selected component.
		/// </summary>
		/// <value>The selected component.</value>
		public TComponent SelectedComponent {
			get {
				if (SelectedIndex==-1)
				{
					return null;
				}
				return components[SelectedIndex];
			}
		}

		/// <summary>
		/// Gets the selected components.
		/// </summary>
		/// <value>The selected components.</value>
		public List<TComponent> SelectedComponents {
			get {
				if (SelectedIndex==-1)
				{
					return null;
				}
				return SelectedIndicies.ConvertAll(x => components[x]);
			}
		}

		/// <summary>
		/// Sort function.
		/// </summary>
		public Func<List<TItem>,List<TItem>> SortFunc;
		
		/// <summary>
		/// What to do when the object selected.
		/// </summary>
		public ListViewCustomEvent OnSelectObject = new ListViewCustomEvent();
		
		/// <summary>
		/// What to do when the object deselected.
		/// </summary>
		public ListViewCustomEvent OnDeselectObject = new ListViewCustomEvent();
		
		/// <summary>
		/// What to do when the event system send a pointer enter Event.
		/// </summary>
		public ListViewCustomEvent OnPointerEnterObject = new ListViewCustomEvent();
		
		/// <summary>
		/// What to do when the event system send a pointer exit Event.
		/// </summary>
		public ListViewCustomEvent OnPointerExitObject = new ListViewCustomEvent();
		
		[SerializeField]
		Color defaultBackgroundColor = Color.white;
		
		[SerializeField]
		Color defaultColor = Color.black;
		
		/// <summary>
		/// Default background color.
		/// </summary>
		public Color DefaultBackgroundColor {
			get {
				return defaultBackgroundColor;
			}
			set {
				defaultBackgroundColor = value;
				UpdateColors();
			}
		}
		
		/// <summary>
		/// Default text color.
		/// </summary>
		public Color DefaultColor {
			get {
				return defaultColor;
			}
			set {
				DefaultColor = value;
				UpdateColors();
			}
		}
		
		/// <summary>
		/// Color of background on pointer over.
		/// </summary>
		[SerializeField]
		public Color HighlightedBackgroundColor = new Color(203, 230, 244, 255);
		
		/// <summary>
		/// Color of text on pointer text.
		/// </summary>
		[SerializeField]
		public Color HighlightedColor = Color.black;
		
		[SerializeField]
		Color selectedBackgroundColor = new Color(53, 83, 227, 255);
		
		[SerializeField]
		Color selectedColor = Color.black;
		
		/// <summary>
		/// Background color of selected item.
		/// </summary>
		public Color SelectedBackgroundColor {
			get {
				return selectedBackgroundColor;
			}
			set {
				selectedBackgroundColor = value;
				UpdateColors();
			}
		}
		
		/// <summary>
		/// Text color of selected item.
		/// </summary>
		public Color SelectedColor {
			get {
				return selectedColor;
			}
			set {
				selectedColor = value;
				UpdateColors();
			}
		}

		[SerializeField]
		protected ScrollRect scrollRect;

		/// <summary>
		/// Gets or sets the ScrollRect.
		/// </summary>
		/// <value>The ScrollRect.</value>
		public ScrollRect ScrollRect {
			get {
				return scrollRect;
			}
			set {
				if (scrollRect!=null)
				{
					scrollRect.verticalScrollbar.onValueChanged.RemoveListener(OnScroll);
				}
				scrollRect = value;
				if (scrollRect!=null)
				{
					scrollRect.verticalScrollbar.onValueChanged.AddListener(OnScroll);
				}
			}
		}

		/// <summary>
		/// The top filler.
		/// </summary>
		RectTransform topFiller;

		/// <summary>
		/// The bottom filler.
		/// </summary>
		RectTransform bottomFiller;

		/// <summary>
		/// The height of the DefaultItem.
		/// </summary>
		protected float itemHeight;

		/// <summary>
		/// The height of the ScrollRect.
		/// </summary>
		protected float scrollHeight;

		/// <summary>
		/// Count of visible items.
		/// </summary>
		protected int maxVisibleItems;

		/// <summary>
		/// Count of visible items.
		/// </summary>
		protected int visibleItems;

		/// <summary>
		/// Count of hidden items by top filler.
		/// </summary>
		protected int topHiddenItems;

		/// <summary>
		/// Count of hidden items by bottom filler.
		/// </summary>
		protected int bottomHiddenItems;

		protected virtual void Awake()
		{
			Start();
		}

		[System.NonSerialized]
		bool isStartedListViewCustom = false;

		protected EasyLayout.EasyLayout layout;

		/// <summary>
		/// Start this instance.
		/// </summary>
		public override void Start()
		{
			if (isStartedListViewCustom)
			{
				return ;
			}
			isStartedListViewCustom = true;
			
			base.Start();

			DestroyGameObjects = false;

			if (DefaultItem==null)
			{
				throw new NullReferenceException(String.Format("DefaultItem is null. Set component of type {0} to DefaultItem.", typeof(TComponent).FullName));
			}
			DefaultItem.gameObject.SetActive(true);

			var topFillerObj = new GameObject("top filler");
			topFillerObj.transform.SetParent(Container);
			topFiller = topFillerObj.AddComponent<RectTransform>();
			topFiller.SetAsFirstSibling();
			topFiller.localScale = Vector3.one;

			var bottomFillerObj = new GameObject("bottom filler");
			bottomFillerObj.transform.SetParent(Container);
			bottomFiller = bottomFillerObj.AddComponent<RectTransform>();
			bottomFiller.localScale = Vector3.one;

			if (CanOptimize())
			{
				ScrollRect = scrollRect;

				scrollHeight = scrollRect.GetComponent<RectTransform>().rect.height;
				itemHeight = DefaultItem.GetComponent<RectTransform>().rect.height;
				layout = Container.GetComponent<EasyLayout.EasyLayout>();
				CalculateMaxVisibleItems();

				var r = scrollRect.gameObject.AddComponent<ResizeListener>();
				r.OnResize.AddListener(Resize);
			}

			customItems = SortItems(customItems);
			UpdateView();

			DefaultItem.gameObject.SetActive(false);

			OnSelect.AddListener(OnSelectCallback);
			OnDeselect.AddListener(OnDeselectCallback);
		}

		protected virtual void CalculateMaxVisibleItems()
		{
			maxVisibleItems = (int)Math.Ceiling(scrollHeight / itemHeight) + 1;
		}

		protected virtual void Resize()
		{
			scrollHeight = scrollRect.GetComponent<RectTransform>().rect.height;
			CalculateMaxVisibleItems();
			UpdateView();
		}

		protected bool CanOptimize()
		{
			return scrollRect!=null && (layout!=null || Container.GetComponent<EasyLayout.EasyLayout>()!=null);
		}

		void OnSelectCallback(int index, ListViewItem item)
		{
			OnSelectObject.Invoke(index);

			if (item!=null)
			{
				SelectColoring(item);
			}
		}
		
		void OnDeselectCallback(int index, ListViewItem item)
		{
			OnDeselectObject.Invoke(index);

			if (item!=null)
			{
				DefaultColoring(item);
			}
		}
		
		void OnPointerEnterCallback(ListViewItem item)
		{
			OnPointerEnterObject.Invoke(item.Index);

			if (!IsSelected(item.Index))
			{
				HighlightColoring(item);
			}
		}
		
		void OnPointerExitCallback(ListViewItem item)
		{
			OnPointerExitObject.Invoke(item.Index);

			if (!IsSelected(item.Index))
			{
				DefaultColoring(item);
			}
		}
		
		/// <summary>
		/// Updates thitemsms.
		/// </summary>
		public override void UpdateItems()
		{
			UpdateItems(customItems);
		}

		/// <summary>
		/// Clear items of this instance.
		/// </summary>
		public override void Clear()
		{
			//customItems.Clear();
			UpdateItems(new List<TItem>());
		}

		/// <summary>
		/// Add the specified item.
		/// </summary>
		/// <param name="item">Item.</param>
		/// <returns>Index of added item.</returns>
		public virtual int Add(TItem item)
		{
			if (item==null)
			{
				throw new ArgumentNullException("Item is null.");
			}
			var newItems = customItems;
			newItems.Add(item);
			UpdateItems(newItems);

			var index = customItems.FindIndex(x => System.Object.ReferenceEquals(x, item));
			
			return index;
		}
		
		/// <summary>
		/// Remove the specified item.
		/// </summary>
		/// <param name="item">Item.</param>
		/// <returns>Index of removed TItem.</returns>
		public virtual int Remove(TItem item)
		{
			if (item==null)
			{
				throw new ArgumentNullException("Item is null.");
			}

			var index = customItems.FindIndex(x => System.Object.ReferenceEquals(x, item));
			if (index==-1)
			{
				return index;
			}

			Remove(index);

			return index;
		}

		/// <summary>
		/// Remove item by specifieitemsex.
		/// </summary>
		/// <returns>Index of removed item.</returns>
		/// <param name="index">Index.</param>
		public virtual void Remove(int index)
		{
			var newItems = customItems;
			newItems.RemoveAt(index);
			UpdateItems(newItems);			
		}

		void RemoveCallback(ListViewItem item, int index)
		{
			if (item==null)
			{
				return ;
			}
			if (callbacksEnter.Count > index)
			{
				item.onPointerEnter.RemoveListener(callbacksEnter[index]);
			}
			if (callbacksExit.Count > index)
			{
				item.onPointerExit.RemoveListener(callbacksExit[index]);
			}
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
				var item_starts = index * (itemHeight + layout.Spacing.y);
				var movement = 1 - scrollRect.verticalScrollbar.size - (item_starts / FullHeight());
				var value_top = movement / (1 - scrollRect.verticalScrollbar.size);

				scrollRect.verticalScrollbar.value = value_top;
			}
			else if (last_visible < index)
			{
				var item_ends = (index + 1) * (itemHeight + layout.Spacing.y) - layout.Spacing.y + layout.GetMarginTop();
				var movement = (FullHeight() - item_ends) / FullHeight();
				var value_bottom = movement / (1 - scrollRect.verticalScrollbar.size);

				scrollRect.verticalScrollbar.value = value_bottom;
			}
		}

		void RemoveCallbacks()
		{
			base.Items.ForEach(RemoveCallback);
			callbacksEnter.Clear();
			callbacksExit.Clear();
		}
		
		void AddCallbacks()
		{
			base.Items.ForEach(AddCallback);
		}
		
		void AddCallback(ListViewItem item, int index)
		{
			callbacksEnter.Add(ev => OnPointerEnterCallback(item));
			callbacksExit.Add(ev => OnPointerExitCallback(item));
			
			item.onPointerEnter.AddListener(callbacksEnter[callbacksEnter.Count - 1]);
			item.onPointerExit.AddListener(callbacksExit[callbacksExit.Count - 1]);
		}
		
		List<TItem> SortItems(List<TItem> newItems)
		{
			var temp = newItems;
			if (SortFunc!=null)
			{
				temp = SortFunc(temp);
			}
			
			return temp;
		}

		/// <summary>
		/// Sets component data with specified item.
		/// </summary>
		/// <param name="component">Component.</param>
		/// <param name="item">Item.</param>
		protected virtual void SetData(TComponent component, TItem item)
		{
		}

		List<TComponent> GetNewComponents()
		{
			componentsCache = componentsCache.Where (x => x != null).ToList ();
			var new_components = new List<TComponent>();
			customItems.ForEach ((x, i) =>  {
				if (i >= visibleItems)
				{
					return;
				}

				if (components.Count > 0)
				{
					new_components.Add(components[0]);
					components.RemoveAt(0);
				}
				else if (componentsCache.Count > 0)
				{
					componentsCache[0].gameObject.SetActive(true);
					new_components.Add(componentsCache[0]);
					componentsCache.RemoveAt(0);
				}
				else
				{
					var component = Instantiate(DefaultItem) as TComponent;
					Utilites.FixInstantiated (DefaultItem, component);
					component.gameObject.SetActive(true);
					new_components.Add(component);
				}
			});

			components.ForEach(x => x.gameObject.SetActive(false));
			componentsCache.AddRange(components);
			components.Clear();

			return new_components;
		}

		protected float FullHeight()
		{
			return layout.UISize[1];
		}

		protected float GetScrollMargin()
		{
			var value_n = (1f - scrollRect.verticalScrollbar.size) * (1f - scrollRect.verticalScrollbar.value);
			return FullHeight() * value_n;
		}

		protected virtual int GetLastVisibleIndex(bool strict=false)
		{
			var window = GetScrollMargin() + scrollHeight;
			var last_visible_index = (strict)
				? (int)Math.Floor(window / (itemHeight + layout.Spacing.y))
				: (int)Math.Ceiling(window / (itemHeight + layout.Spacing.y));
			
			return last_visible_index - 1;
		}

		protected virtual int GetFirstVisibleIndex(bool strict=false)
		{
			var first_visible_index = (strict)
				? (int)Math.Ceiling(GetScrollMargin() / (itemHeight + layout.Spacing.y))
				: (int)Math.Floor(GetScrollMargin() / (itemHeight + layout.Spacing.y));
			if (strict)
			{
				return first_visible_index;
			}

			return Math.Min(first_visible_index, Math.Max(0, customItems.Count - visibleItems));
		}
		
		protected virtual void OnScroll(float value)
		{
			var oldTopHiddenItems = topHiddenItems;

			topHiddenItems = GetFirstVisibleIndex();
			bottomHiddenItems = Math.Max(0, customItems.Count - visibleItems - topHiddenItems);

			//поменять данные отображаемых элементов
			if (oldTopHiddenItems==topHiddenItems)
			{
				//do nothing
			}
			// optimization on +-1 item scroll
			else if (oldTopHiddenItems==(topHiddenItems + 1))
			{
				var bottomComponent = components[components.Count - 1];
				components.RemoveAt(components.Count - 1);
				components.Insert(0, bottomComponent);
				bottomComponent.transform.SetAsFirstSibling();

				bottomComponent.Index = topHiddenItems;
				SetData(bottomComponent, customItems[topHiddenItems]);
				Coloring(bottomComponent as ListViewItem);
			}
			else if (oldTopHiddenItems==(topHiddenItems - 1))
			{
				var topComponent = components[0];
				components.RemoveAt(0);
				components.Add(topComponent);
				topComponent.transform.SetAsLastSibling();

				topComponent.Index = topHiddenItems + visibleItems - 1;
				SetData(topComponent, customItems[topHiddenItems + visibleItems - 1]);
				Coloring(topComponent as ListViewItem);
			}
			// all other cases
			else
			{
				var new_indicies = Enumerable.Range(topHiddenItems, visibleItems).ToArray();
				components.ForEach((x, i) => {
					x.Index = new_indicies[i];
					SetData(x, customItems[new_indicies[i]]);
					Coloring(x as ListViewItem);
				});
			}

			SetTopFiller();
			SetBottomFiller();
			if (layout)
			{
				layout.NeedUpdateLayout();
			}
		}

		protected void UpdateView()
		{
			RemoveCallbacks();

			if ((CanOptimize()) && (customItems.Count > 0))
			{
				visibleItems = (maxVisibleItems < customItems.Count) ? maxVisibleItems : customItems.Count;
			}
			else
			{
				visibleItems = customItems.Count;
			}

			components = GetNewComponents();

			var base_items = new List<ListViewItem>();
			components.ForEach(x => base_items.Add(x));
			
			base.Items = base_items;
			
			components.ForEach((x, i) => {
				x.Index = i;
				SetData(x, customItems[i]);
				Coloring(x as ListViewItem);
			});

			AddCallbacks();
			
			topHiddenItems = 0;
			bottomHiddenItems = customItems.Count() - visibleItems;

			SetTopFiller();
			SetBottomFiller();
			if (layout)
			{
				//force rebuild
				layout.SetLayoutVertical();
			}

			#if UNITY_4_6_1
			// force ContentSizeFitter update text width
			scrollRect.verticalScrollbar.value += 0.1f;
			#endif
		}

		protected virtual void UpdateItems(List<TItem> newItems)
		{
			newItems = SortItems(newItems);

			SelectedIndicies.ForEach(index => {
				var new_index = newItems.FindIndex(x => x.Equals(customItems[index]));
				if (new_index==-1)
				{
					Deselect(index);
				}
			});

			customItems = newItems;

			UpdateView();
		}

		protected virtual float CalculateBottomFillerHeight()
		{
			return bottomHiddenItems * (itemHeight + layout.Spacing.y) - layout.Spacing.y;
		}

		protected virtual float CalculateTopFillerHeight()
		{
			return topHiddenItems * (itemHeight + layout.Spacing.y) - layout.Spacing.y;
		}
		
		void SetBottomFiller()
		{
			bottomFiller.SetAsLastSibling();
			if (bottomHiddenItems==0)
			{
				bottomFiller.gameObject.SetActive(false);
			}
			else
			{
				bottomFiller.gameObject.SetActive(true);
				bottomFiller.sizeDelta = new Vector2(bottomFiller.sizeDelta.x, CalculateBottomFillerHeight());
			}
		}

		void SetTopFiller()
		{
			topFiller.SetAsFirstSibling();
			if (topHiddenItems==0)
			{
				topFiller.gameObject.SetActive(false);
			}
			else
			{
				topFiller.gameObject.SetActive(true);
				topFiller.sizeDelta = new Vector2(bottomFiller.sizeDelta.x, CalculateTopFillerHeight());
			}
		}

		/// <summary>
		/// Determines if item exists with the specified index.
		/// </summary>
		/// <returns><c>true</c> if item exists with the specified index; otherwise, <c>false</c>.</returns>
		/// <param name="index">Index.</param>
		public override bool IsValid(int index)
		{
			return (index >= 0) && (index < customItems.Count);
		}

		/// <summary>
		/// Coloring the specified component.
		/// </summary>
		/// <param name="component">Component.</param>
		protected override void Coloring(ListViewItem component)
		{
			if (component==null)
			{
				return ;
			}
			if (SelectedIndicies.Contains(component.Index))
			{
				SelectColoring(component);
			}
			else
			{
				DefaultColoring(component);
			}
		}

		/// <summary>
		/// Set highlights colors of specified component.
		/// </summary>
		/// <param name="component">Component.</param>
		protected override void HighlightColoring(ListViewItem component)
		{
			if (IsSelected(component.Index))
			{
				return ;
			}
			HighlightColoring(component as TComponent);
		}

		/// <summary>
		/// Set highlights colors of specified component.
		/// </summary>
		/// <param name="component">Component.</param>
		protected virtual void HighlightColoring(TComponent component)
		{
			component.Background.color = HighlightedBackgroundColor;
		}

		/// <summary>
		/// Set select colors of specified component.
		/// </summary>
		/// <param name="component">Component.</param>
		protected virtual void SelectColoring(ListViewItem component)
		{
			if (component==null)
			{
				return ;
			}

			SelectColoring(component as TComponent);
		}

		/// <summary>
		/// Set select colors of specified component.
		/// </summary>
		/// <param name="component">Component.</param>
		protected virtual void SelectColoring(TComponent component)
		{
			component.Background.color = SelectedBackgroundColor;
		}

		/// <summary>
		/// Set default colors of specified component.
		/// </summary>
		/// <param name="component">Component.</param>
		protected virtual void DefaultColoring(ListViewItem component)
		{
			if (component==null)
			{
				return ;
			}

			DefaultColoring(component as TComponent);
		}

		/// <summary>
		/// Set default colors of specified component.
		/// </summary>
		/// <param name="component">Component.</param>
		protected virtual void DefaultColoring(TComponent component)
		{
			component.Background.color = DefaultBackgroundColor;
		}

		void UpdateColors()
		{
			components.ForEach(x => Coloring(x as ListViewItem));
		}

		/// <summary>
		/// This function is called when the MonoBehaviour will be destroyed.
		/// </summary>
		protected override void OnDestroy()
		{
			OnSelect.RemoveListener(OnSelectCallback);
			OnDeselect.RemoveListener(OnDeselectCallback);
			
			RemoveCallbacks();
			
			base.OnDestroy();
		}
	}
}