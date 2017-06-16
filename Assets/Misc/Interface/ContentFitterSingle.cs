using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

[RequireComponent(typeof(RectTransform)), ExecuteInEditMode]
public class ContentFitterSingle : MonoBehaviour, ILayoutElement, ILayoutSelfController
{

	[Header("Horizontal")]
	public ContentSizeFitter.FitMode horizontalFitMode;
	bool applyHorizontal { get { return horizontalFitMode != ContentSizeFitter.FitMode.Unconstrained; } }


	[Header("Vertical")]
	public ContentSizeFitter.FitMode verticalFitMode;
	bool applyVertical { get { return verticalFitMode != ContentSizeFitter.FitMode.Unconstrained; } }


	List<ILayoutElement> m_childLayout;
	List<ILayoutElement> childLayout
	{
		get
		{
			if (m_childLayout == null || m_childLayout.Count == 0)
			{
				m_childLayout = new List<ILayoutElement>();
				foreach (Transform child in transform)
				{
					MaskableGraphic g = child.GetComponent<MaskableGraphic>();
					ILayoutElement e = g.GetComponent<ILayoutElement>();
					LayoutElement e2 = g.GetComponent<LayoutElement>();
					if (e != null && (e2 == null || !e2.ignoreLayout))
						m_childLayout.Add(e);
				}
			}
			return m_childLayout;
		}
	}

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

	private float lastRefWidthSize = -1;
	private float lastRefHeightSize = -1;

	private float lastRefresh = -100;
	float GetRefWidthSize()
	{
		if (horizontalFitMode == ContentSizeFitter.FitMode.PreferredSize)
		{
			float max = 0;
			childLayout.ForEach(c =>
			{
				if (c.preferredWidth > max)
					max = c.preferredWidth;
			});
			return max;
		}
		else if (horizontalFitMode == ContentSizeFitter.FitMode.MinSize)
		{
			float max = 0;
			childLayout.ForEach(c =>
			{
				if (c.minWidth > max)
					max = c.minWidth;
			});
			return max;
		}
		return 0;
	}
	float GetRefHeightSize()
	{
		if (verticalFitMode == ContentSizeFitter.FitMode.PreferredSize)
		{
			float max = 0;
			childLayout.ForEach(c =>
			{
				if (c.preferredHeight > max)
					max = c.preferredHeight;
			});
			return max;
		}
		else if (verticalFitMode == ContentSizeFitter.FitMode.MinSize)
		{
			float max = 0;
			childLayout.ForEach(c =>
			{
				if (c.minHeight > max)
					max = c.minHeight;
			});
			return max;
		}
		return 0;
	}

	public void Refresh()
	{
		lastRefWidthSize = GetRefWidthSize();
		lastRefHeightSize = GetRefHeightSize();
		rectTransform.sizeDelta = new Vector2(applyHorizontal ? lastRefWidthSize : rectTransform.sizeDelta.x,
											  applyVertical ? lastRefHeightSize : rectTransform.sizeDelta.y);
		//rectTransform.anchoredPosition = new Vector2(applyHorizontal && widthPositionOffset != 0 ? widthPositionOffset * lastRefWidthSize : rectTransform.anchoredPosition.x,
		//											 applyVertical && heightPositionOffset != 0 ? heightPositionOffset * lastRefHeightSize : rectTransform.anchoredPosition.y);
	}
	void Update()
	{
		if (!Application.isPlaying || lastRefresh + 1 < Time.time)
		{
			if (lastRefWidthSize != GetRefWidthSize() || lastRefHeightSize != GetRefHeightSize())
			{
				Refresh();
				lastRefresh = Time.time;
			}
		}
	}
#if UNITY_EDITOR
	void OnValidate()
	{
		lastRefWidthSize = -1; // Force Refresh when inspector's value changes
		lastRefHeightSize = -1; // Force Refresh when inspector's value changes
	}
#endif

	void ILayoutElement.CalculateLayoutInputHorizontal()
	{
		Refresh();
	}

	void ILayoutElement.CalculateLayoutInputVertical()
	{
		Refresh();
	}

	int ILayoutElement.layoutPriority
	{
		get { return 1; }
	}

	float ILayoutElement.minHeight
	{
		get { return rectTransform.sizeDelta.y; }
	}

	float ILayoutElement.minWidth
	{
		get { return rectTransform.sizeDelta.x; }
	}

	float ILayoutElement.preferredHeight
	{
		get
		{
			return rectTransform.sizeDelta.y;
		}
	}

	float ILayoutElement.preferredWidth
	{
		get { return rectTransform.sizeDelta.x; }
	}


	float ILayoutElement.flexibleHeight
	{
		get { return rectTransform.sizeDelta.y; }
	}

	float ILayoutElement.flexibleWidth
	{
		get { return rectTransform.sizeDelta.x; }
	}

	void ILayoutController.SetLayoutHorizontal()
	{
		if (applyHorizontal)
			Refresh();
	}

	void ILayoutController.SetLayoutVertical()
	{
		if (applyVertical)
			Refresh();
	}
}
