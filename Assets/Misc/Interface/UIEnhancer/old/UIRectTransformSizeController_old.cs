using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform)), ExecuteInEditMode]
public class UIRectTransformSizeController_old : MonoBehaviour, ILayoutElement, ILayoutSelfController
{
	public Vector2          referenceSizeAjust = Vector2.zero;
	public bool				runtimeRefreshing = true;
	public float            refreshRate = 1;


	public bool             applyOnWidth = false;
	public RectTransform    customWidthReference = null;
	public CustomFitMode    customWidthReferenceFitMode = CustomFitMode.RealSize;
	[Range(0, 10)]
	public float            widthSize = 0.05f;
	public float            widthPositionOffset = 0f;
	public bool             widthBasedOnWidth = false;
	public bool             substractWidth = false;
	public bool             parentWidthIsMax = false;

	public bool             applyOnHeigth = false;
	public RectTransform    customHeightReference = null;
	public CustomFitMode    customHeightReferenceFitMode = CustomFitMode.RealSize;
	[Range(0, 10)]
	public float            heightSize = 0.05f;
	public float            heightPositionOffset = 0f;
	public bool             heightBasedOnWidth = false;
	public bool             substractHeight = false;
	public bool             parentHeightIsMax = false;


	public enum CustomFitMode
	{
		RealSize,
		MinSize,
		PreferredSize,
		FexibleSize,
	}

