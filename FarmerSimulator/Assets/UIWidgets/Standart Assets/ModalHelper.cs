using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace UIWidgets
{
	[RequireComponent(typeof(RectTransform))]
	/// <summary>
	/// Modal helper for UI widgets.
	/// <example>modalKey = ModalHelper.Open(this, modalSprite, modalColor);
	/// //...
	/// ModalHelper.Close(modalKey);</example>
	/// </summary>
	public class ModalHelper : MonoBehaviour, ITemplatable
	{
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

		static Templates<ModalHelper> Templates = new Templates<ModalHelper>();

		static Dictionary<int,ModalHelper> used = new Dictionary<int,ModalHelper>();

		static string key = "ModalTemplate";

		/// <summary>
		/// Create modal helper with the specified parent, sprite and color.
		/// </summary>
		/// <param name="parent">Parent.</param>
		/// <param name="sprite">Sprite.</param>
		/// <param name="color">Color.</param>
		/// <returns>Modal helper index</returns>
		public static int Open(MonoBehaviour parent, Sprite sprite = null, Color? color = null)
		{
			//проверить есть ли в кеше
			if (!Templates.Exists(key))
			{
				Templates.FindTemplates();
				CreateTemplate();
			}

			var modal = Templates.Instance(key);

			modal.transform.SetParent(Utilites.FindCanvas(parent.transform), false);
			modal.gameObject.SetActive(true);
			modal.transform.SetAsLastSibling();

			var rect = modal.GetComponent<RectTransform>();
			rect.sizeDelta = new Vector2(0, 0);
			rect.anchorMin = new Vector2(0, 0);
			rect.anchorMax = new Vector2(1, 1);
			rect.anchoredPosition = new Vector2(0, 0);

			var img = modal.GetComponent<Image>();
			if (sprite!=null)
			{
				img.sprite = sprite;
			}
			if (color!=null)
			{
				img.color = (Color)color;
			}

			used.Add(modal.GetInstanceID(), modal);
			return modal.GetInstanceID();
		}

		static void CreateTemplate()
		{
			var template = new GameObject(key);

			var modal = template.AddComponent<ModalHelper>();
			template.AddComponent<Image>();

			Templates.Add(key, modal);
		}

		/// <summary>
		/// Close modal helper with the specified index.
		/// </summary>
		/// <param name="index">Index.</param>
		public static void Close(int index)
		{
			Templates.ToCache(used[index]);
			used.Remove(index);
		}
	}
}