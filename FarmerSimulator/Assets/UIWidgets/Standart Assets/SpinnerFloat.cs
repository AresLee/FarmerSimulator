using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;
using System.Collections;

namespace UIWidgets {
	/// <summary>
	/// On change event.
	/// </summary>
	public class OnChangeEventFloat : UnityEvent<float>
	{
	}
	/// <summary>
	/// Submit event.
	/// </summary>
	public class SubmitEventFloat : UnityEvent<float>
	{
	}
	
	[AddComponentMenu("UI/SpinnerFloat", 270)]
	/// <summary>
	/// Spinner.
	/// http://ilih.ru/images/unity-assets/UIWidgets/Spinner.png
	/// </summary>
	public class SpinnerFloat : InputField, IPointerDownHandler {
		[SerializeField]
		float _min;
		
		[SerializeField]
		float _max = 100;
		
		[SerializeField]
		float _step = 1;
		
		[SerializeField]
		float _value;
		
		/// <summary>
		/// Delay of hold in seconds for permanent increase/descrease value.
		/// </summary>
		[SerializeField]
		public float HoldStartDelay = 0.5f;
		
		/// <summary>
		/// Delay of hold in seconds between increase/descrease value.
		/// </summary>
		[SerializeField]
		public float HoldChangeDelay = 0.1f;
		
		/// <summary>
		/// Gets or sets the minimum.
		/// </summary>
		/// <value>The minimum.</value>
		public float Min {
			get {
				return _min;
			}
			set {
				_min = value;
			}
		}
		
		/// <summary>
		/// Gets or sets the maximum.
		/// </summary>
		/// <value>The maximum.</value>
		public float Max {
			get {
				return _max;
			}
			set {
				_max = value;
			}
		}
		
		/// <summary>
		/// Gets or sets the step.
		/// </summary>
		/// <value>The step.</value>
		public float Step {
			get {
				return _step;
			}
			set {
				_step = value;
			}
		}
		
		/// <summary>
		/// Gets or sets the value.
		/// </summary>
		/// <value>The value.</value>
		public float Value {
			get {
				return _value;
			}
			set {
				_value = InBounds(value);
				text = _value.ToString("0.00");
			}
		}
		
		[SerializeField]
		ButtonAdvanced _plusButton;
		
		[SerializeField]
		ButtonAdvanced _minusButton;

		System.Globalization.NumberStyles NumberStyle = System.Globalization.NumberStyles.AllowDecimalPoint
			| System.Globalization.NumberStyles.AllowThousands
			| System.Globalization.NumberStyles.AllowLeadingSign;

		System.Globalization.CultureInfo Culture = System.Globalization.CultureInfo.InvariantCulture;

		/// <summary>
		/// Gets or sets the plus button.
		/// </summary>
		/// <value>The plus button.</value>
		public ButtonAdvanced PlusButton {
			get {
				return _plusButton;
			}
			set {
				if (_plusButton!=null)
				{
					_plusButton.onClick.RemoveListener(OnPlusClick);
					_plusButton.onPointerDown.RemoveListener(OnPlusButtonDown);
					_plusButton.onPointerUp.RemoveListener(OnPlusButtonUp);
				}
				_plusButton = value;
				if (_plusButton!=null)
				{
					_plusButton.onClick.AddListener(OnPlusClick);
					_plusButton.onPointerDown.AddListener(OnPlusButtonDown);
					_plusButton.onPointerUp.AddListener(OnPlusButtonUp);
				}
			}
		}
		
		/// <summary>
		/// Gets or sets the minus button.
		/// </summary>
		/// <value>The minus button.</value>
		public ButtonAdvanced MinusButton {
			get {
				return _minusButton;
			}
			set {
				if (_minusButton!=null)
				{
					_minusButton.onClick.RemoveListener(OnMinusClick);
					_minusButton.onPointerDown.RemoveListener(OnMinusButtonDown);
					_minusButton.onPointerUp.RemoveListener(OnMinusButtonUp);
				}
				_minusButton = value;
				if (_minusButton!=null)
				{
					_minusButton.onClick.AddListener(OnMinusClick);
					_minusButton.onPointerDown.AddListener(OnMinusButtonDown);
					_minusButton.onPointerUp.AddListener(OnMinusButtonUp);
				}
			}
		}
		
		/// <summary>
		/// onValueChange event.
		/// </summary>
		public OnChangeEventFloat onValueChangeFloat = new OnChangeEventFloat();
		
		/// <summary>
		/// onEndEdit event.
		/// </summary>
		public SubmitEventFloat onEndEditFloat = new SubmitEventFloat();

		/// <summary>
		/// onPlusClick event.
		/// </summary>
		public UnityEvent onPlusClick = new UnityEvent();
		
