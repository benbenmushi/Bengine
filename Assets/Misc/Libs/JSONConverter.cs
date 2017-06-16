using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

public static class Json
{
	private static bool IsNumericType(this object o)
	{
		switch (Type.GetTypeCode(o.GetType()))
		{
			case TypeCode.Byte:
			case TypeCode.SByte:
			case TypeCode.UInt16:
			case TypeCode.UInt32:
			case TypeCode.UInt64:
			case TypeCode.Int16:
			case TypeCode.Int32:
			case TypeCode.Int64:
			case TypeCode.Decimal:
			case TypeCode.Double:
			case TypeCode.Single:
				return true;
			default:
				return false;
		}
	}

	private static string EncodeNonAsciiCharacters(this string value)
	{
		StringBuilder sb = new StringBuilder();
		foreach (char c in value)
		{
			if (c > 127)
			{
				// This character is too big for ASCII
				string encodedValue = "\\u" + ((int)c).ToString("x4");
				sb.Append(encodedValue);
			}
			else
			{
				sb.Append(c);
			}
		}
		return sb.ToString();
	}

	private static string DecodeEncodedNonAsciiCharacters(this string value)
	{
		return Regex.Replace(
			value,
			@"\\u(?<Value>[a-zA-Z0-9]{4})",
			m =>
			{
				return ((char)int.Parse(m.Groups["Value"].Value, NumberStyles.HexNumber)).ToString();
			});
	}

	public static string Encode(object obj)
	{
		string valueStr;
		
		if (obj == null)
		{
			valueStr = "null";
		}
		else if (obj is bool)
		{
			valueStr = obj.ToString().ToLower();
		}
		else if (obj is Hashtable)
		{
			valueStr = EncodeTable(obj as Hashtable);
		}
		else if (obj is IList)
		{
			valueStr = EncodeArray(obj as IList);
		}
		else if (obj.IsNumericType())
		{
			valueStr = obj.ToString();
		}
		else
		{
			valueStr = "\"" + Escape(obj.ToString()) + "\"";
		}

		return valueStr;
	}

	private static string Escape(string str)
	{
		return str
			.Replace("\\", "\\\\")
			.Replace("\"", "\\\"")
			.Replace("\n", "\\n")
			.Replace("\r", "\\r")
			.EncodeNonAsciiCharacters();
	}

	private static string Unescape(string str)
	{
		return str
			.DecodeEncodedNonAsciiCharacters()
			.Replace("\\r", "\r")
			.Replace("\\n", "\n")
			.Replace("\\\"", "\"")
			.Replace("\\\\", "\\");
	}

	private static string EncodeArray(IList array)
	{
		if (array == null)
			return "null";

		List<string> parts = new List<string>();

		int i = 0;
		foreach (object obj in array)
		{
			parts.Add(Encode(obj));
			i++;
		}

		return "[" + string.Join(",", parts.ToArray()) + "]";
	}

	private static string EncodeTable(Hashtable data)
	{
		if (data == null)
			return "null";

		List<string> parts = new List<string>();
		foreach (DictionaryEntry entry in data)
		{
			if (!string.IsNullOrEmpty(entry.Key.ToString()))
			{
				parts.Add("\"" + entry.Key.ToString() + "\":" + Encode(entry.Value));
			}
		}

		return "{" + string.Join(",", parts.ToArray()) + "}";
	}

	public static object Decode(string str)
	{
		//GUILog.Add(str, Color.green);
		
		if (str == null || str == "null")
			return null;

		if (str == "")
			return "";

		if (str == "true")
			return true;

		if (str == "false")
			return false;
		
		str = str.Replace("\0", "").Trim();
		if (str.Length == 0)
			throw new FormatException("Empty value is not allowed");

		char firstChar = str[0];
		char lastChar = str[str.Length - 1];
		
		if (firstChar == '{' && lastChar == '}')
		{
			//GUILog.Add("Is table", Color.green);
			return DecodeTable(str);
		}

		if (firstChar == '[' && lastChar == ']')
		{
			//GUILog.Add("Is array", Color.green);				
			return DecodeArray(str);
		}

		if (firstChar == '"' && lastChar == '"')
		{
			//GUILog.Add("Is string", Color.green);
			return DecodeString(Unescape(str));
		}

		//GUILog.Add("Is number", Color.green);

		float tmpFloat;
		if (str.Contains(".") && float.TryParse(str, out tmpFloat))
			return tmpFloat;

		int tmpInt;
		if (int.TryParse(str, out tmpInt))
			return tmpInt;

		throw new FormatException("Can't parse json\n" + str + "[EOF]");
	}

	private static string DecodeString(string str)
	{
		str = str.Remove(0, 1);
		str = str.Remove(str.Length - 1);
		return str;
	}
	
