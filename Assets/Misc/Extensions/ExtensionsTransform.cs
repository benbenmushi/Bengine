using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public static class ExtensionsTransform
{
	public static float cosPI4 = 0.70710678118f; // cos (PI / 4) = sqrt(2) / 2
	public static float cosPI6 = 0.86602540378f; // cos (PI / 6) = sqrt(3) / 2
	public static float cosPI8 = 0.9238796f; // cos (PI / 8) = sqrt(2 + sqrt(2)) / 2
	public static float cos3PI8 = 0.3826834f; // cos (3PI / 8) = sqrt(2 - sqrt(2)) / 2

	[System.Serializable, System.Flags]
	public enum TransformPosition
	{
		None = 0,
		Front = 1,
		Back = 2,
		Left = 4,
		Right = 8,
		FrontLeft = 5,
		FrontRight = 9,
		BackLeft = 6,
		BackRight = 10,
		LeftFront = 5,
		LeftBack = 6,
		RightFront = 9,
		RightBack = 10
	}
	public static List<T> GetChildList<T>(this Transform t) where T : Component
	{
		return t.GetChildList().Select((c) => c.GetComponent<T>()).Where((c) => c != null).ToList();
	}
	public static List<Transform> GetChildList(this Transform t)
	{
		List<Transform> list = new List<Transform>();

		t.ForEach((child) =>
			{
				list.Add(child);
			});
		return list;
	}
	public static void ForEach(this Transform t, System.Action<Transform> action)
	{
		for (int i = 0; i < t.childCount; i++)
			action(t.GetChild(i));
	}
	public static Transform AddEmptyChild(this Transform t)
	{
		GameObject go = new GameObject("child");

		go.transform.parent = t;
		go.transform.localPosition = Vector3.zero;
		go.transform.localEulerAngles = Vector3.zero;
		return go.transform;
	}
	public static Vector2 GetTargetTransformPositionTrigo(this Transform me, Transform other)
	{
		Vector3 meToOther = me.position.FromTo(other.position).normalized;
		Vector3 meUp = me.up;
		float dirRight = me.forward.AngleDirReel(meToOther, meUp);
		float dirBack = me.right.AngleDirReel(meToOther, meUp);

		return new Vector2(dirRight, dirBack);
	}
	public static TransformPosition GetTargetTransformPosition(this Transform me, Transform other)
	{
		Vector2 pos = me.GetTargetTransformPositionTrigo(other);
		float dirRight = pos.x;
		float dirBack = pos.y;
		bool isRight;
		bool isBack;

		isRight = dirRight >= 0;
		isBack = dirBack >= 0;
		//Debug.Log("dirRight: " + dirRight);
		//Debug.Log("dirBack: " + dirBack);

		if (isBack)
		{
			if (isRight)
			{
				if (dirRight < cos3PI8)
					return TransformPosition.Back;
				else if (dirRight > cosPI8)
					return TransformPosition.Right;
				else
					return TransformPosition.BackRight;
			}
			else
			{
				if (dirRight > -cos3PI8)
					return TransformPosition.Back;
				else if (dirRight < -cosPI8)
					return TransformPosition.Left;
				else
					return TransformPosition.BackLeft;
			}
		}
		else
		{
			if (isRight)
			{
				if (dirRight < cos3PI8)
					return TransformPosition.Front;
				else if (dirRight > cosPI8)
					return TransformPosition.Right;
				else
					return TransformPosition.FrontRight;
			}
			else
			{
				if (dirRight > -cos3PI8)
					return TransformPosition.Front;
				else if (dirRight < -cosPI8)
					return TransformPosition.Left;
				else
					return TransformPosition.FrontLeft;
			}
		}
	}
	public static Vector3 GetWorldScale(this Transform t)
	{
		if (t == null)
			return Vector3.one;
		Vector3 wScale = t.localScale;

		while (t.parent != null)
		{
			t = t.parent;
			wScale = wScale.Multiply(t.localScale);
		}
		return wScale;
	}

	public static RectTransform AddUIChild(this Transform t, string name)
	{
		GameObject go = new GameObject(name);

		go.AddComponent<RectTransform>();
		go.transform.SetParent(t, false);
		return go.GetComponent<RectTransform>();
	}

	/// <summary>
	/// Sets the layer of a transform and its children
	/// </summary>
	/// <param name="t">Transform to set</param>
	/// <param name="layer">Name of the layer to give</param>
	public static void SetLayerRecursively(this Transform t, string layer)
	{
		SetLayerRecursively(t, LayerMask.NameToLayer(layer));
	}

	/// <summary>
	/// Sets the layer of a transform and its children
	/// </summary>
	/// <param name="t">Transform to set</param>
	/// <param name="layer">Layer to give</param>
	public static void SetLayerRecursively(this Transform t, int layer)
	{
		t.gameObject.layer = layer;
		foreach (Transform child in t)
			SetLayerRecursively(child, layer);
	}

	/// <summary>
	/// Searches for a transform by name recursively
	/// </summary>
	/// <param name="source">Source object (parent)</param>
	/// <param name="objname">Searched object name</param>
	/// <param name="obj">Result (null if not found)</param>
	/// <param name="isSubNameContained">If true, the name can be contained in the result's name</param>
	public static bool FindChildRecursively(this Transform source, string objname, out Transform obj, bool isSubNameContained = false)
	{
		obj = null;
		for (int i = 0; i < source.childCount; i++)
		{
			if ((source.GetChild(i).name == objname) ||
				(isSubNameContained && (source.GetChild(i).name.ToUpper().Contains(objname.ToUpper()))))
			{
				obj = source.GetChild(i);
				return true;
			}
		}

		for (int i = 0; i < source.childCount; i++)
		{
			FindChildRecursively(source.GetChild(i), objname, out obj, isSubNameContained);
			if (obj != null)
				return true;
		}
		return false;
	}
}
