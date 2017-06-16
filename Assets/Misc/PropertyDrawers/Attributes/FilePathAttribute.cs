using UnityEngine;
using System.Collections;
using System;

public class FilePathAttribute : PropertyAttribute
{
	public string fileExtention;
	public string fileChangedCallback;

	/// <summary>
	/// 
	/// </summary>
	/// <param name="_fileExtention">desired file extention (ex: txt, csv, jpg)</param>
	/// <param name="_fileChangedCallback">callback called when a new file is selected in your editor</param>
	public FilePathAttribute(string _fileExtention, string _fileChangedCallback = null)
	{
		fileExtention = _fileExtention;
		fileChangedCallback = _fileChangedCallback;
	}
}
