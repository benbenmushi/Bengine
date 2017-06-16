using UnityEngine;

public static class ExtensionsVector2
{
	public static Vector2 Multiply(this Vector2 v, Vector2 v2)
	{
		return new Vector2(v.x * v2.x, v.y * v2.y);
	}
	public static Vector2 Multiply(this Vector2 v, float x, float y)
	{
		return new Vector2(v.x * x, v.y * y);
	}
	public static Vector2 Divide(this Vector2 v, Vector2 v2)
	{
		return new Vector2(v.x / v2.x, v.y / v2.y);
	}
	public static bool GreaterOrEqual(this Vector2 v, Vector2 other)
	{
		return v.x >= other.x && v.y >= other.y;
	}
	public static bool LowerOrEqual(this Vector2 v, Vector2 other)
	{
		return v.x <= other.x && v.y <= other.y;
	}
	public static Vector3 AsVector3(this Vector2 v)
	{
		return new Vector3(v.x, v.y, 0);
	}
}
