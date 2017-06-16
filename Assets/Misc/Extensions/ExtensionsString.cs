using System.Globalization;
using System.Text;
using System.Linq;
using UnityEngine;
using System.Collections.Generic;

public static class ExtensionsString
{
	public static string RemoveAccents(this string str)
	{
		if (string.IsNullOrEmpty(str))
			return str;

		str = str.Normalize(NormalizationForm.FormD);
		var chars = str.Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark).ToArray();
		return new string(chars).Normalize(NormalizationForm.FormC);
	}

	public static string RemovePunctuation(this string str)
	{
		if (string.IsNullOrEmpty(str))
			return str;

		str = str.Replace('!', ' ');
		str = str.Replace('?', ' ');

		string toReplace = str.Replace("  ", " ");
		while (str != toReplace)
		{
			str = toReplace;
			toReplace = str.Replace("  ", " ");
		}

		str = str.Trim();

		return str;
	}
	public static int GetLetterCount(this string str)
	{
		int lenght = str.Length;

		foreach (char c in str)
		{
			if (char.IsPunctuation(c) || c == ' ')
				lenght--;
		}
		return lenght;
	}
	public static int GetPunctuationAndSpaceCount(this string str)
	{
		return str.Length - str.GetLetterCount();
	}
	public static bool ContainsIgnoreCase(this string str, string target)
	{
		return CultureInfo.CurrentCulture.CompareInfo.IndexOf(str, target, CompareOptions.IgnoreCase) >= 0;
	}
	public static bool ContainsIgnoreCase(this string str, IEnumerable<string> collection)
	{
		return collection.FirstOrDefault(s => str.ContainsIgnoreCase(s)) != null;
	}
}

