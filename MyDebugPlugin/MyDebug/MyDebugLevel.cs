using System;
using System.Text;
using UnityEngine;

namespace MyDebugAPI
{
	[Serializable]
	/// <summary>
	/// Compilation Target : .NET FRAMEWORK 3.5
	/// </summary>
	public sealed class MyDebug
	{
		[Range(0, 3)]
		public int              LogLevel = 3;
		[SerializeField]
		private string          m_prefix;
		[SerializeField]
		private bool            colored;
		[SerializeField]
		private Color           color;
		private string          colorPrefix;

		/// <summary>
		/// MyDebug can be used to display information on Unity's console.
		/// </summary>
		/// <param name="_logLevel"></param>
		/// <param name="prefix">Will be concatained before any message and does not need to contain ':'.</param>
		public MyDebug(int _logLevel, string prefix)
		{
			LogLevel = _logLevel;
			m_prefix = prefix;
			colored = false;
		}

		/// <summary>
		/// MyDebug can be used to display information on Unity's console.
		/// </summary>
		/// <param name="logType"></param>
		/// <param name="prefix">Will be concatained before any message and does not need to contain ':'.</param>
		/// <param name="_color">Color of the logged text.</param>
		public MyDebug(int _logLevel, string prefix, Color _color)
		{
			LogLevel = _logLevel;
			m_prefix = prefix;
			colored = true;
			color = _color;
			colorPrefix = string.Format("<color=#{0:X2}{1:X2}{2:X2}>", (byte)(_color.r), (byte)(_color.g), (byte)(_color.b));
		}

		/// <summary>
		/// Logs message to the Unity Console.
		/// </summary>
		/// <param name="logLevel">Message is displayed if logLevel is less or equal to the log level of the MyObject logType.</param>
		/// <param name="message">String or object to be converted to string representation for display.</param>
		public void Log(int logLevel, string message)
		{
			if (logLevel <= LogLevel)
				Debug.Log(BuildMessage(m_prefix + message));
		}

		/// <summary>
		/// A variant of Debug.Log that logs a warning message to the console.
		/// </summary>
		/// <param name="logLevel">Message is displayed if logLevel is less or equal to the log level of the MyObject logType.</param>
		/// <param name="message">String or object to be converted to string representation for display.</param>
		public void LogWarning(int logLevel, string message)
		{
			if (logLevel <= LogLevel)
				Debug.LogWarning(BuildMessage(message));
		}

		/// <summary>
		/// A variant of Debug.Log that logs a warning message to the console.
		/// </summary>
		/// <param name="logLevel">Message is displayed if logLevel is less or equal to the log level of the MyObject logType.</param>
		/// <param name="message">String or object to be converted to string representation for display.</param>
		public void LogError(int logLevel, string message)
		{
			if (logLevel <= LogLevel)
				Debug.LogError(BuildMessage(message));
		}

		private string BuildMessage(string content)
		{
			if (colored)
				return (new StringBuilder()).Append(colorPrefix).Append(m_prefix).Append(content).Append("</color>").ToString();
			else
				return (new StringBuilder()).Append(m_prefix).Append(content).ToString();
		}
	}
}
