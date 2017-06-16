using UnityEngine;

public static class ExtensionsColor
{
	public static Color R(this Color c, float red)
	{
		c.r = red;
		return c;
	}
	public static Color G(this Color c, float green)
	{
		c.g = green;
		return c;
	}
	public static Color B(this Color c, float blue)
	{
		c.b = blue;
		return c;
	}
	public static Color A(this Color c, float alpha)
	{
		c.a = alpha;
		return c;
	}
}
