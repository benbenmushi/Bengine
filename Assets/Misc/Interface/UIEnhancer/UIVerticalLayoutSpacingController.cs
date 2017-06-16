using UnityEngine;

[RequireComponent(typeof(UnityEngine.UI.VerticalLayoutGroup)), ExecuteInEditMode]
public class UIVerticalLayoutSpacingController : UILayoutSpacingController
{
	protected override void UpdateSpacing(float spacing)
	{
		GetComponent<UnityEngine.UI.VerticalLayoutGroup>().spacing = spacing;
	}
}
