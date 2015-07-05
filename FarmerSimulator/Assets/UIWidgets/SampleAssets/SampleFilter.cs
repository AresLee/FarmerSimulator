using UnityEngine;
using System.Collections;
using UIWidgets;

namespace UIWidgetsSamples {
	[RequireComponent(typeof(Combobox))]
	public class SampleFilter : MonoBehaviour
	{
		public GameObject Container;

		void Start()
		{
			var combobox = GetComponent<Combobox>();
			combobox.OnSelect.AddListener((index, item) => {
				foreach (Transform child in Container.transform)
				{
					var child_active = (item=="All") ? true : child.gameObject.name.StartsWith(item, System.StringComparison.InvariantCulture);
					child.gameObject.SetActive(child_active);
				}
			});
		}
	}
}