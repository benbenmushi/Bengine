using UnityEngine;
using System;

[Serializable]
public class boolTab2DWrapper : tab2DWrapper<bool>
{
	public boolTab2DWrapper(int _w, int _h) : base(_w, _h) { }
}

[Serializable]
public class stringTab4DWrapper : tab2DWrapper<stringTab2DWrapper>
{
	public stringTab4DWrapper(int _w, int _h) : base(_w, _h) { }
}

[Serializable]
public class stringTab2DWrapper : tab2DWrapper<string>
{
	public stringTab2DWrapper(int _w, int _h) : base(_w, _h) { }
}

[Serializable]
public class intTab2DWrapper : tab2DWrapper<int>
{
	public intTab2DWrapper(int _w, int _h) : base(_w, _h) { }
}

[Serializable]
public class vector3Tab2DWrapper : tab2DWrapper<Vector3>
{
	public vector3Tab2DWrapper(int _w, int _h) : base(_w, _h) { }
}

[Serializable]
public class tab2DWrapper<T>
{
	[SerializeField]
	private T[] myPanels;
	[SerializeField]
	private int _w;
	[SerializeField]
	private int _h;

	public int width
	{
		get
		{ return this._w; }
	}
	public int height
	{
		get
		{ return this._h; }
	}

	protected tab2DWrapper(int w, int h)
	{
		if (w < 0)
			w = 0;
		if (h < 0)
			h = 0;
		this._w = w;
		this._h = h;
		this.myPanels = new T[w * h];
	}
	public T this[int x, int y]
	{
		get { return this.myPanels[y * this._w + x]; }
		set { this.myPanels[y * this._w + x] = value; }
	}
	public T this[int i]
	{
		get { return this.myPanels[i]; }
		set { this.myPanels[i] = value; }
	}
	public void ForEach(Action<T> action)
	{
		for (int x = 0; x < this._w; ++x)
			for (int y = 0; y < this._h; ++y)
				action(this[x, y]);
	}

}