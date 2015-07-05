using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using System;

namespace UIWidgets
{
	[Serializable]
	/// <summary>
	/// Tab.
	/// </summary>
	public class Tab
	{
		/// <summary>
		/// Text of button for this tab.
		/// </summary>
		public string Name;

		/// <summary>
		/// The tab object.
		/// </summary>
		public GameObject TabObject;
	}
	
	[AddComponentMenu("UI/Tabs", 290)]
	/// <summary>
	/// Tabs.
	/// http://ilih.ru/images/unity-assets/UIWidgets/Tabs.png
	/// </summary>
	public class Tabs : MonoBehaviour
	{
		[SerializeField]
		/// <summary>
		/// The container for tab toggle buttons.
		/// </summary>
		public Transform Container;

		[SerializeField]
		/// <summary>
		/// The default tab button.
		/// </summary>
		public Button DefaultTabButton;

		[SerializeField]
		/// <summary>
		/// The active tab button.
		/// </summary>
		public Button ActiveTabButton;

		[SerializeField]
		Tab[] tabObjects = new Tab[]{};

		/// <summary>
		/// Gets or sets the tab objects.
		/// </summary>
		/// <value>The tab objects.</value>
		public Tab[] TabObjects {
			get {
				return tabObjects;
			}
			set {
				tabObjects = value;
				UpdateButtons();
			}
		}

		List<Button> defaultButtons = new List<Button>();
		List<Button> activeButtons = new List<Button>();
		List<UnityAction> callbacks = new List<UnityAction>();

		void Start()
		{
			if (Container==null)
			{
				throw new NullReferenceException("Container is null. Set object of type GameObject to Container.");
			}
			if (DefaultTabButton==null)
			{
				throw new NullReferenceException("DefaultTabButton is null. Set object of type GameObject to DefaultTabButton.");
			}
			if (ActiveTabButton==null)
			{
				throw new NullReferenceException("ActiveTabButton is null. Set object of type GameObject to ActiveTabButton.");
			}
			DefaultTabButton.gameObject.SetActive(false);
			ActiveTabButton.gameObject.SetActive(false);

			UpdateButtons();
		}
		
		void UpdateButtons()
		{
			if (tabObjects.Length==0)
			{
				throw new ArgumentException("TabObjects array is empty. Fill it.");
			}

			//remove callbacks
			defaultButtons.ForEach((x, index) => x.onClick.RemoveListener(callbacks[index]));
			callbacks.Clear();

			//update buttons
			CreateButtons();
			
			//set callbacks
			tabObjects.ToList().ForEach((x, index) => {
				var tabName = x.Name;
				UnityAction callback = () => SelectTab(tabName);
				callbacks.Add(callback);

				defaultButtons[index].onClick.AddListener(callbacks[index]);
			});

			SelectTab(tabObjects[0].Name);
		}

		/// <summary>
		/// Selects the tab.
		/// </summary>
		/// <param name="tabName">Tab name.</param>
		public void SelectTab(string tabName)
		{
			var index = Array.FindIndex(tabObjects, x => x.Name==tabName);
			if (index==-1)
			{
				throw new ArgumentException(string.Format("Tab with name \"{0}\" not found.", tabName));
			}
			tabObjects.ForEach(x => x.TabObject.SetActive(false));
			tabObjects[index].TabObject.SetActive(true);

			defaultButtons.ForEach(x => x.gameObject.SetActive(true));
			defaultButtons[index].gameObject.SetActive(false);

			activeButtons.ForEach(x => x.gameObject.SetActive(false));
			activeButtons[index].gameObject.SetActive(true);
		}

		void CreateButtons()
		{
			if (tabObjects.Length > defaultButtons.Count)
			{
				for (var i = defaultButtons.Count; i < tabObjects.Length; i++)
				{
					var defaultButton = Instantiate(DefaultTabButton) as Button;
					defaultButton.transform.SetParent(Container, false);

					Utilites.FixInstantiated(DefaultTabButton, defaultButton);

					defaultButtons.Add(defaultButton);

					var activeButton = Instantiate(ActiveTabButton) as Button;
					activeButton.transform.SetParent(Container, false);

					Utilites.FixInstantiated(ActiveTabButton, activeButton);

					activeButtons.Add(activeButton);
				}
			}
			//del existing ui elements if necessary
			if (tabObjects.Length < defaultButtons.Count)
			{
				for (var i = defaultButtons.Count; i > tabObjects.Length; i--)
				{
					Destroy(defaultButtons[i]);
					Destroy(activeButtons[i]);

					defaultButtons.RemoveAt(i);
					activeButtons.RemoveAt(i);
				}
			}

			defaultButtons.ForEach(SetButtonName);
			activeButtons.ForEach(SetButtonName);
		}

		void SetButtonName(Button button, int index)
		{
			button.gameObject.SetActive(true);
			var text = button.GetComponentInChildren<Text>();
			if (text)
			{
				text.text = tabObjects[index].Name;
			}
		}

#if UNITY_EDITOR
		[UnityEditor.MenuItem("GameObject/UI/Tabs", false, 1180)]
		static void CreateObject()
		{
			Utilites.CreateWidgetFromAsset("Tabs");
		}
#endif
	}
}