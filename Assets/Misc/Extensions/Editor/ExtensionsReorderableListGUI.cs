using UnityEngine;
using Rotorz.ReorderableList;
using UnityEditor;

public static class ReorderableListGUIEx
{

	private static GUIStyle m_foldoutTitleButtonStyle;
	private static GUIStyle FoldoutTitleButtonStyle
	{
		get
		{
			if (m_foldoutTitleButtonStyle == null)
				m_foldoutTitleButtonStyle = GUI.skin.GetStyle("Foldout");
			return m_foldoutTitleButtonStyle;
		}
	}

	static GUIStyle m_miniButton;
	static GUIStyle miniButton
	{
		get
		{
			if (m_miniButton == null)
				m_miniButton = GUI.skin.GetStyle("minibutton");
			return m_miniButton;
		}
	}
	static GUIStyle m_miniButtonLeft;
	static GUIStyle miniButtonLeft
	{
		get
		{
			if (m_miniButtonLeft == null)
				m_miniButtonLeft = GUI.skin.GetStyle("minibuttonleft");
			return m_miniButtonLeft;
		}
	}
	static GUIStyle m_miniButtonMid;
	static GUIStyle miniButtonMid
	{
		get
		{
			if (m_miniButtonMid == null)
				m_miniButtonMid = GUI.skin.GetStyle("minibuttonmid");
			return m_miniButtonMid;
		}
	}
	static GUIStyle m_miniButtonRight;
	static GUIStyle miniButtonRight
	{
		get
		{
			if (m_miniButtonRight == null)
				m_miniButtonRight = GUI.skin.GetStyle("minibuttonright");
			return m_miniButtonRight;
		}
	}

	public static bool FoldoutTitle(bool foldout, string title)
	{
		GUIContent content = new GUIContent("   " + title, foldout ? FoldoutTitleButtonStyle.onActive.background : FoldoutTitleButtonStyle.normal.background);

		ReorderableListGUI.Title(content);
		Rect r = GUILayoutUtility.GetLastRect();
		r.height = 19;
		r.y++;
		if (GUI.Button(r, "", GUIStyle.none))
			foldout = !foldout;
		if (!foldout)
			GUILayout.Box(new GUIContent(""), GUIStyle.none, GUILayout.Height(5));
		return foldout;
	}
	public struct titleButton
	{
		public string           buttonName;
		public float            width;
		public System.Action    callback;
		public titleButton(string _buttonName, float _width, System.Action _callback)
		{
			buttonName = _buttonName;
			callback = _callback;
			width = _width;
		}
	}
	public static void TitleButtons(string title, params titleButton[] buttons)
	{
		ReorderableListGUI.Title(new GUIContent(title));
		Rect r = GUILayoutUtility.GetLastRect();
		float xMax = r.xMax;

		r.height = 17;
		r.y++;
		if (buttons.Length > 1)
		{
			int i = buttons.Length;

			while (i > 0)
			{
				--i;
				r.x = xMax - buttons[i].width;
				r.width = buttons[i].width;
				if (GUI.Button(r, buttons[i].buttonName, GetButtonStyle(i, buttons.Length)))
					buttons[i].callback();
				xMax = r.x;
			}
		}
		else if (buttons.Length == 1)
		{
			r.x = xMax - buttons[0].width;
			r.width = buttons[0].width;
			if (GUI.Button(r, buttons[0].buttonName, miniButton))
				buttons[0].callback();
		}
	}
	private static GUIStyle GetButtonStyle(int index, int totalButton)
	{
		if (index == 0)
			return miniButtonLeft;
		if (index == totalButton - 1)
			return miniButtonRight;
		else
			return miniButtonMid;
	}
}
