using UnityEngine;
using System.Collections;

public abstract class MonoBehaviourEx : MonoBehaviour
{
	/// <summary>
	/// Module color in logs
	/// </summary>
	protected virtual string debugModuleColorCode { get { return "#0044C4"; } }

	/// <summary>
	/// Module error color in logs
	/// </summary>
	protected virtual string debugModuleErrorColorCode { get { return "#FF0000"; } }

	/// <summary>
	/// Module warning color in logs
	/// </summary>
	protected virtual string debugModuleWarningColorCode { get { return "##F7D358"; } }

	/// <summary>
	/// If true, prints time since startup in logs
	/// </summary>
	protected bool debugShowTime = false;

	/// <summary>
	/// Add debug data in a log message
	/// </summary>
	/// <param name="debugString">log to treat</param>
	/// <returns>treated log</returns>
	protected string Log(string debugString, bool useColor = true)
	{
		return _Log(debugString, debugModuleColorCode, useColor);
	}

	/// <summary>
	/// Add debug data in a log warning
	/// </summary>
	/// <param name="debugString">log to treat</param>
	/// <returns>treated log</returns>
	protected string LogWarning(string debugString, bool useColor = true)
	{
		return _Log(debugString, debugModuleWarningColorCode, useColor);
	}

	/// <summary>
	/// Add debug data in a log error
	/// </summary>
	/// <param name="debugString">log to treat</param>
	/// <returns>treated log</returns>
	protected string LogError(string debugString, bool useColor = true)
	{
		return _Log(debugString, debugModuleErrorColorCode, useColor);
	}

	/// <summary>
	/// Add debug data in a log message
	/// </summary>
	/// <param name="debugString">log to treat</param>
	/// <param name="colorCode">data color</param>
	/// <returns>treated log</returns>
	private string _Log(string debugString, string colorCode = "#000000", bool useColor = true)
	{
#if UNITY_EDITOR

		var stackTrace = new System.Diagnostics.StackTrace(true);
		var frame = stackTrace.GetFrame(2);
		var filename = frame.GetFileName();

		// Cas de la coroutine :)
		if (filename == null)
		{
			frame = stackTrace.GetFrame(3);
			filename = frame.GetFileName();
		}

		filename = System.IO.Path.GetFileName(filename);
		filename = filename.Replace(".cs", "");

		var className = this.GetType().Name;
		if (filename != className)
			filename += " (" + className + ")";

		if (useColor)
			return "<color=" + colorCode + ">[" + (debugShowTime ? Time.realtimeSinceStartup.ToString("0000.000 ") : "") +
					filename
					+ "]</color>: " + debugString;
		else
			return "[" + (debugShowTime ? Time.realtimeSinceStartup.ToString("0000.000 ") : "") +
					filename
					+ "]: " + debugString;
#else
		return debugString;
#endif
	}

	public void StopAllTrackedCoroutines()
	{
		CoroutineTracker.StopAllCoroutines(this);
	}
	new public void StopAllCoroutines()
	{
		base.StopAllCoroutines();
		CoroutineTracker.StopAllCoroutines(this);
	}

	// Coroutine utilities
	public Coroutine StartTrackedCoroutine(string c, string name, IEnumerator routine)
	{
		return CoroutineTracker.StartCoroutine(c + '.' + name, this, routine);
	}
	public Coroutine StartTrackedCoroutine(string name, IEnumerator routine, bool showDebug = false)
	{
		return CoroutineTracker.StartCoroutine(this.GetType().Name + '.' + name, this, routine, showDebug);
	}
	public Coroutine DelayedAction(System.Action action, float delay, string trackedName = "delayedActionCoroutine")
	{
		return this.StartTrackedCoroutine(trackedName, delayedActionCoroutine(action, delay));
	}
	public Coroutine StartAsyncLoop(System.Action<float> action, float duration, System.Action endDlg = null, string trackedName = "asyncLoopCoroutine", bool showDebug = false)
	{
		return StartTrackedCoroutine(trackedName, asyncLoopCoroutine(action, duration, endDlg), showDebug);
	}
	public Coroutine StartAsyncLoopUnscaled(System.Action<float> action, float duration, System.Action endDlg = null, string trackedName = "asyncLoopCoroutine", bool showDebug = false)
	{
		return this.StartTrackedCoroutine(trackedName, asyncLoopUnscaledCoroutine(action, duration, endDlg), showDebug);
	}

