using UnityEngine;

public static class ExtensionsImage
{
	public static void OnOffSpriteToggle(this UnityEngine.UI.Image img, Sprite spriteOn, Sprite spriteOff, bool isOn)
	{
		if (isOn)
			img.sprite = spriteOn;
		else
			img.sprite = spriteOff;
	}
}
