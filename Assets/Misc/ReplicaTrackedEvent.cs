using UnityEngine;
using System.Collections;
using System;
using System.Linq;
using System.Collections.Generic;

public static class trackedEventUtility
{
#if UNITY_EDITOR
		const string trackedEventUtilitySettingFileName = "trackedEventUtility";
		static trackedEventUtility()
		{
			ParseSettingsFileContent(EditorUtils.GetEditorSettingFile(trackedEventUtilitySettingFileName, GetSettingsFileContent()));
		}
		static void SaveSettings()
		{
			EditorUtils.SaveEditorSettingsFile(trackedEventUtilitySettingFileName, GetSettingsFileContent());
		}
		static string GetSettingsFileContent()
		{
			return "m_logInvokeEventCalls=" + m_logInvokeEventCalls + "\n" +
				"m_logRegistrationErrors=" + m_logRegistrationErrors;
		}
		static void ParseSettingsFileContent(string fileContent)
		{
			if (fileContent != "")
			{
				string[] settings = fileContent.Split('\n');
				if (settings.Length == 2)
				{
					m_logInvokeEventCalls = bool.Parse(settings[0].Split('=')[1]);
					m_logRegistrationErrors = bool.Parse(settings[1].Split('=')[1]);
				}
			}
		}
		public const string trackedLogPrefix = "<color=#006400>\t/!\\ EVENT CALL /!\\</color>: ";
#else
    public const string trackedLogPrefix = "[ReplicaTrackerEvent]: ";
#endif

    static bool  m_logInvokeEventCalls = false;
    public static bool logInvokeEventCalls
    {
        get { return m_logInvokeEventCalls; }
        set
        {
            m_logInvokeEventCalls = value;
#if UNITY_EDITOR
				SaveSettings();
#endif
        }
    }
    static bool  m_logRegistrationErrors = false;
    public static bool logRegistrationErrors
    {
        get { return m_logRegistrationErrors; }
        set
        {
            m_logRegistrationErrors = value;
#if UNITY_EDITOR
				SaveSettings();
#endif
        }
    }


    public static bool IsRegistered(Action evnt, Delegate method)
    {
        if (evnt != null)
            return evnt.GetInvocationList().Any(m => m == method);
        return false;
    }
    public static bool IsRegistered<T>(Action<T> evnt, Delegate method)
    {
        if (evnt != null)
            return evnt.GetInvocationList().Any(m => m == method);
        return false;
    }
    public static bool IsRegistered<T1, T2>(Action<T1, T2> evnt, Delegate method)
    {
        if (evnt != null)
            return evnt.GetInvocationList().Any(m => m == method);
        return false;
    }
    public static bool IsRegistered<T1, T2, T3>(Action<T1, T2, T3> evnt, Delegate method)
    {
        if (evnt != null)
            return evnt.GetInvocationList().Any(m => m == method);
        return false;
    }
    public static bool IsRegisteredAsync<T>(Func<T> evnt, Delegate method)
    {
        if (evnt != null)
            return evnt.GetInvocationList().Any(m => m == method);
        return false;
    }
    public static bool IsRegisteredAsync<T1, T2>(Func<T1, T2> evnt, Delegate method)
    {
        if (evnt != null)
            return evnt.GetInvocationList().Any(m => m == method);
        return false;
    }
    public static bool IsRegisteredAsync<T1, T2, T3>(Func<T1, T2, T3> evnt, Delegate method)
    {
        if (evnt != null)
            return evnt.GetInvocationList().Any(m => m == method);
        return false;
    }
}
public struct trackedEvent
{
    private event Action    _action;
    public string          sourceName;
    public string          eventName;



    public trackedEvent(string _sourceName, string _eventName)
    {
        _action = null;
        sourceName = _sourceName;
        eventName = _eventName;
    }

    public void InvokeEvent(bool showDebug = false)
    {
        if (_action != null)
        {
            if (trackedEventUtility.logInvokeEventCalls || showDebug)
            {
                Delegate[] dList = _action.GetInvocationList();
                Debug.Log(trackedEventUtility.trackedLogPrefix + sourceName + "." + eventName + ".InvokeEvent(showDebug=true) - " + dList.Length + " subscribed(" + dList.Select(d => d.Method.DeclaringType.Name).Aggregate((s1, s2) => s1 + ", " + s2) + ")");
            }
            _action();
        }
    }

