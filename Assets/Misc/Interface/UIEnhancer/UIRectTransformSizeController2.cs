using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform)), ExecuteInEditMode]
public class UIRectTransformSizeController2 : MonoBehaviour, ILayoutElement, ILayoutSelfController
{
	public UISizeController width = new UISizeController();
	public UISizeController height = new UISizeController();

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

	void OnEnable()
	{
		Refresh();
	}

	public void Refresh()
	{
		width.UpdateRefSize(rectTransform);
		height.UpdateRefSize(rectTransform);

		_internal_Refresh(GetSize(width, rectTransform.sizeDelta.x), GetSize(height, rectTransform.sizeDelta.y));
	}
	private void _internal_Refresh(float widthSize, float heightSize)
	{
		rectTransform.sizeDelta = new Vector2(widthSize, heightSize);
	}

	private float GetSize(UISizeController config, float defaultSize)
	{
		if (config.apply)
			return config.GetSize();
		return defaultSize;
	}

	void Start()
	{
		width.UpdateRefSize(rectTransform);
		height.UpdateRefSize(rectTransform);
		width.SizeChanged += (size) => _internal_Refresh(size, rectTransform.sizeDelta.y);
		height.SizeChanged += (size) => _internal_Refresh(rectTransform.sizeDelta.x, size);
	}

#if UNITY_EDITOR
	void Update()
	{
		if (!Application.isPlaying)
			Refresh();
	}
	[Button("Force size refresh")]
	void ForceRefresh()
	{
		width.UpdateRefSize(rectTransform, true);
		height.UpdateRefSize(rectTransform, true);
	}
	void OnValidate()
	{
		width.UpdateRefSize(rectTransform);
		height.UpdateRefSize(rectTransform);
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
		get
		{
			if (height.apply)
				return rectTransform.rect.height;
			else
				return 0;
		}
	}

	float ILayoutElement.minWidth
	{
		get
		{
			if (width.apply)
				return rectTransform.rect.width;
			else
				return 0;
		}
	}

	float ILayoutElement.preferredHeight
	{
		get
		{
			if (height.apply)
				return rectTransform.rect.height;
			else
				return 0;
		}
	}

	float ILayoutElement.preferredWidth
	{
		get
		{
			if (width.apply)
				return rectTransform.rect.width;
			else
				return 0;
		}
	}

	float ILayoutElement.flexibleHeight
	{
		get
		{
			if (height.apply)
				return rectTransform.rect.height;
			else
				return 0;
		}
	}

	float ILayoutElement.flexibleWidth
	{
		get
		{
			if (width.apply)
				return rectTransform.rect.width;
			else
				return 0;
		}
	}

	void ILayoutController.SetLayoutHorizontal()
	{
		if (width.apply)
			Refresh();
	}

	void ILayoutController.SetLayoutVertical()
	{
		if (height.apply)
			Refresh();
	}
}
