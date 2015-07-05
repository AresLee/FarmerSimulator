using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UIWidgets;

namespace UIWidgetsSamples {
	[RequireComponent(typeof(RangeSliderFloat))]
	public class RangeSliderFloatSample : MonoBehaviour {
		[SerializeField]
		Text Text;
		
		void Start()
		{
			var rs = GetComponent<RangeSliderFloat>();
			rs.OnValuesChange.AddListener(SliderChanged);
			SliderChanged(rs.ValueMin, rs.ValueMax);
		}
		
		void SliderChanged(float min, float max)
		{
			if (Text!=null)
			{
				Text.text = string.Format("Range: {0:000.00} - {1:000.00}", min, max);
			}
		}
	}
}