    public static trackedEvent operator +(trackedEvent s, Action a)
    {
        if (!trackedEventUtility.IsRegistered(s._action, a))
        {
            s._action += a;
#if UNITY_EDITOR
				HooksTracker.StartTracking(s.sourceName, s.eventName, a.Method);
#endif
        }
        else if (trackedEventUtility.logRegistrationErrors)
            Debug.LogError(a.Method.Name + " is already subscribed from " + s.sourceName + "." + s.eventName);
        return s;
    }
    public static trackedEvent operator -(trackedEvent s, Action a)
    {
        if (trackedEventUtility.IsRegistered(s._action, a))
        {
            s._action -= a;
#if UNITY_EDITOR
				HooksTracker.StopTracking(s.sourceName, s.eventName, a.Method);
#endif
        }
        else if (trackedEventUtility.logRegistrationErrors)
            Debug.LogError(a.Method.Name + " is already unsubscribed from " + s.sourceName + "." + s.eventName);
        return s;
    }
}
public struct trackedEvent<T1>
{
    private event Action<T1>    _action;
    public string              sourceName;
    public string              eventName;


    public trackedEvent(string _sourceName, string _eventName)
    {
        _action = null;
        sourceName = _sourceName;
        eventName = _eventName;
    }

    public void InvokeEvent(T1 arg, bool showDebug = false)
    {
        if (_action != null)
        {
            if (trackedEventUtility.logInvokeEventCalls || showDebug)
            {
                Delegate[] dList = _action.GetInvocationList();
                Debug.Log(trackedEventUtility.trackedLogPrefix + sourceName + "." + eventName + ".InvokeEvent(" + arg + ", showDebug=true) - " + dList.Length + " subscribed(" + dList.Select(d => d.Method.DeclaringType.Name).Aggregate((s1, s2) => s1 + ", " + s2) + ")");
            }
            _action(arg);
        }
    }

    public static trackedEvent<T1> operator +(trackedEvent<T1> s, Action<T1> a)
    {
        if (!trackedEventUtility.IsRegistered(s._action, a))
        {
            s._action += a;
#if UNITY_EDITOR
				HooksTracker.StartTracking(s.sourceName, s.eventName, a.Method);
#endif
        }
        else if (trackedEventUtility.logRegistrationErrors)
            Debug.LogError(a.Method.Name + " is already subscribed from " + s.sourceName + "." + s.eventName);
        return s;
    }
    public static trackedEvent<T1> operator -(trackedEvent<T1> s, Action<T1> a)
    {
        if (trackedEventUtility.IsRegistered(s._action, a))
        {
            s._action -= a;
#if UNITY_EDITOR
				HooksTracker.StopTracking(s.sourceName, s.eventName, a.Method);
#endif
        }
        else if (trackedEventUtility.logRegistrationErrors)
            Debug.LogError(a.Method.Name + " is already unsubscribed from " + s.sourceName + "." + s.eventName);
        return s;
    }
}
public struct trackedEvent<T1, T2>
{
    private event Action<T1, T2>    _action;
    public string                  sourceName;
    public string                  eventName;

    public trackedEvent(string _sourceName, string _eventName)
    {
        _action = null;
        sourceName = _sourceName;
        eventName = _eventName;
    }

    public void InvokeEvent(T1 arg, T2 arg2, bool showDebug = false)
    {
        if (_action != null)
        {
            if (trackedEventUtility.logInvokeEventCalls || showDebug)
            {
                Delegate[] dList = _action.GetInvocationList();
                Debug.Log(trackedEventUtility.trackedLogPrefix + sourceName + "." + eventName + ".InvokeEvent(" + arg + ", " + arg2 + ", showDebug=true) - " + dList.Length + " subscribed(" + dList.Select(d => d.Method.DeclaringType.Name).Aggregate((s1, s2) => s1 + ", " + s2) + ")");
            }
            _action(arg, arg2);
        }
    }

    public static trackedEvent<T1, T2> operator +(trackedEvent<T1, T2> s, Action<T1, T2> a)
    {
        if (!trackedEventUtility.IsRegistered(s._action, a))
        {
            s._action += a;
#if UNITY_EDITOR
				HooksTracker.StartTracking(s.sourceName, s.eventName, a.Method);
#endif
        }
        else if (trackedEventUtility.logRegistrationErrors)
            Debug.LogError(a.Method.Name + " is already subscribed from " + s.sourceName + "." + s.eventName);
        return s;
    }
    public static trackedEvent<T1, T2> operator -(trackedEvent<T1, T2> s, Action<T1, T2> a)
    {
        if (trackedEventUtility.IsRegistered(s._action, a))
        {
            s._action -= a;
#if UNITY_EDITOR
				HooksTracker.StopTracking(s.sourceName, s.eventName, a.Method);
#endif
        }
        else if (trackedEventUtility.logRegistrationErrors)
            Debug.LogError(a.Method.Name + " is already unsubscribed from " + s.sourceName + "." + s.eventName);
        return s;
    }
}
public struct trackedEvent<T1, T2, T3>
{
    private event Action<T1, T2, T3>    _action;
    public string                      sourceName;
    public string                      eventName;

