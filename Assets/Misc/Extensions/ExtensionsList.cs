using System.Collections.Generic;

public static class ExtensionsList
{
	public static T GetLast<T>(this List<T> list)
	{
		return list[list.Count - 1];
	}

	public static int GetElementIndex<T>(this List<T> list, T element)
	{
		for (int i = 0; i < list.Count; i++)
		{
			if (list[i].Equals(element))
				return i;
		}
		throw new System.Collections.Generic.KeyNotFoundException();
	}

	/// <summary>
	/// Shuffles elements in given list
	/// </summary>
	/// <typeparam name="T">Type of elements to shuffle</typeparam>
	/// <param name="list">list to shuffle</param>
	public static void Shuffle<T>(this List<T> list)
	{
		int n = list.Count;
		while (n > 1)
		{
			n--;
			int k = UnityEngine.Random.Range(0, n + 1);
			T value = list[k];
			list[k] = list[n];
			list[n] = value;
		}
	}
	public static List<T> Swap<T>(this List<T> list, int indexA, int indexB)
	{
		T tmp = list[indexA];
		list[indexA] = list[indexB];
		list[indexB] = tmp;
		return list;
	}
}