	private static Hashtable DecodeTable(string str)
	{
		str = str.Remove(0, 1);
		str = str.Remove(str.Length - 1);

		Hashtable table = new Hashtable();

		string key = "";
		string value = "";

		Stack<bool> classArrayStack = new Stack<bool>();
		const bool ARRAY = false;
		const bool CLASS = true;

		bool inString = false;
		bool waitKey = true;
		bool commaNeeded = false;
		bool colonsNeeded = false;
		bool canBeNumber = true;
		bool hasDigits = false;

		for (int i = 0; i < str.Length; i++)
		{
			if (inString)
			{
				if (str[i] == '\\')
				{
					if (waitKey)
						key += '\\';
					else
						value += '\\';

					i++;

					if (waitKey)
						key += str[i];
					else
						value += str[i];
				}
				else
				{
					if (str[i] == '"')
					{
						inString = false;
						if (waitKey)
							colonsNeeded = true;
						else
							commaNeeded = true;

						if (!waitKey)
							value += '"';
					}
					else
					{
						if (waitKey)
							key += str[i];
						else
							value += str[i];
					}
				}
			}
			else if (classArrayStack.Count > 0)
			{
				bool currentToClose = classArrayStack.Peek();

				switch (str[i])
				{					
					case '[':
						classArrayStack.Push(ARRAY);
						break;
					case ']':
						if (currentToClose != ARRAY)
							throw new FormatException("Expected '[' before ']' (c:" + i + ")\n" + str.Remove(i));
						classArrayStack.Pop();
						break;
					case '{':
						classArrayStack.Push(CLASS);
						break;
					case '}':
						if (currentToClose != CLASS)
							throw new FormatException("Expected '[' before ']' (c:" + i + ")\n" + str.Remove(i));
						classArrayStack.Pop();
						break;	
				}

				value += str[i];

				if (classArrayStack.Count == 0)
				{
					commaNeeded = true;
				}
			}
			else
			{
				switch (str[i])
				{
					case '{':						
						if (waitKey)
							throw new FormatException("Expected '\"' before '{' (c:" + i + ")\n" + str.Remove(i));

						if (commaNeeded)
							throw new FormatException("Expected ',' before '{' (c:" + i + ")\n" + str.Remove(i));

						value = "{";
						canBeNumber = false;
						classArrayStack.Push(CLASS);
						break;		
	
					case ':':						
						if (commaNeeded)
							throw new FormatException("Expected ',' before ':' (c:" + i + ")\n" + str.Remove(i));

						colonsNeeded = false;

						if (key.Length > 0 && waitKey)
							waitKey = false;
						else
							throw new FormatException("Unexpected ':' at c:" + i + ")\n" + str.Remove(i));
						
						break;

					case '[':						
						if (commaNeeded)
							throw new FormatException("Expected ',' before '[' (c:" + i + ")\n" + str.Remove(i));

						if (waitKey)
							throw new FormatException("Expected '\"' before '[' (c:" + i + ")\n" + str.Remove(i));

						canBeNumber = false;
						value = "[";
						classArrayStack.Push(ARRAY);
						break;

					case ',':
						if (waitKey)
							throw new FormatException("Expected ':' instead of ',' (c:" + i + ")\n" + str.Remove(i));

						if (!commaNeeded && !hasDigits)
							throw new FormatException("Unexpected ',' (c:" + i + ")\n" + str.Remove(i));

						//GUILog.Add(key, Color.blue);
						//GUILog.Add(value, Color.cyan);
												
						table.Add(key, Decode(value));
						
						key = "";
						value = "";
						canBeNumber = true;
						commaNeeded = false;
						waitKey = true;
						hasDigits = false;
						break;

					case '"':
						if(colonsNeeded)
							throw new FormatException("Expected ':' before '\"' (c:" + i + ")\n" + str.Remove(i));

						if (commaNeeded)
							throw new FormatException("Expected ',' before '\"' (c:" + i + ")\n" + str.Remove(i));

						inString = true;
						
						if(!waitKey)
						{
							value = "\"";
							canBeNumber = false;
						}

						break;
					default:
						if (!char.IsWhiteSpace(str[i]))
						{
							if (!commaNeeded && !waitKey && canBeNumber && (char.IsDigit(str[i]) || str[i] == '.' || str[i] == '-'))
							{
								hasDigits = true;
								value += str[i];
							}
							else
							{
								if (str.Length >= (i + 4) && str.Substring(i, 4).Equals("null"))
								{
									if (waitKey)
										throw new FormatException("Key can't be null");

									value = "null";
									commaNeeded = true;
									i += 3;
								}
								else if (str.Length >= (i + 4) && str.Substring(i, 4).Equals("true"))
								{
									if (waitKey)
										throw new FormatException("Key can't be true");

									value = "true";
									commaNeeded = true;
									i += 3;
								}
								else if (str.Length >= (i + 5) && str.Substring(i, 5).Equals("false"))
								{
									if (waitKey)
										throw new FormatException("Key can't be false");

									value = "false";
									commaNeeded = true;
									i += 4;
								}
								else
								{
									//GUILog.Add(str[i] + " is digit = " + char.IsDigit(str[i]), Color.red);
									throw new FormatException("Unexpected '" + str[i] + "' (c:" + i + ")\n" + str.Remove(i));
								}
							}
						}
						break;
				}
			}
		}

		if (classArrayStack.Count > 0)
		{
			if (classArrayStack.Peek() == CLASS)
				throw new FormatException("Expected '}'");
			else
				throw new FormatException("Expected ']'");
		}

		if (commaNeeded || hasDigits)
		{
			//GUILog.Add(key, Color.blue);
			//GUILog.Add(value, Color.cyan);

			table.Add(key, Decode(value));
		}

		return table;
	}