    public trackedEvent(string _sourceName, string _eventName)
    {
        _action = null;
        sourceName = _sourceName;
        eventName = _eventName;
    }

    public void InvokeEvent(T1 arg, T2 arg2, T3 arg3, bool showDebug = false)
    {
        if (_action != null)
        {
            if (trackedEventUtility.logInvokeEventCalls || showDebug)
            {
                Delegate[] dList = _action.GetInvocationList();
                Debug.Log(trackedEventUtility.trackedLogPrefix + sourceName + "." + eventName + ".InvokeEvent(" + arg + ", " + arg2 + ", " + arg3 + ", showDebug=true) - " + dList.Length + " subscribed(" + dList.Select(d => d.Method.DeclaringType.Name).Aggregate((s1, s2) => s1 + ", " + s2) + ")");
            }
            _action(arg, arg2, arg3);
        }
    }

    public static trackedEvent<T1, T2, T3> operator +(trackedEvent<T1, T2, T3> s, Action<T1, T2, T3> a)
    {
        if (!trackedEventUtility.IsRegistered(s._action, a))
        {
            s._action += a;
#if UNITY_EDITOR
				HooksTracker.StartTracking(s.sourceName, s.eventName, a.Method);
#endif
        }
        else if (trackedEventUtility.logRegistrationErrors)
            Debug.LogError(a.Method.Name + " is already subscribed from " + s.sourceName + "." + s.eventName);
        return s;
    }
    public static trackedEvent<T1, T2, T3> operator -(trackedEvent<T1, T2, T3> s, Action<T1, T2, T3> a)
    {
        if (trackedEventUtility.IsRegistered(s._action, a))
        {
            s._action -= a;
#if UNITY_EDITOR
				HooksTracker.StopTracking(s.sourceName, s.eventName, a.Method);
#endif
        }
        else if (trackedEventUtility.logRegistrationErrors)
            Debug.LogError(a.Method.Name + " is already unsubscribed from " + s.sourceName + "." + s.eventName);
        return s;
    }
}
public struct trackedEventAsync
{
    private Func<IEnumerator>   _asyncAction;
    public string              sourceName;
    public string              eventName;

    public trackedEventAsync(string _sourceName, string _eventName)
    {
        _asyncAction = null;
        sourceName = _sourceName;
        eventName = _eventName;
    }

    public Coroutine InvokeEvent(MonoBehaviourEx caller, Action endCallback = null, bool showDebug = false)
    {
#if UNITY_EDITOR
			return caller.StartTrackedCoroutine(sourceName, eventName, InvokeEventTracked_internal(caller, endCallback, showDebug));
#else
        return caller.StartTrackedCoroutine(sourceName, eventName, InvokeEvent_internal(caller, endCallback, showDebug));
#endif
    }
    private IEnumerator InvokeEvent_internal(MonoBehaviour caller, Action endCallback = null, bool showDebug = false)
    {
        if (caller != null)
        {
            if (_asyncAction != null && (trackedEventUtility.logInvokeEventCalls || showDebug))
            {
                Delegate[] dList = _asyncAction.GetInvocationList();
                Debug.Log(trackedEventUtility.trackedLogPrefix + sourceName + "." + eventName + ".InvokeEventAsync(showDebug=true) - " + dList.Length + " subscribed(" + dList.Select(d => d.Method.DeclaringType.Name).Aggregate((s1, s2) => s1 + ", " + s2) + ")");
            }
            for (int i = 0; _asyncAction != null && i < _asyncAction.GetInvocationList().Length; i++)
            {
                Func<IEnumerator> coroutineEvent = _asyncAction.GetInvocationList()[i] as Func<IEnumerator>;

                yield return caller.StartCoroutine(coroutineEvent());
            }
            if (endCallback != null)
                endCallback();
        }
    }
    private IEnumerator InvokeEventTracked_internal(MonoBehaviourEx caller, Action endCallback = null, bool showDebug = false)
    {
        if (caller != null)
        {
            if (_asyncAction != null && (trackedEventUtility.logInvokeEventCalls || showDebug))
            {
                Delegate[] dList = _asyncAction.GetInvocationList();
                Debug.Log(trackedEventUtility.trackedLogPrefix + sourceName + "." + eventName + ".InvokeEventTrackedAsync(showDebug=true) - " + dList.Length + " subscribed(" + dList.Select(d => d.Method.DeclaringType.Name).Aggregate((s1, s2) => s1 + ", " + s2) + ")");
            }
            for (int i = 0; _asyncAction != null && i < _asyncAction.GetInvocationList().Length; i++)
            {
                Func<IEnumerator> coroutineEvent = _asyncAction.GetInvocationList()[i] as Func<IEnumerator>;

                yield return caller.StartTrackedCoroutine(caller.name, coroutineEvent.Method.Name, coroutineEvent());
            }
            if (endCallback != null)
                endCallback();
        }
    }

