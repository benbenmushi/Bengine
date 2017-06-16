using UnityEngine;
using UnityEditor;
using System.Reflection;

[CustomPropertyDrawer(typeof(StaticFieldAttritube))]
public class StaticFieldDrawer : PropertyDrawer
{
	public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
	{
		return EditorGUIUtility.singleLineHeight;
	}

	StaticFieldAttritube myAttribute { get { return attribute as StaticFieldAttritube; } }

	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		Object targetObject = property.serializedObject.targetObject;
		System.Type componentType = targetObject.GetType();
		FieldInfo staticFieldInfo = componentType.GetField(myAttribute.staticFieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
		FieldInfo nonstaticFieldInfo = componentType.GetField(property.name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

		if (staticFieldInfo != null && nonstaticFieldInfo != null &&
			staticFieldInfo.FieldType == nonstaticFieldInfo.FieldType)
		{
			// before drawing property set his value to the static
			nonstaticFieldInfo.SetValue(targetObject, staticFieldInfo.GetValue(null));

			EditorGUI.BeginChangeCheck();
			EditorGUI.PropertyField(position, property);
			if (EditorGUI.EndChangeCheck())
			{
				// On Value changed, then set the static value
				staticFieldInfo.SetValue(null, nonstaticFieldInfo.GetValue(targetObject));
			}
		}
		else
			EditorGUI.PropertyField(position, property);
	}
}