	// Extra coroutine controls
	/// <summary>
	/// Interrupt any async loop started before now (allow you to start another)
	/// </summary>
	protected void InterruptLastAsyncLoops()
	{
		m_lastAsyncLoopExpirationTime = Time.time;
	}
	/// <summary>
	/// Interrupt any async loop started before and in this framecall.
	/// You must wait until next update to get async loop started.
	/// </summary>
	protected void InterruptCurrentFrameAsyncLoops()
	{
		m_nowAsyncLoopExpirationTime = Time.time;
	}
	/// <summary>
	/// Interrupt all async loop started and execute the last frame.
	/// </summary>
	public void ForceEndAsyncLoops()
	{
		m_endAsyncLoopTime = Time.time;
	}/// <summary>
	 /// Interrupt any async loop started before now (allow you to start another)
	 /// </summary>
	protected void InterruptLastAsyncLoopsUnscaled()
	{
		m_lastAsyncLoopExpirationUnscaledTime = Time.unscaledTime;
	}
	/// <summary>
	/// Interrupt any async loop started before and in this framecall.
	/// You must wait until next update to get async loop started.
	/// </summary>
	protected void InterruptCurrentFrameAsyncLoopsUnscaled()
	{
		m_nowAsyncLoopExpirationUnscaledTime = Time.unscaledTime;
	}
	/// <summary>
	/// Interrupt all async loop started and execute the last frame.
	/// </summary>
	protected void ForceEndAsyncLoopsUnscaled()
	{
		m_endAsyncLoopUnscaledTime = Time.unscaledTime;
	}
	/// <summary>
	/// Interrupt every delayed action started on this MonoBehaviourEx.
	/// This will cancel the invokation of every actions.
	/// </summary>
	protected void StopAllDelayedActions()
	{
		m_stopAllDelayedActionTime = Time.time;
	}
	/// <summary>
	/// Execute all delayed action started on this MonoBehaviour.
	/// This will stop this waiting and execute the action.
	/// </summary>
	protected void ExecuteAllDelayedActionsNow()
	{
		m_forceExcecutionAllDelayedActionTime = Time.time;
	}
	public bool pauseCoroutines { get { return m_pauseCoroutines; } protected set { m_pauseCoroutines = value; } }

	#region Coroutine utility privates
	//[SerializeField]
	bool m_pauseCoroutines = false;
	float m_lastAsyncLoopExpirationTime = -1;
	float m_nowAsyncLoopExpirationTime = -1;
	float m_endAsyncLoopTime = -1;
	float m_lastAsyncLoopExpirationUnscaledTime = -1;
	float m_nowAsyncLoopExpirationUnscaledTime = -1;
	float m_endAsyncLoopUnscaledTime = -1;
	float m_stopAllDelayedActionTime = -1;
	float m_forceExcecutionAllDelayedActionTime = -1;
	bool isAsyncInterrupt(float startTime)
	{
		return startTime < m_lastAsyncLoopExpirationTime || startTime <= m_nowAsyncLoopExpirationTime;
	}
	bool shouldForceEndAsyncLoop(float startTime)
	{
		return startTime < m_endAsyncLoopTime;
	}
	bool isAsyncUnscaledInterrupt(float startTime)
	{
		return startTime < m_lastAsyncLoopExpirationUnscaledTime || startTime <= m_nowAsyncLoopExpirationUnscaledTime;
	}
	bool shouldForceEndAsyncLoopUnscaled(float startTime)
	{
		return startTime < m_endAsyncLoopUnscaledTime;
	}
	bool isDelayedActionStoped(float startTime)
	{
		return startTime < m_stopAllDelayedActionTime;
	}
	bool shouldForceExecuteDelayedAction(float startTime)
	{
		return startTime < m_forceExcecutionAllDelayedActionTime;
	}
	IEnumerator delayedActionCoroutine(System.Action action, float delay)
	{
		if (action != null)
		{
			float startTime = Time.time;
			float currentTime = 0;

			while (currentTime < delay && !isDelayedActionStoped(startTime) && !shouldForceExecuteDelayedAction(startTime))
			{
				yield return null;
				while (pauseCoroutines)
					yield return null;
				currentTime += Time.deltaTime;
			}
			if (!isDelayedActionStoped(startTime))
				action();
		}
	}
	IEnumerator asyncLoopCoroutine(System.Action<float> action, float duration, System.Action endDlg = null)
	{
		float currentTime = 0;
		float startTime = Time.time;

		while (currentTime < duration && !isAsyncInterrupt(startTime) && !shouldForceEndAsyncLoop(startTime))
		{
			action(currentTime / duration);
			yield return null;
			while (pauseCoroutines)
				yield return null;
			currentTime += Time.deltaTime;
		}
		if (!isAsyncInterrupt(startTime))
		{
			action(1f);
			if (endDlg != null)
				endDlg();
		}
	}
	IEnumerator asyncLoopUnscaledCoroutine(System.Action<float> action, float duration, System.Action endDlg = null)
	{
		float currentTime = 0;
		float startTime = Time.unscaledTime;

		while (currentTime < duration && !isAsyncUnscaledInterrupt(startTime) && !shouldForceEndAsyncLoopUnscaled(startTime))
		{
			action(currentTime / duration);
			yield return null;
			while (pauseCoroutines)
				yield return null;
			currentTime += Time.unscaledDeltaTime;
		}
		if (!isAsyncUnscaledInterrupt(startTime))
		{
			action(1f);
			if (endDlg != null)
				endDlg();
		}
	}
	#endregion
}
