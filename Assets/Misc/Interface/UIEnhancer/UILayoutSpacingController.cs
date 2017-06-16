using UnityEngine;
using System.Collections;

public class UILayoutSpacingController : MonoBehaviour
{
	public RectTransform    customReference = null;
	public float            refreshRate = 1;
	public bool             runtimeRefreshing = true;
	[Range(0, 1)]
	public float            size = 0.05f;
	public bool             baseOnWidth = false;

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


	private float lastRefSize = -1;

	void Start()
	{
		this.Refresh();
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
			if (lastRefSize != (baseOnWidth ? canvasRect.rect.width : canvasRect.rect.height))
				RefreshSpacingSize();
			this.lastRefresh = Time.time;
		}
	}
	public void Refresh()
	{
		this.RefreshSpacingSize();
	}
	private void RefreshSpacingSize()
	{
		lastRefSize = baseOnWidth ? canvasRect.rect.width : canvasRect.rect.height;
		UpdateSpacing(lastRefSize * size);
	}

	protected virtual void UpdateSpacing(float spacing)
	{

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
