using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Linq;
using System;

namespace EasyLayout {
	[Flags]
	/// <summary>
	/// Anchors.
	/// </summary>
	public enum Anchors {
		UpperLeft = 0,
		UpperCenter = 1,
		UpperRight = 2,
		
		MiddleLeft = 3,
		MiddleCenter = 4,
		MiddleRight = 5,
		
		LowerLeft = 6,
		LowerCenter = 7,
		LowerRight = 8,
	};

	[Flags]
	/// <summary>
	/// Stackings.
	/// </summary>
	public enum Stackings {
		Horizontal = 0,
		Vertical = 1,
	};

	[Flags]
	/// <summary>
	/// Horizontal aligns.
	/// </summary>
	public enum HorizontalAligns {
		Left = 0,
		Center = 1,
		Right = 2,
	};

	[Flags]
	/// <summary>
	/// Inner aligns.
	/// </summary>
	public enum InnerAligns {
		Top = 0,
		Middle = 1,
		Bottom = 2,
	};

	[Flags]
	/// <summary>
	/// Layout types.
	/// </summary>
	public enum LayoutTypes {
		Compact = 0,
		Grid = 1,
	};

	[ExecuteInEditMode]
	[RequireComponent(typeof(RectTransform))]
	/// <summary>
	/// EasyLayout.
	/// </summary>
	public class EasyLayout : UnityEngine.UI.LayoutGroup  {
		[SerializeField]
		/// <summary>
		/// The group position.
		/// </summary>
		public Anchors GroupPosition = Anchors.UpperLeft;

		[SerializeField]
		/// <summary>
		/// The stacking type.
		/// </summary>
		public Stackings Stacking = Stackings.Horizontal;
		
		[SerializeField]
		/// <summary>
		/// The type of the layout.
		/// </summary>
		public LayoutTypes LayoutType = LayoutTypes.Compact;

		[SerializeField]
		/// <summary>
		/// The row align.
		/// </summary>
		public HorizontalAligns RowAlign = HorizontalAligns.Left;

		[SerializeField]
		/// <summary>
		/// The inner align.
		/// </summary>
		public InnerAligns InnerAlign = InnerAligns.Top;

		[SerializeField]
		/// <summary>
		/// The cell align.
		/// </summary>
		public Anchors CellAlign = Anchors.UpperLeft;

		[SerializeField]
		/// <summary>
		/// The spacing.
		/// </summary>
		public Vector2 Spacing = new Vector2(5, 5);

		[SerializeField]
		/// <summary>
		/// Symmetric margin.
		/// </summary>
		public bool Symmetric = true;

		[SerializeField]
		/// <summary>
		/// The margin.
		/// </summary>
		public Vector2 Margin = new Vector2(5, 5);

		[SerializeField]
		/// <summary>
		/// The margin top.
		/// </summary>
		public float MarginTop = 5f;

		[SerializeField]
		/// <summary>
		/// The margin bottom.
		/// </summary>
		public float MarginBottom = 5f;

		[SerializeField]
		/// <summary>
		/// The margin left.
		/// </summary>
		public float MarginLeft = 5f;

		[SerializeField]
		/// <summary>
		/// The margin right.
		/// </summary>
		public float MarginRight = 5f;

		[SerializeField]
		/// <summary>
		/// The right to left stacking.
		/// </summary>
		public bool RightToLeft = false;

		[SerializeField]
		/// <summary>
		/// The top to bottom stacking.
		/// </summary>
		public bool TopToBottom = true;

		[SerializeField]
		/// <summary>
		/// The skip inactive.
		/// </summary>
		public bool SkipInactive = true;

		/// <summary>
		/// The filter.
		/// </summary>
		public Func<IEnumerable<GameObject>,IEnumerable<GameObject>> Filter = null;

		Vector2 _blockSize;

		/// <summary>
		/// Gets or sets the size of the inner block.
		/// </summary>
		/// <value>The size of the inner block.</value>
		public Vector2 BlockSize {
			get {
				return _blockSize;
			}
			protected set {
				_blockSize = value;
			}
		}

		Vector2 _uiSize;
		/// <summary>
		/// Gets or sets the UI size.
		/// </summary>
		/// <value>The UI size.</value>
		public Vector2 UISize {
			get {
				return _uiSize;
			}
			protected set {
				_uiSize = value;
			}
		}

