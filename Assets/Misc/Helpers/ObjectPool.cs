using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class ObjectPool<T> where T : Component
{
	// PUBLIC
	public T originalObject { get; private set; }

	// PRIVATE FIELDS
	[System.Serializable]
	private struct PoolElement
	{
		public T	element;
		public bool	isAvailable;

		public PoolElement(T _element, bool _isAvailable)
		{
			element = _element;
			isAvailable = _isAvailable;
		}
	}
	private List<PoolElement>   objectList = new List<PoolElement>();
	private System.Action<T>    onCreated;
	private System.Action<T>    onReset;


	// PUBLIC METHODS
	public ObjectPool(T _originalObject, System.Action<T> _onCreated = null, System.Action<T> _onReset = null)
	{
		originalObject = _originalObject;
		onCreated = _onCreated;
		onReset = _onReset;
	}
	public void ResetPool()
	{
		objectList.ForEach((b) =>
		{
			if (onReset != null)
				onReset(b.element);
			b.isAvailable = true;
		});
	}
	public void Release(T releasedTarget)
	{
		PoolElement e = objectList.FirstOrDefault(e2 => e2.element == releasedTarget);

		if (e.element == releasedTarget)
		{
			if (onReset != null)
				onReset(e.element);
			e.isAvailable = true;
		}
	}
	public T GetNew()
	{
		PoolElement newObject = objectList.FirstOrDefault(b => b.isAvailable);

		if (newObject.element == default(T))
		{
			newObject = new PoolElement(GameObject.Instantiate(originalObject.gameObject).GetComponent<T>(), false);
			objectList.Add(newObject);
		}
		if (onCreated != null)
			onCreated(newObject.element);
		return newObject.element;
	}
}