    public static trackedEventAsync operator +(trackedEventAsync s, Func<IEnumerator> a)
    {
        if (!trackedEventUtility.IsRegisteredAsync(s._asyncAction, a))
        {
            s._asyncAction += a;
#if UNITY_EDITOR
				HooksTracker.StartTracking(s.sourceName, s.eventName, a.Method);
#endif
        }
        else if (trackedEventUtility.logRegistrationErrors)
            Debug.LogError(a.Method.Name + " is already subscribed from " + s.sourceName + "." + s.eventName);
        return s;
    }
    public static trackedEventAsync operator -(trackedEventAsync s, Func<IEnumerator> a)
    {
        if (trackedEventUtility.IsRegisteredAsync(s._asyncAction, a))
        {
            s._asyncAction -= a;
#if UNITY_EDITOR
				HooksTracker.StopTracking(s.sourceName, s.eventName, a.Method);
#endif
        }
        else if (trackedEventUtility.logRegistrationErrors)
            Debug.LogError(a.Method.Name + " is already unsubscribed from " + s.sourceName + "." + s.eventName);
        return s;
    }
}
public struct trackedEventAsync<T1>
{
    private Func<T1, IEnumerator>   _asyncAction;
    public string                  sourceName;
    public string                  eventName;

    public trackedEventAsync(string _sourceName, string _eventName)
    {
        _asyncAction = null;
        sourceName = _sourceName;
        eventName = _eventName;
    }

    public Coroutine InvokeEvent(MonoBehaviourEx caller, T1 arg, Action endCallback = null, bool showDebug = false)
    {
#if UNITY_EDITOR
			return caller.StartTrackedCoroutine(sourceName, eventName, InvokeEventTracked_internal(caller, arg, endCallback, showDebug));
#else
        return caller.StartTrackedCoroutine(sourceName, eventName, InvokeEvent_internal(caller, arg, endCallback, showDebug));
#endif
    }

    private IEnumerator InvokeEvent_internal(MonoBehaviour caller, T1 arg, Action endCallback = null, bool showDebug = false)
    {
        if (caller != null)
        {
            if (_asyncAction != null && (trackedEventUtility.logInvokeEventCalls || showDebug))
            {
                Delegate[] dList = _asyncAction.GetInvocationList();
                Debug.Log(trackedEventUtility.trackedLogPrefix + sourceName + "." + eventName + ".InvokeEventAsync(" + arg + ", showDebug=true) - " + dList.Length + " subscribed(" + dList.Select(d => d.Method.DeclaringType.Name).Aggregate((s1, s2) => s1 + ", " + s2) + ")");
            }
            for (int i = 0; _asyncAction != null && i < _asyncAction.GetInvocationList().Length; i++)
            {
                Func<T1, IEnumerator> coroutineEvent = _asyncAction.GetInvocationList()[i] as Func<T1, IEnumerator>;

                yield return caller.StartCoroutine(coroutineEvent(arg));
            }
            if (endCallback != null)
                endCallback();
        }
    }
    private IEnumerator InvokeEventTracked_internal(MonoBehaviourEx caller, T1 arg, Action endCallback = null, bool showDebug = false)
    {
        if (caller != null)
        {
            if (_asyncAction != null && (trackedEventUtility.logInvokeEventCalls || showDebug))
            {
                Delegate[] dList = _asyncAction.GetInvocationList();
                Debug.Log(trackedEventUtility.trackedLogPrefix + sourceName + "." + eventName + ".InvokeEventTrackedAsync(" + arg + ", showDebug=true) - " + dList.Length + " subscribed(" + dList.Select(d => d.Method.DeclaringType.Name).Aggregate((s1, s2) => s1 + ", " + s2) + ")");
            }
            for (int i = 0; _asyncAction != null && i < _asyncAction.GetInvocationList().Length; i++)
            {
                Func<T1, IEnumerator> coroutineEvent = _asyncAction.GetInvocationList()[i] as Func<T1, IEnumerator>;

                yield return caller.StartTrackedCoroutine(caller.name, coroutineEvent.Method.Name, coroutineEvent(arg));
            }
            if (endCallback != null)
                endCallback();
        }
    }

