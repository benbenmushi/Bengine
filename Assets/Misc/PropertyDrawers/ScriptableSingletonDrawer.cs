using UnityEngine;
using System.Reflection;

#if UNITY_EDITOR
using UnityEditor;
[CustomPropertyDrawer(typeof(ScriptablesSingletonAttribute))]
public class ScriptableSingletonDrawer : PropertyDrawer
{

	ScriptablesSingletonAttribute singletonAttribute
	{
		get
		{
			return attribute as ScriptablesSingletonAttribute;
		}
	}

	MethodInfo m_existMethodInfo;
	MethodInfo existMethodInfo
	{
		get
		{
			if (m_existMethodInfo == null)
			{
				PropertyInfo prop = singletonAttribute.singletonType.GetProperty("Exist", BindingFlags.Static | BindingFlags.Public | BindingFlags.GetProperty | BindingFlags.FlattenHierarchy);
				if (prop != null)
					m_existMethodInfo = prop.GetGetMethod();
			}
			return m_existMethodInfo;
		}
	}

	bool Exist
	{
		get
		{
			return (bool)existMethodInfo.Invoke(null, null);
		}
	}
	Object instance
	{
		get
		{
			return (Object)singletonAttribute.singletonType.GetProperty("instance", BindingFlags.Static | BindingFlags.Public | BindingFlags.GetProperty | BindingFlags.FlattenHierarchy).GetGetMethod().Invoke(null, null);
		}
	}
	GUIStyle m_warningStyle;
	GUIStyle warningStyle
	{
		get
		{
			if (m_warningStyle == null)
			{
				m_warningStyle = new GUIStyle(GUI.skin.GetStyle("CN EntryWarn"));

				m_warningStyle.fontSize = 12;
				m_warningStyle.fontStyle = FontStyle.Bold;
			}
			return m_warningStyle;
		}
	}

	public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
	{
		if (Exist)
			return base.GetPropertyHeight(property, label);
		else
			return 30f;
	}
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		if (property.objectReferenceValue != null)
			EditorGUI.PropertyField(position, property);
		else
		{
			if (!Exist)
				GUI.Label(position, "Missing " + singletonAttribute.singletonType.Name + "", warningStyle);
			else
				property.objectReferenceValue = instance;
		}
	}
}
#endif
