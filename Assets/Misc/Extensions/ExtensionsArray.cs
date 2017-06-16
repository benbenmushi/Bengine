using UnityEngine;
using System.Collections.Generic;

public static class ExtensionsArray
{
	// List

	public static int GetIndexOf<T>(this IList<T> list, T element, bool throwError = false)
	{
		int index = 0;

		foreach (T e in list)
		{
			if (e.Equals(element))
				return index;
			index++;
		}
		if (throwError)
			throw new UnityException("Element not found in list");
		return -1;
	}

	// Array
	public static void Add<T>(ref T[] array, T element)
	{
		System.Array.Resize(ref array, array.Length + 1);
		array[array.Length - 1] = element;
	}
	public static void AddMultiple<T>(ref T[] array, T element, int addCount)
	{
		System.Array.Resize(ref array, array.Length + addCount);

		for (int i = array.Length - addCount; i < array.Length; i++)
			array[i] = element;
	}
	public static T[] AddRange<T>(T[] array, IEnumerable<T> arrayRange)
	{
		List<T> newList = new List<T>();

		if (array != null)
			newList.AddRange(array);

		newList.AddRange(arrayRange);
		return newList.ToArray();
	}
	public static bool Contains<T>(T[] array, T element)
	{
		for (int i = 0; i < array.Length; i++)
			if (array[i].Equals(element))
				return true;
		return false;
	}
	public static T[] Remove<T>(T[] array, T item)
	{
		List<T> newList = new List<T>();

		if (array != null)
			newList.AddRange(array);

		newList.Remove(item);
		return newList.ToArray();
	}
	public static T[] RemoveAt<T>(T[] array, int index)
	{
		List<T> newList = new List<T>();

		if (array != null)
			newList.AddRange(array);

		newList.RemoveAt(index);
		return newList.ToArray();
	}
	public static T[] RemoveAll<T>(T[] array, System.Predicate<T> predicate)
	{
		List<T> newList = new List<T>();

		if (array != null)
			newList.AddRange(array);

		newList.RemoveAll(predicate);
		return newList.ToArray();
	}
	public static T[] Sort<T>(T[] array, System.Comparison<T> compare)
	{
		List<T> newList = new List<T>();

		if (array != null)
			newList.AddRange(array);

		newList.Sort(compare);
		return newList.ToArray();
	}
	public static void Unique<T>(ref T[] array)
	{
		array = Unique(array);
	}
	public static T[] Unique<T>(T[] array)
	{
		HashSet<T> hash = new HashSet<T>();
		List<T> list = new List<T>();

		for (int i = 0; i < array.Length; i++)
			if (hash.Add(array[i]))
				list.Add(array[i]);
		return list.ToArray();
	}
	public static T[] InsertAfter<T>(T[] array, int index, T o)
	{
		List<T> newList = new List<T>();

		if (array != null)
			newList.AddRange(array);

		if (index + 1 >= newList.Count)
			newList.Add(o);
		else
			newList.Insert(index + 1, o);
		return newList.ToArray();
	}
	public static T Find<T>(T[] array, System.Predicate<T> predicate)
	{
		return (new List<T>(array)).Find(predicate);
	}
	public static void Resize<T>(ref T[] array, int size)
	{
		if (array.Length != size)
		{
			while (array.Length > size)
				array = RemoveAt(array, array.Length - 1);
			AddMultiple(ref array, default(T), size - array.Length);
		}
	}
}
