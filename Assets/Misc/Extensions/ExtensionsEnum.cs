using System;

public static class ExtensionsEnum
{
	public static bool Contains(this Enum val, Enum val2)
	{
		return (Convert.ToInt32(val) & Convert.ToInt32(val2)) != 0;
	}
	public static bool Contains(this ExtensionsTransform.TransformPosition val, ExtensionsTransform.TransformPosition val2)
	{
		return (val & val2) != 0;
	}

	public static T Add<T>(this T me, Enum toAdd) where T : struct
	{
		return (T)(object)(Convert.ToInt32(me) | Convert.ToInt32(toAdd));
	}
	public static T Toggle<T>(this T me, Enum toAdd) where T : struct
	{
		return (T)(object)(Convert.ToInt32(me) ^ Convert.ToInt32(toAdd));
	}
	public static T Remove<T>(this T me, Enum toAdd) where T : struct
	{
		return (T)(object)(Convert.ToInt32(me) & ~Convert.ToInt32(toAdd));
	}
}
