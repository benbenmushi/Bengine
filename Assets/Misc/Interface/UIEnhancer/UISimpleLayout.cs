using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform)), ExecuteInEditMode]
public class UISimpleLayout : HorizontalOrVerticalLayoutGroup
{
	public LayoutGroupType  layourGroupType = LayoutGroupType.None;

	//[Header("Layout Properties")]
	//public RectOffset       padding;
	//public TextAnchor       childAlignment;

	public enum LayoutGroupType
	{
		None = 2,
		Horizontal = 0,
		Vertical = 1,
	}

	public bool             arrangeSize = false;
	public float            minimumLayoutSize = 0;
	public float            minimumElementSize = 0;

	bool isVerticalLayout
	{
		get
		{
			return layourGroupType == LayoutGroupType.Vertical;
		}
	}
	bool isHorizontalLayout
	{
		get
		{
			return layourGroupType == LayoutGroupType.Horizontal;
		}
	}


	/// <summary>
	/// Called when the width of the layout changed
	/// </summary>
	public event Action<float> widthResizedEvent;
	/// <summary>
	/// Called when the height of the layout changed
	/// </summary>
	public event Action<float> heightResizedEvent;


	//private float GetSizeVertical(RectTransform r)
	//{
	//	if (minimumElementSize > 0)
	//		return Mathf.Max(minimumElementSize, r.rect.height) + spacing;
	//	else
	//		return r.rect.height + spacing;
	//}
	//private float GetSizeHorizontal(RectTransform r)
	//{
	//	if (minimumElementSize > 0)
	//		return Mathf.Max(minimumElementSize, r.rect.height) + spacing;
	//	else
	//		return r.rect.height + spacing;
	//}
	//private void ApplySizeVertical(RectTransform r, float y)
	//{
	//	r.anchoredPosition = new Vector2(r.anchoredPosition.x, y);
	//}
	//private void ApplySizeHorizontal(RectTransform r, float x)
	//{
	//	r.anchoredPosition = new Vector2(x, r.anchoredPosition.y);
	//}
	//private Vector2 GetAlignementPivot()
	//{
	//	return new Vector2(((int)childAlignment % 3) * 0.5f, 1 - ((int)childAlignment / 3) * 0.5f);
	//}

	private float ComputeWantedHeight()
	{
		float h = padding.vertical;

		foreach (RectTransform r in this.rectChildren)
		{
			if (minimumElementSize > 0)
				h += Mathf.Max(minimumElementSize, r.rect.height);
			else
				h += r.rect.height;
		}

		if (rectChildren.Count > 0)
			h += (rectChildren.Count - 1) * spacing;
		if (minimumLayoutSize > 0)
			h = Mathf.Max(h, minimumLayoutSize);

		return h;
	}
	private float ComputeWantedWidth()
	{
		float w = padding.horizontal;

		foreach (RectTransform r in this.rectChildren)
		{
			if (minimumElementSize > 0)
				w += Mathf.Max(minimumElementSize, r.rect.width);
			else
				w += r.rect.width;
		}
		if (rectChildren.Count > 0)
			w += (rectChildren.Count - 1) * spacing;
		if (minimumLayoutSize > 0)
			w = Mathf.Max(w, minimumLayoutSize);

		return w;
	}


	public override void CalculateLayoutInputVertical()
	{
	}

	public override void SetLayoutHorizontal()
	{
		SetChildrenAlongAxis(0, isVerticalLayout);
		if (isHorizontalLayout)
		{
			//Vector2 pivot = GetAlignementPivot();
			float width = ComputeWantedWidth();
			//float beginX = -((width * (1 - pivot.x)) + rectTransform.rect.width * pivot.x);

			//Debug.Log("width=" + width);
			//Debug.Log("rectTransform.rect.width=" + rectTransform.rect.width);
			//Debug.Log("beginX=" + beginX);
			if (arrangeSize)
			{
				this.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
				if (widthResizedEvent != null)
					widthResizedEvent(width);
			}

			/////////////////////////
			//float x = beginX - padding.left;

			//foreach (RectTransform r in rectChildren)
			//{
			//	r.anchoredPosition = new Vector2(x + (r.rect.width * (1 - pivot.x)), r.anchoredPosition.y);
			//	r.pivot = pivot;

			//	x += GetSizeHorizontal(r);
			//}
		}
	}

	public override void SetLayoutVertical()
	{
		SetChildrenAlongAxis(1, isVerticalLayout);
		if (isVerticalLayout)
		{
			//Vector2 pivot = GetAlignementPivot();
			float height = ComputeWantedHeight();
			//float beginY = (height * (1 - pivot.y)) - rectTransform.rect.height * pivot.y;

			if (arrangeSize)
			{
				this.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
				if (heightResizedEvent != null)
					heightResizedEvent(height);
			}

			/////////////////////////
			//float y = beginY - padding.top;

			//foreach (RectTransform r in rectChildren)
			//{
			//	r.anchoredPosition = new Vector2(r.anchoredPosition.x, y - (r.rect.height * (1 - pivot.y)));
			//	r.pivot = pivot;

			//	y -= GetSizeVertical(r);
			//}
		}
	}
}
