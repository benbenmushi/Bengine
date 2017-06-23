using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor.Animations;
#endif
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class OpenCloseAnim : MonoBehaviourEx
{
#if UNITY_EDITOR
	[Button, ShowIf("AnimatorHasController")]
	void ClearAnim()
	{
		GetComponent<Animator>().runtimeAnimatorController = null;
	}

	[Button, HideIf("AnimatorHasController")]
	void AutoGenerateAnim()
	{
		AnimatorController controller = new AnimatorController();

		controller.AddLayer("ButtonBucket");
		AnimatorState closeState = controller.layers[0].stateMachine.AddState("Closed");
		AnimatorState openState = controller.layers[0].stateMachine.AddState("Open");

		openState.AddTransition(closeState);
		closeState.AddTransition(openState);
		AnimatorController.SetAnimatorController(GetComponent<Animator>(), controller);
	}

	[SerializeField, HideInInspector]
	AnimatorController currentController;
#endif

	bool AnimatorHasController
	{
		get
		{
			return GetComponent<Animator>().runtimeAnimatorController != null;
		}
	}
}
