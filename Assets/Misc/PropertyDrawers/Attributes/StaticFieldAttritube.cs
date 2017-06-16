using UnityEngine;
using System.Collections;

public class StaticFieldAttritube : PropertyAttribute
{
	public string staticFieldName;
	public StaticFieldAttritube(string _staticFieldName)
	{
		staticFieldName = _staticFieldName;
	}
}
