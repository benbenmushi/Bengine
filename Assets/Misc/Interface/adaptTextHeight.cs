using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class adaptTextHeight : MonoBehaviour
{

	public bool forceUpdate;
	public bool adaptOnUpdate = true;

	public float min;
	public float offset;

	public RectTransform toUpdate;

	void LateUpdate()
	{
		if (adaptOnUpdate)
		{
			Refresh();
		}
	}

	public float Refresh()
	{
		Text t = GetComponent<Text>();

		if (toUpdate == null)
			toUpdate = t.rectTransform;

		float h = Mathf.Max(min, t.preferredHeight) + offset;

		toUpdate.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, h);
		return h;
	}

#if UNITY_EDITOR
	void OnValidate()
	{
		if (!Application.isPlaying)
		{
			forceUpdate = false;
			Refresh();
		}
	}
#endif
}
