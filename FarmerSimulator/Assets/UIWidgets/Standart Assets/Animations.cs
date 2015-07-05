using UnityEngine;
using System.Collections;

namespace UIWidgets
{
	/// <summary>
	/// Animations.
	/// </summary>
	public static class Animations
	{
		/// <summary>
		/// Rotate animation.
		/// </summary>
		/// <returns>Animation.</returns>
		/// <param name="rect">Rect.</param>
		/// <param name="time">Time.</param>
		static public IEnumerator Rotate(RectTransform rect, float time=0.5f)
		{
			if (rect!=null)
			{
				var start_rotarion = rect.rotation.eulerAngles;

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
		}

		/// <summary>
		/// Collapse animation.
		/// </summary>
		/// <returns>Animation.</returns>
		/// <param name="rect">Rect.</param>
		/// <param name="time">Time.</param>
		static public IEnumerator Collapse(RectTransform rect, float time=0.5f)
		{
			if (rect!=null)
			{
				var layout = rect.GetComponentInParent<EasyLayout.EasyLayout>();
				var max_height = rect.rect.height;

				var end_time = Time.time + time;
				
				while (Time.time <= end_time)
				{
					var height = Mathf.Lerp(max_height, 0, 1 - (end_time - Time.time) / time);
					rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
					if (layout!=null)
					{
						layout.NeedUpdateLayout();
					}
					yield return null;
				}
				
				//return height back for future use
				rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, max_height);
			}
		}

		/// <summary>
		/// Open animation.
		/// </summary>
		/// <returns>Animation.</returns>
		/// <param name="rect">Rect.</param>
		/// <param name="time">Time.</param>
		static public IEnumerator Open(RectTransform rect, float time=0.5f)
		{
			if (rect!=null)
			{
				var layout = rect.GetComponentInParent<EasyLayout.EasyLayout>();
				var max_height = rect.rect.height;
				
				var end_time = Time.time + time;
				
				while (Time.time <= end_time)
				{
					var height = Mathf.Lerp(0, max_height, 1 - (end_time - Time.time) / time);
					rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
					if (layout!=null)
					{
						layout.NeedUpdateLayout();
					}
					yield return null;
				}
				
				//return height back for future use
				rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, max_height);
			}
		}
	}
}