    public static trackedEventAsync<T1> operator +(trackedEventAsync<T1> s, Func<T1, IEnumerator> a)
    {
        if (!trackedEventUtility.IsRegisteredAsync(s._asyncAction, a))
        {
            s._asyncAction += a;
#if UNITY_EDITOR
				HooksTracker.StartTracking(s.sourceName, s.eventName, a.Method);
#endif
        }
        else if (trackedEventUtility.logRegistrationErrors)
            Debug.LogError(a.Method.Name + " is already subscribed from " + s.sourceName + "." + s.eventName);
        return s;
    }
    public static trackedEventAsync<T1> operator -(trackedEventAsync<T1> s, Func<T1, IEnumerator> a)
    {
        if (trackedEventUtility.IsRegisteredAsync(s._asyncAction, a))
        {
            s._asyncAction -= a;
#if UNITY_EDITOR
				HooksTracker.StopTracking(s.sourceName, s.eventName, a.Method);
#endif
        }
        else if (trackedEventUtility.logRegistrationErrors)
            Debug.LogError(a.Method.Name + " is already unsubscribed from " + s.sourceName + "." + s.eventName);
        return s;
    }
}
public struct trackedEventAsync<T1, T2>
{
    private Func<T1, T2, IEnumerator>   _asyncAction;
    public string                      sourceName;
    public string                      eventName;

    public trackedEventAsync(string _sourceName, string _eventName)
    {
        _asyncAction = null;
        sourceName = _sourceName;
        eventName = _eventName;
    }

    public Coroutine InvokeEvent(MonoBehaviourEx caller, T1 arg, T2 arg2, Action endCallback = null, bool showDebug = false)
    {
#if UNITY_EDITOR
			return caller.StartTrackedCoroutine(sourceName, eventName, InvokeEventTracked_internal(caller, arg, arg2, endCallback, showDebug));
#else
        return caller.StartTrackedCoroutine(sourceName, eventName, InvokeEvent_internal(caller, arg, arg2, endCallback, showDebug));
#endif
    }
    private IEnumerator InvokeEvent_internal(MonoBehaviour caller, T1 arg, T2 arg2, Action endCallback = null, bool showDebug = false)
    {
        if (caller != null)
        {
            if (_asyncAction != null && (trackedEventUtility.logInvokeEventCalls || showDebug))
            {
                Delegate[] dList = _asyncAction.GetInvocationList();
                Debug.Log(trackedEventUtility.trackedLogPrefix + sourceName + "." + eventName + ".InvokeEventAsync(" + arg + ", " + arg2 + ", showDebug=true) - " + dList.Length + " subscribed(" + dList.Select(d => d.Method.DeclaringType.Name).Aggregate((s1, s2) => s1 + ", " + s2) + ")");
            }
            for (int i = 0; _asyncAction != null && i < _asyncAction.GetInvocationList().Length; i++)
            {
                Func<T1, T2, IEnumerator> coroutineEvent = _asyncAction.GetInvocationList()[i] as Func<T1, T2, IEnumerator>;

                yield return caller.StartCoroutine(coroutineEvent(arg, arg2));
            }
            if (endCallback != null)
                endCallback();
        }
    }
    private IEnumerator InvokeEventTracked_internal(MonoBehaviourEx caller, T1 arg, T2 arg2, Action endCallback = null, bool showDebug = false)
    {
        if (caller != null)
        {
            if (_asyncAction != null && (trackedEventUtility.logInvokeEventCalls || showDebug))
            {
                Delegate[] dList = _asyncAction.GetInvocationList();
                Debug.Log(trackedEventUtility.trackedLogPrefix + sourceName + "." + eventName + ".InvokeEventTrackedAsync(" + arg + ", " + arg2 + ", showDebug=true) - " + dList.Length + " subscribed(" + dList.Select(d => d.Method.DeclaringType.Name).Aggregate((s1, s2) => s1 + ", " + s2) + ")");
            }
            for (int i = 0; _asyncAction != null && i < _asyncAction.GetInvocationList().Length; i++)
            {
                Func<T1, T2, IEnumerator> coroutineEvent = _asyncAction.GetInvocationList()[i] as Func<T1, T2, IEnumerator>;

                yield return caller.StartTrackedCoroutine(caller.name, coroutineEvent.Method.Name, coroutineEvent(arg, arg2));
            }
            if (endCallback != null)
                endCallback();
        }
    }

