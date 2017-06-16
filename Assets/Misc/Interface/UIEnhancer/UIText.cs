using System;
using UnityEngine;
using UnityEngine.UI;

public class UIText : Text
{
	[Serializable]
	public class textChangedEvent : UnityEngine.Events.UnityEvent<string>
	{
		public textChangedEvent()
		{
		}
	}

	public textChangedEvent m_textChangedEvent = new textChangedEvent();
	public textChangedEvent textChanged
	{
		get
		{
			return m_textChangedEvent;
		}
		private set
		{
			m_textChangedEvent = value;
		}
	}

	new public string text
	{
		get
		{
			return base.text;
		}
		set
		{
			base.text = value;
			if (m_textChangedEvent != null)
				m_textChangedEvent.Invoke(value);
		}
	}

	public enum RelativeSizeTarget
	{
		Width,
		Height,
	}
	public enum TextSizeType
	{
		Fixed,
		Relative
	}

	public bool                 enableSizeController = false;
	public RectTransform        customReference = null;
	public float                refreshRate = 1;
	public bool                 runtimeRefreshing = true;
	public TextSizeType         sizeType = TextSizeType.Relative;
	public bool sizeIsFixedValue { get { return sizeType == TextSizeType.Fixed; } }
	[Range(0, 1)]
	public float                size = 0.05f;
	public int                  fixedSize = 12;
	public RelativeSizeTarget   sizeBasedOn = RelativeSizeTarget.Height;
	public bool baseOnWidth { get { return sizeBasedOn == RelativeSizeTarget.Width; } }
	public bool                 bestFit = false;
	public TextSizeType         minSizeType = TextSizeType.Relative;
	public bool minSizeIsFixedValue { get { return minSizeType == TextSizeType.Fixed; } }
	[Range(0, 1)]
	public float                minSize = 0.01f;
	public int                  fixedMinSize = 12;
	public RelativeSizeTarget   minSizeBasedOn = RelativeSizeTarget.Height;
	public bool minSizeBasedOnWidth { get { return minSizeBasedOn == RelativeSizeTarget.Width; } }

	public bool                 adaptTransformHeight = false;
	public bool                 adaptOnUpdate = true;
	public float                adaptMinHeight;
	public UISizeController     adaptOffset;


	bool isValidCanvasRect = false;
	private RectTransform m_canvasRect = null;
	private RectTransform canvasRect
	{
		get
		{
			if (customReference != null)
				return customReference;
			if (m_canvasRect == null || !isValidCanvasRect)
			{
				m_canvasRect = GetComponent<RectTransform>();

				while (m_canvasRect.parent != null)
				{
					if (m_canvasRect.parent.GetComponent<RectTransform>() != null)
						m_canvasRect = m_canvasRect.parent.GetComponent<RectTransform>();
					else
						break;
				}
				isValidCanvasRect = m_canvasRect.GetComponent<Canvas>() != null;
			}
			return m_canvasRect;
		}
	}
	private float lastRefSize = -1;
	private float lastRefMinSize = -1;

	private float lastRefresh = -100;

	void LateUpdate()
	{
		if (enableSizeController)
		{
			if (!Application.isPlaying || (runtimeRefreshing && lastRefresh + refreshRate < Time.time))
			{
				if (sizeIsFixedValue || lastRefSize != (baseOnWidth ? canvasRect.rect.width : canvasRect.rect.height) ||
					(bestFit && (minSizeIsFixedValue || lastRefMinSize != (minSizeBasedOnWidth ? canvasRect.rect.width : canvasRect.rect.height))))
					RefreshFontSize();
				lastRefresh = Time.time;
			}
		}
		if (adaptTransformHeight && adaptOnUpdate)
			_RefreshHeight();
	}
	public void RefreshHeight()
	{
		_RefreshHeight();
	}
	public void RefreshFontSize()
	{
		if (enableSizeController)
		{
			resizeTextForBestFit = bestFit;
			if (!sizeIsFixedValue)
			{
				lastRefSize = baseOnWidth ? canvasRect.rect.width : canvasRect.rect.height;
				fontSize = Mathf.RoundToInt(lastRefSize * size);
				fixedSize = fontSize;
			}
			else
				fontSize = fixedSize;
			if (resizeTextForBestFit)
			{
				if (!minSizeIsFixedValue)
				{
					lastRefMinSize = minSizeBasedOnWidth ? canvasRect.rect.width : canvasRect.rect.height;
					resizeTextMinSize = Mathf.RoundToInt(lastRefSize * minSize);
					fixedMinSize = resizeTextMinSize;
				}
				else
					resizeTextMinSize = fixedMinSize;
				resizeTextMaxSize = fontSize;
			}
		}
	}
	void _RefreshHeight()
	{
		if (adaptTransformHeight)
		{
			adaptOffset.UpdateRefSize(rectTransform);
			float h = Mathf.Max(adaptMinHeight, preferredHeight) + (adaptOffset.apply ? adaptOffset.GetSize(): 0);

			rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, h);
		}
	}
#if UNITY_EDITOR

	[ButtonField("_RefreshHeight")]
	public bool __RefreshHeight;
	

	[ButtonField("_RefreshSize")]
	public bool RefreshSize = false;
	void _RefreshSize()
	{
		m_canvasRect = null;
		RefreshFontSize();
	}
	new void OnValidate()
	{
		base.OnValidate();
		lastRefSize = -1; // Force Refresh when inspector's value changes
		_RefreshHeight();
	}
#endif

#if UNITY_EDITOR
	[UnityEditor.MenuItem("CONTEXT/Text/Replace With UIText")]
	private static void ReplaceTextCommand(UnityEditor.MenuCommand menuCommand)
	{
		Text sourceText = menuCommand.context as Text;

		if (sourceText != null && sourceText.GetComponent<UIText>() == null)
			ReplicaText(sourceText);
	}
	public static void ReplicaText(Text sourceText)
	{
		GameObject go = sourceText.gameObject;
		string text = sourceText.text;
		Font font = sourceText.font;
		int fontSize = sourceText.fontSize;
		float lineSpacing = sourceText.lineSpacing;
		bool richText = sourceText.supportRichText;
		TextAnchor alignment = sourceText.alignment;
		bool alignByGeometry = sourceText.alignByGeometry;
		HorizontalWrapMode hWrapMode = sourceText.horizontalOverflow;
		VerticalWrapMode vWrapMode = sourceText.verticalOverflow;
		bool bestFit = sourceText.resizeTextForBestFit;
		Color color = sourceText.color;
		bool raycastTarget = sourceText.raycastTarget;

		FontData fontData = (FontData)typeof(Text).GetField("m_FontData", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(sourceText);

		UnityEditor.Undo.DestroyObjectImmediate(sourceText);
		UIText newText = go.AddComponent<UIText>();
		UnityEditor.Undo.RegisterCreatedObjectUndo(newText, "new UIText");


		newText.text = text;
		newText.font = font;
		newText.fontSize = fontSize;
		newText.lineSpacing = lineSpacing;
		newText.supportRichText = richText;
		newText.alignment = alignment;
		newText.alignByGeometry = alignByGeometry;
		newText.horizontalOverflow = hWrapMode;
		newText.verticalOverflow = vWrapMode;
		newText.resizeTextForBestFit = bestFit;
		newText.color = color;
		newText.raycastTarget = raycastTarget;
	}
#endif
}
