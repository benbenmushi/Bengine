using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(GridLayoutGroup)), ExecuteInEditMode]
public class UIGridLayoutController : MonoBehaviour
{
	public float            refreshRate = 1;
	public bool             runtimeRefreshing = true;

	public UISizeController width = new UISizeController();
	public UISizeController height = new UISizeController();
	public UISizeController spacingX = new UISizeController();
	public UISizeController spacingY = new UISizeController();


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
	private float lastRefresh = -100;

	void OnEnable()
	{
		Refresh();
	}

	public void Refresh()
	{
		width.UpdateRefSize(rectTransform);
		height.UpdateRefSize(rectTransform);
		spacingX.UpdateRefSize(rectTransform);
		spacingY.UpdateRefSize(rectTransform);

		_internal_Refresh();
	}
	private void _internal_Refresh()
	{
		GridLayoutGroup grid = GetComponent<GridLayoutGroup>();

		grid.cellSize = new Vector2(GetSize(width, grid.cellSize.x), GetSize(height, grid.cellSize.y));
		grid.spacing = new Vector2(GetSize(spacingX, grid.spacing.x), GetSize(spacingY, grid.spacing.y));
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
		spacingX.UpdateRefSize(rectTransform);
		spacingY.UpdateRefSize(rectTransform);
	}
	void Update()
	{
		if (!Application.isPlaying || (runtimeRefreshing && lastRefresh + refreshRate < Time.time))
		{
			Refresh();
			lastRefresh = Time.time;
		}
	}

#if UNITY_EDITOR
	[ButtonField("_ForceRefresh")]
	public bool             ForceRefresh = false;
	void _ForceRefresh()
	{
		width.UpdateRefSize(rectTransform, true);
		height.UpdateRefSize(rectTransform, true);
		spacingX.UpdateRefSize(rectTransform, true);
		spacingY.UpdateRefSize(rectTransform, true);
	}
	void OnValidate()
	{
		width.UpdateRefSize(rectTransform);
		height.UpdateRefSize(rectTransform);
		spacingX.UpdateRefSize(rectTransform);
		spacingY.UpdateRefSize(rectTransform);
	}
#endif
}
