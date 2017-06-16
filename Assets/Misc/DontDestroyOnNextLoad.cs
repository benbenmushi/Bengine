using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class DontDestroyOnNextLoad : MonoBehaviour
{
	private static GameObject perservingGO;

	public static void PreserveGameObject(GameObject go)
	{
		if (perservingGO == null)
		{
			perservingGO = new GameObject("DontDestroyOnLoad_Holder");
			DontDestroyOnLoad(perservingGO);
			perservingGO.AddComponent<DontDestroyOnNextLoad>();
		}
		go.transform.SetParent(perservingGO.transform, true);
	}

	void OnLevelWasLoaded()
	{
		while (transform.childCount > 0)
			transform.GetChild(0).SetParent(null, true);
		Destroy(gameObject);
	}
}