    public static trackedEventAsync<T1, T2> operator +(trackedEventAsync<T1, T2> s, Func<T1, T2, IEnumerator> a)
    {
        if (!trackedEventUtility.IsRegisteredAsync(s._asyncAction, a))
        {
            s._asyncAction += a;
#if UNITY_EDITOR
				HooksTracker.StartTracking(s.sourceName, s.eventName, a.Method);
#endif
        }
        else if (trackedEventUtility.logRegistrationErrors)
            Debug.LogError(a.Method.Name + " is already subscribed from " + s.sourceName + "." + s.eventName);
        return s;
    }
    public static trackedEventAsync<T1, T2> operator -(trackedEventAsync<T1, T2> s, Func<T1, T2, IEnumerator> a)
    {
        if (trackedEventUtility.IsRegisteredAsync(s._asyncAction, a))
        {
            s._asyncAction -= a;
#if UNITY_EDITOR
				HooksTracker.StopTracking(s.sourceName, s.eventName, a.Method);
#endif
        }
        else if (trackedEventUtility.logRegistrationErrors)
            Debug.LogError(a.Method.Name + " is already unsubscribed from " + s.sourceName + "." + s.eventName);
        return s;
    }
}

public struct trackedEventSynchro
{
    private Func<IEnumerator>   _synchroAction;
    public string               sourceName;
    public string               eventName;

    Dictionary<int, int>        leftForID;

    public trackedEventSynchro(string _sourceName, string _eventName)
    {
        _synchroAction = null;
        sourceName = _sourceName;
        eventName = _eventName;
        leftForID = new Dictionary<int, int>();
    }

    public Coroutine InvokeEvent(MonoBehaviourEx caller, Action endCallback = null, bool showDebug = false)
    {
        return caller.StartTrackedCoroutine(sourceName, eventName, InvokeEventMultiThread_internal(caller, endCallback, showDebug));
    }
    private IEnumerator InvokeEventMultiThread_internal(MonoBehaviourEx caller, Action endCallback = null, bool showDebug = false)
    {
        if (_synchroAction != null && (trackedEventUtility.logInvokeEventCalls || showDebug))
        {
            Delegate[] dList = _synchroAction.GetInvocationList();
            Debug.Log(trackedEventUtility.trackedLogPrefix + sourceName + "." + eventName + ".InvokeEventTrackedSynchro(showDebug=true) - " + dList.Length + " subscribed(" + dList.Select(d => d.Method.DeclaringType.Name).Aggregate((s1, s2) => s1 + ", " + s2) + ")");
        }
        int localID = leftForID.Count;

        leftForID.Add(localID, 0);
        for (int i = 0; _synchroAction != null && i < _synchroAction.GetInvocationList().Length; i++)
        {
            Func<IEnumerator> coroutineEvent = _synchroAction.GetInvocationList()[i] as Func<IEnumerator>;

            leftForID[localID] += 1;
            caller.StartCoroutine(WrappedCall(caller, coroutineEvent, localID));
        }
        while (leftForID[localID] > 0)
            yield return null;
        if (endCallback != null)
            endCallback();
    }
    private IEnumerator WrappedCall(MonoBehaviourEx caller, Func<IEnumerator> toCall, int callID)
    {
#if UNITY_EDITOR
			yield return caller.StartTrackedCoroutine(eventName, toCall.Method.Name, toCall());
#else
        yield return caller.StartCoroutine(toCall());
#endif
        leftForID[callID] -= 1;
    }

    public static trackedEventSynchro operator +(trackedEventSynchro s, Func<IEnumerator> a)
    {
        if (!trackedEventUtility.IsRegisteredAsync(s._synchroAction, a))
        {
            s._synchroAction += a;
#if UNITY_EDITOR
				HooksTracker.StartTracking(s.sourceName, s.eventName, a.Method);
#endif
        }
        else if (trackedEventUtility.logRegistrationErrors)
            Debug.LogError(a.Method.Name + " is already subscribed from " + s.sourceName + "." + s.eventName);
        return s;
    }
    public static trackedEventSynchro operator -(trackedEventSynchro s, Func<IEnumerator> a)
    {
        if (trackedEventUtility.IsRegisteredAsync(s._synchroAction, a))
        {
            s._synchroAction -= a;
#if UNITY_EDITOR
				HooksTracker.StopTracking(s.sourceName, s.eventName, a.Method);
#endif
        }
        else if (trackedEventUtility.logRegistrationErrors)
            Debug.LogError(a.Method.Name + " is already unsubscribed from " + s.sourceName + "." + s.eventName);
        return s;
    }
}
public struct trackedEventSynchro<T1>
{
    private Func<T1, IEnumerator>   _synchroAction;
    public string                   sourceName;
    public string                   eventName;

