using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class UISizeController
{
	[System.Serializable]
	public struct ReferenceSize
	{
		public enum CustomFitMode
		{
			RealSize,
			MinSize,
			PreferredSize,
			FexibleSize,
		}
		public enum RelativeSizeTarget
		{
			Width,
			Height,
		}

		public RectTransform        customReference;
		public CustomFitMode        customReferenceFitMode;
		public RelativeSizeTarget   basedOn;
		public float refSize { get; private set; }
		public event System.Action  SizeChanged;

		private RectTransform listeningRectTransform;
		private bool isValidCanvasRect;
		private RectTransform m_target;
		private RectTransform m_canvasRect;
		public RectTransform GetCanvasRect(RectTransform target)
		{
			if (m_canvasRect == null || !isValidCanvasRect)
			{
				m_canvasRect = target.GetComponent<RectTransform>();

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

		void UpdateListenningRectTransform(RectTransform reference)
		{
			if (reference != listeningRectTransform)
			{
				if (listeningRectTransform != null)
					UIEventRectTransformDimensionChanged.RemoveListenner(listeningRectTransform.gameObject, OnDimensionsChanged);
				if (reference != null)
					UIEventRectTransformDimensionChanged.AddListenner(reference.gameObject, OnDimensionsChanged);
				listeningRectTransform = reference;
			}
		}
		void OnDimensionsChanged()
		{
			if (SizeChanged != null)
				SizeChanged();
		}

		public bool UpdateRefSize(RectTransform target, bool forceUpdateCanvasRect = false)
		{
			float _size = 0;
			if (forceUpdateCanvasRect)
				m_canvasRect = null;

			if (customReference)
			{
				UpdateListenningRectTransform(customReference);
				if (customReferenceFitMode != CustomFitMode.RealSize)
				{
					int axis = (int)basedOn;

					if (customReferenceFitMode == CustomFitMode.PreferredSize)
						_size += LayoutUtility.GetPreferredSize(customReference, axis);
					else if (customReferenceFitMode == CustomFitMode.MinSize)
						_size += LayoutUtility.GetMinSize(customReference, axis);
					else if (customReferenceFitMode == CustomFitMode.FexibleSize)
						_size += LayoutUtility.GetFlexibleSize(customReference, axis);
				}
				else
					_size += (basedOn == RelativeSizeTarget.Width ? customReference.rect.width : customReference.rect.height);
			}
			else
			{
				RectTransform canvasRect = GetCanvasRect(target);

				UpdateListenningRectTransform(canvasRect);
				if (basedOn == RelativeSizeTarget.Width)
					_size += canvasRect.rect.width;
				else
					_size += canvasRect.rect.height;
			}
			if (_size != refSize)
			{
				refSize = _size;
				return true;
			}
			return false;
		}

	}

	public enum SizeType
	{
		Relative,
		Fixed
	}

	public bool                 apply;
	public ReferenceSize        sizeReference;
	[Range(0, 10)]
	public float                relativeSizePercent;
	public SizeType             sizeType = SizeType.Relative;
	public float                fixedSize;
	public bool                 invert;
	[SerializeField]
	float                       size;
	RectTransform               m_lastTarget;
	int                         m_lastFrameSizeChangedEvent;

	public event System.Action<float> SizeChanged;

	public bool UpdateRefSize(RectTransform target, bool forceUpdateCanvasRect = false)
	{
		sizeReference.SizeChanged += OnSizeChanged;
		m_lastTarget = target;
		return sizeReference.UpdateRefSize(target, forceUpdateCanvasRect);
	}

	void OnSizeChanged()
	{
		if (SizeChanged != null && apply)
		{
			sizeReference.UpdateRefSize(m_lastTarget);
			if (m_lastFrameSizeChangedEvent != Time.frameCount)
			{
				m_lastFrameSizeChangedEvent = Time.frameCount;
				SizeChanged(GetSize());
			}
		}
	}

	public float GetSize()
	{
		size = sizeType == SizeType.Relative ? sizeReference.refSize * relativeSizePercent : fixedSize;
		if (invert)
			return -size;
		return size;
	}
}
