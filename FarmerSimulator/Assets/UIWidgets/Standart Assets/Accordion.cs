using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

namespace UIWidgets {
	[System.Serializable]
	/// <summary>
	/// Accordion item.
	/// </summary>
	public class AccordionItem {
		/// <summary>
		/// The toggle object.
		/// </summary>
		public GameObject ToggleObject;
		/// <summary>
		/// The content object.
		/// </summary>
		public GameObject ContentObject;
		/// <summary>
		/// Default state of content object.
		/// </summary>
		public bool Open;

		[HideInInspector]
		/// <summary>
		/// The current corutine.
		/// </summary>
		public Coroutine CurrentCorutine;

		[HideInInspector]
		/// <summary>
		/// The content object RectTransform.
		/// </summary>
		public RectTransform ContentObjectRect;

		[HideInInspector]
		/// <summary>
		/// The height of the content object.
		/// </summary>
		public float ContentObjectHeight;
	}

	/// <summary>
	/// Accordion event.
	/// </summary>
	public class AccordionEvent : UnityEvent<AccordionItem> {

	}

	[AddComponentMenu("UI/Accordion", 350)]
	/// <summary>
	/// Accordion.
	/// </summary>
	public class Accordion : MonoBehaviour {
		/// <summary>
		/// The items.
		/// </summary>
		public List<AccordionItem> Items = new List<AccordionItem>();

		/// <summary>
		/// OnToggleItem event.
		/// </summary>
		public AccordionEvent OnToggleItem = new AccordionEvent();

		/// <summary>
		/// Only one item can be opened.
		/// </summary>
		public bool OnlyOneOpen = true;

		/// <summary>
		/// Animate open and close.
		/// </summary>
		public bool Animate = true;

		void Start()
		{
			Items.ForEach(x => {
				if (x.Open)
				{
					Open(x);
				}
				else
				{
					Close(x);
				}
				x.ToggleObject.AddComponent<AccordionItemComponent>().OnClick.AddListener(() => ToggleItem(x));
				x.ContentObjectRect = x.ContentObject.GetComponent<RectTransform>();
				x.ContentObjectHeight = x.ContentObjectRect.rect.height;
			});
		}

		void ToggleItem(AccordionItem item)
		{
			if (item.Open)
			{
				if (!OnlyOneOpen)
				{
					Close(item, Animate);
				}
			}
			else
			{
				if (OnlyOneOpen)
				{
					Items.Where(x => x.Open).ForEach(x => {
						Close(x, Animate);
					});
				}

				Open(item, Animate);
			}
		}

		void Open(AccordionItem item, bool animate=false)
		{
			if (item.CurrentCorutine!=null)
			{
				StopCoroutine(item.CurrentCorutine);
				item.ContentObjectRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, item.ContentObjectHeight);
				item.ContentObject.SetActive(false);
			}
			if (animate)
			{
				item.CurrentCorutine = StartCoroutine(OpenCorutine(item));
			}
			else
			{
				item.ContentObject.SetActive(true);
				OnToggleItem.Invoke(item);
			}

			item.ContentObject.SetActive(true);
			item.Open = true;
		}

		void Close(AccordionItem item, bool animate=false)
		{
			if (item.CurrentCorutine!=null)
			{
				StopCoroutine(item.CurrentCorutine);
				item.ContentObject.SetActive(true);
				item.ContentObjectRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, item.ContentObjectHeight);
			}
			if (animate)
			{
				item.CurrentCorutine = StartCoroutine(HideCorutine(item));
			}
			else
			{
				item.ContentObject.SetActive(false);
				OnToggleItem.Invoke(item);
			}
		}

		IEnumerator OpenCorutine(AccordionItem item)
		{
			item.ContentObject.SetActive(true);
			item.Open = true;

			yield return StartCoroutine(Animations.Open(item.ContentObjectRect));

			OnToggleItem.Invoke(item);
		}

		IEnumerator HideCorutine(AccordionItem item)
		{
			yield return StartCoroutine(Animations.Collapse(item.ContentObjectRect));

			item.Open = false;
			item.ContentObject.SetActive(false);

			OnToggleItem.Invoke(item);
		}

		#if UNITY_EDITOR
		[UnityEditor.MenuItem("GameObject/UI/Accordion", false, 1000)]
		static void CreateObject()
		{
			Utilites.CreateWidgetFromAsset("Accordion");
		}
		#endif
	}
}
