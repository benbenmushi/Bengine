using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System;

public class SerialNumberLock : MonoBehaviour
{
    public static string getExternalStorageDirectory
    {
        get
        {
#if UNITY_ANDROID && !UNITY_EDITOR
			return new AndroidJavaClass("android.os.SystemProperties")
						.CallStatic<AndroidJavaObject>("getExternalStorageDirectory")
						.Call<string>("getPath")
						+ "/" + (new AndroidJavaClass("android.os.Environment").GetStatic<string>("DIRECTORY_DCIM"));
#else
            return Application.persistentDataPath;
#endif
        }
    }

    public static string serialNumber
    {
        get
        {
            try
            {
#if UNITY_ANDROID && !UNITY_EDITOR
            return new AndroidJavaClass("android.os.Build")
                                    .GetStatic<string>("SERIAL");
#endif
            }
            catch (Exception e)
            {
            }
            return "";
        }
    }
    public static string serialNumber2
    {
        get
        {
            try
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                return new AndroidJavaClass("android.os.SystemProperties")
                                            .CallStatic<string>("get", new object[] { "sys.serialnumber", "error" });
#endif
            }
            catch (Exception e)
            {
            }
            return "";
        }
    }
    public static string serialNumber3
    {
        get
        {
            try
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                return new AndroidJavaClass("android.os.SystemProperties")
                                        .CallStatic<string>("get", new object[] { "ril.serialnumber", "error" });
#endif
            }
            catch (Exception e)
            {
            }
            return "";
        }
    }


    IEnumerator Start()
    {
        _serialNumber = serialNumber;
        _serialNumber2 = serialNumber2;
        _serialNumber3 = serialNumber3;

#if UNITY_EDITOR
        WWW serialNUmberList = new WWW("file://" + Application.streamingAssetsPath + "/cfg");
#else
        WWW serialNUmberList = new WWW("jar:file://" + Application.dataPath + "!/assets/cfg");
#endif

        yield return serialNUmberList;
        readSerialNumber = serialNUmberList.text.Split('\n');
    }

    string[] readSerialNumber;
    private string _serialNumber;
    private string _serialNumber2;
    private string _serialNumber3;

    void OnGUI()
    {
        for (int i = 0; i < readSerialNumber.Length; i++)
        {
            GUILayout.Label("ReadSerial=" + readSerialNumber[i]);
            if (readSerialNumber[i].ToLower().Equals(serialNumber.ToLower()))
                GUILayout.Label("FOUND !");
        }
        GUILayout.Label("serialNumber=" + _serialNumber);
        GUILayout.Label("serialNumber2=" + _serialNumber2);
        GUILayout.Label("serialNumber3=" + _serialNumber3);
    }
}
