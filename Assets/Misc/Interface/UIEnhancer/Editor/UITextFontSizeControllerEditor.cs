using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Reflection;
using System.Linq;

[CustomEditor(typeof(UITextFontSizeController)), CanEditMultipleObjects]
public class UITextFontSizeControllerEditor : Editor
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

	SerializedObject        objS;

	SerializedProperty      customReference;
	SerializedProperty      refreshRate;
	SerializedProperty      runtimeRefreshing;

	SerializedProperty      sizeIsFixedValue;
	SerializedProperty      size;
	SerializedProperty      fixedSize;
	SerializedProperty      baseOnWidth;
	SerializedProperty      bestFit;
	SerializedProperty      minSizeIsFixedValue;
	SerializedProperty      minSizeBasedOnWidth;
	SerializedProperty      minSize;
	SerializedProperty      fixedMinSize;


	SerializedProperty      lastRefSize;
	SerializedProperty      lastRefMinSize;

	SerializedProperty      RefreshSize;
#pragma warning restore 0649

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

	public override void OnInspectorGUI()
	{
		objS.Update();

		EditorGUILayout.PropertyField(customReference);
		EditorGUILayout.PropertyField(runtimeRefreshing);
		if (runtimeRefreshing.boolValue)
			EditorGUILayout.PropertyField(refreshRate);

		EditorGUILayout.Space();

		EditorGUILayout.LabelField("Size " + GetSizeString(), GUI.skin.GetStyle("HeaderLabel"));
		EditorGUILayout.PropertyField(sizeIsFixedValue);
		if (!sizeIsFixedValue.hasMultipleDifferentValues)
		{
			if (!sizeIsFixedValue.boolValue)
			{
				EditorGUILayout.PropertyField(size);
				EditorGUILayout.PropertyField(baseOnWidth);
			}
			else
				EditorGUILayout.PropertyField(fixedSize);
		}
		EditorGUILayout.Space();
		EditorGUILayout.PropertyField(bestFit);
		if (bestFit.boolValue && !bestFit.hasMultipleDifferentValues)
		{
			EditorGUILayout.LabelField("Min Size " + GetMinSizeString(), GUI.skin.GetStyle("HeaderLabel"));
			EditorGUILayout.PropertyField(minSizeIsFixedValue);
			if (!minSizeIsFixedValue.hasMultipleDifferentValues)
			{
				if (!minSizeIsFixedValue.boolValue)
				{
					EditorGUILayout.PropertyField(minSize);
					EditorGUILayout.PropertyField(minSizeBasedOnWidth);
				}
				else
					EditorGUILayout.PropertyField(fixedMinSize);
			}
		}

		EditorGUILayout.PropertyField(RefreshSize);

		objS.ApplyModifiedProperties();
	}

	string GetSizeString()
	{
		if (!fixedSize.hasMultipleDifferentValues)
			return "" + fixedSize.intValue;
		return "";
	}
	string GetMinSizeString()
	{
		if (!fixedMinSize.hasMultipleDifferentValues)
			return "" + fixedMinSize.intValue;
		return "";
	}

}
