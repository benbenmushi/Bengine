using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEditor;
using System.Reflection;
using System;
using UnityEngine.UI;
using UnityEditor.SceneManagement;


[CustomEditor(typeof(UISizeControllerGeneric))]
public class UISizeControllerGenericEditor : Editor
{
	UISizeControllerGeneric controller;

	void OnEnable()
	{
		controller = target as UISizeControllerGeneric;
	}
	public override void OnInspectorGUI()
	{
		serializedObject.Update();
		EditorGUI.BeginChangeCheck();
		EditorGUILayout.PropertyField(serializedObject.FindProperty("m_target"));

		if (controller.target != null)
		{
			EditorGUI.BeginChangeCheck();
			controller.searchDepth = EditorGUILayout.IntField(new GUIContent("Search Depth"), controller.searchDepth);
			if (EditorGUI.EndChangeCheck())
				controller.SetPossibleFieldsDirty();
			controller.fieldTargetIndex = EditorGUILayout.Popup(new GUIContent("TargetField"), controller.fieldTargetIndex, controller.possibleFields.Select(f => new GUIContent(f.Select(m => controller.GetMemberInfoName(m)).AggregateAuto("."))).ToArray());
			EditorGUILayout.PropertyField(serializedObject.FindProperty("SizeController"));
		}
		if (EditorGUI.EndChangeCheck())
		{
			EditorUtility.SetDirty(target);
			if (!Application.isPlaying)
				EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
		}
		serializedObject.ApplyModifiedProperties();
	}

}