		/// <summary>
		/// Gets the minimum height.
		/// </summary>
		/// <value>The minimum height.</value>
		public override float minHeight
		{
			get
			{
				return BlockSize[1];
			}
		}

		/// <summary>
		/// Gets the minimum width.
		/// </summary>
		/// <value>The minimum width.</value>
		public override float minWidth
		{
			get
			{
				return BlockSize[0];
			}
		}

		/// <summary>
		/// Gets the preferred height.
		/// </summary>
		/// <value>The preferred height.</value>
		public override float preferredHeight
		{
			get
			{
				return BlockSize[1];
			}
		}

		/// <summary>
		/// Gets the preferred width.
		/// </summary>
		/// <value>The preferred width.</value>
		public override float preferredWidth
		{
			get
			{
				return BlockSize[0];
			}
		}

		static readonly Dictionary<Anchors,Vector2> groupPositions = new Dictionary<Anchors,Vector2>{
			{Anchors.LowerLeft,    new Vector2(0.0f, 0.0f)},
			{Anchors.LowerCenter,  new Vector2(0.5f, 0.0f)},
			{Anchors.LowerRight,   new Vector2(1.0f, 0.0f)},
			
			{Anchors.MiddleLeft,   new Vector2(0.0f, 0.5f)},
			{Anchors.MiddleCenter, new Vector2(0.5f, 0.5f)},
			{Anchors.MiddleRight,  new Vector2(1.0f, 0.5f)},
			
			{Anchors.UpperLeft,    new Vector2(0.0f, 1.0f)},
			{Anchors.UpperCenter,  new Vector2(0.5f, 1.0f)},
			{Anchors.UpperRight,   new Vector2(1.0f, 1.0f)},
		};

		static readonly Dictionary<HorizontalAligns,float> rowAligns = new Dictionary<HorizontalAligns,float>{
			{HorizontalAligns.Left,   0.0f},
			{HorizontalAligns.Center, 0.5f},
			{HorizontalAligns.Right,  1.0f},
		};
		static readonly Dictionary<InnerAligns,float> innerAligns = new Dictionary<InnerAligns,float>{
			{InnerAligns.Top,   0.0f},
			{InnerAligns.Middle, 0.5f},
			{InnerAligns.Bottom,  1.0f},
		};

		bool rebuild;

		/// <summary>
		/// Raises the enable event.
		/// </summary>
		protected override void OnEnable()
		{
			rebuild = true;
			base.OnEnable();
		}

		/// <summary>
		/// Raises the disable event.
		/// </summary>
		protected override void OnDisable()
		{
			base.OnDisable();
		}

		/// <summary>
		/// Raises the did apply animation properties event.
		/// </summary>
		protected override void OnDidApplyAnimationProperties()
		{
			rebuild = true;
			base.OnDidApplyAnimationProperties();
		}

		/// <summary>
		/// Raises the transform parent changed event.
		/// </summary>
		protected override void OnTransformParentChanged()
		{
			rebuild = true;
			base.OnTransformParentChanged();
		}

		/// <summary>
		/// Raises the rect transform removed event.
		/// </summary>
		void OnRectTransformRemoved()
		{
			rebuild = true;
		}

		/// <summary>
		/// Raises the canvas group changed event.
		/// </summary>
		protected override void OnCanvasGroupChanged()
		{
			rebuild = true;
			base.OnCanvasGroupChanged();
		}

		/// <summary>
		/// Raises the rect transform dimensions change event.
		/// </summary>
		protected override void OnRectTransformDimensionsChange()
		{
			rebuild = true;
			base.OnRectTransformDimensionsChange();
		}

		/// <summary>
		/// Raises the transform children changed event.
		/// </summary>
		protected override void OnTransformChildrenChanged()
		{
			rebuild = true;
			base.OnTransformChildrenChanged();
		}

		/// <summary>
		/// Sets the layout horizontal.
		/// </summary>
		public override void SetLayoutHorizontal()
		{
			rebuild = true;
		}

		/// <summary>
		/// Sets the layout vertical.
		/// </summary>
		public override void SetLayoutVertical()
		{
			rebuild = true;
		}

