using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Reflection;
using System.Linq;

[CustomEditor(typeof(UIRectTransformSizeController)), CanEditMultipleObjects]
public class UIRectTransformSizeControllerEditor : Editor
{

	private GUIStyle m_headerStyle;
	private GUIStyle headerStyle
	{
		get
		{
			if (m_headerStyle == null)
			{
				m_headerStyle = new GUIStyle(GUI.skin.label);
				m_headerStyle.fontSize = 16;
				m_headerStyle.fontStyle = FontStyle.Bold;
			}
			return m_headerStyle;
		}
	}
#pragma warning disable 0649
	SerializedObject    objS;

	SerializedProperty  referenceSizeAjust;
	SerializedProperty  runtimeRefreshing;
	SerializedProperty  refreshRate;

	// WIDTH
	SerializedProperty  applyOnWidth;
	SerializedProperty  customWidthReference;
	SerializedProperty  customWidthReferenceFitMode;
	SerializedProperty  widthSize;
	SerializedProperty  widthPositionOffset;
	SerializedProperty  widthBasedOnWidth;
	SerializedProperty  substractWidth;
	SerializedProperty  parentWidthIsMax;
	SerializedProperty  lastRefWidthSize;

	// HEIGHT
	SerializedProperty  applyOnHeigth;
	SerializedProperty  customHeightReference;
	SerializedProperty  customHeightReferenceFitMode;
	SerializedProperty  heightSize;
	SerializedProperty  heightPositionOffset;
	SerializedProperty  heightBasedOnWidth;
	SerializedProperty  substractHeight;
	SerializedProperty  parentHeightIsMax;
	SerializedProperty  lastRefHeightSize;

	SerializedProperty  ForceRefresh;
#pragma warning restore 0649

	static bool foldoutWidthSettings = false;
	static bool foldoutHeightSettings = false;

	void OnEnable()
	{
		objS = new SerializedObject(targets);

		// Get every "SerializedProperty" field in this class
		FieldInfo[] serializedPropertyFieldInfoTab = GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance)
															  .Where(f => f.FieldType == typeof(SerializedProperty)).ToArray();

		// And set his value with the property of our targer Serialized Object
		for (int i = 0; i < serializedPropertyFieldInfoTab.Length; i++)
			serializedPropertyFieldInfoTab[i].SetValue(this, objS.FindProperty(serializedPropertyFieldInfoTab[i].Name));
	}

	string GetWidthTitle()
	{
		return "Width " + (!lastRefWidthSize.hasMultipleDifferentValues ? "" + Mathf.Max(0, Mathf.RoundToInt(lastRefWidthSize.floatValue)) : "") + (applyOnWidth.boolValue ? " (Applied)" : "");
	}
	string GetHeightTitle()
	{
		return "Height " + (!lastRefHeightSize.hasMultipleDifferentValues ? "" + Mathf.Max(0, Mathf.RoundToInt(lastRefHeightSize.floatValue)) : "") + (applyOnHeigth.boolValue ? " (Applied)" : "");
	}

	public override void OnInspectorGUI()
	{
		objS.Update();


		EditorGUILayout.PropertyField(referenceSizeAjust);
		EditorGUILayout.PropertyField(runtimeRefreshing);
		if (runtimeRefreshing.boolValue)
			EditorGUILayout.PropertyField(refreshRate);

		EditorGUILayout.Space();
		EditorGUILayout.BeginVertical(GUI.skin.box);
		foldoutWidthSettings = EditorUtils.SexyFoldout(foldoutWidthSettings, GetWidthTitle());
		if (foldoutWidthSettings)
		{
			EditorGUILayout.PropertyField(applyOnWidth);
			if (applyOnWidth.boolValue || applyOnWidth.hasMultipleDifferentValues)
			{
				EditorGUILayout.PropertyField(customWidthReference);
				if (customWidthReference.objectReferenceValue != null || customWidthReference.hasMultipleDifferentValues)
					EditorGUILayout.PropertyField(customWidthReferenceFitMode);
				EditorGUILayout.PropertyField(widthSize);
				EditorGUILayout.PropertyField(widthPositionOffset);
				EditorGUILayout.PropertyField(widthBasedOnWidth);
				EditorGUILayout.PropertyField(substractWidth);
				EditorGUILayout.PropertyField(parentWidthIsMax);
			}
			EditorGUILayout.Space();
		}
		EditorGUILayout.EndVertical();
		EditorGUILayout.BeginVertical(GUI.skin.box);
		foldoutHeightSettings = EditorUtils.SexyFoldout(foldoutHeightSettings, GetHeightTitle());
		if (foldoutHeightSettings)
		{
			EditorGUILayout.PropertyField(applyOnHeigth);
			if (applyOnHeigth.boolValue || applyOnHeigth.hasMultipleDifferentValues)
			{
				EditorGUILayout.PropertyField(customHeightReference);
				if (customHeightReference.objectReferenceValue != null || customHeightReference.hasMultipleDifferentValues)
					EditorGUILayout.PropertyField(customHeightReferenceFitMode);
				EditorGUILayout.PropertyField(heightSize);
				EditorGUILayout.PropertyField(heightPositionOffset);
				EditorGUILayout.PropertyField(heightBasedOnWidth);
				EditorGUILayout.PropertyField(substractHeight);
				EditorGUILayout.PropertyField(parentHeightIsMax);
			}
		}
		EditorGUILayout.EndVertical();
		EditorGUILayout.PropertyField(ForceRefresh);
		//if (GUILayout.Button("Refresh"))
		//{
		//	foreach (var o in targets)
		//		((UIRectTransformSizeController)o).Refresh();
		//}
		objS.ApplyModifiedProperties();
	}

}
