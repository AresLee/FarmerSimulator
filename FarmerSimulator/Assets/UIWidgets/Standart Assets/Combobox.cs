using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

namespace UIWidgets
{
	[RequireComponent(typeof(InputField))]
	[AddComponentMenu("UI/Combobox", 220)]
	/// <summary>
	/// Combobox.
	/// http://ilih.ru/images/unity-assets/UIWidgets/Combobox.png
	/// </summary>
	public class Combobox : MonoBehaviour, ISubmitHandler
	{
		[SerializeField]
		ListView listView;

		/// <summary>
		/// Gets or sets the ListView.
		/// </summary>
		/// <value>ListView component.</value>
		public ListView ListView {
			get {
				return listView;
			}
			set {
				if (listView!=null)
				{
					listParent = null;

					listView.OnSelectString.RemoveListener(SelectItem);
					listView.OnFocusOut.RemoveListener(onFocusHideList);

					listView.onCancel.AddListener(OnListViewCancel);
					listView.onItemCancel.AddListener(OnListViewCancel);

					RemoveDeselectCallbacks();
				}
				listView = value;
				if (listView!=null)
				{
					listParent = listView.transform.parent;

					listView.OnSelectString.AddListener(SelectItem);
					listView.OnFocusOut.AddListener(onFocusHideList);

					listView.onCancel.AddListener(OnListViewCancel);
					listView.onItemCancel.AddListener(OnListViewCancel);

					AddDeselectCallbacks();
				}
			}
		}

		[SerializeField]
		Button toggleButton;

		/// <summary>
		/// Gets or sets the toggle button.
		/// </summary>
		/// <value>The toggle button.</value>
		public Button ToggleButton {
			get {
				return toggleButton;
			}
			set {
				if (toggleButton!=null)
				{
					toggleButton.onClick.RemoveListener(ToggleList);
				}
				toggleButton = value;
				if (toggleButton!=null)
				{
					toggleButton.onClick.AddListener(ToggleList);
				}
			}
		}

		[SerializeField]
		bool editable = true;

		/// <summary>
		/// Gets or sets a value indicating whether this is editable and user can add new items.
		/// </summary>
		/// <value><c>true</c> if editable; otherwise, <c>false</c>.</value>
		public bool Editable {
			get {
				return editable;
			}
			set {
				editable = value;
				input.interactable = editable;
			}
		}
		
		InputField input;

		/// <summary>
		/// OnSelect event.
		/// </summary>
		public ListViewEvent OnSelect = new ListViewEvent();

		Transform _listCanvas;
		Transform listCanvas {
			get {
				if (_listCanvas==null)
				{
					_listCanvas = Utilites.FindCanvas(listView.transform.parent);
				}
				return _listCanvas;
			}
		}
		Transform listParent;

		void Awake()
		{
			Start();
		}

		[System.NonSerialized]
		bool isStartedCombobox;

		/// <summary>
		/// Start this instance.
		/// </summary>
		public void Start()
		{
			if (isStartedCombobox)
			{
				return ;
			}
			isStartedCombobox = true;

			input = GetComponent<InputField>();
			input.onEndEdit.AddListener(InputItem);
			Editable = editable;

			ToggleButton = toggleButton;

			ListView = listView;

			if (listView!=null)
			{
				listView.OnSelectString.RemoveListener(SelectItem);

				listView.OnSelectString.AddListener((index, item) => OnSelect.Invoke(index, item));

				listView.gameObject.SetActive(true);
				listView.Start();
				if ((listView.SelectedIndex==-1) && (listView.Strings.Count > 0))
				{
					listView.SelectedIndex = 0;
				}
				if (listView.SelectedIndex!=-1)
				{
					input.text = listView.Strings[listView.SelectedIndex];
				}
				listView.gameObject.SetActive(false);
				listView.OnSelectString.AddListener(SelectItem);
			}
		}

		/// <summary>
		/// Clear listview and selected item.
		/// </summary>
		public virtual void Clear()
		{
			listView.Clear();
			input.text = string.Empty;
		}

		/// <summary>
		/// Toggles the list visibility.
		/// </summary>
		public void ToggleList()
		{
			if (listView==null)
			{
				return ;
			}
			if (listView.gameObject.activeSelf)
			{
				HideList();
			}
			else
			{
				ShowList();
			}

		}

