﻿using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class CreateAssetBundles
{
	[MenuItem("Assets/Build AssetBundles")]
	static void BuildRedBundle()
	{
		string assetBundleDirectory = "Assets/AssetBundles";
		if (!Directory.Exists(assetBundleDirectory))
			Directory.CreateDirectory(assetBundleDirectory);
		BuildPipeline.BuildAssetBundles(assetBundleDirectory, BuildAssetBundleOptions.ChunkBasedCompression, EditorUserBuildSettings.activeBuildTarget);
	}

}
