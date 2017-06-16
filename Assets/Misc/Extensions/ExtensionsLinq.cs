using System.Collections.Generic;

namespace System.Linq
{
	public static class ExtensionsLinq
	{
		public static string AggregateAuto<T>(this IEnumerable<T> source, string between = " ")
		{
			return source.Select(s => s.ToString()).DefaultIfEmpty().Aggregate((s1, s2) => s1 + between + s2);
		}
		public static bool AnyIgnoreCase(this IEnumerable<string> source, string target)
		{
			return source.Any(s => s.Equals(target, StringComparison.CurrentCultureIgnoreCase));
		}
		public static bool ContainsIgnoreCase(this IEnumerable<string> source, string target)
		{
			return source.FirstOrDefault(s => s.Equals(target, StringComparison.CurrentCultureIgnoreCase)) != null;
		}
		public static TSource BestStringMatch<TSource>(this IEnumerable<TSource> source, Func<TSource, string> selector, string matchingVal)
		{
			IEnumerable<TSource> tmp = source.Where(s => selector(s).Contains(matchingVal));

			if (tmp.Count() > 0)
				return tmp.OrderBy(s => selector(s).Length).First();
			return default(TSource);
		}
		public static int FindIndex<T>(this IEnumerable<T> items, Func<T, bool> predicate)
		{
			if (items == null) throw new ArgumentNullException("items");
			if (predicate == null) throw new ArgumentNullException("predicate");

			int retVal = 0;
			foreach (var item in items)
			{
				if (predicate(item)) return retVal;
				retVal++;
			}
			return -1;
		}
	}
}