		/// <summary>
		/// Shows the list.
		/// </summary>
		public void ShowList()
		{
			if (listView==null)
			{
				return ;
			}
			if (listCanvas!=null)
			{
				listParent = listView.transform.parent;
				listView.transform.SetParent(listCanvas);
			}
			listView.gameObject.SetActive(true);
			if (listView.SelectComponent())
			{
				SetChildDeselectListener(EventSystem.current.currentSelectedGameObject);
			}
			else
			{
				EventSystem.current.SetSelectedGameObject(listView.gameObject);
			}
		}

		/// <summary>
		/// Hides the list.
		/// </summary>
		public void HideList()
		{
			if (listView==null)
			{
				return ;
			}
			listView.gameObject.SetActive(false);
			if (listCanvas!=null)
			{
				listView.transform.SetParent(listParent);
			}
		}

		List<SelectListener> childrenDeselect = new List<SelectListener>();
		void onFocusHideList(BaseEventData eventData)
		{
			if (eventData.selectedObject==gameObject)
			{
				return ;
			}

			var ev_item = eventData as ListViewItemEventData;
			if (ev_item!=null)
			{
				if (ev_item.NewSelectedObject!=null)
				{
					SetChildDeselectListener(ev_item.NewSelectedObject);
				}
				return ;
			}

			var ev = eventData as PointerEventData;
			if (ev==null)
			{
				HideList();
				return ;
			}

			var go = ev.pointerPressRaycast.gameObject;//ev.pointerEnter
			if (go==null)
			{
				return ;
			}

			if (go.Equals(toggleButton.gameObject))
			{
				return ;
			}
			if (go.transform.IsChildOf(listView.transform))
			{
				SetChildDeselectListener(go);
				return ;
			}
			
			HideList();
		}

		void SetChildDeselectListener(GameObject child)
		{
			var deselectListener = GetDeselectListener(child);
			if (!childrenDeselect.Contains(deselectListener))
			{
				deselectListener.onDeselect.AddListener(onFocusHideList);
				childrenDeselect.Add(deselectListener);
			}
		}

		SelectListener GetDeselectListener(GameObject go)
		{
			return go.GetComponent<SelectListener>() ?? go.AddComponent<SelectListener>();
		}

		void AddDeselectCallbacks()
		{
			if (listView.ScrollRect!=null)
			{
				var scrollbar = listView.ScrollRect.verticalScrollbar.gameObject;
				var deselectListener = GetDeselectListener(scrollbar);
				
				deselectListener.onDeselect.AddListener(onFocusHideList);
				childrenDeselect.Add(deselectListener);
				
			}
		}

		void RemoveDeselectCallbacks()
		{
			childrenDeselect.ForEach(x => {
				if (x!=null)
				{
					x.onDeselect.RemoveListener(onFocusHideList);
				}
			});
			childrenDeselect.Clear();
		}

		/// <summary>
		/// Set the specified item.
		/// </summary>
		/// <param name="item">Item.</param>
		/// <returns>Index of item.</returns>
		public int Set(string item, bool allowDuplicate=true)
		{
			int index;
			if (!allowDuplicate)
			{
				index = listView.Strings.FindIndex(x => x==item);
				if (index==-1)
				{
					index = listView.Add(item);
				}
			}
			else
			{
				index = listView.Add(item);
			}
			listView.Select(index);

			return index;
		}

		void SelectItem(int index, string text)
		{
			input.text = text;

			HideList();

			if ((EventSystem.current!=null) && (!EventSystem.current.alreadySelecting))
			{
				EventSystem.current.SetSelectedGameObject(gameObject);
			}
		}

		void InputItem(string item)
		{
			if (!editable)
			{
				return ;
			}
			if (item==string.Empty)
			{
				return ;
			}

			if (!listView.Strings.Contains(item))
			{
				var index = listView.Add(item);
				listView.Select(index);
			}
		}

		void OnDestroy()
		{
			if (input!=null)
			{
				input.onEndEdit.RemoveListener(InputItem);
			}
		}

		void OnListViewCancel()
		{
			HideList();
		}

		/// <summary>
		/// Raises the submit event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		void ISubmitHandler.OnSubmit(BaseEventData eventData)
		{
			ShowList();
		}

#if UNITY_EDITOR
		[UnityEditor.MenuItem("GameObject/UI/Combobox", false, 1030)]
		static void CreateObject()
		{
			Utilites.CreateWidgetFromAsset("Combobox");
		}
#endif

	}
}