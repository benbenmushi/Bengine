using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform)), ExecuteInEditMode]
public class UIContentSizeFitter : MonoBehaviour
{
	public UISizeController.ReferenceSize.CustomFitMode widthSizeType;
	public UISizeController.ReferenceSize.CustomFitMode heightSizeType;

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

	private UISizeController m_widthController = null;
	private UISizeController widthController
	{
		get
		{
			if (m_widthController == null)
			{
				m_widthController = new UISizeController();
				m_widthController.apply = true;
				m_widthController.sizeReference.customReference = rectTransform;
				m_widthController.sizeReference.customReferenceFitMode = widthSizeType;
				m_widthController.sizeReference.basedOn = UISizeController.ReferenceSize.RelativeSizeTarget.Width;
				m_widthController.relativeSizePercent = 1;
			}
			return m_widthController;
		}
	}
	private UISizeController m_heightController = null;

	private UISizeController heightController
	{
		get
		{
			if (m_heightController == null)
			{
				m_heightController = new UISizeController();
				m_heightController.apply = true;
				m_heightController.sizeReference.customReference = rectTransform;
				m_heightController.sizeReference.customReferenceFitMode = heightSizeType;
				m_heightController.sizeReference.basedOn = UISizeController.ReferenceSize.RelativeSizeTarget.Height;
				m_heightController.relativeSizePercent = 1;
			}
			return m_heightController;
		}
	}

	void Start()
	{
		widthController.UpdateRefSize(rectTransform);
		heightController.UpdateRefSize(rectTransform);
		widthController.SizeChanged += (size) => rectTransform.SetWidth(size);
		heightController.SizeChanged += (size) => rectTransform.SetHeight(size);
	}

#if UNITY_EDITOR
	void Update()
	{
		if (!Application.isPlaying)
		{
			if (widthSizeType != UISizeController.ReferenceSize.CustomFitMode.RealSize)
			{
				widthController.sizeReference.customReferenceFitMode = widthSizeType;
				widthController.UpdateRefSize(rectTransform);
				rectTransform.SetWidth(widthController.GetSize());
			}
			if (heightSizeType != UISizeController.ReferenceSize.CustomFitMode.RealSize)
			{
				heightController.sizeReference.customReferenceFitMode = heightSizeType;
				heightController.UpdateRefSize(rectTransform);
				rectTransform.SetHeight(heightController.GetSize());
			}
		}
	}
	void OnValidate()
	{
		Update();
	}
#endif
}
