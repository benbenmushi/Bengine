using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.IO;

[System.Serializable]
public struct SceneAssetEx
{
	public string scenePath;

	public string sceneName
	{
		get { return Path.GetFileNameWithoutExtension(scenePath); }
	}

	public void LoadScene(LoadSceneMode loadMode = LoadSceneMode.Single)
	{
		SceneManager.LoadScene(sceneName, loadMode);
	}
	public AsyncOperation LoadSceneAsync(LoadSceneMode loadMode = LoadSceneMode.Single)
	{
		return SceneManager.LoadSceneAsync(sceneName, loadMode);
	}
	public void UnloadScene()
	{
		SceneManager.UnloadScene(sceneName);
	}

	public override string ToString()
	{
		return sceneName;
	}
}
