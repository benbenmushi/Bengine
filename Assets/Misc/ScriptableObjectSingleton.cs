using UnityEngine;
using System.Collections;
using System;

public class ScriptableObjectSingleton<T> : ScriptableObject where T : ScriptableObject
{
	#region Singleton
	static T m_instance;
	public static T instance
	{
		get
		{
#if UNITY_EDITOR
			if (m_instance == null)
				FindSingletonInProject();
#endif
			return m_instance;
		}
	}
	public static bool Exist
	{
		get
		{
			return m_instance != null;
		}
	}
	public ScriptableObjectSingleton()
	{
		if (m_instance == null)
		{
			m_instance = this as T;
		}
#if UNITY_EDITOR
		else
		{
			SelectEditorInstance(true);
		}
#endif
	}
	#endregion

#if UNITY_EDITOR
	protected static T CreateAndSelectSingleton()
	{
		FindSingletonInProject();

		if (m_instance != null)
			SelectEditorInstance(true);
		else
			m_instance = EditorUtils.CreateAsset<T>(typeof(T).Name);
		return m_instance;
	}

	public static void SelectEditorInstance(bool showError = false)
	{
		if (showError)
			Debug.LogWarning("One " + typeof(T).Name + " is already present in your project. You cant create another one.");
		UnityEditor.Selection.activeObject = m_instance as UnityEngine.Object;
	}
	static string assetPath;
	static void FindSingletonInProject()
	{
		if (m_instance == null)
		{
			string[] singletonPathArray = EditorUtils.GetAssetsPathByType(typeof(T));

			if (singletonPathArray.Length > 1)
			{
				if (m_instance != null)
				{
					for (int i = 0; i < singletonPathArray.Length; i++)
					{
						T m = UnityEditor.AssetDatabase.LoadAssetAtPath(singletonPathArray[i], typeof(T)) as T;

						if (m != m_instance)
							UnityEditor.AssetDatabase.DeleteAsset(singletonPathArray[i]);
					}
				}
				else
				{
					for (int i = 1; i < singletonPathArray.Length; i++)
					{
						T m = UnityEditor.AssetDatabase.LoadAssetAtPath(singletonPathArray[i], typeof(T)) as T;

						if (i != 0)
							UnityEditor.AssetDatabase.DeleteAsset(singletonPathArray[i]);
						else
						{
							assetPath = singletonPathArray[i];
							m_instance = m;
						}
					}
				}
			}
			else if (singletonPathArray.Length == 1)
			{
				assetPath = singletonPathArray[0];
				m_instance = UnityEditor.AssetDatabase.LoadAssetAtPath(assetPath, typeof(T)) as T;
			}
		}
	}

#endif
}
