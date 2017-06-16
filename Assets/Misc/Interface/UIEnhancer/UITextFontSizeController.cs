using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[RequireComponent(typeof(Text)), ExecuteInEditMode]
public class UITextFontSizeController : MonoBehaviour
{
	public RectTransform    customReference = null;
	public float            refreshRate = 1;
	public bool             runtimeRefreshing = true;
	public bool             sizeIsFixedValue = false;
	[Range(0, 1)]
	public float            size = 0.05f;
	public int              fixedSize = 12;
	public bool             baseOnWidth = false;
	public bool             bestFit = false;
	public bool             minSizeIsFixedValue = false;
	[Range(0, 1)]
	public float            minSize = 0.01f;
	public int              fixedMinSize = 12;
	public bool             minSizeBasedOnWidth = false;
	private RectTransform m_canvasRect = null;
	private RectTransform canvasRect
	{
		get
		{
			if (customReference != null)
				return customReference;
			if (this.m_canvasRect == null)
			{
				m_canvasRect = this.GetComponent<RectTransform>();

				while (m_canvasRect.parent != null)
				{
					if (canvasRect.parent.GetComponent<RectTransform>() != null)
						m_canvasRect = canvasRect.parent.GetComponent<RectTransform>();
					else
						break;
				}
			}
			return this.m_canvasRect;
		}
	}
	private Text m_text;
	private Text text
	{

		get
		{
			if (this.m_text == null)
				this.m_text = this.GetComponent<Text>();
			return this.m_text;
		}
	}
	private float lastRefSize = -1;
	private float lastRefMinSize = -1;

	void Start()
	{
		Refresh();
	}

	void OnEnable()
	{
		this.Refresh();
	}

	private float lastRefresh = -100;

	void Update()
	{
		if (!Application.isPlaying || (runtimeRefreshing && lastRefresh + refreshRate < Time.time))
		{
			if (sizeIsFixedValue || lastRefSize != (baseOnWidth ? canvasRect.rect.width : canvasRect.rect.height) ||
				(bestFit && (minSizeIsFixedValue || lastRefMinSize != (minSizeBasedOnWidth ? canvasRect.rect.width : canvasRect.rect.height))))
				RefreshFontSize();
			this.lastRefresh = Time.time;
		}
	}
	public void Refresh()
	{

		this.RefreshFontSize();
	}
	private void RefreshFontSize()
	{
		text.resizeTextForBestFit = bestFit;
		if (!sizeIsFixedValue)
		{
			lastRefSize = baseOnWidth ? canvasRect.rect.width : canvasRect.rect.height;
			text.fontSize = Mathf.RoundToInt(lastRefSize * size);
			fixedSize = text.fontSize;
		}
		else
			this.text.fontSize = fixedSize;
		if (text.resizeTextForBestFit)
		{
			if (!minSizeIsFixedValue)
			{
				lastRefMinSize = minSizeBasedOnWidth ? canvasRect.rect.width : canvasRect.rect.height;
				text.resizeTextMinSize = Mathf.RoundToInt(lastRefSize * minSize);
				fixedMinSize = text.resizeTextMinSize;
			}
			else
				text.resizeTextMinSize = fixedMinSize;
			text.resizeTextMaxSize = text.fontSize;
		}
	}

#if UNITY_EDITOR

	[ButtonField("_RefreshSize")]
	public bool RefreshSize = false;
	void _RefreshSize()
	{
		m_canvasRect = null;
		this.Refresh();
	}
	void OnValidate()
	{
		this.lastRefSize = -1; // Force Refresh when inspector's value changes
	}
#endif
}
