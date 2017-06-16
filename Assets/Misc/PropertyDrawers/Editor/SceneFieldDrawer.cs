using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine.SceneManagement;

[CustomPropertyDrawer(typeof(SceneAssetEx))]
public class SceneFieldDrawer : PropertyDrawer
{
	bool missingInBuildSettings = false;

	SceneAsset GetSceneAssetFromPath(string scenePath)
	{
		return AssetDatabase.LoadAssetAtPath(scenePath, typeof(SceneAsset)) as SceneAsset;
	}
	string GetSceneAssetPathFromProperty(SerializedProperty property)
	{
		if (!string.IsNullOrEmpty(property.stringValue))
		{
			string[] sceneFound = AssetDatabase.FindAssets("t:scene " + property.stringValue).Select(guid => AssetDatabase.GUIDToAssetPath(guid)).ToArray();

			if (sceneFound.Length > 0)
				return sceneFound.FirstOrDefault(s => s.EndsWith(property.stringValue  + ".unity"));
		}
		return "";
	}

	public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
	{
		string scenePath = GetSceneAssetPathFromProperty(property.FindPropertyRelative("scenePath"));

		if (!string.IsNullOrEmpty(scenePath))
		{
			if (EditorBuildSettings.scenes.FirstOrDefault(s => s.path == scenePath) == null)
			{
				missingInBuildSettings = true;
				return base.GetPropertyHeight(property, label) + 30;
			}
		}
		missingInBuildSettings = false;
		return base.GetPropertyHeight(property, label);
	}
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		property.serializedObject.Update();
		string scenePath = property.FindPropertyRelative("scenePath").stringValue;
		SceneAsset sceneObject = GetSceneAssetFromPath(scenePath);

		if (sceneObject == null && !string.IsNullOrEmpty(scenePath))
			Debug.LogError("[SceneField] Scene " + scenePath + " was not found in your project. Reference has been lost.");
		sceneObject = EditorGUI.ObjectField(EditorGUI.PrefixLabel(position.SetHeight(17), label), sceneObject, typeof(SceneAsset), false) as SceneAsset;
		if (sceneObject != null)
			property.FindPropertyRelative("scenePath").stringValue = AssetDatabase.GetAssetPath(sceneObject);
		else
			property.FindPropertyRelative("scenePath").stringValue = "";

		property.serializedObject.ApplyModifiedProperties();
		if (missingInBuildSettings)
		{
			Color guiColor = GUI.color;
			GUI.color = Color.yellow;
			position = position.SetHeight(30).SetY(position.y + 18);
			GUI.Box(position, "");
			EditorGUI.LabelField(position.SetWidth(40), "", EditorUtils.warnEntry);
			position.y += 5;
			position.height -= 10;
			position.x += 40;
			position.width -= 45;
			GUI.color = guiColor;
			if (GUI.Button(position, "Add to BuildSettings"))
				EditorUtils.AddSceneToBuildSettings(scenePath);
		}
	}
}