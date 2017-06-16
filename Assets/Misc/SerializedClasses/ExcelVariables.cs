using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ExcelVariables : IDictionary
{
	[SerializeField]
	private string[] keys = new string[0];
	[SerializeField]
	private string[] values = new string[0];


	public ExcelVariables()
	{

	}
	public ExcelVariables(IDictionary<string, string> dic)
	{
		IEnumerator<string> it = dic.Keys.GetEnumerator();

		while (it.MoveNext())
			Add(it.Current, dic[it.Current]);
	}
	public ExcelVariables(IDictionary dic)
	{
		IEnumerator it = dic.Keys.GetEnumerator();

		while (it.MoveNext())
			Add(it.Current, dic[it.Current]);
	}

	public object this[object key]
	{
		get
		{
			if (key is string)
			{
				string keyString = key.ToString();

				for (int i = 0; i < keys.Length; i++)
					if (keys[i] == keyString)
						return values[i];
			}
			return null;
		}

		set
		{
			if (key is string && value is string)
			{
				string keyString = key.ToString();

				for (int i = 0; i < keys.Length; i++)
					if (keys[i] == keyString)
					{
						values[i] = value.ToString();
						return;
					}
				__Add(key, value);
			}
		}
	}

	public int Count
	{
		get
		{
			return keys.Length;
		}
	}

	public bool IsFixedSize
	{
		get
		{
			throw new NotImplementedException();
		}
	}

	public bool IsReadOnly
	{
		get
		{
			throw new NotImplementedException();
		}
	}

	public bool IsSynchronized
	{
		get
		{
			throw new NotImplementedException();
		}
	}

	public ICollection Keys
	{
		get
		{
			object[] tmp_keys = new object[values.Length];
			for (Int32 n = 0; n < tmp_keys.Length; n++)
				tmp_keys[n] = keys[n];
			return tmp_keys;
		}
	}

	public object SyncRoot
	{
		get
		{
			throw new NotImplementedException();
		}
	}

	public ICollection Values
	{
		get
		{
			object[] tmp_values = new object[values.Length];
			for (Int32 n = 0; n < tmp_values.Length; n++)
				tmp_values[n] = values[n];
			return tmp_values;
		}
	}

	public void __Add(object key, object value)
	{
		if (!Contains(key))
		{
			Array.Resize(ref keys, keys.Length + 1);
			Array.Resize(ref values, values.Length + 1);
			keys[keys.Length - 1] = key.ToString();
			values[values.Length - 1] = value.ToString();
		}
		else
			throw new ArgumentException("Key duplication when adding: " + key);
	}
	public void Add(object key, object value)
	{
		if (key is string && value is string)
			__Add(key, value);
	}
	public void Add(string key, string value)
	{
		if (key is string && value is string)
			__Add(key, value);
	}

	public void Clear()
	{
		keys = new string[0];
		values = new string[0];
	}

	public bool Contains(object key)
	{
		if (key is string)
		{
			string keyString = key.ToString();

			for (int i = 0; i < keys.Length; i++)
				if (keys[i] == keyString)
					return true;
		}
		return false;
	}

	public void CopyTo(Array array, int index)
	{
		throw new NotImplementedException();
	}
	private class SimpleDictionaryEnumerator : IDictionaryEnumerator
	{
		// A copy of the SimpleDictionary object's key/value pairs.
		DictionaryEntry[] items;
		Int32 index = -1;

		public SimpleDictionaryEnumerator(ExcelVariables sd)
		{
			// Make a copy of the dictionary entries currently in the SimpleDictionary object.
			items = new DictionaryEntry[sd.Count];

			for (int i = 0; i < items.Length; i++)
				items[i] = new DictionaryEntry(sd.keys[i], sd.values[i]);
		}

		// Return the current item.
		public object Current { get { ValidateIndex(); return items[index]; } }

		// Return the current dictionary entry.
		public DictionaryEntry Entry
		{
			get { return (DictionaryEntry)Current; }
		}

		// Return the key of the current item.
		public object Key { get { ValidateIndex(); return items[index].Key; } }

		// Return the value of the current item.
		public object Value { get { ValidateIndex(); return items[index].Value; } }

		// Advance to the next item.
		public Boolean MoveNext()
		{
			if (index < items.Length - 1) { index++; return true; }
			return false;
		}

		// Validate the enumeration index and throw an exception if the index is out of range.
		private void ValidateIndex()
		{
			if (index < 0 || index >= items.Length)
				throw new InvalidOperationException("Enumerator is before or after the collection.");
		}

		// Reset the index to restart the enumeration.
		public void Reset()
		{
			index = -1;
		}
	}
	public IDictionaryEnumerator GetEnumerator()
	{
		return new SimpleDictionaryEnumerator(this);
	}

	public void Remove(object key)
	{
		if (key is string)
		{
			string keyString = key.ToString();

			for (int i = 0; i < keys.Length; i++)
				if (keys[i] == keyString)
				{
					keys = ExtensionsArray.RemoveAt(keys, i);
					values = ExtensionsArray.RemoveAt(values, i);
					return;
				}
		}
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		throw new NotImplementedException();
	}
}
