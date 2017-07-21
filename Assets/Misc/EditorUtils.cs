#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using UnityEditor.SceneManagement;

public static class EditorUtils
{
	public static trackedEvent<ScriptableObject, string> OnScriptableObjectCreatedEvent = new trackedEvent<ScriptableObject, string>("EditorUtils", "OnScriptableObjectCreatedEvent");

	/// <summary>
	/// Searches the editor script among every created scripts
	/// <remarks>Should be called by 'OnEnable'</remarks>
	/// </summary>
	/// <param name="editor">The editor that wants to get its Monoscript</param>
	/// <returns>the Monoscript</returns>
	public static MonoScript GetScript(this Editor editor)
	{
		Type scriptType = editor.target.GetType();

		string[] guids = AssetDatabase.FindAssets("t:Script");
		string path;
		MonoScript script;

		foreach (string guid in guids)
		{
			path = AssetDatabase.GUIDToAssetPath(guid);
			script = AssetDatabase.LoadAssetAtPath(path, typeof(MonoScript)) as MonoScript;

			if (script.GetClass() == scriptType)
			{
				return script;
			}
		}
		return null;
	}

	internal static bool IsSceneCurrentlyLoaded(string externalGraphScene)
	{
		for (int i = 0; i < EditorSceneManager.sceneCount; i++)
		{
			if (EditorSceneManager.GetSceneAt(i).name == externalGraphScene)
				return true;
		}
		return false;
	}
	public static SceneAsset GetSceneAssetFromPath(string scenePath)
	{
		return AssetDatabase.LoadAssetAtPath(scenePath, typeof(SceneAsset)) as SceneAsset;
	}
	public static string GetSceneAssetPath(string sceneName)
	{
		if (!string.IsNullOrEmpty(sceneName))
		{
			string[] sceneFound = AssetDatabase.FindAssets("t:scene " + sceneName).Select(guid => AssetDatabase.GUIDToAssetPath(guid)).ToArray();

			if (sceneFound.Length > 0)
				return sceneFound.FirstOrDefault(s => s.EndsWith(sceneName + ".unity"));
		}
		return "";
	}
	public static SceneAsset GetSceneAsset(string sceneName)
	{
		return GetSceneAssetFromPath(GetSceneAssetPath(sceneName));
	}
	public static string[] GetAssetsPathByType(Type type)
	{
		return AssetDatabase.FindAssets("t:" + type.Name).Select(guid => AssetDatabase.GUIDToAssetPath(guid)).ToArray();
	}
	public static T CreateAsset<T>(string assetName = null, bool selectObject = true) where T : ScriptableObject
	{
		T asset = ScriptableObject.CreateInstance<T>();

		string path = AssetDatabase.GetAssetPath(Selection.activeObject);
		if (string.IsNullOrEmpty(path))
		{
			path = "Assets";
		}
		else if (Path.GetExtension(path) != "")
		{
			path = path.Replace(Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)), "");
		}

		string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path + "/" + (assetName != null ? assetName :"New " + typeof(T).ToString()) + ".asset");

		AssetDatabase.CreateAsset(asset, assetPathAndName);

		AssetDatabase.SaveAssets();
		EditorUtility.FocusProjectWindow();
		if (selectObject)
			Selection.activeObject = asset;

		OnScriptableObjectCreatedEvent.InvokeEvent(asset, assetPathAndName);
		return asset;
	}

	

	public static string GetEditorSettingFile(string fileName, string defaultContent = "")
	{
		string fullFilePath = Application.dataPath.Replace("Assets", "ProjectSettings/") + fileName;
		if (!File.Exists(fullFilePath))
			File.WriteAllText(fullFilePath, defaultContent);
		return File.ReadAllText(fullFilePath);
	}
	public static void SaveEditorSettingsFile(string fileName, string fileContent)
	{
		string fullFilePath = Application.dataPath.Replace("Assets", "ProjectSettings/") + fileName;
		File.WriteAllText(fullFilePath, fileContent);
	}
	public static EditorWindow InspectTarget(UnityEngine.Object target)
	{
		// Get a reference to the `InspectorWindow` type object
		var inspectorType = typeof(Editor).Assembly.GetType("UnityEditor.InspectorWindow");
		// Create an InspectorWindow instance
		var inspectorInstance = ScriptableObject.CreateInstance(inspectorType) as EditorWindow;
		// We display it - currently, it will inspect whatever gameObject is currently selected
		// So we need to find a way to let it inspect/aim at our target GO that we passed
		// For that we do a simple trick:
		// 1- Cache the current selected gameObject
		// 2- Set the current selection to our target GO (so now all inspectors are targeting it)
		// 3- Lock our created inspector to that target
		// 4- Fallback to our previous selection
		inspectorInstance.Show();
		// Cache previous selected gameObject
		var prevSelection = Selection.activeObject;
		// Set the selection to GO we want to inspect
		Selection.activeObject = target;
		// Get a ref to the "locked" property, which will lock the state of the inspector to the current inspected target
		var isLocked = inspectorType.GetProperty("isLocked", BindingFlags.Instance | BindingFlags.Public);
		// Invoke `isLocked` setter method passing 'true' to lock the inspector
		isLocked.GetSetMethod().Invoke(inspectorInstance, new object[] { true });
		// Finally revert back to the previous selection so that other inspectors continue to inspect whatever they were inspecting...
		Selection.activeObject = prevSelection;
		return inspectorInstance;
	}

	public static Texture2D GetEditorGUIUtilityIcon(string name)
	{
		return typeof(EditorGUIUtility).GetMethod("LoadIcon", BindingFlags.Static | BindingFlags.NonPublic).Invoke(null, new object[] { name }) as Texture2D;
	}



	#region Custom GUIStyles
	static GUIStyle m_warnEntry;
	public static GUIStyle warnEntry
	{
		get
		{
			if (m_warnEntry == null)
			{
				m_warnEntry = new GUIStyle(GUI.skin.GetStyle("CN EntryWarn"));
				m_warnEntry.stretchHeight = true;
				m_warnEntry.clipping = TextClipping.Overflow;
				m_warnEntry.fontSize = 11;
				m_warnEntry.imagePosition = ImagePosition.ImageLeft;
				m_warnEntry.wordWrap = true;
			}
			return m_warnEntry;
		}
	}
	static GUIStyle m_errorEntry;
	public static GUIStyle errorEntry
	{
		get
		{
			if (m_errorEntry == null)
			{
				m_errorEntry = new GUIStyle(GUI.skin.GetStyle("CN EntryError"));
				m_errorEntry.stretchHeight = true;
				m_errorEntry.clipping = TextClipping.Overflow;
				m_errorEntry.fontSize = 11;
				m_errorEntry.imagePosition = ImagePosition.ImageLeft;
				m_errorEntry.wordWrap = true;
			}
			return m_errorEntry;
		}
	}
	static GUIStyle m_boldLabel;
	public static GUIStyle boldLabel
	{
		get
		{
			if (m_boldLabel == null)
			{
				m_boldLabel = new GUIStyle(GUI.skin.label);
				m_boldLabel.fontStyle = FontStyle.Bold;
				m_boldLabel.stretchWidth = true;
			}
			return m_boldLabel;
		}
	}

	static GUIStyle m_labelArea;
	public static GUIStyle labelArea
	{
		get
		{
			if (m_labelArea == null)
			{
				m_labelArea = new GUIStyle(GUI.skin.label);
				m_labelArea.wordWrap = true;
			}
			return m_labelArea;
		}
	}
	static GUIStyle m_boldTitleLabel;
	public static GUIStyle boldTitleLabel
	{
		get
		{
			if (m_boldTitleLabel == null)
			{
				m_boldTitleLabel = new GUIStyle(GUI.skin.label);
				m_boldTitleLabel.fontStyle = FontStyle.Bold;
				m_boldTitleLabel.alignment = TextAnchor.MiddleCenter;
				m_boldTitleLabel.stretchWidth = true;
			}
			return m_boldTitleLabel;
		}
	}
	static GUIStyle m_whiteBox;
	public static GUIStyle whiteBox
	{
		get
		{
			if (m_whiteBox == null)
			{
				m_whiteBox = new GUIStyle(GUI.skin.GetStyle("sv_iconselector_labelselection"));

				m_whiteBox.padding = new RectOffset(5, 5, 5, 5);
				m_whiteBox.stretchHeight = false;
			}
			return m_whiteBox;
		}
	}
	static GUIStyle m_sexyFoldoutOpen;
	static GUIStyle sexyFoldoutOpen
	{
		get
		{
			if (m_sexyFoldoutOpen == null)
			{
				m_sexyFoldoutOpen = new GUIStyle(GUI.skin.GetStyle("OL Plus"));
			}
			return m_sexyFoldoutOpen;
		}
	}
	static GUIStyle m_sexyFoldoutClose;
	static GUIStyle sexyFoldoutClose
	{
		get
		{
			if (m_sexyFoldoutClose == null)
			{
				m_sexyFoldoutClose = new GUIStyle(GUI.skin.GetStyle("OL Minus"));
			}
			return m_sexyFoldoutClose;
		}
	}
	static GUIStyle m_foldoutOpen;
	static GUIStyle foldoutOpen
	{
		get
		{
			if (m_foldoutOpen == null)
			{
				m_foldoutOpen = new GUIStyle(GUI.skin.label);
				m_foldoutOpen.normal.background = EditorStyles.foldout.normal.background;
			}
			return m_foldoutOpen;
		}
	}
	static GUIStyle m_foldoutClose;
	static GUIStyle foldoutClose
	{
		get
		{
			if (m_foldoutClose == null)
			{
				m_foldoutClose = new GUIStyle(GUI.skin.label);
				m_foldoutClose.normal.background = EditorStyles.foldout.onNormal.background;
			}
			return m_foldoutClose;
		}
	}
	static GUIStyle m_miniButton;
	public static GUIStyle miniButton
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
	// Colors
	static Dictionary<string, GUIStyle> m_labelWithColorBGMap = new Dictionary<string, GUIStyle>();
	public static GUIStyle LabelWithColorBG(Color color)
	{
		string colorKey = color.ToString();

		if (!m_labelWithColorBGMap.ContainsKey(colorKey))
		{
			GUIStyle style = new GUIStyle(GUI.skin.label);

			style.normal.background = new Texture2D(2, 2);
			style.normal.background.SetPixels(new Color[] { color, color, color, color });
			//m_labelWithColorBGMap.Add(colorKey, style);
			return style;
		}
		return m_labelWithColorBGMap[colorKey];
	}
	static GUIStyle GetEditorStyle(string name)
	{
		PropertyInfo p = typeof(EditorStyles).GetProperty(name, BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);

		if (p != null)
			return (GUIStyle)p.GetGetMethod(true).Invoke(null, new object[0]);
		return null;
	}
	#endregion

	#region EditorIcons
	static Texture m_warnIconSmall;
	public static Texture warnIconSmall
	{
		get
		{
			if (m_warnIconSmall == null)
				m_warnIconSmall = GetEditorGUIUtilityIcon("console.warnicon.sml");
			return m_warnIconSmall;
		}
	}
	static Texture m_errorIconSmall;
	public static Texture errorIconSmall
	{
		get
		{
			if (m_errorIconSmall == null)
				m_errorIconSmall = GetEditorGUIUtilityIcon("console.erroricon.sml");
			return m_errorIconSmall;
		}
	}
	#endregion

	public static bool ConfirmDialogPopup()
	{
		return EditorUtility.DisplayDialog("Are you sure ?", "Are you sure ?", "Yes", "No");
	}
	public static void BeginVerticalColoredLayout(Color color)
	{
		Color oldGUIColor = GUI.color;
		GUI.color = color;
		EditorGUILayout.BeginVertical(EditorStyles.helpBox);
		GUI.color = oldGUIColor;
	}
	public static void BeginHorizontalColoredLayout(Color color)
	{
		Color oldGUIColor = GUI.color;
		GUI.color = color;
		EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
		GUI.color = oldGUIColor;
	}
	public static void BigSeparator()
	{
		bool guiWasEnabled = GUI.enabled;
		GUI.enabled = false;
		GUILayout.Box("", GUI.skin.horizontalSlider, GUILayout.MaxHeight(10));
		GUI.enabled = guiWasEnabled;
	}
	public static void ArrowSeparator(int widthCount = 1, int heightCount = 1)
	{
		int arrowWidth = 13;
		int arrowHeight = 13;

		GUILayout.Label("", GUILayout.Height(heightCount * (arrowHeight + 1)));
		Rect lastRect = GUILayoutUtility.GetLastRect();

		lastRect.x += (lastRect.width * 0.5f) - ((widthCount * (arrowWidth + 1) * 0.5f));
		float beginY = lastRect.y;
		for (int x = 0; x < widthCount; x++)
		{
			lastRect.y = beginY;
			for (int y = 0; y < heightCount; y++)
			{
				GUI.Button(lastRect.SetHeight(arrowHeight).SetWidth(arrowWidth), "", foldoutClose);
				lastRect.y += arrowWidth + 2;
			}
			lastRect.x += arrowHeight + 2;
		}
	}
	public static bool Foldout(bool foldout, GUIContent content)
	{
		EditorGUILayout.BeginHorizontal();
		if (foldout && GUILayout.Button("", foldoutClose, GUILayout.Width(13), GUILayout.Height(13)))
			foldout = false;
		else if (!foldout && GUILayout.Button("", foldoutOpen, GUILayout.Width(13), GUILayout.Height(13)))
			foldout = true;
		EditorGUILayout.LabelField(content);
		EditorGUILayout.EndHorizontal();
		return foldout;
	}
	public static bool SexyFoldout(bool foldout, string label)
	{
		return SexyFoldout(foldout, new GUIContent(label));
	}
	public static bool SexyFoldout(bool foldout, GUIContent content)
	{
		EditorGUILayout.BeginHorizontal();
		if (GUILayout.Button("", foldout ? sexyFoldoutClose : sexyFoldoutOpen, GUILayout.Width(20)))
			foldout = !foldout;
		EditorGUILayout.LabelField(content);
		EditorGUILayout.EndHorizontal();
		return foldout;
	}
	public static bool SexyFoldout(bool foldout, Action titleDrawer)
	{
		EditorGUILayout.BeginHorizontal();
		if (GUILayout.Button("", foldout ? sexyFoldoutClose : sexyFoldoutOpen, GUILayout.Width(20)))
			foldout = !foldout;
		titleDrawer();
		EditorGUILayout.EndHorizontal();
		return foldout;
	}
	public static bool SexyFoldout(Rect rect, bool foldout, string content)
	{
		return SexyFoldout(rect, foldout, new GUIContent(content));
	}
	public static bool SexyFoldout(Rect rect, bool foldout, GUIContent content)
	{
		rect.x += 2;
		rect.y += 2;
		rect.height -= 2;
		if (GUI.Button(rect.SetWidth(20), "", foldout ? sexyFoldoutClose : sexyFoldoutOpen))
			foldout = !foldout;
		GUI.Label(rect.SetWidth(rect.width - 20).SetX(rect.x + 20), content);
		return foldout;
	}
	public static int IntPopup(int index, int min, int max, string postfix = "")
	{
		GUIContent[] contentTab = new GUIContent[(max - min) + 1];

		for (int i = 0; i < contentTab.Length; i++)
			contentTab[i] = new GUIContent("" + (min + i) + postfix);
		return EditorGUILayout.Popup(index, contentTab);
	}
	public static string Popup(GUIContent content, string value, string[] values)
	{
		int selectedIndex = values.GetIndexOf(value);
		GUIContent[] contentTab = new GUIContent[values.Length];

		for (int i = 0; i < contentTab.Length; i++)
			contentTab[i] = new GUIContent(values[i]);
		selectedIndex = EditorGUILayout.Popup(content, selectedIndex, contentTab);
		if (selectedIndex >= 0 && selectedIndex < values.Length)
			return values[selectedIndex];
		return value;
	}

	public static string ScenePathField(GUIContent content, string scenePath)
	{
		SceneAsset sceneObject = GetSceneAssetFromPath(scenePath);

		if (sceneObject == null && !string.IsNullOrEmpty(scenePath))
			Debug.LogError("[SceneField] Scene by the name of \"" + scenePath + "\" was not found in your project. Your old reference has been lost.");
		sceneObject = EditorGUILayout.ObjectField(content, sceneObject, typeof(SceneAsset), false) as SceneAsset;
		if (sceneObject != null)
			scenePath = AssetDatabase.GetAssetPath(sceneObject);
		else
			scenePath = "";
		if (!string.IsNullOrEmpty(scenePath) && EditorBuildSettings.scenes.FirstOrDefault(s => s.path == scenePath) == null)
		{
			Color guiColor = GUI.color;
			GUI.color = Color.yellow;
			GUILayout.Box("", GUILayout.Height(30), GUILayout.ExpandWidth(true));
			Rect r = GUILayoutUtility.GetLastRect();
			EditorGUI.LabelField(r.SetWidth(40), "", EditorUtils.warnEntry);
			r.y += 5;
			r.height -= 10;
			r.x += 40;
			r.width -= 45;
			GUI.color = guiColor;
			if (GUI.Button(r, "Add to BuildSettings"))
				EditorUtils.AddSceneToBuildSettings(scenePath);
		}
		return scenePath;
	}

	static int MultiTextSelectionFieldControlID = -1;
	static MultiTextSelectionWizard wizard = null;
	public static void MultiTextSelectionField(GUIContent content, string text, Action<string> setText, params string[] choices)
	{
		EditorGUILayout.BeginHorizontal();
		if (content != null)
			EditorGUILayout.PrefixLabel(content);
		if (GUILayout.Button(text, EditorStyles.popup))
		{
			wizard = ScriptableWizard.DisplayWizard<MultiTextSelectionWizard>("Select one Text", "Close");

			wizard.choices = choices;
			wizard.choiceIndex = wizard.choices.FindIndex(c => c.Equals(text));
			wizard.textSelected += setText;
		}
		EditorGUILayout.EndHorizontal();
	}
	public struct OverflowButton
	{
		public GUIContent   content;
		public bool         buttonEnabled;
		public Action       callback;

		public OverflowButton(GUIContent _content, bool _buttonEnabled, Action _cb) : this()
		{
			content = _content;
			callback = _cb;
			buttonEnabled = _buttonEnabled;
		}
		public OverflowButton(string _content, bool _buttonEnabled, Action _cb) : this()
		{
			content = new GUIContent(_content);
			callback = _cb;
			buttonEnabled = _buttonEnabled;
		}
	}
	public static void ButtonOverflow(params OverflowButton[] buttons)
	{
		Rect lastRect = GUILayoutUtility.GetLastRect();
		lastRect.x += lastRect.width;

		for (int i = buttons.Length; i > 0;)
		{
			i--;
			GUIStyle buttonStyle = miniButtonMid;

			if (i == 0)
				buttonStyle = buttons.Length != 1 ? miniButtonLeft : miniButton;
			else if (i == buttons.Length - 1)
				buttonStyle = miniButtonRight;
			lastRect.width = buttonStyle.CalcSize(buttons[i].content).x;
			lastRect.x -= lastRect.width;
			bool guiWasEnabled = GUI.enabled;
			GUI.enabled = buttons[i].buttonEnabled;
			if (GUI.Button(lastRect, buttons[i].content, buttonStyle) && buttons[i].callback != null)
				buttons[i].callback();
			GUI.enabled = guiWasEnabled;
		}
	}
	public static void IconOverflow(params Texture[] icons)
	{
		Rect lastRect = GUILayoutUtility.GetLastRect();

		lastRect.height += 4;
		lastRect.y -= 2;
		for (int i = 0; i < icons.Length; i++)
		{
			lastRect.width = lastRect.height * icons[i].width / icons[i].height;
			GUI.Box(lastRect, new GUIContent(icons[i]), GUI.skin.label);
			lastRect.x += lastRect.width;
		}
	}
	
	public static void AddSceneToBuildSettings(string scenePath)
	{
		var scenes = EditorBuildSettings.scenes;

		ExtensionsArray.Add(ref scenes, new EditorBuildSettingsScene(scenePath, true));
		EditorBuildSettings.scenes = scenes;
	}
}

