using UnityEngine;
using System.Collections;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections.Generic;

public class HooksTracker
#if UNITY_EDITOR
	: EditorWindow
#endif
{
#if UNITY_EDITOR


	class HookableEvent
	{
		public List<System.Reflection.MethodInfo> hooks;
		public bool folded;

		public HookableEvent()
		{
			hooks = new List<System.Reflection.MethodInfo>();
		}
	}

	class HookableClass
	{
		public Dictionary<string, HookableEvent> events;
		public bool folded;

		public HookableClass()
		{
			events = new Dictionary<string, HookableEvent>();
		}
	}

	[MenuItem("REPLICA/Developper/Hooks tracker")]
	public static void ShowWindow()
	{
		HooksTracker window = EditorWindow.GetWindow<HooksTracker>();
		window.Show();
	}

	private static Dictionary<string, HookableClass> TrackedHooks;

	public static void StartTracking(string c, string m, System.Reflection.MethodInfo fct)
	{
		if (TrackedHooks == null)
			TrackedHooks = new Dictionary<string, HookableClass>();

		if (!TrackedHooks.ContainsKey(c))
			TrackedHooks.Add(c, new HookableClass());

		if (!TrackedHooks[c].events.ContainsKey(m))
			TrackedHooks[c].events.Add(m, new HookableEvent());

		TrackedHooks[c].events[m].hooks.Add(fct);
	}

	public static void StopTracking(string c, string m, System.Reflection.MethodInfo fct)
	{
		if (TrackedHooks != null)
		{
			if (TrackedHooks.ContainsKey(c))
			{
				if (TrackedHooks[c].events.ContainsKey(m))
				{
					TrackedHooks[c].events[m].hooks.Remove(fct);

					if (TrackedHooks[c].events[m].hooks.Count == 0)
						TrackedHooks[c].events.Remove(m);
				}

				if (TrackedHooks[c].events.Count == 0)
					TrackedHooks.Remove(c);
			}

			if (TrackedHooks.Count == 0)
				TrackedHooks = null;
		}
	}

	void OnGUI()
	{
		trackedEventUtility.logInvokeEventCalls = EditorGUILayout.Toggle("logInvokeEventCalls", trackedEventUtility.logInvokeEventCalls);
		trackedEventUtility.logRegistrationErrors = EditorGUILayout.Toggle("logRegistrationErrors", trackedEventUtility.logRegistrationErrors);
		if (TrackedHooks != null)
		{
			foreach (KeyValuePair<string, HookableClass> hClass in TrackedHooks)
			{
				hClass.Value.folded = EditorGUILayout.Foldout(hClass.Value.folded, hClass.Key + " (" + hClass.Value.events.Count + ")");
				if (hClass.Value.folded)
				{
					EditorGUI.indentLevel++;

					foreach (KeyValuePair<string, HookableEvent> hEvent in hClass.Value.events)
					{
						hEvent.Value.folded = EditorGUILayout.Foldout(hEvent.Value.folded, hEvent.Key + " (" + hEvent.Value.hooks.Count + ")");
						if (hEvent.Value.folded)
						{
							EditorGUI.indentLevel++;

							foreach (System.Reflection.MethodInfo hooked in hEvent.Value.hooks)
							{
								EditorGUILayout.LabelField(hooked.DeclaringType.Name + "." + hooked.Name);
							}
							EditorGUI.indentLevel--;
						}
					}
					EditorGUI.indentLevel--;
				}
			}
		}
	}

	void Update()
	{
		this.Repaint();
	}

#else
	public static void StartTracking(string c, string m, System.Reflection.MethodInfo fct)
	{

	}

	public static void StopTracking(string c, string m, System.Reflection.MethodInfo fct)
	{

	}
#endif
}
