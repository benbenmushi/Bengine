using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomPropertyDrawer(typeof(UISizeController))]
public class UISizeControllerDrawer : PropertyDrawer
{

	public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
	{
		float height = 20f;

		if (property.isExpanded)
		{
			height += 18f;
			if (property.FindPropertyRelative("apply").boolValue)
			{
				height += 36f;
				if (property.FindPropertyRelative("sizeType").enumValueIndex == (int)UISizeController.SizeType.Relative)
				{
					height += 54f;
					if (property.FindPropertyRelative("sizeReference.customReference").objectReferenceValue != null)
						height += 18f;
				}
				else
					height += 18f;
			}
		}
		return height;
	}
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		GUI.Box(position, "");
		GUIContent foldoutContent;

		if (property.FindPropertyRelative("apply").boolValue)
			foldoutContent = new GUIContent(property.name + " (=" + property.FindPropertyRelative("size").floatValue + ")");
		else
			foldoutContent = new GUIContent(property.name + " (inactive)");

		property.isExpanded = EditorUtils.SexyFoldout(position.SetHeight(20f), property.isExpanded, foldoutContent);

		if (property.isExpanded)
		{
			position.width -= 6;
			position.x += 2;
			position.height = 16f;
			position.y += 18f;
			EditorGUI.PropertyField(position, property.FindPropertyRelative("apply"));
			if (property.FindPropertyRelative("apply").boolValue)
			{
				position.y += 18f;
				EditorGUI.PropertyField(position, property.FindPropertyRelative("sizeType"));
				if (property.FindPropertyRelative("sizeType").enumValueIndex == (int)UISizeController.SizeType.Relative)
				{
					position.y += 18f;
					EditorGUI.PropertyField(position, property.FindPropertyRelative("sizeReference.customReference"));
					if (property.FindPropertyRelative("sizeReference.customReference").objectReferenceValue != null)
					{
						position.y += 18f;
						EditorGUI.PropertyField(position, property.FindPropertyRelative("sizeReference.customReferenceFitMode"));
					}
					position.y += 18f;
					EditorGUI.PropertyField(position, property.FindPropertyRelative("relativeSizePercent"));
					position.y += 18f;
					EditorGUI.PropertyField(position, property.FindPropertyRelative("sizeReference.basedOn"));
				}
				else
				{
					position.y += 18f;
					EditorGUI.PropertyField(position, property.FindPropertyRelative("fixedSize"));
				}
				position.y += 18f;
				EditorGUI.PropertyField(position, property.FindPropertyRelative("invert"));
			}
		}
	}
}
