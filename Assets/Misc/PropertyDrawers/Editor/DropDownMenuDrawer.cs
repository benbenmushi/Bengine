using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Reflection;
using System.Linq;

[CustomPropertyDrawer(typeof(DropDownMenuAttribute))]
public class DropDownMenuDrawer : PropertyDrawer
{
	public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
	{
		return EditorGUIUtility.singleLineHeight;
	}

	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		FieldInfo arrayFieldInfo = fieldInfo.DeclaringType.GetField((attribute as DropDownMenuAttribute).arrayFieldName);

		if (arrayFieldInfo != null && arrayFieldInfo.FieldType.IsArray)
		{
			position.height = EditorGUIUtility.singleLineHeight;
			property.intValue = EditorGUI.Popup(position, label, property.intValue, (arrayFieldInfo.GetValue(property.serializedObject.targetObject) as object[]).DefaultIfEmpty().Select(e =>
				{
					if (e != null)
						return new GUIContent(e.ToString());
					return new GUIContent("null");
				}).ToArray());
		}
		else
			GUI.Label(position, new GUIContent((attribute as DropDownMenuAttribute).arrayFieldName + " array not found."));
	}
}