    Dictionary<int, int>            leftForID;

    public trackedEventSynchro(string _sourceName, string _eventName)
    {
        _synchroAction = null;
        sourceName = _sourceName;
        eventName = _eventName;
        leftForID = new Dictionary<int, int>();
    }

    public Coroutine InvokeEvent(MonoBehaviourEx caller, T1 arg, Action endCallback = null, bool showDebug = false)
    {
        return caller.StartTrackedCoroutine(sourceName, eventName, InvokeEventMultiThread_internal(caller, arg, endCallback, showDebug));
    }
    private IEnumerator InvokeEventMultiThread_internal(MonoBehaviourEx caller, T1 arg, Action endCallback = null, bool showDebug = false)
    {
        if (_synchroAction != null && (trackedEventUtility.logInvokeEventCalls || showDebug))
        {
            Delegate[] dList = _synchroAction.GetInvocationList();
            Debug.Log(trackedEventUtility.trackedLogPrefix + sourceName + "." + eventName + ".InvokeEventTrackedSynchro(" + arg + ", showDebug=true) - " + dList.Length + " subscribed(" + dList.Select(d => d.Method.DeclaringType.Name).Aggregate((s1, s2) => s1 + ", " + s2) + ")");
        }
        int localID = leftForID.Count;

        leftForID.Add(localID, 0);
        for (int i = 0; _synchroAction != null && i < _synchroAction.GetInvocationList().Length; i++)
        {
            Func<T1, IEnumerator> coroutineEvent = _synchroAction.GetInvocationList()[i] as Func<T1, IEnumerator>;

            leftForID[localID] += 1;
            caller.StartCoroutine(WrappedCall(caller, coroutineEvent, arg, localID));
        }
        while (leftForID[localID] > 0)
            yield return null;
        if (endCallback != null)
            endCallback();
    }
    private IEnumerator WrappedCall(MonoBehaviourEx caller, Func<T1, IEnumerator> toCall, T1 arg, int callID)
    {
#if UNITY_EDITOR
			yield return caller.StartTrackedCoroutine(eventName, toCall.Method.Name, toCall(arg));
#else
        yield return caller.StartCoroutine(toCall(arg));
#endif
        leftForID[callID] -= 1;
    }


    public static trackedEventSynchro<T1> operator +(trackedEventSynchro<T1> s, Func<T1, IEnumerator> a)
    {
        if (!trackedEventUtility.IsRegisteredAsync(s._synchroAction, a))
        {
            s._synchroAction += a;
#if UNITY_EDITOR
				HooksTracker.StartTracking(s.sourceName, s.eventName, a.Method);
#endif
        }
        else if (trackedEventUtility.logRegistrationErrors)
            Debug.LogError(a.Method.Name + " is already subscribed from " + s.sourceName + "." + s.eventName);
        return s;
    }
    public static trackedEventSynchro<T1> operator -(trackedEventSynchro<T1> s, Func<T1, IEnumerator> a)
    {
        if (trackedEventUtility.IsRegisteredAsync(s._synchroAction, a))
        {
            s._synchroAction -= a;
#if UNITY_EDITOR
				HooksTracker.StopTracking(s.sourceName, s.eventName, a.Method);
#endif
        }
        else if (trackedEventUtility.logRegistrationErrors)
            Debug.LogError(a.Method.Name + " is already unsubscribed from " + s.sourceName + "." + s.eventName);
        return s;
    }
}

public enum InputCollectorType
{
    Any,
    EveryOne,
}

public struct trackedInputCollector
{
    private Func<bool>          _inputCallbacks;
    public string               sourceName;
    public string               collectorName;

    private Func<bool, bool, bool>  evalResult;

    public trackedInputCollector(string _sourceName, string _collectorName, InputCollectorType _inputType)
    {
        _inputCallbacks = null;
        sourceName = _sourceName;
        collectorName = _collectorName;
        switch (_inputType)
        {
            case InputCollectorType.Any:
                evalResult = (current, newVal) => current || newVal;
                break;
            case InputCollectorType.EveryOne:
                evalResult = (current, newVal) => current && newVal;
                break;
            default:
                evalResult = (current, newVal) => current || newVal;
                break;
        }
    }

