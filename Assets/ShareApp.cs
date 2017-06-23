using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.IO;

public class ShareApp : MonoBehaviour
{
	public Text dcmiPathText;

	string dcmiPath;

	public void GetThumbnails()
	{
		if (string.IsNullOrEmpty(dcmiPath))
			GetDCMIPath();
		AndroidJavaClass unity = new AndroidJavaClass ("com.unity3d.player.UnityPlayer");
		AndroidJavaObject currentActivity = unity.GetStatic<AndroidJavaObject> ("currentActivity");

		string bytes = currentActivity.Call<string>("GetImage", Directory.GetFiles(dcmiPath)[0]);

		Debug.Log("bytes= " + bytes);
	}

	public void GetDCMIPath()
	{
		AndroidJavaClass unity = new AndroidJavaClass ("com.unity3d.player.UnityPlayer");
		AndroidJavaObject currentActivity = unity.GetStatic<AndroidJavaObject> ("currentActivity");

		dcmiPath = currentActivity.CallStatic<string>("GetDCMIPath");
		dcmiPathText.text = "DCMI=" + dcmiPath;
	}

}
