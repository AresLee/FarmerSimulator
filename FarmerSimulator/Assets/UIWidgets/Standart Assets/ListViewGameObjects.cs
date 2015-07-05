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
	/// ListViewGameObjects event.
	/// </summary>
	public class ListViewGameObjectsEvent : UnityEvent<int,GameObject>
	{
		
	}
	
	[AddComponentMenu("UI/ListViewGameObjects", 255)]
	/// <summary>
	/// List view with GameObjects.
	/// </summary>
	public class ListViewGameObjects : ListViewBase {
		[SerializeField]
		List<GameObject> objects = new List<GameObject>();
		
		/// <summary>
		/// Gets the objects.
		/// </summary>
		/// <value>The objects.</value>
		public List<GameObject> Objects {
			get {
				return new List<GameObject>(objects);
			}
			private set {
				UpdateItems(value);
			}
		}
		
		List<UnityAction<PointerEventData>> callbacksEnter = new List<UnityAction<PointerEventData>>();
		List<UnityAction<PointerEventData>> callbacksExit = new List<UnityAction<PointerEventData>>();
		
		/// <summary>
		/// Sort function.
		/// </summary>
		public Func<IEnumerable<GameObject>,IEnumerable<GameObject>> SortFunc = null;
		
		/// <summary>
		/// What to do when the object selected.
		/// </summary>
		public ListViewGameObjectsEvent OnSelectObject = new ListViewGameObjectsEvent();
		
		/// <summary>
		/// What to do when the object deselected.
		/// </summary>
		public ListViewGameObjectsEvent OnDeselectObject = new ListViewGameObjectsEvent();

		/// <summary>
		/// What to do when the event system send a pointer enter Event.
		/// </summary>
		public ListViewGameObjectsEvent OnPointerEnterObject = new ListViewGameObjectsEvent();

		/// <summary>
		/// What to do when the event system send a pointer exit Event.
		/// </summary>
		public ListViewGameObjectsEvent OnPointerExitObject = new ListViewGameObjectsEvent();

		void Awake()
		{
			Start();
		}
		
		[System.NonSerialized]
		bool isStartedListViewGameObjects = false;

		/// <summary>
		/// Start this instance.
		/// </summary>
		public override void Start()
		{
			if (isStartedListViewGameObjects)
			{
				return ;
			}
			isStartedListViewGameObjects = true;
			
			base.Start();
			
			DestroyGameObjects = true;
			
			UpdateItems();
			
			OnSelect.AddListener(OnSelectCallback);
			OnDeselect.AddListener(OnDeselectCallback);
		}

		void OnSelectCallback(int index, ListViewItem item)
		{
			OnSelectObject.Invoke(index, objects[index]);
		}
		
		void OnDeselectCallback(int index, ListViewItem item)
		{
			OnDeselectObject.Invoke(index, objects[index]);
		}

		void OnPointerEnterCallback(int index)
		{
			OnPointerEnterObject.Invoke(index, objects[index]);
		}
		
		void OnPointerExitCallback(int index)
		{
			OnPointerExitObject.Invoke(index, objects[index]);
		}

		/// <summary>
		/// Updates the items.
		/// </summary>
		public override void UpdateItems()
		{
			UpdateItems(objects);
		}
		
		/// <summary>
		/// Add the specified item.
		/// </summary>
		/// <param name="item">Item.</param>
		/// <returns>Index of added item.</returns>
		public virtual int Add(GameObject item)
		{
			var newObjects = Objects;
			newObjects.Add(item);
			UpdateItems(newObjects);
			
			var index = objects.FindIndex(x => x==item);
			
			return index;
		}
		
		/// <summary>
		/// Remove the specified item.
		/// </summary>
		/// <param name="item">Item.</param>
		/// <returns>Index of removed item.</returns>
		public virtual int Remove(GameObject item)
		{
			var index = objects.FindIndex(x => x==item);
			if (index==-1)
			{
				return index;
			}
			
			var newObjects = Objects;
			newObjects.Remove(item);
			UpdateItems(newObjects);
			
			return index;
		}
		
		void RemoveCallbacks()
		{
			base.Items.ForEach((item, index) => {
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
			});
			callbacksEnter.Clear();
			callbacksExit.Clear();
		}
		
		void AddCallbacks()
		{
			base.Items.ForEach((item, index) => AddCallback(item, index));
		}
		
		void AddCallback(ListViewItem item, int index)
		{
			callbacksEnter.Add(ev => OnPointerEnterCallback(index));
			callbacksExit.Add(ev => OnPointerExitCallback(index));
			
			item.onPointerEnter.AddListener(callbacksEnter[index]);
			item.onPointerExit.AddListener(callbacksExit[index]);
		}

		List<GameObject> SortItems(IEnumerable<GameObject> newItems)
		{
			var temp = newItems;
			if (SortFunc!=null)
			{
				temp = SortFunc(temp);
			}
			
			return temp.ToList();
		}
		
		void UpdateItems(List<GameObject> newItems)
		{
			newItems = SortItems(newItems);
			
			RemoveCallbacks();

			var selected_indicies = new List<int>();
			SelectedIndicies.ForEach(index => {
				var new_index = newItems.FindIndex(x => x==objects[index]);
				if (new_index!=-1)
				{
					selected_indicies.Add(index);
				}
			});
			
			var base_items = new List<ListViewItem>();
			newItems.ForEach(x => {
				var item = x.GetComponent<ListViewItem>() ?? x.AddComponent<ListViewItem>();
				base_items.Add(item);
			});

			objects = newItems;
			base.Items = base_items;

			selected_indicies.ForEach(x => Select(x));

			AddCallbacks();
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
		
		#if UNITY_EDITOR
		[UnityEditor.MenuItem("GameObject/UI/ListViewGameObjects", false, 1070)]
		static void CreateObject()
		{
			Utilites.CreateWidgetFromAsset("ListViewGameObjects");
		}
		#endif
	}
}