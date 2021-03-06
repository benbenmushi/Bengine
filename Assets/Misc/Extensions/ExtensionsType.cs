﻿public static class ExtensionsType
{
	/// <summary>
	/// Returns the 'actual' generic type of a 'unprecised' generic type in legacy hierarchy
	/// </summary>
	/// <example>
	/// We have a SuperModule that extends Module&lt;SuperModule&gt;
	/// To retrieve Module&lt;SuperModule&gt, we can call typeof(SuperModule).GetActualGeneric(typeof(Module<>));
	/// </example>
	/// <param name="toCheck">Type we want the 'actual' generic base-type</param>
	/// <param name="generic">'unprecised' generic type</param>
	/// <returns>Found type. Null if not found</returns>
	public static System.Type GetActualGeneric(this System.Type toCheck, System.Type generic)
	{
		while (toCheck != null && toCheck != typeof(object))
		{
			var cur = toCheck.IsGenericType ? toCheck.GetGenericTypeDefinition() : toCheck;
			if (generic == cur)
			{
				return toCheck;
			}
			toCheck = toCheck.BaseType;
		}
		return null;
	}

	/// <summary>
	/// Returns true if the type if a subclass of the given 'unprecised' generic type.
	/// </summary>
	/// <example>
	/// We have a SuperModule that extends Module&lt;SuperModule&gt;
	/// typeof(SuperModule).IsSubclassOfRawGeneric(typeof(Module<>)) returns true.
	/// </example>
	/// <param name="toCheck">Type we want to check</param>
	/// <param name="generic">'unprecised' generic type</param>
	/// <returns>True if a basetype matches the generic type</returns>
	public static bool IsSubclassOfRawGeneric(this System.Type toCheck, System.Type generic)
	{
		return GetActualGeneric(toCheck, generic) != null;
	}

}
