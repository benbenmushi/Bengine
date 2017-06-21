using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Remoting.Messaging;

public static class ExtensionsObject
{
	public static byte[] SerializeToBytes(this object obj)
	{
		if (obj == null)
			return null;

		BinaryFormatter bf = new BinaryFormatter();
		MemoryStream ms = new MemoryStream();
		bf.Serialize(ms, obj);
		return ms.ToArray();
	}
	public static T DeserializeBytes<T>(this object arrBytes)
	{
		if (arrBytes.GetType() == typeof(byte[]))
		{
			byte[] bytes = (byte[])arrBytes;
			MemoryStream memStream = new MemoryStream();
			BinaryFormatter binForm = new BinaryFormatter();
			memStream.Write(bytes, 0, bytes.Length);
			memStream.Seek(0, SeekOrigin.Begin);
			return (T)binForm.Deserialize(memStream);
		}
		return default(T);
	}
}
