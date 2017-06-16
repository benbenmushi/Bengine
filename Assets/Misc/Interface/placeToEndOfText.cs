using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[ExecuteInEditMode]
public class placeToEndOfText : MonoBehaviour {

	public Text target;

	void Update () 
	{
		if (target)
		{
			RectTransform rt = (this.transform as RectTransform);
			if (rt)
			{
				Vector2 pos = rt.anchoredPosition;
				pos.x = target.rectTransform.anchoredPosition.x + target.preferredWidth;
				rt.anchoredPosition = pos;
			}
		}
	}
}
