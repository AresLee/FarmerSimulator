using UnityEngine;
using UnityEngine.UI;
using UIWidgets;

namespace UIWidgetsSamples {
	public class SampleProgressBar2 : MonoBehaviour {
		public Progressbar bar;
		
		// Use this for initialization
		void Start()
		{
			var button = GetComponent<Button>();
			button.onClick.AddListener(Click);
		}
		
		void Click()
		{
			if (bar.IsAnimationRun)
			{
				bar.Stop();
			}
			else
			{
				if (bar.Value==0)
				{
					bar.Animate(bar.Max);
				}
				else
				{
					bar.Animate(0);
				}
			}
		}
		
	}
}