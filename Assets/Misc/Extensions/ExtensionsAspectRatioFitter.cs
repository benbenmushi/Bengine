using UnityEngine.UI;

public static class ExtensionsAspectRatioFitter
{
#if UNITY_EDITOR
	[UnityEditor.MenuItem("CONTEXT/AspectRatioFitter/FitImageAspect")]
	private static void FitImageAspect(UnityEditor.MenuCommand menuCommand)
	{
		AspectRatioFitter f = menuCommand.context as AspectRatioFitter;
		if (f != null)
		{
			Image img = f.GetComponent<Image>();
			if (img == null && f.transform.parent != null)
				img = f.transform.parent.GetComponent<Image>();
			if (img != null && img.sprite != null)
			{
				UnityEditor.Undo.RecordObject(f, "FitImageAspect");

				f.aspectRatio = (float)img.sprite.texture.width / img.sprite.texture.height;
				UnityEditor.EditorUtility.SetDirty(f);
			}
		}
	}
#endif
}
