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
	/// ListViewBase event.
	/// </summary>
	public class ListViewBaseEvent : UnityEvent<int, ListViewItem>
	{
	}

	/// <summary>
	/// ListViewFocus event.
	/// </summary>
	public class ListViewFocusEvent : UnityEvent<BaseEventData>
	{

	}

	/// <summary>
	/// ListViewBase.
	/// You can use it for creating custom ListViews.
	/// </summary>
	abstract public class ListViewBase : MonoBehaviour,
			ISelectHandler, IDeselectHandler,
			ISubmitHandler, ICancelHandler
	{

		[SerializeField]
		[HideInInspector]
		List<ListViewItem> items = new List<ListViewItem>();
		List<UnityAction> callbacks = new List<UnityAction>();

		/// <summary>
		/// Gets or sets the items.
		/// </summary>
		/// <value>Items.</value>
		public List<ListViewItem> Items {
			get {
				return new List<ListViewItem>(items);
			}
			set {
				UpdateItems(value);
			}
		}

		/// <summary>
		/// The destroy game objects after setting new items.
		/// </summary>
		[SerializeField]
		[HideInInspector]
		public bool DestroyGameObjects = true;

		/// <summary>
		/// Allow select multiple items.
		/// </summary>
		[SerializeField]
		public bool Multiple;

		[SerializeField]
		int selectedIndex = -1;

		/// <summary>
		/// Gets or sets the index of the selected item.
		/// </summary>
		/// <value>The index of the selected.</value>
		public int SelectedIndex {
			get {
				return selectedIndex;
			}
			set {
				if (value==-1)
				{
					if (selectedIndex!=-1)
					{
						Deselect(selectedIndex);
					}

					selectedIndex = value;
				}
				else
				{
					Select(value);
				}
			}
		}

		[SerializeField]
		List<int> selectedIndicies = new List<int>();

		/// <summary>
		/// Gets or sets indicies of the selected items.
		/// </summary>
		/// <value>The selected indicies.</value>
		public List<int> SelectedIndicies {
			get {
				return new List<int>(selectedIndicies);
			}
			set {
				var deselect = selectedIndicies.Where(index => !value.Contains(index)).ToList();
				var select = value.Where(index => !selectedIndicies.Contains(index)).ToList();

				deselect.ForEach(index => Deselect(index));
				select.ForEach(index => Select(index));
			}
		}

		/// <summary>
		/// OnSelect event.
		/// </summary>
		public ListViewBaseEvent OnSelect = new ListViewBaseEvent();

		/// <summary>
		/// OnDeselect event.
		/// </summary>
		public ListViewBaseEvent OnDeselect = new ListViewBaseEvent();

		/// <summary>
		/// OnSubmit event.
		/// </summary>
		public UnityEvent onSubmit = new UnityEvent();

		/// <summary>
		/// OnCancel event.
		/// </summary>
		public UnityEvent onCancel = new UnityEvent();

		/// <summary>
		/// OnItemSelect event.
		/// </summary>
		public UnityEvent onItemSelect = new UnityEvent();

		/// <summary>
		/// onItemCancel event.
		/// </summary>
		public UnityEvent onItemCancel = new UnityEvent();

		/// <summary>
		/// The container for items objects.
		/// </summary>
		[SerializeField]
		public Transform Container;

		/// <summary>
		/// OnFocusIn event.
		/// </summary>
		public ListViewFocusEvent OnFocusIn = new ListViewFocusEvent();
		
		/// <summary>
		/// OnFocusOut event.
		/// </summary>
		public ListViewFocusEvent OnFocusOut = new ListViewFocusEvent();

		GameObject unused;

		void Awake()
		{
			Start();
		}

		[System.NonSerialized]
		bool isStartedListViewBase;

		/// <summary>
		/// Start this instance.
		/// </summary>
		public virtual void Start()
		{
			if (isStartedListViewBase)
			{
				return ;
			}
			isStartedListViewBase = true;

			unused = new GameObject("unused base");
			unused.SetActive(false);
			unused.transform.SetParent(transform, false);
			if ((selectedIndex!=-1) && (selectedIndicies.Count==0))
			{
				selectedIndicies.Add(selectedIndex);
			}
			selectedIndicies = selectedIndicies.Where(x => IsValid(x)).ToList();
			if (selectedIndicies.Count==0)
			{
				selectedIndex = -1;
			}

			//UpdateItems();
		}

		/// <summary>
		/// Updates the items.
		/// </summary>
		public virtual void UpdateItems()
		{
			UpdateItems(items);
		}

		void RemoveCallback(ListViewItem item, int index)
		{
			if (item == null)
			{
				return;
			}
			if (callbacks.Count > index)
			{
				item.onClick.RemoveListener(callbacks[index]);
			}
			item.onSubmit.RemoveListener(Toggle);
			item.onCancel.RemoveListener(OnItemCancel);

			item.onSelect.RemoveListener(HighlightColoring);
			item.onDeselect.RemoveListener(Coloring);

			item.onMove.RemoveListener(OnItemMove);
		}

		void OnItemCancel(ListViewItem item)
		{
			if (EventSystem.current.alreadySelecting)
			{
				return;
			}

			EventSystem.current.SetSelectedGameObject(gameObject);

			onItemCancel.Invoke();
		}

		void RemoveCallbacks()
		{
			if (callbacks.Count > 0)
			{
				items.ForEach(RemoveCallback);
			}
			callbacks.Clear();
		}
		
		void AddCallbacks()
		{
			items.ForEach(AddCallback);
		}

		void AddCallback(ListViewItem item, int index)
		{
			callbacks.Insert(index, () => {
				Toggle(item);
			});

			item.onClick.AddListener(callbacks[index]);

			item.onSubmit.AddListener(OnItemSubmit);
			item.onCancel.AddListener(OnItemCancel);

			item.onSelect.AddListener(HighlightColoring);
			item.onDeselect.AddListener(Coloring);

			item.onMove.AddListener(OnItemMove);
		}

		void OnItemSelect(ListViewItem item)
		{
			onItemSelect.Invoke();
		}

		void OnItemSubmit(ListViewItem item)
		{
			Toggle(item);
			if (!IsSelected(item.Index))
			{
				HighlightColoring(item);
			}
		}
		
		void OnItemMove(AxisEventData eventData, ListViewItem item)
		{
			switch (eventData.moveDir)
			{
				case MoveDirection.Left:
					break;
				case MoveDirection.Right:
					break;
				case MoveDirection.Up:
					if (item.Index > 0)
					{
						SelectComponentByIndex(item.Index - 1);
						//EventSystem.current.SetSelectedGameObject(GetItem(item.Index - 1).gameObject);
						//ScrollTo(item.Index - 1);
					}
					break;
				case MoveDirection.Down:
					if (IsValid(item.Index + 1))
					{
						SelectComponentByIndex(item.Index + 1);
						//EventSystem.current.SetSelectedGameObject(GetItem(item.Index + 1).gameObject);
						//ScrollTo(item.Index + 1);
					}
					break;
			}
		}

		/// <summary>
		/// Scrolls to item with specifid index.
		/// </summary>
		/// <param name="index">Index.</param>
		protected virtual void ScrollTo(int index)
		{

		}


		/// <summary>
		/// Add the specified item.
		/// </summary>
		/// <param name="item">Item.</param>
		/// <returns>Index of added item.</returns>
		public virtual int Add(ListViewItem item)
		{
			item.transform.SetParent(Container, false);
			AddCallback(item, items.Count);

			items.Add(item);
			item.Index = callbacks.Count - 1;

			return callbacks.Count - 1;
		}

		/// <summary>
		/// Clear items of this instance.
		/// </summary>
		public virtual void Clear()
		{
			items.Clear();
			UpdateItems();
		}

		/// <summary>
		/// Remove the specified item.
		/// </summary>
		/// <param name="item">Item.</param>
		/// <returns>Index of removed item.</returns>
		protected virtual int Remove(ListViewItem item)
		{
			RemoveCallbacks();

			var index = item.Index;

			selectedIndicies = selectedIndicies.Where(x => x!=index).Select(x => x > index ? x-- : x).ToList();
			if (selectedIndex==index)
			{
				Deselect(index);
				selectedIndex = selectedIndicies.Count > 0 ? selectedIndicies.Last() : -1;
			}
			else if (selectedIndex > index)
			{
				selectedIndex -= 1;
			}

			items.Remove(item);
			Free(item);

			AddCallbacks();

			return index;
		}
		
		void Free(Component item)
		{
			if (item==null)
			{
				return ;
			}
			if (DestroyGameObjects)
			{
				if (item.gameObject==null)
				{
					return ;
				}
				Destroy(item.gameObject);
			}
			else
			{
				if (item.transform==null)
				{
					return ;
				}
				item.transform.SetParent(unused.transform, false);
			}
		}
		
		void UpdateItems(List<ListViewItem> newItems)
		{
			RemoveCallbacks();

			items.Where(item => !newItems.Contains(item)).ForEach(item => Free(item));

			newItems.ForEach((x, i) => {
				x.Index = i;
				x.transform.SetParent(Container, false);
			});

			//selectedIndicies.Clear();
			//selectedIndex = -1;

			items = newItems;

			AddCallbacks();
		}

		/// <summary>
		/// Determines if item exists with the specified index.
		/// </summary>
		/// <returns><c>true</c> if item exists with the specified index; otherwise, <c>false</c>.</returns>
		/// <param name="index">Index.</param>
		public virtual bool IsValid(int index)
		{
			return (index >= 0) && (index < items.Count);
		}

		/// <summary>
		/// Gets the item.
		/// </summary>
		/// <returns>The item.</returns>
		/// <param name="index">Index.</param>
		protected ListViewItem GetItem(int index)
		{
			return items.Find(x => x.Index==index);
		}

		/// <summary>
		/// Select item by the specified index.
		/// </summary>
		/// <param name="index">Index.</param>
		public virtual void Select(int index)
		{
			if (index==-1)
			{
				return ;
			}

			if (!IsValid(index))
			{
				var message = string.Format("Index must be between 0 and Items.Count ({0}). Gameobject {1}.", items.Count - 1, name);
				throw new IndexOutOfRangeException(message);
			}

			if (IsSelected(index) && Multiple)
			{
				return ;
			}

			if (!Multiple)
			{
				if ((selectedIndex!=-1) && (selectedIndex!=index))
				{
					Deselect(selectedIndex);
				}

				selectedIndicies.Clear();
			}

			selectedIndicies.Add(index);
			selectedIndex = index;

			SelectItem(index);

			OnSelect.Invoke(index, GetItem(index));
		}

		/// <summary>
		/// Deselect item by the specified index.
		/// </summary>
		/// <param name="index">Index.</param>
		public void Deselect(int index)
		{
			if (index==-1)
			{
				return ;
			}
			if (!IsSelected(index))
			{
				return ;
			}

			selectedIndicies.Remove(index);
			selectedIndex = (selectedIndicies.Count > 0)
				? selectedIndicies.Last()
				: - 1;

			DeselectItem(index);
			
			OnDeselect.Invoke(index, GetItem(index));
		}

		/// <summary>
		/// Determines if item is selected with the specified index.
		/// </summary>
		/// <returns><c>true</c> if item is selected with the specified index; otherwise, <c>false</c>.</returns>
		/// <param name="index">Index.</param>
		public bool IsSelected(int index)
		{
			return selectedIndicies.Contains(index);
		}

		/// <summary>
		/// Toggle item by the specified index.
		/// </summary>
		/// <param name="index">Index.</param>
		public void Toggle(int index)
		{
			if (IsSelected(index) && Multiple)
			{
				Deselect(index);
			}
			else
			{
				Select(index);
			}
		}

		/// <summary>
		/// Toggle the specified item.
		/// </summary>
		/// <param name="item">Item.</param>
		void Toggle(ListViewItem item)
		{
			Toggle(item.Index);
		}
		
		/// <summary>
		/// Called when item selected.
		/// Use it for change visible style of selected item.
		/// </summary>
		/// <param name="index">Index.</param>
		protected virtual void SelectItem(int index)
		{

		}

		/// <summary>
		/// Called when item deselected.
		/// Use it for change visible style of deselected item.
		/// </summary>
		/// <param name="index">Index.</param>
		protected virtual void DeselectItem(int index)
		{
			
		}

		/// <summary>
		/// Coloring the specified component.
		/// </summary>
		/// <param name="component">Component.</param>
		protected virtual void Coloring(ListViewItem component)
		{
		}
		
		/// <summary>
		/// Set highlights colors of specified component.
		/// </summary>
		/// <param name="component">Component.</param>
		protected virtual void HighlightColoring(ListViewItem component)
		{
		}

		/// <summary>
		/// This function is called when the MonoBehaviour will be destroyed.
		/// </summary>
		protected virtual void OnDestroy()
		{
			RemoveCallbacks();
			
			items.ForEach(x => Free(x));
		}

		/// <summary>
		/// Set EventSystem.current.SetSelectedGameObject with selected or first item.
		/// </summary>
		/// <returns><c>true</c>, if component was selected, <c>false</c> otherwise.</returns>
		public virtual bool SelectComponent()
		{
			if (items.Count==0)
			{
				return false;
			}
			var index = (SelectedIndex!=-1) ? SelectedIndex : 0;
			SelectComponentByIndex(index);

			return true;
		}

		void SelectComponentByIndex(int index)
		{
			ScrollTo(index);
			
			var ev = new ListViewItemEventData(EventSystem.current) {
				NewSelectedObject = GetItem(index).gameObject
			};
			ExecuteEvents.Execute<ISelectHandler>(ev.NewSelectedObject, ev, ExecuteEvents.selectHandler);
			//EventSystem.current.SetSelectedGameObject(ev.NewSelectedObject, ev);
		}

		/// <summary>
		/// Raises the select event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		void ISelectHandler.OnSelect(BaseEventData eventData)
		{
			if (!EventSystem.current.alreadySelecting)
			{
				EventSystem.current.SetSelectedGameObject(gameObject);
			}
			OnFocusIn.Invoke(eventData);
		}

		/// <summary>
		/// Raises the deselect event.
		/// </summary>
		/// <param name="eventData">Current event data.</param>
		void IDeselectHandler.OnDeselect(BaseEventData eventData)
		{
			OnFocusOut.Invoke(eventData);
		}

		/// <summary>
		/// Raises the submit event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		void ISubmitHandler.OnSubmit(BaseEventData eventData)
		{
			SelectComponent();
			onSubmit.Invoke();
		}

		/// <summary>
		/// Raises the cancel event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		void ICancelHandler.OnCancel(BaseEventData eventData)
		{
			onCancel.Invoke();
		}
	}
}