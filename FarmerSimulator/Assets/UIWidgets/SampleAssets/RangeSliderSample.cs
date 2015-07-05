using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UIWidgets;

namespace UIWidgetsSamples {
	[RequireComponent(typeof(RangeSlider))]
	public class RangeSliderSample : MonoBehaviour {
		[SerializeField]
		Text Text;

		void Start()
		{
			var rs = GetComponent<RangeSlider>();
			rs.OnValuesChange.AddListener(SliderChanged);
			SliderChanged(rs.ValueMin, rs.ValueMax);
		}

		void SliderChanged(int min, int max)
		{
			if (Text!=null)
			{
				Text.text = string.Format("Range: {0:000} - {1:000}", min, max);
			}
		}
	}
}