using UnityEngine;
using System.Collections;

public static class ExtensionsFloat
{
	public static bool EqualWithGap(this float val, float compared, float gap)
	{
		return val + gap > compared && val - gap < compared;
	}
	public static float Diff(this float val, float val2)
	{
		if (val < 0 && val2 > 0)
			return Mathf.Abs(-val + val2);
		else if (val > 0 && val2 < 0)
			return Mathf.Abs(-val2 + val);
		return Mathf.Abs(val - val2);
	}
	public static int Diff(this int val, int val2)
	{
		if (val < 0 && val2 > 0)
			return Mathf.Abs(-val + val2);
		else if (val > 0 && val2 < 0)
			return Mathf.Abs(-val2 + val);
		return Mathf.Abs(val - val2);
	}
	public static float Snap(this float val, float snap)
	{
		if (snap > 0)
		{
			float f = Mathf.Floor(val);
			float k = val - f;

			return f + Mathf.RoundToInt(k * 1 / snap) * snap;
		}
		Debug.LogError("Incorrect usage of snap with negativ or null value");
		return val;
	}
}
