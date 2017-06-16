using UnityEditor;
using UnityEngine;

//[CustomPropertyDrawer(typeof(ReplicaScenarioAssetTab2DWrapper))]
public class Array2DPropertyDrawer : PropertyDrawer
{
	public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
	{
		return base.GetPropertyHeight(property, label) * 3;
	}

	public override void OnGUI(Rect position, SerializedProperty tabWrapper, GUIContent label)
	{
		ResizeArray(tabWrapper);
		position.height = 17;
		string labelString = "" + tabWrapper.FindPropertyRelative("myPanels").arraySize;
		labelString += ";" + tabWrapper.FindPropertyRelative("_w").intValue;
		labelString += ";" + tabWrapper.FindPropertyRelative("_h").intValue;

		// h = x
		// w = y
		GUI.Label(position, labelString);
		position.y += 17;
		EditorGUI.PropertyField(position, tabWrapper.FindPropertyRelative("_w"));
		position.y += 17;
		EditorGUI.PropertyField(position, tabWrapper.FindPropertyRelative("_h"));
	}

	void ResizeArray(SerializedProperty tabWrapper)
	{
		int size = tabWrapper.FindPropertyRelative("_w").intValue * tabWrapper.FindPropertyRelative("_h").intValue;
		int index = tabWrapper.FindPropertyRelative("myPanels").arraySize;

		while (index < size)
		{
			tabWrapper.FindPropertyRelative("myPanels").InsertArrayElementAtIndex(0);
			index++;
		}
	}
}
