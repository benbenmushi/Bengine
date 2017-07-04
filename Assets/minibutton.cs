using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class minibutton : MonoBehaviour
{
	public Text         buttonText;
	public event Action onClick;

	public void Clicked()
	{
		if (onClick != null)
			onClick();
	}
}