		/// <summary>
		/// onMinusClick event.
		/// </summary>
		public UnityEvent onMinusClick = new UnityEvent();

		/// <summary>
		/// Increase value on step.
		/// </summary>
		public void Plus()
		{
			Value += Step;
		}
		
		/// <summary>
		/// Decrease value on step.
		/// </summary>
		public void Minus()
		{
			Value -= Step;
		}
		
		/// <summary>
		/// Start this instance.
		/// </summary>
		protected override void Start()
		{
			base.Start();
			
			onValidateInput = IntValidate;
			
			base.onValueChange.AddListener(ValueChange);
			base.onEndEdit.AddListener(ValueEndEdit);
			
			PlusButton = _plusButton;
			MinusButton = _minusButton;
			Value = _value;
		}
		
		IEnumerator HoldPlus()
		{
			yield return new WaitForSeconds(HoldStartDelay);
			while (true)
			{
				Plus();
				yield return new WaitForSeconds(HoldChangeDelay);
			}
		}
		
		IEnumerator HoldMinus()
		{
			yield return new WaitForSeconds(HoldStartDelay);
			while (true)
			{
				Minus();
				yield return new WaitForSeconds(HoldChangeDelay);
			}
		}
		
		/// <summary>
		/// Raises the minus click event.
		/// </summary>
		public void OnMinusClick()
		{
			Minus();
			onMinusClick.Invoke();
		}
		
		/// <summary>
		/// Raises the plus click event.
		/// </summary>
		public void OnPlusClick()
		{
			Plus();
			onPlusClick.Invoke();
		}
		
		IEnumerator corutine;
		
		/// <summary>
		/// Raises the plus button down event.
		/// </summary>
		/// <param name="eventData">Current event data.</param>
		public void OnPlusButtonDown(PointerEventData eventData)
		{
			corutine = HoldPlus();
			StartCoroutine(corutine);
		}
		
		/// <summary>
		/// Raises the plus button up event.
		/// </summary>
		/// <param name="eventData">Current event data.</param>
		public void OnPlusButtonUp(PointerEventData eventData)
		{
			StopCoroutine(corutine);
		}
		
		/// <summary>
		/// Raises the minus button down event.
		/// </summary>
		/// <param name="eventData">Current event data.</param>
		public void OnMinusButtonDown(PointerEventData eventData)
		{
			corutine = HoldMinus();
			StartCoroutine(corutine);
		}
		
		/// <summary>
		/// Raises the minus button up event.
		/// </summary>
		/// <param name="eventData">Current event data.</param>
		public void OnMinusButtonUp(PointerEventData eventData)
		{
			StopCoroutine(corutine);
		}
		
		/// <summary>
		/// Ons the destroy.
		/// </summary>
		protected void onDestroy()
		{
			base.onValueChange.RemoveListener(ValueChange);
			base.onEndEdit.RemoveListener(ValueEndEdit);
			
			PlusButton = null;
			MinusButton = null;
		}
		
		void ValueChange(string value)
		{
			if (value.Length==0)
			{
				value = "0";
			}
			float new_value;
			if (float.TryParse(value, NumberStyle, Culture, out new_value))
			{
				_value = InBounds(new_value);
			}
			else
			{
				Value = _value;
			}
			onValueChangeFloat.Invoke(Value);
		}
		
		void ValueEndEdit(string value)
		{
			if (value.Length==0)
			{
				value = "0";
			}
			float new_value;
			if (float.TryParse(value, NumberStyle, Culture, out new_value))
			{
				Value = new_value;
			}
			else
			{
				Value = _value;
			}
			onEndEditFloat.Invoke(Value);
		}
		
		char IntValidate(string validateText, int charIndex, char addedChar)
		{
			var number = validateText.Insert(charIndex, addedChar.ToString());
			
			if ((Min > 0) && (charIndex==0) && (charIndex=='-'))
			{
				return (char)0;
			}
			
			var min_parse_length = ((number.Length > 0) && (number[0]=='-')) ? 2 : 1;
			if (number.Length >= min_parse_length)
			{
				float new_value;
				if (!float.TryParse(number, NumberStyle, Culture, out new_value))
				{
					return (char)0;
				}

				if (new_value!=InBounds(new_value))
				{
					return (char)0;
				}
				
				_value = new_value;
			}
			
			return addedChar;
		}
		
		float InBounds(float value)
		{
			if (value < _min)
			{
				return _min;
			}
			if (value > _max)
			{
				return _max;
			}
			return value;
		}

		#if UNITY_EDITOR
		[UnityEditor.MenuItem("GameObject/UI/SpinnerFloat", false, 1170)]
		static void CreateObject()
		{
			Utilites.CreateWidgetFromAsset("SpinnerFloat");
		}
		#endif
	}
}