	private static IList DecodeArray(string str)
	{
		str = str.Remove(0, 1);
		str = str.Remove(str.Length - 1);

		List<object> array = new List<object>();

		string value = "";

		Stack<bool> classArrayStack = new Stack<bool>();
		const bool ARRAY = false;
		const bool CLASS = true;

		bool inString = false;
		bool commaNeeded = false;
		bool canBeNumber = true;
		bool hasDigits = false;

		for (int i = 0; i < str.Length; i++)
		{
			if (inString)
			{
				if (str[i] == '\\')
				{
					value += '\\';

					i++;

					value += str[i];
				}
				else
				{
					if (str[i] == '"')
					{
						inString = false;
						commaNeeded = true;
						value += '"';
					}
					else
					{
						value += str[i];
					}
				}
			}
			else if (classArrayStack.Count > 0)
			{
				bool currentToClose = classArrayStack.Peek();

				switch (str[i])
				{
					case '[':
						classArrayStack.Push(ARRAY);
						break;
					case ']':
						if (currentToClose != ARRAY)
							throw new FormatException("Expected '[' before ']' (c:" + i + ")\n" + str.Remove(i));
						classArrayStack.Pop();
						break;
					case '{':
						classArrayStack.Push(CLASS);
						break;
					case '}':
						if (currentToClose != CLASS)
							throw new FormatException("Expected '[' before ']' (c:" + i + ")\n" + str.Remove(i));
						classArrayStack.Pop();
						break;
				}

				value += str[i];

				if (classArrayStack.Count == 0)
				{
					commaNeeded = true;
				}
			}
			else
			{
				switch (str[i])
				{
					case '{':						
						if (commaNeeded)
							throw new FormatException("Expected ',' before '{' (c:" + i + ")\n" + str.Remove(i));

						value = "{";
						canBeNumber = false;
						classArrayStack.Push(CLASS);
						break;
				
					case '[':
						if (commaNeeded)
							throw new FormatException("Expected ',' before '[' (c:" + i + ")\n" + str.Remove(i));
	
						canBeNumber = false;
						value = "[";
						classArrayStack.Push(ARRAY);
						break;

					case ',':
						
						if (!commaNeeded && !hasDigits)
							throw new FormatException("Unexpected ',' (c:" + i + ")\n" + str.Remove(i));

					//	GUILog.Add(value, Color.cyan);

						array.Add(Decode(value));
						value = "";
						canBeNumber = true;
						commaNeeded = false;
						hasDigits = false;
						break;

					case '"':						
						if (commaNeeded)
							throw new FormatException("Expected ',' before '\"' (c:" + i + ")\n" + str.Remove(i));

						inString = true;						
						value = "\"";
						canBeNumber = false;						

						break;
					default:
						if (!char.IsWhiteSpace(str[i]))
						{
							if (!commaNeeded && canBeNumber && (char.IsDigit(str[i]) || str[i] == '.' || str[i] == '-'))
							{
								hasDigits = true;
								value += str[i];
							}
							else
							{
								if (str.Length >= (i + 4) && str.Substring(i, 4).Equals("null"))
								{
									value = "null";
									commaNeeded = true;
									i += 3;
								}
								else if (str.Length >= (i + 4) && str.Substring(i, 4).Equals("true"))
								{
									value = "true";
									commaNeeded = true;
									i += 3;
								}
								else if (str.Length >= (i + 5) && str.Substring(i, 5).Equals("false"))
								{
									value = "false";
									commaNeeded = true;
									i += 4;
								}
								else
								{
									//GUILog.Add(str[i] + " is digit = " + char.IsDigit(str[i]), Color.red);
									throw new FormatException("Unexpected '" + str[i] + "' (c:" + i + ")\n" + str.Remove(i));
								}
							}
						}
						break;
				}
			}
		}

		if (classArrayStack.Count > 0)
		{
			if (classArrayStack.Peek() == CLASS)
				throw new FormatException("Expected '}'");
			else
				throw new FormatException("Expected ']'");
		}

		if (commaNeeded || hasDigits)
		{
			//GUILog.Add(value, Color.cyan);
			array.Add(Decode(value));
		}

		return array;
	}
}