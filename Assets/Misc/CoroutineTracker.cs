using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections.Generic;

public class CoroutineTracker
#if UNITY_EDITOR
	: EditorWindow
#endif
{
#if UNITY_EDITOR
	class CoroutineData
	{
		public MonoBehaviour runner;
		public Coroutine trackingCoroutine;
		public Coroutine unityCoroutine;
		public string coroutineName;
	}

	private static List<CoroutineData> CoroutinesList = new List<CoroutineData>();

	[MenuItem("REPLICA/Developper/Coroutine tracker")]
	public static void ShowWindow()
	{
		CoroutineTracker window = EditorWindow.GetWindow<CoroutineTracker>();
		window.Show();
	}

	public static void StopAllCoroutines(MonoBehaviour runner)
	{
		runner.StopAllCoroutines();

		List<CoroutineData> updated = new List<CoroutineData>();
		foreach (CoroutineData data in CoroutinesList)
		{
			if (data.runner && data.runner != runner)
			{
				updated.Add(data);
			}
		}

		CoroutinesList = updated;
	}

	public static Coroutine StartCoroutine(string name, MonoBehaviour runner, IEnumerator routine, bool showDebug = false)
	{
		CoroutineData data = new CoroutineData();

		data.coroutineName = name;
		data.runner = runner;
		data.unityCoroutine = runner.StartCoroutine(routine);
		return data.trackingCoroutine = runner.StartCoroutine(TrackRoutineEnum(data, showDebug));
	}

	private static IEnumerator TrackRoutineEnum(CoroutineData data, bool showDebug = false)
	{
		if (data.unityCoroutine != null) // yield break without returns.
		{
			CoroutinesList.Add(data);
			if (showDebug)
				Debug.Log("AddCoroutine");
			yield return data.unityCoroutine;
			CoroutinesList.Remove(data);
			if (showDebug)
				Debug.Log("StopCoroutine");
		}
	}

	void OnGUI()
	{
		if (CoroutinesList != null)
		{
			List<CoroutineData> updated = new List<CoroutineData>();
			foreach (CoroutineData data in CoroutinesList)
			{
				if (data.runner)
				{
					EditorGUILayout.LabelField(data.coroutineName);
					updated.Add(data);
				}
			}

			CoroutinesList = updated;
		}
	}

	void Update()
	{
		this.Repaint();
	}

#else
	public static Coroutine StartCoroutine(string name, MonoBehaviour runner, IEnumerator routine, bool showDebug = false)
	{
		return runner.StartCoroutine(routine);
	}

	public static void StopAllCoroutines(MonoBehaviour runner)
	{
		runner.StopAllCoroutines();
	}
#endif
}
