using System;
using UnityEngine;

public class ScriptablesSingletonAttribute : PropertyAttribute
{
	public Type singletonType;

	public ScriptablesSingletonAttribute(Type _singletonType)
	{
		singletonType = _singletonType;
	}
}
