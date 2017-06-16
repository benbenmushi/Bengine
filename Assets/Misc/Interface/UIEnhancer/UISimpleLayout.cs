using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform)), ExecuteInEditMode]
public class UISimpleLayout : MonoBehaviour, ILayoutElement, ILayoutController, ILayoutGroup
{
	public LayoutGroupType  layourGroupType = LayoutGroupType.None;

	[Header("Layout Properties")]
	public RectOffset       padding;
	public TextAnchor       childAlignment;

	[Flags]
	public enum LayoutGroupType
	{
		None = 2,
		Horizontal = 0,
		Vertical = 1,
	}

	public bool             arrangeSize = false;
	public float            minimumLayoutSize = 0;
	public float            minimumElementSize = 0;
	public float            spacing = 0;

	public float            refreshRate = 1;

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


	private RectTransform m_rectTransform;
	private RectTransform rectTransform
	{
		get
		{
			if (m_rectTransform == null)
				m_rectTransform = GetComponent<RectTransform>();
			return m_rectTransform;
		}
	}
	List<RectTransform> rectChildren
	{
		get
		{
			List<RectTransform> childrenList = new List<RectTransform>();

			foreach (Transform child in transform)
			{
				RectTransform rect = child as RectTransform;
				if (rect == null || !rect.gameObject.activeInHierarchy || (rect.GetComponent<LayoutElement>() != null && rect.GetComponent<LayoutElement>().ignoreLayout))
					continue;
				childrenList.Add(rect);
			}
			return childrenList;
		}
	}

	private float lastRefresh = -100;

	void Update()
	{
		if (!Application.isPlaying || lastRefresh + refreshRate < Time.time)
		{
			Refresh();
			lastRefresh = Time.time;
		}
	}
	void OnValidate()
	{
	}
	public void Refresh()
	{
		if (isHorizontalLayout)
		{
			float width = ComputeWantedWidth();
			Vector2 pivot = GetAlignementPivot();

			if (arrangeSize)
			{
				this.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
				if (widthResizedEvent != null)
					widthResizedEvent(width);
			}

			///////////////////////
			float x = -padding.top;

			foreach (RectTransform r in rectChildren)
			{
				r.anchoredPosition = new Vector2(x, r.anchoredPosition.y);
				r.pivot = pivot;

				x += GetSizeHorizontal(r);
			}
		}
		else if (isVerticalLayout)
		{
			Vector2 pivot = GetAlignementPivot();
			float height = ComputeWantedHeight();
			float beginY = 0;

			if (arrangeSize)
			{
				this.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
				if (heightResizedEvent != null)
					heightResizedEvent(height);
			}
			beginY = (height - rectTransform.rect.height) * (1 - pivot.y);


			///////////////////////
			float y = beginY - padding.top;

			foreach (RectTransform r in rectChildren)
			{
				r.anchoredPosition = new Vector2(r.anchoredPosition.x, y - (r.rect.height * (1 - pivot.y)));
				r.pivot = pivot;

				y -= GetSizeVertical(r);
			}
		}
	}

	private float GetSizeV(RectTransform arg1)
	{
		throw new NotImplementedException();
	}

	private float GetSizeVertical(RectTransform r)
	{
		if (minimumElementSize > 0)
			return Mathf.Max(minimumElementSize, r.rect.height) + spacing;
		else
			return r.rect.height + spacing;
	}
	private float GetSizeHorizontal(RectTransform r)
	{
		if (minimumElementSize > 0)
			return Mathf.Max(minimumElementSize, r.rect.height) + spacing;
		else
			return r.rect.height + spacing;
	}
	private void ApplySizeVertical(RectTransform r, float y)
	{
		r.anchoredPosition = new Vector2(r.anchoredPosition.x, y);
	}
	private void ApplySizeHorizontal(RectTransform r, float x)
	{
		r.anchoredPosition = new Vector2(x, r.anchoredPosition.y);
	}
	private Vector2 GetAlignementPivot()
	{
		return new Vector2(((int)childAlignment % 3) * 0.5f, 1 - ((int)childAlignment / 3) * 0.5f);
	}

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


	float ILayoutElement.minWidth
	{
		get
		{
			if (isHorizontalLayout)
				return minimumLayoutSize;
			else
				return 0;
		}
	}

	float ILayoutElement.preferredWidth
	{
		get
		{
			if (isHorizontalLayout)
				return minimumLayoutSize;
			else
				return 0;
		}
	}

	float ILayoutElement.flexibleWidth
	{
		get
		{
			if (isHorizontalLayout)
				return minimumLayoutSize;
			else
				return 0;
		}
	}

	float ILayoutElement.minHeight
	{
		get
		{
			if (isVerticalLayout)
				return minimumLayoutSize;
			else
				return 0;
		}
	}

	float ILayoutElement.preferredHeight
	{
		get
		{
			if (isVerticalLayout)
				return minimumLayoutSize;
			else
				return 0;
		}
	}

	float ILayoutElement.flexibleHeight
	{
		get
		{
			if (isVerticalLayout)
				return minimumLayoutSize;
			else
				return 0;
		}
	}

	int ILayoutElement.layoutPriority
	{
		get
		{
			return 1;
		}
	}

	void ILayoutElement.CalculateLayoutInputHorizontal()
	{
		Refresh();
	}

	void ILayoutElement.CalculateLayoutInputVertical()
	{
		Refresh();
	}

	void ILayoutController.SetLayoutHorizontal()
	{
		Refresh();
	}

	void ILayoutController.SetLayoutVertical()
	{
		Refresh();
	}
}
