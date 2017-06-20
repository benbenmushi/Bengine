using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Reflection;
using System;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[ExecuteInEditMode]
public class UISizeControllerGeneric : MonoBehaviourEx
#if UNITY_EDITOR
	, ISerializationCallbackReceiver
#endif
{
	public int                  searchDepth = 2;
	[SerializeField]
	MonoBehaviour                   m_target;
	public MonoBehaviour target
	{
		get { return m_target; }
		set
		{
			if (m_target != value)
			{
				fieldTargetIndex = -1;
#if UNITY_EDITOR
				m_possibleFieldsContent = null;
#endif
				m_possibleFields = null;
				m_target = value;
			}
		}
	}
	public int                  fieldTargetIndex = -1;
	public UISizeController     SizeController = new UISizeController();


	private RectTransform m_rectTransform;
	private RectTransform rectTransform
	{
		get
		{
			if (m_rectTransform == null)
				m_rectTransform = GetComponent<RectTransform>();
			return m_rectTransform;
		}
	}

	public void Refresh()
	{
		_internal_Refresh();
	}
	private void _internal_Refresh()
	{
		if (target != null && SizeController.apply)
		{
			SizeController.UpdateRefSize(rectTransform);
			SetTargetFieldValue(SizeController.GetSize());
			target.Invoke("OnEnable", 0);
		}
	}

	void Start()
	{
		if (Application.isPlaying)
		{
			SizeController.UpdateRefSize(rectTransform);
			SizeController.SizeChanged += (size) =>
			{
				SetTargetFieldValue(size);
				target.Invoke("OnEnable", 0);
			};
		}
	}


	[SerializeField]
	MonoBehaviour   m_lastTarget;
	[SerializeField]
	List<MemberInfo>[] m_possibleFields;
	public List<MemberInfo>[] possibleFields
	{
		get
		{
			if (m_possibleFields == null || (m_target != null && m_lastTarget != m_target))
			{
				m_possibleFields = FilterPossibleTypes(_GetPossibleFields(target.GetType()));
				m_lastTarget = m_target;
			}
			return m_possibleFields;
		}
	}
	public void SetPossibleFieldsDirty()
	{
		m_possibleFields = null;
	}

	public void SetTargetFieldValue(float val)
	{
		if (m_target != null && fieldTargetIndex != -1)
		{
			List<MemberInfo> fieldPath = possibleFields[fieldTargetIndex];
			MemberInfo memberParent = null;
			MemberInfo member = null;
			object target = m_target;
			object targetParent = null;

			for (int i = 0; i < fieldPath.Count - 1 && target != null; i++)
			{
				memberParent = member;
				member = fieldPath[i];
				targetParent = target;
				target = GetValue(member, target);
			}
			memberParent = member;
			member = fieldPath[fieldPath.Count - 1];
			if (targetParent == null)
				targetParent = m_lastTarget;
			if (target != null)
			{
				//Debug.Log(val + " == " + GetValue(member, target));
				SetValue(member, target, val);
				if (memberParent != null && MemberInfoIsStruct(memberParent))
					SetValue(memberParent, targetParent, target);
			}
		}
	}
	object GetValue(MemberInfo memberInfo, object target)
	{
		switch (memberInfo.MemberType)
		{
			case MemberTypes.Field:
				return (memberInfo as FieldInfo).GetValue(target);
			case MemberTypes.Property:
				return (memberInfo as PropertyInfo).GetGetMethod(true).Invoke(target, null);
			default:
				throw new Exception("Error");
		}
	}
	bool MemberInfoIsStruct(MemberInfo memberInfo)
	{
		switch (memberInfo.MemberType)
		{
			case MemberTypes.Field:
				return !(memberInfo as FieldInfo).FieldType.IsClass;
			case MemberTypes.Property:
				return !(memberInfo as PropertyInfo).PropertyType.IsClass;
			default:
				return false;
		}
	}
	void SetValue(MemberInfo memberInfo, object target, float value)
	{
		switch (memberInfo.MemberType)
		{
			case MemberTypes.Field:
				FieldInfo f = (memberInfo as FieldInfo);

				if (f.FieldType == typeof(int))
					f.SetValue(target, Mathf.RoundToInt(value));
				else if (f.FieldType == typeof(double))
					f.SetValue(target, (double)value);
				else if (f.FieldType == typeof(float))
					f.SetValue(target, value);
				else
					Debug.LogError(f.FieldType.Name + " is not a valid type for asignation from float.");
				break;
			case MemberTypes.Property:
				PropertyInfo p = (memberInfo as PropertyInfo);

				if (p.PropertyType == typeof(int))
					p.GetSetMethod(true).Invoke(target, new object[] { Mathf.RoundToInt(value) });
				else if (p.PropertyType == typeof(double))
					p.GetSetMethod(true).Invoke(target, new object[] { (double)value });
				else if (p.PropertyType == typeof(float))
					p.GetSetMethod(true).Invoke(target, new object[] { value });
				else
					Debug.LogError(p.PropertyType.Name + " is not a valid type for asignation from float.");
				break;
			default:
				throw new Exception("Error");
		}
	}
	void SetValue(MemberInfo memberInfo, object target, object value)
	{
		switch (memberInfo.MemberType)
		{
			case MemberTypes.Field:
				(memberInfo as FieldInfo).SetValue(target, value);
				break;
			case MemberTypes.Property:
				(memberInfo as PropertyInfo).GetSetMethod(true).Invoke(target, new object[] { value });
				break;
			default:
				throw new Exception("Error");
		}
	}

#if UNITY_EDITOR
	void Update()
	{
		if (!Application.isPlaying)
			Refresh();
	}
	GUIContent[] m_possibleFieldsContent;
	public GUIContent[] possibleFieldsContent
	{
		get
		{
			if (m_possibleFieldsContent == null)
				m_possibleFieldsContent = possibleFields.Select(f => new GUIContent(f.Select(m => GetMemberInfoName(m)).AggregateAuto("."))).ToArray();
			return m_possibleFieldsContent;
		}
	}
	public string GetMemberInfoName(MemberInfo m)
	{
		switch (m.MemberType)
		{
			case MemberTypes.Field:
				return m.Name + "(" + (m as FieldInfo).FieldType.Name + ")";
			case MemberTypes.Property:
				return m.Name + "(" + (m as PropertyInfo).PropertyType.Name + ")";
			default:
				return "error";
		}
	}

	void ISerializationCallbackReceiver.OnBeforeSerialize()
	{
	}
	void ISerializationCallbackReceiver.OnAfterDeserialize()
	{
		m_possibleFieldsContent = possibleFields.Select(f => new GUIContent(f.Select(m => GetMemberInfoName(m)).AggregateAuto("."))).ToArray();
	}

	[ButtonField("_ForceRefresh")]
	public bool             ForceRefresh = false;
	void _ForceRefresh()
	{
		SizeController.UpdateRefSize(rectTransform, true);
	}
	void OnValidate()
	{
		if (!Application.isPlaying)
			SizeController.UpdateRefSize(rectTransform);
	}
#endif
	List<MemberInfo>[] _GetPossibleFields(Type type, int depth = 0)
	{
		if (depth++ > searchDepth)
			return new List<MemberInfo>[0];
		FieldInfo[] fields = type.GetFields(flags);
		PropertyInfo[] properties = type.GetProperties(flags);
		List<List<MemberInfo>> fieldNames = new List<List<MemberInfo>>();

		for (int i = 0; i < fields.Length; i++)
		{
			FieldInfo f = fields[i];

			if ((f.FieldType == typeof(float) || f.FieldType == typeof(double) || f.FieldType == typeof(int)))
				fieldNames.Add(new List<MemberInfo>(new MemberInfo[] { f }));
			else if (!excludedTypes.Contains(f.DeclaringType) &&
					 !excludedTypes.Contains(f.FieldType) &&
					 !f.FieldType.FullName.Contains("System.") &&
					 !f.FieldType.IsEnum)
			{
				List<MemberInfo>[] Children = _GetPossibleFields(f.FieldType, depth);
				for (int j = 0; j < Children.Length; j++)
				{
					if (Children[j].Count > 0 && !Children[j].Contains(f.FieldType))
					{
						List<MemberInfo> currentList = new List<MemberInfo>(new MemberInfo[] { f });

						currentList.AddRange(Children[j]);
						fieldNames.Add(currentList);
					}
				}
			}
		}
		for (int i = 0; i < properties.Length; i++)
		{
			PropertyInfo p = properties[i];

			if ((p.PropertyType == typeof(float) || p.PropertyType == typeof(double) || p.PropertyType == typeof(int)) &&
				p.GetSetMethod(true) != null)
				fieldNames.Add(new List<MemberInfo>(new MemberInfo[] { p }));
			else if (!excludedTypes.Contains(p.DeclaringType) &&
					 !excludedTypes.Contains(p.PropertyType) &&
					 !p.PropertyType.FullName.Contains("System.") &&
					 !p.PropertyType.IsEnum)
			{
				List<MemberInfo>[] Children = _GetPossibleFields(p.PropertyType, depth);
				for (int j = 0; j < Children.Length; j++)
				{
					if (Children[j].Count > 0 && !Children[j].Contains(p.PropertyType))
					{
						List<MemberInfo> currentList = new List<MemberInfo>(new MemberInfo[] { p });

						currentList.AddRange(Children[j]);
						fieldNames.Add(currentList);
					}
				}
			}
		}
		return fieldNames.ToArray();
	}
	List<MemberInfo>[] FilterPossibleTypes(List<MemberInfo>[] fieldNames)
	{
		List<Type> typeCount = new List<Type>();
		List<List<MemberInfo>> newList = new List<List<MemberInfo>>();
		for (int i = 0; i < fieldNames.Length; i++)
		{
			typeCount.Clear();
			for (int j = 0; j < fieldNames[i].Count; j++)
			{
				if (!typeCount.Contains(fieldNames[i][j].DeclaringType))
					typeCount.Add(fieldNames[i][j].DeclaringType);
				else
				{
					typeCount.Clear();
					break;
				}
			}
			if (typeCount.Count > 0)
				newList.Add(fieldNames[i]);
		}
		return newList.ToArray();
	}


	static BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetProperty;
	static Type[] excludedTypes = new Type[]
		{
			typeof(RectTransform),
			typeof(Component),
			typeof(GUIStyle),
			typeof(Color),
			typeof(Texture),
			typeof(RenderTexture),
			typeof(Texture2D),
			typeof(Texture3D),
			typeof(Matrix4x4),
			typeof(Canvas),
			typeof(Material),
			typeof(TextGenerator),
			typeof(CanvasRenderer),
		};
}
