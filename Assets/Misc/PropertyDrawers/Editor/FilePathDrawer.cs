using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Reflection;
using System;
using System.IO;

[CustomPropertyDrawer(typeof(FilePathAttribute))]
public class FilePathDrawer : PropertyDrawer
{
	public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
	{
		return EditorGUIUtility.singleLineHeight;
	}
	FilePathAttribute fileAttribute { get { return attribute as FilePathAttribute; } }

	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		if (property.propertyType == SerializedPropertyType.String)
		{
			string directoryName = string.IsNullOrEmpty(property.stringValue) ? "" : Path.GetDirectoryName(property.stringValue);

			Rect buttonRect = EditorGUI.PrefixLabel(position, new GUIContent(property.displayName));
			buttonRect.width -= 37;
			EditorGUI.LabelField(buttonRect, Path.GetFileName(property.stringValue), GUI.skin.GetStyle("MiniPopup"));
			if (GUI.Button(buttonRect, "", GUIStyle.none))
			{
				string newValue = EditorUtility.OpenFilePanel("Select your File", directoryName, fileAttribute.fileExtention);

				if (!string.IsNullOrEmpty(newValue) && property.stringValue != newValue)
				{
					if (fileAttribute.fileChangedCallback != null)
					{
						MethodInfo method = fieldInfo.DeclaringType.GetMethod(fileAttribute.fileChangedCallback, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);

						if (method != null)
						{
							if (method.IsStatic)
								method.Invoke(null, null);
							else
							{
								foreach (UnityEngine.Object obj in property.serializedObject.targetObjects)
									method.Invoke(obj, new object[] { newValue });
							}
						}
						else
							Debug.LogError("[FilePath]: Method \"" + fileAttribute.fileChangedCallback + "\" not found");
					}
					property.stringValue = newValue;
				}
			}
			buttonRect.x += buttonRect.width;
			buttonRect.width = 37;
			if (GUI.Button(buttonRect, "Open", GUI.skin.GetStyle("minibutton")))
			{
				Application.OpenURL("file://" + property.stringValue);
			}
		}
		else
			Debug.LogError("[FilePath]: PropertyAttributeTargetError: must be a string");
	}
}
