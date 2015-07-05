using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

namespace UIWidgets
{
	/// <summary>
	/// Notify.
	/// Manage notifications.
	/// 
	/// How to use:
	/// 1. Create container or containers with layout component. Notifications will be shown in those containers. You can check how it works with NotifyContainer in sample scene.
	/// 2. Create template for notification with Notify component.
	/// 3. If you want change text in runtime set Text property in Notify component.
	/// 4. If you want close notification by button set Hide button property in Notify component.
	/// 5. Write code to show notification
	/// <example>
	/// UIWidgets.Notify.Template("NotifyTemplateSimple").Show("Sticky Notification. Click on the × above to close.");
	/// </example>
	/// UIWidgets.Notify.Template("NotifyTemplateSimple") - return the notification instance by template name.
	/// Show("Sticky Notification. Click on the × above to close.") - show notification with following text;
	/// or
	/// Show(message: "Simple Notification.", customHideDelay = 4.5f, hideAnimation = UIWidgets.Notify.AnimationCollapse, slideUpOnHide = false);
	/// Show notification with following text, hide it after 4.5 seconds, run specified animation on hide without SlideUpOnHide.
	/// </summary>
	public class Notify : MonoBehaviour, ITemplatable
	{
		[SerializeField]
		Button hideButton;

		/// <summary>
		/// Gets or sets the button that close current notification.
		/// </summary>
		/// <value>The hide button.</value>
		public Button HideButton {
			get {
				if (hideButton!=null)
				{
					hideButton.onClick.AddListener(Hide);
				}
				return hideButton;
			}
			set {
				if (hideButton!=null)
				{
					hideButton.onClick.RemoveListener(Hide);
				}
				hideButton = value;
			}
		}

		[SerializeField]
		Text text;

		/// <summary>
		/// Gets or sets the text component.
		/// </summary>
		/// <value>The text.</value>
		public Text Text {
			get {
				return text;
			}
			set {
				text = value;
			}
		}

		[SerializeField]
		float HideDelay = 10f;

		bool isTemplate = true;

		/// <summary>
		/// Gets a value indicating whether this instance is template.
		/// </summary>
		/// <value><c>true</c> if this instance is template; otherwise, <c>false</c>.</value>
		public bool IsTemplate {
			get {
				return isTemplate;
			}
			set {
				isTemplate = value;
			}
		}

		/// <summary>
		/// Gets the name of the template.
		/// </summary>
		/// <value>The name of the template.</value>
		public string TemplateName { get; set; }

		static Templates<Notify> templates;

		/// <summary>
		/// Notify templates.
		/// </summary>
		public static Templates<Notify> Templates {
			get {
				if (templates==null)
				{
					templates = new Templates<Notify>(AddCloseCallback);
				}
				return templates;
			}
			set {
				templates = value;
			}
		}

		/// <summary>
		/// Function used to run show animation.
		/// </summary>
		public Func<Notify,IEnumerator> ShowAnimation;

		/// <summary>
		/// Function used to run hide animation.
		/// </summary>
		public Func<Notify,IEnumerator> HideAnimation;
		Func<Notify,IEnumerator> oldShowAnimation;
		Func<Notify,IEnumerator> oldHideAnimation;

		IEnumerator showCorutine;
		IEnumerator hideCorutine;

		/// <summary>
		/// Start slide up animations after hide current notification. Turn it off if its managed with HideAnimation.
		/// </summary>
		public bool SlideUpOnHide = true;

		void Awake()
		{
			if (IsTemplate)
			{
				gameObject.SetActive(false);
			}
		}

		static void FindTemplates()
		{
			Templates.FindTemplates();
		}

		void OnDestroy()
		{
			if (!IsTemplate)
			{
				templates = null;
				return ;
			}
			//if FindTemplates never called than TemplateName==null
			if (TemplateName!=null)
			{
				DeleteTemplate(TemplateName);
			}
		}

		/// <summary>
		/// Clears the cached instance of templates.
		/// </summary>
		static public void ClearCache()
		{
			Templates.ClearCache();
		}

		/// <summary>
		/// Clears the cached instance of specified template.
		/// </summary>
		/// <param name="templateName">Template name.</param>
		static public void ClearCache(string templateName)
		{
			Templates.ClearCache(templateName);
		}

		/// <summary>
		/// Gets the template by name.
		/// </summary>
		/// <returns>The template.</returns>
		/// <param name="template">Template name.</param>
		static public Notify GetTemplate(string template)
		{
			return Templates.Get(template);
		}

		/// <summary>
		/// Deletes the template by name.
		/// </summary>
		/// <param name="template">Template.</param>
		static public void DeleteTemplate(string template)
		{
			Templates.Delete(template);
		}

		/// <summary>
		/// Adds the template.
		/// </summary>
		/// <param name="template">Template name.</param>
		/// <param name="notifyTemplate">Notify template object.</param>
		/// <param name="replace">If set to <c>true</c> replace.</param>
		static public void AddTemplate(string template, Notify notifyTemplate, bool replace = true)
		{
			Templates.Add(template, notifyTemplate, replace);
		}

		/// <summary>
		/// Return notification by the specified template name.
		/// </summary>
		/// <param name="template">Template name.</param>
		static public Notify Template(string template)
		{
			return Templates.Instance(template);
		}

		static void AddCloseCallback(Notify notify)
		{
			if (notify.hideButton==null)
			{
				return ;
			}
			notify.hideButton.onClick.AddListener(notify.Hide);
		}

