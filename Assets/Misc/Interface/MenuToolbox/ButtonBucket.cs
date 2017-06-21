using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class ButtonBucket : MonoBehaviourEx
{
	public AnimationCurve   openCurve;
	public float            openDuration = 1;
	public float            closeDuration = 1;
	public Transform[]      targets;

	[System.Serializable]
	struct FrameInfo
	{
		public Vector3      localPosition;
	}

	[SerializeField, HideInInspector]
	FrameInfo[] closedInfoTab;
	[SerializeField, HideInInspector]
	FrameInfo[] opennedInfoTab;
	float       currentAlpha = 0;

	void Start()
	{
		SetAnimPosition(currentAlpha);
	}

	[DisableInEditorMode, Button, ButtonGroup("playmodeButtons")]
	public void Open()
	{
		float beginAlpha = currentAlpha;
		float targetAlpha = 1;

		StartAsyncLoop(a => SetAnimPosition(Mathf.Lerp(beginAlpha, targetAlpha, a)), Mathf.Lerp(0, openDuration, Mathf.Abs(targetAlpha - currentAlpha)));
	}
	[DisableInEditorMode, Button, ButtonGroup("playmodeButtons")]
	public void Close()
	{
		float beginAlpha = currentAlpha;
		float targetAlpha = 0;

		StartAsyncLoop(a => SetAnimPosition(Mathf.Lerp(beginAlpha, targetAlpha, a)), Mathf.Lerp(0, openDuration, Mathf.Abs(targetAlpha - currentAlpha)));
	}
	[DisableInEditorMode, Button, ButtonGroup("playmodeButtons")]
	public void Toggle()
	{
		if (currentAlpha > 0.5)
			Close();
		else
			Open();
	}

	void SetAnimPosition(float a)
	{
		currentAlpha = a;
		for (int i = 0; i < closedInfoTab.Length; i++)
			targets[i].localPosition = Vector3.LerpUnclamped(closedInfoTab[i].localPosition, opennedInfoTab[i].localPosition, openCurve.Evaluate(a));
	}

	[DisableInPlayMode, Button, ButtonGroup("editormodeButtons")]
	void SaveOpenPositions()
	{
		Undo.RegisterCompleteObjectUndo(this.gameObject, "SaveOpenPositions");
		opennedInfoTab = GetChildrenInfoTab();
	}
	[DisableInPlayMode, Button, ButtonGroup("editormodeButtons")]
	void SaveClosedPositions()
	{
		Undo.RegisterCompleteObjectUndo(this.gameObject, "SaveClosedPositions");
		closedInfoTab = GetChildrenInfoTab();
	}

	FrameInfo[] GetChildrenInfoTab()
	{
		FrameInfo[] tab = new FrameInfo[targets.Length];

		for (int i = 0; i < targets.Length; i++)
			tab[i].localPosition = targets[i].localPosition;
		return tab;
	}
}
