using UnityEngine;
using System.Linq;

public static class ExtensionsRectTransform
{

#if UNITY_EDITOR

	[UnityEditor.MenuItem("CONTEXT/RectTransform/AnchorFit")]
	private static void AnchorFit(UnityEditor.MenuCommand menuCommand)
	{
		RectTransform r = menuCommand.context as RectTransform;
		if (r != null && r.parent != null)
		{
			UnityEditor.Undo.RecordObject(r, "AnchorFit");
			AnchorFitPoint(menuCommand, false);
			RectTransform parentRt = r.parent.GetComponent<RectTransform>();
			Vector2 newAnchorMin, newAnchorMax;
			Vector3 position = r.position;

			r.anchorMin = Vector2.zero;
			r.anchorMax = Vector2.zero;
			r.position = position;
			{
				//Debug.Log(r.anchoredPosition);
				//Debug.Log(parentRt.anchoredPosition);
				//Debug.Log(r.anchoredPosition.x / parentRt.rect.width);
				//Debug.Log(r.anchoredPosition.y / parentRt.rect.height);
				// FINALLY FIXED PIVOT PROBLEM ! :D
				newAnchorMin.x = (r.anchoredPosition.x - (r.rect.width * r.pivot.x)) / parentRt.rect.width;
				newAnchorMax.x = (r.anchoredPosition.x + (r.rect.width * (1f - r.pivot.x))) / parentRt.rect.width;
				newAnchorMin.y = (r.anchoredPosition.y - (r.rect.height * r.pivot.y)) / parentRt.rect.height;
				newAnchorMax.y = (r.anchoredPosition.y + (r.rect.height * (1f - r.pivot.y))) / parentRt.rect.height;
			}
			r.anchorMin = newAnchorMin;
			r.anchorMax = newAnchorMax;
			r.sizeDelta = Vector2.zero;
			r.position = position;
		}
	}
	[UnityEditor.MenuItem("CONTEXT/RectTransform/AnchorFitPoint")]
	private static void _AnchorFitPoint(UnityEditor.MenuCommand menuCommand)
	{
		AnchorFitPoint(menuCommand);
	}
	private static void AnchorFitPoint(UnityEditor.MenuCommand menuCommand, bool undo = true)
	{
		RectTransform r = menuCommand.context as RectTransform;
		if (r != null && r.parent != null)
		{
			if (undo)
				UnityEditor.Undo.RecordObject(r, "AnchorFitPoint");
			RectTransform parentRt = r.parent.GetComponent<RectTransform>();
			Vector2 newAnchorMin, newAnchorMax;
			Vector3 position = r.position;
			Vector2 sizeDelta = new Vector2(r.rect.width, r.rect.height);

			r.anchorMin = Vector2.zero;
			r.anchorMax = Vector2.zero;
			r.position = position;
			{
				//Debug.Log(r.anchoredPosition);
				//Debug.Log(parentRt.anchoredPosition);
				newAnchorMin.x = r.anchoredPosition.x / parentRt.rect.width;
				newAnchorMax.x = newAnchorMin.x;
				newAnchorMin.y = r.anchoredPosition.y / parentRt.rect.height;
				newAnchorMax.y = newAnchorMin.y;
			}
			r.anchorMin = newAnchorMin;
			r.anchorMax = newAnchorMax;
			r.sizeDelta = sizeDelta;
			r.position = position;
		}
	}
#endif


