using UnityEngine;
using UnityEditor;
using System.Reflection;

[CustomPropertyDrawer(typeof(ButtonFieldAttribute))]
public class ButtonFieldDrawer : PropertyDrawer
{
	public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
	{
		return EditorGUIUtility.singleLineHeight + 2;
	}

	ButtonFieldAttribute buttonAttribute { get { return attribute as ButtonFieldAttribute; } }

	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		if (GUI.Button(position, property.name.Replace("_", "")))
		{
			MethodInfo method = fieldInfo.DeclaringType.GetMethod(buttonAttribute.buttonCallbackName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);

			if (method != null)
			{
				if (!buttonAttribute.showConfirmPopup || EditorUtility.DisplayDialog(buttonAttribute.popupTitle, buttonAttribute.popupMessage, "Ok", "Cancel"))
				{
					if (method.IsStatic)
						method.Invoke(null, null);
					else
					{
						foreach (Object obj in property.serializedObject.targetObjects)
							method.Invoke(obj, null);
					}
				}
			}
			else
				Debug.LogError("ButtonField: Method \"" + buttonAttribute.buttonCallbackName + "\" not found");
		}
	}
}
