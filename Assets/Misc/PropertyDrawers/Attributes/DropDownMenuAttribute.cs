using UnityEngine;
using System.Collections;

public class DropDownMenuAttribute : PropertyAttribute
{
	public string arrayFieldName;

	public DropDownMenuAttribute(string _arrayFieldName)
	{
		arrayFieldName = _arrayFieldName;
	}
}
