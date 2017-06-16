using System;

public static class ExtensionsAttributes
{
	public static T GetAttribute<T>(this Type t) where T : Attribute
	{
		if (t.IsDefined(typeof(T), true))
		{
			foreach (object o in t.GetCustomAttributes(true))
			{
				if (o is T)
					return o as T;
			}
		}

		return null;
	}
}
