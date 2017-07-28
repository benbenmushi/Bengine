using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BundleContent", menuName = "Bengine/BundleContent")]
public class AssetBundleContent : ScriptableObject
{
	public UnityEngine.Object[] references;

	internal void OnGUI()
	{
		if (references != null && references.Length > 0)
		{
			for (int i = 0; i < references.Length; i++)
				GUILayout.Label("\t" + references[i]);
		}
	}
}
