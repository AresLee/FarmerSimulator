using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.UI;
using System.Collections.Generic;
using System;

namespace UIWidgets
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(SpinnerFloat), true)]
	public class SpinnerFloatEditor : SpinnerEditor
	{
	}
}