using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System;

namespace UIWidgets {
	/// <summary>
	/// Draggable handle.
	/// </summary>
	public class DraggableHandle : MonoBehaviour, IDragHandler {
		RectTransform drag;
		Canvas canvas;
		RectTransform canvasRect;

		/// <summary>
		/// Set the specified draggable object.
		/// </summary>
		/// <param name="newDrag">New drag.</param>
		public void Drag(RectTransform newDrag)
		{
			drag = newDrag;
			canvas = Utilites.FindCanvas(transform).GetComponent<Canvas>();
			canvasRect = canvas.GetComponent<RectTransform>();
		}

		/// <summary>
		/// Raises the drag event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public void OnDrag(PointerEventData eventData)
		{
			var delta = Utilites.CalculateDragPosition(eventData.position, canvas, canvasRect)
				- Utilites.CalculateDragPosition(eventData.position - eventData.delta, canvas, canvasRect);
			drag.localPosition += delta;
		}
	}
}