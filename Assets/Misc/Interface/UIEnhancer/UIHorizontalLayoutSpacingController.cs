using UnityEngine;

[RequireComponent(typeof(UnityEngine.UI.HorizontalLayoutGroup)), ExecuteInEditMode]
public class UIHorizontalLayoutSpacingController : UILayoutSpacingController
{
	protected override void UpdateSpacing(float spacing)
	{
		GetComponent<UnityEngine.UI.HorizontalLayoutGroup>().spacing = spacing;
	}
}
