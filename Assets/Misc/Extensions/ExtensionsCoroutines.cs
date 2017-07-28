using System;
using System.Collections;
using UnityEngine;

public static class ExtensionsCoroutine
{
	public class LoadSceneRoutine
	{
		public AsyncOperation sceneLoading;
		public Coroutine previousOperation;

		public LoadSceneRoutine(Coroutine c, AsyncOperation loadScene)
		{
			this.previousOperation = c;
			this.sceneLoading = loadScene;
		}
	}

	#region Yield Instruction : Then

	public static YieldInstruction Then(this YieldInstruction coroutine, MonoBehaviour runner, Action action)
	{
		return runner.StartCoroutine(ThenRoutine(coroutine, action));
	}

	public static YieldInstruction Then(this YieldInstruction coroutine, MonoBehaviour runner, Func<YieldInstruction> routine)
	{
		return runner.StartCoroutine(ThenRoutine(coroutine, routine));
	}

	public static YieldInstruction Then(this YieldInstruction coroutine, MonoBehaviour runner, Func<IEnumerator> routine)
	{
		return runner.StartCoroutine(ThenRoutine(coroutine, routine));
	}

	#endregion

	#region CoroutineEx

	public static CoroutineEx Then(this CoroutineEx coroutine, Action action)
	{
		return new CoroutineEx(coroutine.runner, ThenRoutine(coroutine.routine, action));
	}
	public static CoroutineEx Then(this CoroutineEx coroutine, IEnumerator routine)
	{
		return new CoroutineEx(coroutine.runner, routine);
	}
	public static CoroutineEx Then(this CoroutineEx coroutine, CoroutineEx otherCoroutine)
	{
		return new CoroutineEx(coroutine.runner, otherCoroutine);
	}
	public static CoroutineEx WaitForSeconds(this CoroutineEx coroutine, float seconds)
	{
		return new CoroutineEx(coroutine.runner, WaitForSeconds(seconds));
	}

	#endregion

	#region LoadSceneRoutine : Then

	public static LoadSceneRoutine Then(this LoadSceneRoutine loadScene, MonoBehaviour runner, Action action)
	{
		loadScene.previousOperation = runner.StartCoroutine(ThenRoutine(loadScene.previousOperation, action));
		return loadScene;
	}

	public static LoadSceneRoutine Then(this LoadSceneRoutine loadScene, MonoBehaviour runner, Func<YieldInstruction> routine)
	{
		loadScene.previousOperation = runner.StartCoroutine(ThenRoutine(loadScene.previousOperation, routine));
		return loadScene;
	}

	public static LoadSceneRoutine Then(this LoadSceneRoutine loadScene, MonoBehaviour runner, Func<IEnumerator> routine)
	{
		loadScene.previousOperation = runner.StartCoroutine(ThenRoutine(loadScene.previousOperation, routine));
		return loadScene;
	}

	#endregion

	#region Then routine

	private static IEnumerator ThenRoutine(YieldInstruction coroutine, Func<YieldInstruction> routine)
	{
		yield return coroutine;
		if (routine != null)
			yield return routine();
	}

	private static IEnumerator ThenRoutine(YieldInstruction coroutine, Func<IEnumerator> routine)
	{
		yield return coroutine;
		if (routine != null)
			yield return routine();
	}

	private static IEnumerator ThenRoutine(YieldInstruction coroutine, Action action)
	{
		yield return coroutine;
		if (action != null)
			action();
	}

	#endregion

	#region Wait before load

	public static LoadSceneRoutine WaitBeforeLoad(this AsyncOperation loadScene, MonoBehaviour runner)
	{
		Coroutine c = runner.StartCoroutine(WaitBeforeLoadRoutine(null, loadScene));
		return new LoadSceneRoutine(c, loadScene);
	}

	public static LoadSceneRoutine WaitBeforeLoad(this LoadSceneRoutine loadScene, MonoBehaviour runner)
	{
		loadScene.previousOperation = runner.StartCoroutine(WaitBeforeLoadRoutine(loadScene.previousOperation, loadScene.sceneLoading));
		return loadScene;
	}

	private static IEnumerator WaitBeforeLoadRoutine(Coroutine c, AsyncOperation loadScene)
	{
		loadScene.allowSceneActivation = false;
		if (c != null)
			yield return c;
		while (loadScene.progress < 0.89f)
			yield return new WaitForEndOfFrame();
	}

	#endregion

	#region Wait load

	public static LoadSceneRoutine WaitLoad(this AsyncOperation loadScene, MonoBehaviour runner)
	{
		Coroutine c = runner.StartCoroutine(WaitLoadRoutine(null, loadScene));
		return new LoadSceneRoutine(c, loadScene);
	}

	public static LoadSceneRoutine WaitLoad(this LoadSceneRoutine loadScene, MonoBehaviour runner)
	{
		loadScene.previousOperation = runner.StartCoroutine(WaitLoadRoutine(loadScene.previousOperation, loadScene.sceneLoading));
		return loadScene;
	}

	private static IEnumerator WaitLoadRoutine(Coroutine c, AsyncOperation loadScene)
	{
		if (c != null)
			yield return c;
		loadScene.allowSceneActivation = true;
		yield return loadScene;
	}

	#endregion

	#region Wait seconds

	private static IEnumerator WaitForSeconds(float seconds)
	{
		yield return new WaitForSeconds(seconds);
	}

	#endregion

	#region Yield for each

	public static IEnumerator YieldForEachRoutine(this System.MulticastDelegate evnt, System.Func<System.Delegate, Coroutine> action)
	{
		if (evnt != null)
		{
			System.Delegate[] delegates = evnt.GetInvocationList();
			foreach (Delegate d in delegates)
			{
				yield return action(d);
			}
		}
	}

	public static Coroutine YieldForEach(this System.MulticastDelegate evnt, MonoBehaviour runner, System.Func<System.Delegate, IEnumerator> action)
	{
		return runner.StartCoroutine(evnt.YieldForEachRoutine((d) => runner.StartCoroutine(action(d))));
	}

	#endregion
}

public class CoroutineEx
{
	public readonly MonoBehaviour runner;
	public readonly Coroutine routine;

	public CoroutineEx(MonoBehaviour _runner, IEnumerator _function)
	{
		runner = _runner;
		routine = runner.StartCoroutine(_function);
	}
	public CoroutineEx(MonoBehaviour _runner, Coroutine _routine)
	{
		runner = _runner;
		routine = _routine;
	}
	public CoroutineEx(MonoBehaviour _runner, CoroutineEx _routine)
	{
		runner = _runner;
		routine = _routine.routine;
	}
}