	private Transform lastParent = null;
	private RectTransform m_canvasRectWidth = null;
	private RectTransform canvasRectWidth
	{
		get
		{
			if (customWidthReference != null)
				return customWidthReference;
			if (m_canvasRectWidth == null || lastParent != transform.parent)
			{
				m_canvasRectWidth = GetComponent<RectTransform>();

				while (m_canvasRectWidth.parent != null)
				{
					if (m_canvasRectWidth.parent.GetComponent<RectTransform>() != null)
						m_canvasRectWidth = m_canvasRectWidth.parent.GetComponent<RectTransform>();
					else
						break;
				}
				lastParent = transform.parent;
			}
			return m_canvasRectWidth;
		}
	}
	private RectTransform m_canvasRectHeight = null;
	private RectTransform canvasRectHeight
	{
		get
		{
			if (customHeightReference != null)
				return customHeightReference;
			if (m_canvasRectHeight == null || lastParent != transform.parent)
			{
				m_canvasRectHeight = GetComponent<RectTransform>();

				while (m_canvasRectHeight.parent != null)
				{
					if (m_canvasRectHeight.parent.GetComponent<RectTransform>() != null)
						m_canvasRectHeight = m_canvasRectHeight.parent.GetComponent<RectTransform>();
					else
						break;
				}
				lastParent = transform.parent;
			}
			return m_canvasRectHeight;
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
	private RectTransform m_parentRectTransform;
	private RectTransform parentRectTransform
	{
		get
		{
			if (m_parentRectTransform == null && transform.parent != null)
				m_parentRectTransform = transform.parent.GetComponent<RectTransform>();
			return m_parentRectTransform;
		}
	}

	[SerializeField]
	private float lastRefWidthSize = -1;
	[SerializeField]
	private float lastRefHeightSize = -1;
	private float lastRefresh = -100;

	void OnEnable()
	{
		Refresh();
	}
	float GetRefWidthSize()
	{
		float size = referenceSizeAjust.x;

		if (customWidthReference != null)
		{
			if (customWidthReferenceFitMode != CustomFitMode.RealSize)
			{
				int axis = widthBasedOnWidth ? 0 : 1;

				if (customWidthReferenceFitMode == CustomFitMode.PreferredSize)
					return size + LayoutUtility.GetPreferredSize(canvasRectWidth, axis);
				else if (customWidthReferenceFitMode == CustomFitMode.MinSize)
					return size + LayoutUtility.GetMinSize(canvasRectWidth, axis);
				else if (customWidthReferenceFitMode == CustomFitMode.FexibleSize)
					return size + LayoutUtility.GetFlexibleSize(canvasRectWidth, axis);
			}
			else if (customWidthReference.parent == rectTransform)
			{
				customWidthReferenceFitMode = CustomFitMode.PreferredSize;
				throw new UnityException(name + "(" + GetType().Name + ") customWidthReference is child of this object, your customWidthReferenceFitMode cant be \"RealSize\". This might cause infinite Loops.");
			}
		}
		return size + (widthBasedOnWidth ? canvasRectWidth.rect.width : canvasRectWidth.rect.height);
	}
	float GetRefHeightSize()
	{
		float size = referenceSizeAjust.y;

		if (customHeightReference != null)
		{
			if (customHeightReferenceFitMode != CustomFitMode.RealSize)
			{
				int axis = heightBasedOnWidth ? 0 : 1;
				if (customHeightReferenceFitMode == CustomFitMode.PreferredSize)
					return size + LayoutUtility.GetPreferredSize(canvasRectHeight, axis);
				else if (customHeightReferenceFitMode == CustomFitMode.MinSize)
					return size + LayoutUtility.GetMinSize(canvasRectHeight, axis);
				else if (customHeightReferenceFitMode == CustomFitMode.FexibleSize)
					return size + LayoutUtility.GetFlexibleSize(canvasRectHeight, axis);
			}
			else if (customHeightReference.parent == rectTransform)
			{
				customHeightReferenceFitMode = CustomFitMode.PreferredSize;
				throw new UnityException(name + "(" + GetType().Name + ") customHeightReference is child of this object, your customHeightReferenceFitMode cant be \"RealSize\". This might cause infinite Loops.");
			}
		}
		return size + (heightBasedOnWidth ? canvasRectHeight.rect.width : canvasRectHeight.rect.height);
	}

	public void Refresh()
	{
		lastRefWidthSize = GetRefWidthSize();
		lastRefHeightSize = GetRefHeightSize();
		_internal_Refresh(lastRefWidthSize, lastRefHeightSize);
	}
	private void _internal_Refresh(float ref_widthSize, float ref_heightSize)
	{
		Vector2 sizeDelta = new Vector2(applyOnWidth ? (substractWidth ? -1 : 1) * ref_widthSize * widthSize : rectTransform.sizeDelta.x,
											  applyOnHeigth ? (substractHeight ? -1 : 1) * ref_heightSize * heightSize : rectTransform.sizeDelta.y);
		Vector2 anchoredPosition =  new Vector2(applyOnWidth && widthPositionOffset != 0 ? widthPositionOffset * ref_widthSize : rectTransform.anchoredPosition.x,
													 applyOnHeigth && heightPositionOffset != 0 ? heightPositionOffset * ref_heightSize : rectTransform.anchoredPosition.y);

		if (parentWidthIsMax)
			sizeDelta.x = Mathf.Min(sizeDelta.x, parentRectTransform.rect.width);
		if (parentHeightIsMax)
			sizeDelta.y = Mathf.Min(sizeDelta.y, parentRectTransform.rect.height);
		rectTransform.sizeDelta = sizeDelta;
		rectTransform.anchoredPosition = anchoredPosition;
	}

	void Start()
	{
		lastRefWidthSize = -1; // Force Refresh when inspector's value changes
		lastRefHeightSize = -1; // Force Refresh when inspector's value changes
		m_canvasRectWidth = null;
		m_canvasRectHeight = null;
	}
	void Update()
	{
		if (!Application.isPlaying || (runtimeRefreshing && lastRefresh + refreshRate < Time.time))
		{
			float newWidthSize = GetRefWidthSize();
			float newHeightSize = GetRefHeightSize();
			if (lastRefWidthSize != newWidthSize || lastRefHeightSize != newHeightSize)
				_internal_Refresh(newWidthSize, newHeightSize);
			lastRefresh = Time.time;
		}
	}

#if UNITY_EDITOR
	[ButtonField("_ForceRefresh")]
	public bool             ForceRefresh = false;
	void _ForceRefresh()
	{
		m_canvasRectWidth = null;
		m_canvasRectHeight = null;
	}
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
		get
		{
			if (applyOnHeigth)
				return rectTransform.sizeDelta.y;
			else
				return 0;
		}
	}

	float ILayoutElement.minWidth
	{
		get
		{
			if (applyOnWidth)
				return rectTransform.sizeDelta.x;
			else
				return 0;
		}
	}

	float ILayoutElement.preferredHeight
	{
		get
		{
			if (applyOnHeigth)
				return rectTransform.sizeDelta.y;
			else
				return 0;
		}
	}

	float ILayoutElement.preferredWidth
	{
		get
		{
			if (applyOnWidth)
				return rectTransform.sizeDelta.x;
			else
				return 0;
		}
	}


	float ILayoutElement.flexibleHeight
	{
		get
		{
			if (applyOnHeigth)
				return rectTransform.sizeDelta.y;
			else
				return 0;
		}
	}

	float ILayoutElement.flexibleWidth
	{
		get
		{
			if (applyOnWidth)
				return rectTransform.sizeDelta.x;
			else
				return 0;
		}
	}

	void ILayoutController.SetLayoutHorizontal()
	{
		if (applyOnWidth)
			Refresh();
	}

	void ILayoutController.SetLayoutVertical()
	{
		if (applyOnHeigth)
			Refresh();
	}
}
