using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

namespace UIWidgets
{
	[AddComponentMenu("UI/CenteredSlider", 300)]
	/// <summary>
	/// Centered slider (zero at center, positive and negative parts have different scales).
	/// </summary>
	public class CenteredSlider : CenteredSliderBase<int>
	{
		/// <summary>
		/// Value to position.
		/// </summary>
		/// <returns>Position.</returns>
		/// <param name="value">Value.</param>
		protected override float ValueToPosition(int value)
		{
			value = InBounds(value);
			var center = (RangeSize() + HandleSize() / 2) / 2;
			if (value==0)
			{
				return center + GetStartPoint();
			}

			if (value > 0)
			{
				var points_per_unit = (center) / limitMax;
				return (points_per_unit * value) + GetStartPoint() + center;
			}
			else
			{
				var points_per_unit = center / limitMin;
				return (points_per_unit * (limitMin - value)) + GetStartPoint();
			}
		}
		
		/// <summary>
		/// Position to value.
		/// </summary>
		/// <returns>Value.</returns>
		/// <param name="position">Position.</param>
		protected override int PositionToValue(float position)
		{
			var center = (RangeSize() + HandleSize() / 2) / 2;

			if (position==center)
			{
				return 0;
			}

			float value = 0;
			if (position > center)
			{
				var points_per_unit = (center) / limitMax;
			
				value = (position - center) / points_per_unit;
			}
			else
			{
				var points_per_unit = center / limitMin;
				
				value = - position / points_per_unit + LimitMin;
			}
			
			if (WholeNumberOfSteps)
			{
				return InBounds((int)Math.Round(value / step) * step);
			}
			else
			{
				return InBounds((int)Math.Round(value));
			}
		}
		
		/// <summary>
		/// Position range for handle.
		/// </summary>
		/// <returns>The position limits.</returns>
		protected override Vector2 PositionLimits()
		{
			return new Vector2(ValueToPosition(LimitMin), ValueToPosition(LimitMax));
		}

		/// <summary>
		/// Fit value to bounds.
		/// </summary>
		/// <returns>Value.</returns>
		/// <param name="value">Value.</param>
		protected override int InBounds(int value)
		{
			if (value < limitMin)
			{
				return limitMin;
			}
			if (value > limitMax)
			{
				return limitMax;
			}
			return value;
		}

		/// <summary>
		/// Increases the value.
		/// </summary>
		protected override void Increase()
		{
			Value += step;
		}
		
		/// <summary>
		/// Decreases the value.
		/// </summary>
		protected override void Decrease()
		{
			Value -= step;
		}

		/// <summary>
		/// Determines whether this instance is positive value.
		/// </summary>
		/// <returns><c>true</c> if this instance is positive value; otherwise, <c>false</c>.</returns>
		protected override bool IsPositiveValue()
		{
			return Value > 0;
		}


		#if UNITY_EDITOR
		[UnityEditor.MenuItem("GameObject/UI/CenteredSlider", false, 1010)]
		static void CreateObject()
		{
			Utilites.CreateWidgetFromAsset("CenteredSlider");
		}
		#endif
	}
}