		/// <summary>
		/// Calculates the layout input horizontal.
		/// </summary>
		public override void CalculateLayoutInputHorizontal()
		{
			rebuild = true;
		}

		/// <summary>
		/// Calculates the layout input vertical.
		/// </summary>
		public override void CalculateLayoutInputVertical()
		{
			rebuild = true;
		}

		void Update()
		{
			if (!rebuild)
			{
				return ;
			}
			rebuild = false;

			UpdateLayout();
		}

		float GetLength(RectTransform ui, bool scaled=true)
		{
			if (scaled)
			{
				return Stacking==Stackings.Horizontal ? ScaledWidth(ui) : ScaledHeight(ui);
			}
			return Stacking==Stackings.Horizontal ? ui.rect.width : ui.rect.height;
		}
		
		Vector2 CalculateGroupSize(List<List<RectTransform>> group)
		{
			float width;
			if (LayoutType==LayoutTypes.Compact)
			{
				width = group.Select(row => row.Select(x => ScaledWidth(x) + Spacing.x).Sum()).Max() - Spacing.x;
			}
			else
			{
				var widths = GetMaxColumnsWidths(group);
				width = widths.Sum() + widths.Length * Spacing.x - Spacing.x;
			}
			float height = group.Select(row => row.Select(x => ScaledHeight(x)).Max() + Spacing.y).Sum() - Spacing.y;

			return new Vector2(width, height);
		}

		/// <summary>
		/// Marks layout to update.
		/// </summary>
		public void NeedUpdateLayout()
		{
			rebuild = true;
		}

		void UpdateBlockSize()
		{
			if (Symmetric)
			{
				BlockSize = new Vector2(UISize.x + Margin.x * 2, UISize.y + Margin.y * 2);
			}
			else
			{
				BlockSize = new Vector2(UISize.x + MarginLeft + MarginRight, UISize.y + MarginLeft + MarginRight);
			}
		}

		/// <summary>
		/// Updates the layout.
		/// </summary>
		public void UpdateLayout()
		{
			var group = GroupUIElements();
			if (group.Count==0)
			{
				UISize = new Vector2(0, 0);
				UpdateBlockSize();
				return ;
			}

			UISize = CalculateGroupSize(group);
			UpdateBlockSize();

			var anchor_position = groupPositions[GroupPosition];
			var start_position = new Vector2(
				rectTransform.rect.width * (anchor_position.x - rectTransform.pivot.x),
				rectTransform.rect.height * (anchor_position.y - rectTransform.pivot.y)
			);
			
			start_position.x -= anchor_position.x * UISize.x;
			start_position.y += (1 - anchor_position.y) * UISize.y;

			start_position.x += GetMarginLeft() * ((1 - anchor_position.x) * 2 - 1);
			start_position.y += GetMarginTop() * ((1 - anchor_position.y) * 2 - 1);

			SetPositions(group, start_position, UISize);
		}
		
		Vector2 GetUIPosition(RectTransform ui, Vector2 position, Vector2 align)
		{
			var pivot_fix_x = ScaledWidth(ui) * ui.pivot.x;
			var pivox_fix_y = ScaledHeight(ui) * ui.pivot.y;
			var new_x = position.x + pivot_fix_x + align.x;
			var new_y = position.y - ScaledHeight(ui) + pivox_fix_y - align.y;
			
			return new Vector2(new_x, new_y);
		}
		
		List<float> GetRowsWidths(IList<List<RectTransform>> group)
		{
			var widths = new List<float>();

			foreach (var row_index in Enumerable.Range(0, group.Count))
			{
				var cur_widths = group[row_index].Select(x => ScaledWidth(x)).ToList();
				widths.Add(cur_widths.Sum() + ((cur_widths.Count - 1) * Spacing.x));
			}
			return widths;
		}

		float[] GetMaxColumnsWidths(List<List<RectTransform>> group)
		{
			return Transpose(group).Select(column => column.Select(x => ScaledWidth(x)).Max()).ToArray();
		}

		Vector2 GetMaxCellSize(List<List<RectTransform>> group)
		{
			var rows_max = group.Select(row => GetMaxCellSize(row)).ToList();

			return new Vector2(
				rows_max.Select(x => x.x).Max(),
				rows_max.Select(x => x.y).Max()
			);
		}

