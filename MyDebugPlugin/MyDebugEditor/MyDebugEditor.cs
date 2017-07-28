using UnityEditor;
using UnityEngine;

namespace MyDebugAPI
{
	[CustomPropertyDrawer(typeof(MyDebug))]
	public class MyDebugDrawer : PropertyDrawer
	{

		bool foldout
		{
			get { return EditorPrefs.GetBool("MyDebugDrawerFoldout"); }
			set { EditorPrefs.SetBool("MyDebugDrawerFoldout", value); }
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			if (foldout)
			{
				if (property.FindPropertyRelative("colored").boolValue)
					return EditorGUIUtility.singleLineHeight * 5;
				else
					return EditorGUIUtility.singleLineHeight * 4;
			}
			else
				return EditorGUIUtility.singleLineHeight;
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			position.height = EditorGUIUtility.singleLineHeight;
			foldout = EditorGUI.Foldout(position, foldout, label);
			if (foldout)
			{
				position.x += 9;
				position.width -= 9;
				position.y += position.height;
				EditorGUI.PropertyField(position, property.FindPropertyRelative("LogLevel"));
				position.y += position.height;
				EditorGUI.PropertyField(position, property.FindPropertyRelative("m_prefix"));
				position.y += position.height;
				EditorGUI.PropertyField(position, property.FindPropertyRelative("colored"));
				if (property.FindPropertyRelative("colored").boolValue)
				{
					position.x += 9;
					position.width -= 9;
					position.y += position.height;
					EditorGUI.BeginChangeCheck();
					EditorGUI.PropertyField(position, property.FindPropertyRelative("color"));
					if (EditorGUI.EndChangeCheck())
					{
						Color _color = property.FindPropertyRelative("color").colorValue;

						property.FindPropertyRelative("colorPrefix").stringValue = string.Format("<color=#{0:X2}{1:X2}{2:X2}>", (byte)(_color.r), (byte)(_color.g), (byte)(_color.b));
					}
				}
			}
		}
	}
}