	public static Canvas GetCanvas(this RectTransform trans)
	{
		Canvas c = null;
		Transform t = trans;
		while (t != null && c == null)
		{
			c = t.GetComponent<Canvas>();
			t = t.parent;
		}
		return c;
	}
	public static RectTransform GetCanvasRectTransform(this RectTransform child)
	{
		return child.GetCanvas().GetComponent<RectTransform>();
	}
	public static Vector2 GetCanvasPosition(this RectTransform rT)
	{
		Vector2             canvasPosition = Vector2.zero;
		RectTransform[]     parents = new RectTransform[] { rT };
		RectTransform       myParent;
		Rect                parentRect;
		Rect                anchoredRect;
		bool                showDebug = (rT.name == "View") && false;

		while (rT.parent != null) // Get every parents
		{
			rT = rT.parent.GetComponent<RectTransform>();
			System.Array.Resize(ref parents, parents.Length + 1);
			parents[parents.Length - 1] = rT;
		}
		if (showDebug)
			Debug.Log(parents.Select(p => p.name).Aggregate((s1, s2) => s1 + "<" + s2));
		System.Array.Reverse(parents); // reverse
		parentRect = parents[0].rect; // first is canvas RectTransform
		myParent = parents[0];
		anchoredRect = new Rect();
		for (int i = 0; i < parents.Length; i++)
		{
			if (i == 0)
				anchoredRect = new Rect(0, 0, 0, 0);
			else
			{
				anchoredRect.xMin = Mathf.Lerp(parentRect.xMin, parentRect.xMax, parents[i].anchorMin.x);// * parentRect.width;
				anchoredRect.xMax = Mathf.Lerp(parentRect.xMin, parentRect.xMax, parents[i].anchorMax.x);// * parentRect.width;
				anchoredRect.yMin = Mathf.Lerp(parentRect.yMin, parentRect.yMax, parents[i].anchorMin.y);// * parentRect.height;
				anchoredRect.yMax = Mathf.Lerp(parentRect.yMin, parentRect.yMax, parents[i].anchorMax.y);// * parentRect.height;
			}

			Vector2 oldPos = canvasPosition;
			canvasPosition = oldPos + parents[i].anchoredPosition
			+ new Vector2(anchoredRect.xMin + (anchoredRect.size.x * parents[i].pivot.x),
						  anchoredRect.yMin + (anchoredRect.size.y * parents[i].pivot.y));

			if (showDebug)
			{
				Debug.Log("xMin=" + anchoredRect.xMin + " _ xMax=" + anchoredRect.xMax + " _ yMin=" + anchoredRect.yMin + " _ yMax=" + anchoredRect.yMax);
				Debug.Log(oldPos + " + " + parents[i].anchoredPosition + " + " + new Vector2(anchoredRect.xMin + (anchoredRect.size.x * parents[i].pivot.x),
						  anchoredRect.yMin + (anchoredRect.size.y * parents[i].pivot.y)) + " = " + canvasPosition);
			}
			parentRect = parents[i].rect;
		}
		return canvasPosition;
	}
	public static Vector2 GetParentPivotInsideMyRect(this RectTransform rT, bool debug = false)
	{
		if (rT.parent != null)
		{
			RectTransform parent = rT.parent.GetComponent<RectTransform>();
			Vector2 parentPivotInsideMyRect = new Vector2();

			parentPivotInsideMyRect.x = (((parent.rect.size.x * parent.pivot.x) + (parent.parent != null ? parent.anchoredPosition.x : 0)) - (parent.rect.size.x * rT.anchorMin.x)) / rT.rect.size.x;
			if (debug)
			{
				Debug.Log("(((" + parent.rect.size.x + " * " + parent.pivot.x + ") + " + parent.anchoredPosition.x + ") - (" + parent.rect.size.x + " * " + rT.anchorMin.x + ")) / " + rT.rect.size.x);
				Debug.Log("(((" + parent.rect.size.x * parent.pivot.x + ") + " + parent.anchoredPosition.x + ") - " + parent.rect.size.x * rT.anchorMin.x + ") / " + rT.rect.size.x);
				Debug.Log("(" + ((parent.rect.size.x * parent.pivot.x) + parent.anchoredPosition.x) + " - " + (parent.rect.size.x * rT.anchorMin.x) + ") / " + rT.rect.size.x);
				Debug.Log("" + (((parent.rect.size.x * parent.pivot.x) + parent.anchoredPosition.x) - (parent.rect.size.x * rT.anchorMin.x)) + " / " + rT.rect.size.x);
				Debug.Log("x=" + parentPivotInsideMyRect.x);
			}
			parentPivotInsideMyRect.y = (((parent.rect.size.y * parent.pivot.y) + (parent.parent != null ? parent.anchoredPosition.y : 0)) - (parent.rect.size.y * rT.anchorMin.y)) / rT.rect.size.y;
			if (debug)
			{
				Debug.Log("(((" + parent.rect.size.y + " * " + parent.pivot.y + ") + " + parent.anchoredPosition.y + ") - (" + parent.rect.size.y + " * " + rT.anchorMin.y + ")) / " + rT.rect.size.y);
				Debug.Log("y=" + parentPivotInsideMyRect.y);
			}
			return parentPivotInsideMyRect;
		}
		return rT.pivot;
	}
	public static Vector2 GetMousePositionInCanvas(this RectTransform rT)
	{
		return Camera.main.ScreenToViewportPoint(Input.mousePosition).Multiply(rT.GetCanvasRectTransform().rect.size);
	}
	public static Vector3 GetAnchorMinPosition(this RectTransform rT)
	{
		return rT.parent.GetComponent<RectTransform>().NormalizedToPosition(rT.anchorMin);
	}
	public static Vector3 GetAnchorMaxPosition(this RectTransform rT)
	{
		return rT.parent.GetComponent<RectTransform>().NormalizedToPosition(rT.anchorMax);
	}
	public static Vector2 NormalizedToPosition(this RectTransform rT, Vector2 normalizedPosition)
	{
		//Canvas canvas = rT.GetCanvasRectTransform().GetComponent<Canvas>();
		Vector2 ret = (rT.position.AsVector2() + new Vector2(rT.rect.xMin, rT.rect.yMin));

		ret.x = ret.x + (rT.rect.width * normalizedPosition.x);
		ret.y = ret.y + (rT.rect.height * normalizedPosition.y);
		return ret;
	}
	public static Vector3 PositionToNormalized(this RectTransform rT, Vector3 position)
	{
		//Canvas canvas = rT.GetCanvasRectTransform().GetComponent<Canvas>();
		Vector3 ret = position - (rT.position - (rT.sizeDelta * 0.5f).AsVector3());

		ret.x = ret.x / rT.rect.width;
		ret.y = ret.y / rT.rect.height;
		return ret;
	}