		Vector2 GetMaxCellSize(List<RectTransform> row)
		{
			return new Vector2(
				row.Select(x => ScaledWidth(x)).Max(),
				row.Select(x => ScaledHeight(x)).Max()
			);
		}

		Vector2 GetAlign(RectTransform ui, float maxWidth, Vector2 cellMaxSize, float emptyWidth)
		{
			if (LayoutType==LayoutTypes.Compact)
			{
				return new Vector2(
					emptyWidth * rowAligns[RowAlign],
					(cellMaxSize.y - ScaledHeight(ui)) * innerAligns[InnerAlign]
				);
			}
			else
			{
				var cell_align = groupPositions[CellAlign];

				return new Vector2(
					(maxWidth - ScaledWidth(ui)) * cell_align.x,
					(cellMaxSize.y - ScaledHeight(ui)) * (1 - cell_align.y)
				);
			}
		}

		void SetPositions(List<List<RectTransform>> group, Vector2 startPosition, Vector2 groupSize)
		{
			var position = startPosition;
			var rows_widths = GetRowsWidths(group);
			var max_widths = GetMaxColumnsWidths(group);

			var align = new Vector2(0, 0);

			int coord_x = 0;
			foreach (var row in group)
			{
				var row_cell_max_size = GetMaxCellSize(row);

				int coord_y = 0;
				foreach (var ui_element in row)
				{
					align = GetAlign(ui_element, max_widths[coord_y], row_cell_max_size, groupSize.x - rows_widths[coord_x]);

					var new_position = GetUIPosition(ui_element, position, align);;
					if (ui_element.localPosition.x != new_position.x || ui_element.localPosition.y != new_position.y)
					{
						ui_element.localPosition = new_position;
					}

					position.x += ((LayoutType==LayoutTypes.Compact)
					    ? ScaledWidth(ui_element)
						: max_widths[coord_y]) + Spacing.x;

					coord_y += 1;
				}
				position.x = startPosition.x;
				position.y -= row_cell_max_size.y + Spacing.y;

				coord_x += 1;
			}
		}

		List<RectTransform> GetUIElements()
		{
			var children = new List<RectTransform>();
			foreach (Transform child in transform)
			{
				children.Add(child as RectTransform);
			}
			if (SkipInactive)
			{
				children = children.Where(x => x.gameObject.activeSelf).ToList();
			}

			if (Filter!=null)
			{
				var temp = Filter(children.ConvertAll(x => x.gameObject));
				children = temp.Select(x => x.transform as RectTransform).ToList();
			}

			#if (UNITY_4_6 || UNITY_4_7)
			var ui_elements = children.Where(x => {
				var ignorer = x.GetComponent(typeof(ILayoutIgnorer)) as ILayoutIgnorer;//
				return (ignorer==null) || !ignorer.ignoreLayout;
			}).ToList();
			#else
			var ui_elements = children.Where(x => {
				var ignorer = x.GetComponent<ILayoutIgnorer>();
				return (ignorer==null) || !ignorer.ignoreLayout;
			}).ToList();
			#endif

			return ui_elements;
		}

		List<List<RectTransform>> CompactGrouping(List<RectTransform> uiElements, float baseLength)
		{
			var length = baseLength;
			
			var spacing = (Stacking==Stackings.Horizontal) ? Spacing.x : Spacing.y;

			var group = new List<List<RectTransform>>();
			
			var row = new List<RectTransform>();

			foreach (var ui_element in GetUIElements())
			{
				var ui_length = GetLength(ui_element);

				if (row.Count == 0)
				{
					length -= ui_length;
					row.Add(ui_element);
					continue;
				}

				if (length >= (ui_length + spacing))
				{
					length -= ui_length + spacing;
					row.Add(ui_element);
				}
				else
				{
					group.Add(row);
					length = baseLength;
					length -= ui_length;
					row = new List<RectTransform>();
					row.Add(ui_element);
				}
			}
			if (row.Count > 0)
			{
				group.Add(row);
			}

			return group;
		}

