using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.IO;
using System.Linq;
using VacuumShaders.TextureExtensions;
using System;

public class ShareApp : MonoBehaviour
{
	public minibutton firstButton;
	public MeshRenderer mesh;

	public static string getExternalStorageDirectory
	{
		get
		{
#if UNITY_ANDROID && !UNITY_EDITOR
			return new AndroidJavaClass("android.os.Environment")
						.CallStatic<AndroidJavaObject>("getExternalStorageDirectory")
						.Call<string>("getPath")
						+ "/" + (new AndroidJavaClass("android.os.Environment").GetStatic<string>("DIRECTORY_DCIM"));
#else
			return Application.persistentDataPath;
#endif
		}
	}

	string[] files;

	void Start()
	{
		Debug.Log(getExternalStorageDirectory);
		files = Directory.GetFiles(getExternalStorageDirectory, "*", SearchOption.AllDirectories).Where(p => pathIsValid(p.ToLower())).Take(20).ToArray();

		//firstButton.buttonText.text = files.Length + " files.";

		//for (int i = 0; i < files.Length; i++)
		//{
		//	minibutton newButton = GameObject.Instantiate(firstButton.gameObject).GetComponent<minibutton>();

		//	newButton.transform.SetParent(firstButton.transform.parent, false);
		//	newButton.buttonText.text = files[i];
		//	newButton.onClick += () =>
		//	{
		//		Texture2D tex = new Texture2D(2, 2);
		//		tex.LoadImage(File.ReadAllBytes(newButton.buttonText.text));
		//		tex.ResizePro(64, 64);
		//		mesh.material.mainTexture = tex;
		//	};
		//}
	}

	void OnGUI()
	{
		for (int i = 0; i < files.Length; i++)
		{
			if (GUILayout.Button(Path.GetFileNameWithoutExtension(files[i])))
			{
				StartCoroutine(LoadFile(files[i], (www) =>
				{
					mesh.material.mainTexture = www.texture;
				}));
				//Texture2D tex = new Texture2D(2, 2);
				//tex.LoadImage(File.ReadAllBytes(files[i]));
				//tex.ResizePro(64, 64);
			}
		}
	}
	IEnumerator LoadFile(string filePath,  Action<WWW> callback)
	{
		if (callback == null) throw new ArgumentNullException("callback");
		if (string.IsNullOrEmpty(filePath)) throw new ArgumentNullException("filePath");

		WWW loadRequest = new WWW("file://" + filePath.Replace("\\", "/"));

		while (!loadRequest.isDone)
			yield return null;
		callback(loadRequest);
	}

	bool pathIsValid(string path)
	{
		return //!path.Contains("/.") &&
				path.EndsWith(".jpg") ||
				path.EndsWith(".png");
	}

}