	public static Vector2 NormalizedPositionToLocalPosition(this RectTransform rT, Vector2 normalizedPointPosition)
	{
		return rT.sizeDelta.Multiply(normalizedPointPosition - rT.pivot);
	}


	public static RectTransform GetRT(this GameObject g)
	{
		if (g == null)
			return null;
		if (!(g.transform is RectTransform))
			return null;
		return g.transform as RectTransform;
	}

	public static RectTransform GetRT(this Component g)
	{
		if (g == null)
			return null;
		if (!(g.transform is RectTransform))
			return null;
		return g.transform as RectTransform;
	}

	public static void SetDefaultScale(this RectTransform trans)
	{
		trans.localScale = new Vector3(1, 1, 1);
	}
	public static void SetPivotAndAnchors(this RectTransform trans, Vector2 aVec)
	{
		trans.pivot = aVec;
		trans.anchorMin = aVec;
		trans.anchorMax = aVec;
	}

	public static Vector2 GetSize(this RectTransform trans)
	{
		return trans.rect.size;
	}
	public static float GetWidth(this RectTransform trans)
	{
		return trans.rect.width;
	}
	public static float GetHeight(this RectTransform trans)
	{
		return trans.rect.height;
	}
	public static float GetAspect(this RectTransform trans)
	{
		return trans.GetWidth() / trans.GetHeight();
	}


	public static void SetPositionOfPivot(this RectTransform trans, Vector2 newPos)
	{
		trans.localPosition = new Vector3(newPos.x, newPos.y, trans.localPosition.z);
	}

	public static void SetLeftBottomPosition(this RectTransform trans, Vector2 newPos)
	{
		trans.localPosition = new Vector3(newPos.x + (trans.pivot.x * trans.rect.width), newPos.y + (trans.pivot.y * trans.rect.height), trans.localPosition.z);
	}
	public static void SetLeftTopPosition(this RectTransform trans, Vector2 newPos)
	{
		trans.localPosition = new Vector3(newPos.x + (trans.pivot.x * trans.rect.width), newPos.y - ((1f - trans.pivot.y) * trans.rect.height), trans.localPosition.z);
	}
	public static void SetRightBottomPosition(this RectTransform trans, Vector2 newPos)
	{
		trans.localPosition = new Vector3(newPos.x - ((1f - trans.pivot.x) * trans.rect.width), newPos.y + (trans.pivot.y * trans.rect.height), trans.localPosition.z);
	}
	public static void SetRightTopPosition(this RectTransform trans, Vector2 newPos)
	{
		trans.localPosition = new Vector3(newPos.x - ((1f - trans.pivot.x) * trans.rect.width), newPos.y - ((1f - trans.pivot.y) * trans.rect.height), trans.localPosition.z);
	}

	public static void SetSize(this RectTransform trans, Vector2 newSize)
	{
		Vector2 oldSize = trans.rect.size;
		Vector2 deltaSize = newSize - oldSize;
		trans.offsetMin = trans.offsetMin - new Vector2(deltaSize.x * trans.pivot.x, deltaSize.y * trans.pivot.y);
		trans.offsetMax = trans.offsetMax + new Vector2(deltaSize.x * (1f - trans.pivot.x), deltaSize.y * (1f - trans.pivot.y));
	}
	public static void SetWidth(this RectTransform trans, float newSize)
	{
		SetSize(trans, new Vector2(newSize, trans.rect.size.y));
	}
	public static void SetHeight(this RectTransform trans, float newSize)
	{
		SetSize(trans, new Vector2(trans.rect.size.x, newSize));
	}

	public static void FitIntoParent(this RectTransform rT)
	{
		rT.anchorMin = Vector2.zero;
		rT.anchorMax = Vector2.one;
		rT.offsetMin = Vector2.zero;
		rT.offsetMax = Vector2.zero;
	}
}
