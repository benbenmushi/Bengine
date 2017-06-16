using UnityEngine;
using System.Collections;

public static class ExtensionsCanvasGroup
{
	public static void SetAlpha(this CanvasGroup group, float alpha, float raycastBlockAlphaLimit = 0.38f)
	{
		group.alpha = alpha;
		group.blocksRaycasts = alpha > raycastBlockAlphaLimit;
	}
}