#region MultiTextSelection
public class MultiTextSelectionWizard : ScriptableWizard
{
	public string               filter;
	public int                  choiceIndex;
	public string[]             choices;
	public bool[]               filteredChoices;
	public event Action<string> textSelected;

	Vector2 scroll;

	protected override bool DrawWizardGUI()
	{
		if (textSelected == null)
			Close();
		if (choices != null)
		{
			FilterToolbarGUI();
			if (filteredChoices == null)
				RefreshFilteredChoicesArray();
			scroll = EditorGUILayout.BeginScrollView(scroll);
			for (int i = 0; i < choices.Length; i++)
			{
				if (filteredChoices[i])
				{
					if (GUILayout.Button(choices[i], choiceIndex == i ? EditorUtils.boldLabel : EditorStyles.label))
					{
						choiceIndex = i;
						textSelected(choices[choiceIndex]);
						Close();
					}
				}
			}
			EditorGUILayout.EndScrollView();
		}
		else
			GUILayout.Label("choices == null");
		return true;
	}
	void FilterToolbarGUI()
	{
		Rect position = EditorGUILayout.GetControlRect(false, 18f, GUI.skin.GetStyle("ToolbarSeachTextField"));

		GUI.Box(position, "", EditorStyles.toolbar);
		position.width -= 18;
		position.x += 2;
		position.y++;
		position.height -= 2;
		EditorGUI.BeginChangeCheck();
		filter = EditorGUI.TextField(position, filter, GUI.skin.GetStyle("ToolbarSeachTextField"));
		if (EditorGUI.EndChangeCheck())
			RefreshFilteredChoicesArray();
		position.x += position.width;
		position.width = 16;
		if (!string.IsNullOrEmpty(filter))
		{
			if (GUI.Button(position, "", GUI.skin.GetStyle("ToolbarSeachCancelButton")))
			{
				filter = "";
				RefreshFilteredChoicesArray();
			}
		}
		else
			GUI.Label(position, "", GUI.skin.GetStyle("ToolbarSeachCancelButtonEmpty"));
	}

	void RefreshFilteredChoicesArray()
	{
		if (!string.IsNullOrEmpty(filter))
		{
			if (filteredChoices.Length != choices.Length)
				Array.Resize(ref filteredChoices, choices.Length);
			for (int i = 0; i < choices.Length; i++)
				filteredChoices[i] = choices[i].ContainsIgnoreCase(filter);
		}
		else
			filteredChoices = choices.Select(c => true).ToArray();
	}
	void OnWizardCreate()
	{
	}
}
#endregion

#endif
