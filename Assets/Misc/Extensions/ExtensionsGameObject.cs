using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class ExtensionsGameObject
{
	public static string GetFullHierarchyPath(this GameObject go)
	{
		string parentPath = go.GetParentHierarchyPath();
		return string.IsNullOrEmpty(parentPath) ? go.name : parentPath + "/" + go.name;
	}
	public static string GetParentHierarchyPath(this GameObject go)
	{
		return GetParentName(go.transform);
	}
	static string GetParentName(Transform t)
	{
		if (t != null)
		{
			if (t.parent != null)
			{
				string parentName = GetParentName(t.parent);
				return string.IsNullOrEmpty(parentName) ? t.name : parentName + "/" + t.name;
			}
			return t.name;
		}
		return "";
	}
}
