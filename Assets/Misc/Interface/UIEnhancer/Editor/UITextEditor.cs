using UnityEngine.UI;
using UnityEditor;
using System.Reflection;
using System.Linq;
using UnityEditor.UI;
using UnityEngine;

[CustomEditor(typeof(UIText), true)]
[CanEditMultipleObjects]
public class TextEditor : GraphicEditor
{
#pragma warning disable 0649
	SerializedProperty m_Text;
	SerializedProperty m_FontData;
	SerializedProperty m_textChangedEvent;

	#region SizeController
	SerializedProperty      enableSizeController;
	SerializedProperty      customReference;
	SerializedProperty      refreshRate;
	SerializedProperty      runtimeRefreshing;

	SerializedProperty      sizeType;
	SerializedProperty      size;
	SerializedProperty      fixedSize;
	SerializedProperty      sizeBasedOn;
	SerializedProperty      bestFit;
	SerializedProperty      minSizeType;
	SerializedProperty      minSize;
	SerializedProperty      fixedMinSize;
	SerializedProperty      minSizeBasedOn;


	SerializedProperty      lastRefSize;
	SerializedProperty      lastRefMinSize;

	SerializedProperty      RefreshSize;


	SerializedProperty          adaptTransformHeight;
	SerializedProperty          adaptOnUpdate;
	SerializedProperty          adaptMinHeight;
	SerializedProperty          adaptOffset;
	SerializedProperty          __RefreshHeight;
	#endregion
#pragma warning restore 0649

	static bool foldoutAdvancedSettings = false;

	protected override void OnEnable()
	{
		base.OnEnable();
		m_Text = serializedObject.FindProperty("m_Text");
		m_FontData = serializedObject.FindProperty("m_FontData");
		m_textChangedEvent = serializedObject.FindProperty("m_textChangedEvent");

		FieldInfo[] serializedPropertyFieldInfoTab = GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance)
															  .Where(f => f.FieldType == typeof(SerializedProperty)).ToArray();

		// And set his value with the property of our target Serialized Object
		for (int i = 0; i < serializedPropertyFieldInfoTab.Length; i++)
			serializedPropertyFieldInfoTab[i].SetValue(this, serializedObject.FindProperty(serializedPropertyFieldInfoTab[i].Name));
	}

	public override void OnInspectorGUI()
	{
		serializedObject.Update();

		EditorGUILayout.PropertyField(m_Text);
		EditorGUILayout.PropertyField(m_FontData);
		AppearanceControlsGUI();
		RaycastControlsGUI();
		EditorGUILayout.BeginVertical(GUI.skin.box);
		foldoutAdvancedSettings = EditorUtils.SexyFoldout(foldoutAdvancedSettings, new GUIContent("Advanced Configuration"));
		if (foldoutAdvancedSettings)
		{
			EditorGUILayout.PropertyField(enableSizeController, new GUIContent("Size Controller"));
			if (enableSizeController.boolValue)
			{
				EditorGUILayout.PropertyField(customReference);
				EditorGUILayout.PropertyField(runtimeRefreshing);
				if (runtimeRefreshing.boolValue)
					EditorGUILayout.PropertyField(refreshRate);

				EditorGUILayout.Space();

				EditorGUILayout.PropertyField(sizeType);
				if (!sizeType.hasMultipleDifferentValues)
				{
					if (sizeType.enumValueIndex == (int)UIText.TextSizeType.Relative)
					{
						EditorGUILayout.PropertyField(size, new GUIContent("Size (=" + GetSizeString() + ")"));
						EditorGUILayout.PropertyField(sizeBasedOn);
					}
					else
						EditorGUILayout.PropertyField(fixedSize);
				}
				EditorGUILayout.Space();
				EditorGUILayout.PropertyField(bestFit);
				if (bestFit.boolValue && !bestFit.hasMultipleDifferentValues)
				{
					EditorGUILayout.PropertyField(minSizeType);
					if (!minSizeType.hasMultipleDifferentValues)
					{
						if (minSizeType.enumValueIndex == (int)UIText.TextSizeType.Relative)
						{
							EditorGUILayout.PropertyField(minSize, new GUIContent("Min Size (=" + GetMinSizeString() + ")"));
							EditorGUILayout.PropertyField(minSizeBasedOn);
						}
						else
							EditorGUILayout.PropertyField(fixedMinSize);
					}
				}
				EditorGUILayout.PropertyField(RefreshSize);
				EditorGUILayout.Space();
			}
			EditorGUILayout.PropertyField(adaptTransformHeight, new GUIContent("Adapt Height"));
			if (adaptTransformHeight.boolValue)
			{
				EditorGUILayout.PropertyField(adaptMinHeight);
				EditorGUILayout.PropertyField(adaptOffset);
				EditorGUILayout.PropertyField(adaptOnUpdate);
				EditorGUILayout.Space();
			}
			EditorGUILayout.PropertyField(m_textChangedEvent);
		}
		EditorGUILayout.EndVertical();
		serializedObject.ApplyModifiedProperties();
	}
	string GetSizeString()
	{
		if (!sizeType.hasMultipleDifferentValues)
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
