using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class UIEventRectTransformDimensionChanged : UIBehaviour
{
	public static void AddListenner(GameObject go, System.Action onChange)
	{
#if UNITY_EDITOR
		if (Application.isPlaying)
		{
#endif
			UIEventRectTransformDimensionChanged listenner = go.GetComponent<UIEventRectTransformDimensionChanged>();

			if (listenner == null)
				listenner = go.AddComponent<UIEventRectTransformDimensionChanged>();
			listenner.RectTransformDimensionsChanged += onChange;
#if UNITY_EDITOR
		}
#endif
	}
	public static void RemoveListenner(GameObject go, System.Action onChange)
	{
#if UNITY_EDITOR
		if (Application.isPlaying)
		{
#endif
			UIEventRectTransformDimensionChanged listenner = go.GetComponent<UIEventRectTransformDimensionChanged>();
			if (listenner != null)
				listenner.RectTransformDimensionsChanged -= onChange;
#if UNITY_EDITOR
		}
#endif
	}

	public event System.Action RectTransformDimensionsChanged;

	protected override void OnRectTransformDimensionsChange()
	{
		base.OnBeforeTransformParentChanged();
		if (RectTransformDimensionsChanged != null)
			RectTransformDimensionsChanged();
	}
}
