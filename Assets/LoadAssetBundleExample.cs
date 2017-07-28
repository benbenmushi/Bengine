using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class LoadAssetBundleExample : MonoBehaviour
{
	MyAssetBundle red, blue, yellow;

	public struct MyAssetBundle
	{
		public AssetBundle bundle;
		public string path;
		public Object[] content;

		public MyAssetBundle(string bundleFilePath)
		{
			path = bundleFilePath;
			bundle = null;
			content = null;
		}
		public void LoadAllAssets()
		{
			bundle = AssetBundle.LoadFromFile(path);
			if (bundle != null)
				content = bundle.LoadAllAssets();
		}

		public void OnGUI()
		{
			if (bundle == null)
			{
				if (GUILayout.Button("LoadAllAssets"))
					LoadAllAssets();
			}
			else
			{
				if (content != null && content.Length > 0)
				{
					for (int i = 0; i < content.Length; i++)
					{
						GUILayout.Label(content[i].name + " (" + content[i].GetType() + ")");
						if (content[i].GetType() == typeof(AssetBundleContent))
							(content[i] as AssetBundleContent).OnGUI();
					}
					if (GUILayout.Button("Unload All"))
						bundle.Unload(true);
				}
			}
		}

	}

	void Start()
	{
		red = new MyAssetBundle(Path.Combine(Application.streamingAssetsPath, "AssetBundles/red"));
		blue = new MyAssetBundle(Path.Combine(Application.streamingAssetsPath, "AssetBundles/blue"));
		yellow = new MyAssetBundle(Path.Combine(Application.streamingAssetsPath, "AssetBundles/yellow"));
	}

	Object[] assetBundleContent;

	void OnGUI()
	{
		GUILayout.BeginVertical(GUI.skin.box);
		GUILayout.Label("<==== red ====>");
		red.OnGUI();
		GUILayout.EndVertical();
		GUILayout.BeginVertical(GUI.skin.box);
		GUILayout.Label("<==== blue ====>");
		blue.OnGUI();
		GUILayout.EndVertical();
		GUILayout.BeginVertical(GUI.skin.box);
		GUILayout.Label("<==== yellow ====>");
		yellow.OnGUI();
		GUILayout.EndVertical();
	}
}