		int GetMaxColumnsCount(List<RectTransform> uiElements, float baseLength, int maxColumns)
		{
			var length = baseLength;
			var spacing = (Stacking==Stackings.Horizontal) ? Spacing.x : Spacing.y;

			bool min_columns_setted = false;
			int min_columns = maxColumns;
			int current_columns = 0;

			foreach (var ui_element in uiElements)
			{
				var ui_length = GetLength(ui_element);
				
				if (current_columns==maxColumns)
				{
					min_columns_setted = true;
					min_columns = Math.Min(min_columns, current_columns);
					
					current_columns = 1;
					length = baseLength - ui_length;
					continue;
				}
				if (current_columns == 0)
				{
					current_columns = 1;
					length = baseLength - ui_length;
					continue;
				}
				
				if (length >= (ui_length + spacing))
				{
					length -= ui_length + spacing;
					current_columns++;
				}
				else
				{
					min_columns_setted = true;
					min_columns = Math.Min(min_columns, current_columns);
					
					current_columns = 1;
					length = baseLength - ui_length;
				}
			}
			if (!min_columns_setted)
			{
				min_columns = current_columns;
			}

			return min_columns;
		}

		List<List<RectTransform>> GridGrouping(List<RectTransform> uiElements, float baseLength, int maxColumns=0)
		{
			int max_columns = 999999;
			while (true)
			{
				var new_max_columns = GetMaxColumnsCount(uiElements, baseLength, max_columns);

				if ((max_columns==new_max_columns) || (new_max_columns==1))
				{
					break;
				}
				max_columns = new_max_columns;
			}

			var group = new List<List<RectTransform>>();
			var row = new List<RectTransform>();

			int i = 0;
			foreach (var ui_element in uiElements)
			{
				if ((i > 0) && ((i % max_columns)==0))
				{
					group.Add(row);
					row = new List<RectTransform>();
				}
				row.Add(ui_element);

				i++;
			}
			if (row.Count > 0)
			{
				group.Add(row);
			}

			return group;
		}

		/// <summary>
		/// Gets the margin top.
		/// </summary>
		public float GetMarginTop()
		{
			return Symmetric ? Margin.y : MarginTop;
		}
		
		/// <summary>
		/// Gets the margin bottom.
		/// </summary>
		public float GetMarginBottom()
		{
			return Symmetric ? Margin.y : MarginBottom;
		}

		/// <summary>
		/// Gets the margin left.
		/// </summary>
		public float GetMarginLeft()
		{
			return Symmetric ? Margin.x : MarginLeft;
		}

		/// <summary>
		/// Gets the margin right.
		/// </summary>
		public float GetMarginRight()
		{
			return Symmetric ? Margin.y : MarginRight;
		}

		List<List<RectTransform>> GroupUIElements()
		{
			var base_length = GetLength(rectTransform, false);
			base_length -= (Stacking==Stackings.Horizontal) ? (GetMarginLeft() + GetMarginRight()) : (GetMarginTop() + GetMarginBottom());

			var ui_elements = GetUIElements();

			var group = (LayoutType==LayoutTypes.Compact)
				? CompactGrouping(ui_elements, base_length)
				: GridGrouping(ui_elements, base_length);

			if (Stacking==Stackings.Vertical)
			{
				group = Transpose(group);
			}

			if (!TopToBottom)
			{
				group.Reverse();
			}
			
			if (RightToLeft)
			{
				group.ForEach(x => x.Reverse());
			}

			return group;
		}

		static List<List<T>> Transpose<T>(List<List<T>> group)
		{
			var result = new List<List<T>>();

			int i = 0;
			foreach (var row in group)
			{
				int j = 0;
				foreach (var element in row)
				{
					if (result.Count<=j)
					{
						result.Add(new List<T>());
					}
					result[j].Add(element);

					j += 1;
				}

				i += 1;
			}

			return result;
		}

		static void Log(IEnumerable<float> values)
		{
			Debug.Log("[" + string.Join("; ", values.Select(x => x.ToString()).ToArray()) + "]");
		}

		static float ScaledWidth(RectTransform ui)
		{
			return ui.rect.width * ui.localScale.x;
		}
		static float ScaledHeight(RectTransform ui)
		{
			return ui.rect.height * ui.localScale.y;
		}
	}
}
