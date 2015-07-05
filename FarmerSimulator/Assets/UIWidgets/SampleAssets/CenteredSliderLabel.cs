using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UIWidgets;

namespace UIWidgetsSamples
{
	[RequireComponent(typeof(CenteredSlider))]
	public class CenteredSliderLabel : MonoBehaviour
	{
		[SerializeField]
		Text label;


		void Start()
		{
			var slider = GetComponent<CenteredSlider>();
			slider.OnValuesChange.AddListener(ValueChanged);
			ValueChanged(slider.Value);
		}
		
		void ValueChanged(int value)
		{
			label.text = value.ToString();
		}
	}
}