    public bool Collect(bool showDebug = false)
    {
        if (_inputCallbacks != null && (showDebug))
        {
            Delegate[] dList = _inputCallbacks.GetInvocationList();
            Debug.Log(trackedEventUtility.trackedLogPrefix + sourceName + "." + collectorName + ".Collect(showDebug=true) - " + dList.Length + " subscribed(" + dList.Select(d => d.Method.DeclaringType.Name).Aggregate((s1, s2) => s1 + ", " + s2) + ")");
        }
        bool result = false;
        for (int i = 0; _inputCallbacks != null && i < _inputCallbacks.GetInvocationList().Length; i++)
            result = evalResult(result, (_inputCallbacks.GetInvocationList()[i] as Func<bool>)());
        return result;
    }

    public static trackedInputCollector operator +(trackedInputCollector s, Func<bool> a)
    {
        if (!trackedEventUtility.IsRegisteredAsync(s._inputCallbacks, a))
        {
            s._inputCallbacks += a;
#if UNITY_EDITOR
				HooksTracker.StartTracking(s.sourceName, s.collectorName, a.Method);
#endif
        }
        else if (trackedEventUtility.logRegistrationErrors)
            Debug.LogError(a.Method.Name + " is already subscribed from " + s.sourceName + "." + s.collectorName);
        return s;
    }
    public static trackedInputCollector operator -(trackedInputCollector s, Func<bool> a)
    {
        if (trackedEventUtility.IsRegisteredAsync(s._inputCallbacks, a))
        {
            s._inputCallbacks -= a;
#if UNITY_EDITOR
				HooksTracker.StopTracking(s.sourceName, s.collectorName, a.Method);
#endif
        }
        else if (trackedEventUtility.logRegistrationErrors)
            Debug.LogError(a.Method.Name + " is already unsubscribed from " + s.sourceName + "." + s.collectorName);
        return s;
    }
}
public struct trackedInputCollector<T1>
{
    private Func<T1, bool>  _inputCallbacks;
    public string           sourceName;
    public string           collectorName;
    private Func<bool, bool, bool>  evalResult;

    public trackedInputCollector(string _sourceName, string _collectorName, InputCollectorType _inputType)
    {
        _inputCallbacks = null;
        sourceName = _sourceName;
        collectorName = _collectorName;
        switch (_inputType)
        {
            case InputCollectorType.Any:
                evalResult = (current, newVal) => current || newVal;
                break;
            case InputCollectorType.EveryOne:
                evalResult = (current, newVal) => current && newVal;
                break;
            default:
                evalResult = (current, newVal) => current || newVal;
                break;
        }
    }

    public bool Collect(T1 arg, bool showDebug = false)
    {
        if (_inputCallbacks != null && (showDebug))
        {
            Delegate[] dList = _inputCallbacks.GetInvocationList();
            Debug.Log(trackedEventUtility.trackedLogPrefix + sourceName + "." + collectorName + ".Collect(showDebug=true) - " + dList.Length + " subscribed(" + dList.Select(d => d.Method.DeclaringType.Name).Aggregate((s1, s2) => s1 + ", " + s2) + ")");
        }
        bool result = false;
        for (int i = 0; _inputCallbacks != null && i < _inputCallbacks.GetInvocationList().Length; i++)
            result = evalResult(result, (_inputCallbacks.GetInvocationList()[i] as Func<T1, bool>)(arg));
        return result;
    }

    public static trackedInputCollector<T1> operator +(trackedInputCollector<T1> s, Func<T1, bool> a)
    {
        if (!trackedEventUtility.IsRegisteredAsync(s._inputCallbacks, a))
        {
            s._inputCallbacks += a;
#if UNITY_EDITOR
				HooksTracker.StartTracking(s.sourceName, s.collectorName, a.Method);
#endif
        }
        else if (trackedEventUtility.logRegistrationErrors)
            Debug.LogError(a.Method.Name + " is already subscribed from " + s.sourceName + "." + s.collectorName);
        return s;
    }
    public static trackedInputCollector<T1> operator -(trackedInputCollector<T1> s, Func<T1, bool> a)
    {
        if (trackedEventUtility.IsRegisteredAsync(s._inputCallbacks, a))
        {
            s._inputCallbacks -= a;
#if UNITY_EDITOR
				HooksTracker.StopTracking(s.sourceName, s.collectorName, a.Method);
#endif
        }
        else if (trackedEventUtility.logRegistrationErrors)
            Debug.LogError(a.Method.Name + " is already unsubscribed from " + s.sourceName + "." + s.collectorName);
        return s;
    }
}