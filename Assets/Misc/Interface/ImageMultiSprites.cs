using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class ImageMultiSprites : MonoBehaviour
{
	[SerializeField]
	private int m_currentSpriteIndex;

	public int currentSpriteIndex
	{
		get
		{
			return m_currentSpriteIndex;
		}
		set
		{
			m_currentSpriteIndex = Mathf.Clamp(value, 0, sprites.Length);
			if (sprites.Length == 0)
				GetComponent<Image>().sprite = null;
			else
				GetComponent<Image>().sprite = sprites[m_currentSpriteIndex];
		}
	}

	public Sprite[] sprites;


	void OnValidate()
	{
		currentSpriteIndex = m_currentSpriteIndex;
	}

}