		/// <summary>
		/// Show the notification.
		/// </summary>
		/// <param name="message">Message.</param>
		/// <param name="customHideDelay">Custom hide delay.</param>
		/// <param name="container">Container. Parent object for current notification.</param>
		/// <param name="showAnimation">Function used to run show animation.</param>
		/// <param name="hideAnimation">Function used to run hide animation.</param>
		/// <param name="slideUpOnHide">Start slide up animations after hide current notification.</param>
		public void Show(string message = null,
		                 float? customHideDelay = null,
		                 Transform container = null,
		                 Func<Notify,IEnumerator> showAnimation = null,
		                 Func<Notify,IEnumerator> hideAnimation = null,
		                 bool? slideUpOnHide = null)
		{
			oldShowAnimation = ShowAnimation;
			oldHideAnimation = HideAnimation;
			if ((message!=null) && (text!=null))
			{
				text.text = message;
			}

			if (container!=null)
			{
				transform.SetParent(container, false);
			}

			if (customHideDelay!=null)
			{
				HideDelay = (float)customHideDelay;
			}

			if (slideUpOnHide!=null)
			{
				SlideUpOnHide = (bool)slideUpOnHide;
			}

			if (showAnimation!=null)
			{
				ShowAnimation = showAnimation;
			}

			if (hideAnimation!=null)
			{
				HideAnimation = hideAnimation;
			}

			gameObject.SetActive(true);
			transform.SetAsLastSibling();

			if (ShowAnimation!=null)
			{
				showCorutine = ShowAnimation(this);
				StartCoroutine(showCorutine);
			}

			if (HideDelay > 0.0f)
			{
				hideCorutine = HideCorutine();
				StartCoroutine(hideCorutine);
			}
		}

		IEnumerator HideCorutine()
		{
			yield return new WaitForSeconds(HideDelay);
			if (HideAnimation!=null)
			{
				yield return StartCoroutine(HideAnimation(this));
			}
			Hide();
		}

		/// <summary>
		/// Hide notification.
		/// </summary>
		public void Hide()
		{
			if (SlideUpOnHide)
			{
				SlideUp();
			}

			Return();
		}

		void Return()
		{
			Templates.ToCache(this);

			ShowAnimation = oldShowAnimation;
			HideAnimation = oldHideAnimation;
			
			if (text!=null)
			{
				text.text = Templates.Get(TemplateName).text.text;
			}
		}

		static Stack<RectTransform> slides = new Stack<RectTransform>();
		void SlideUp()
		{
			RectTransform rect;
			SlideUp slide;
			slides = new Stack<RectTransform>(slides.Where(x => x!=null));
			if (slides.Count==0)
			{
				var obj = new GameObject("SlideUp");
				obj.SetActive(false);
				rect = obj.AddComponent<RectTransform>();
				slide = obj.AddComponent<SlideUp>();
				
				//change height don't work without graphic component
				var image = obj.AddComponent<Image>();
				image.color = new Color(0, 0, 0, 0);
			}
			else
			{
				rect = slides.Pop();
				slide = rect.GetComponent<SlideUp>();
			}
			var sourceRect = GetComponent<RectTransform>();
			
			rect.localRotation = sourceRect.localRotation;
			rect.localPosition = sourceRect.localPosition;
			rect.localScale = sourceRect.localScale;
			rect.anchorMin = sourceRect.anchorMin;
			rect.anchorMax = sourceRect.anchorMax;
			rect.anchoredPosition = sourceRect.anchoredPosition;
			rect.anchoredPosition3D = sourceRect.anchoredPosition3D;
			rect.sizeDelta = sourceRect.sizeDelta;
			rect.pivot = sourceRect.pivot;
			
			rect.transform.SetParent(transform.parent, false);
			rect.transform.SetSiblingIndex(transform.GetSiblingIndex());
			
			rect.gameObject.SetActive(true);
			slide.Run();
		}

		/// <summary>
		/// Returns slide to cache.
		/// </summary>
		/// <param name="slide">Slide.</param>
		public static void FreeSlide(RectTransform slide)
		{
			slides.Push(slide);
		}

		/// <summary>
		/// Rotate animation.
		/// </summary>
		/// <param name="notify">Notify.</param>
		static public IEnumerator AnimationRotate(Notify notify)
		{
			var rect = notify.GetComponent<RectTransform>();
			var start_rotarion = rect.rotation.eulerAngles;
			var time = 0.5f;

			var end_time = Time.time + time;
			
			while (Time.time <= end_time)
			{
				var rotation_x = Mathf.Lerp(0, 90, 1 - (end_time - Time.time) / time);

				rect.rotation = Quaternion.Euler(rotation_x, start_rotarion.y, start_rotarion.z);
				yield return null;
			}
			
			//return rotation back for future use
			rect.rotation = Quaternion.Euler(start_rotarion);
		}

		/// <summary>
		/// Collapse animation.
		/// </summary>
		/// <param name="notify">Notify.</param>
		static public IEnumerator AnimationCollapse(Notify notify)
		{
			var rect = notify.GetComponent<RectTransform>();
			var layout = notify.GetComponentInParent<EasyLayout.EasyLayout>();
			var max_height = rect.rect.height;
			var speed = 200f;//pixels per second

			var time = max_height / speed;
			var end_time = Time.time + time;

			while (Time.time <= end_time)
			{
				var height = Mathf.Lerp(max_height, 0, 1 - (end_time - Time.time) / time);
				rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
				if (layout!=null)
				{
					layout.UpdateLayout();
				}
				yield return null;
			}

			//return height back for future use
			rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, max_height);
		}

		#if UNITY_EDITOR
		[UnityEditor.MenuItem("GameObject/UI/Notify Template", false, 1090)]
		static void CreateObject()
		{
			Utilites.CreateWidgetFromAsset("NotifyTemplate");
		}
		#endif
	}
}