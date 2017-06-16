using UnityEngine;
using System.Collections;

enum FONTION_AFFINE_TYPE
{
	VERTICAL,	//X = A
	HORIZONTAL, //Y = B
	NORMAL, 	//Y = AX+B
};

public class FonctionAffine {
	
	private float 						a;
	private float 						b;
	private FONTION_AFFINE_TYPE			type;
	
	public FonctionAffine(Vector2 pt1, Vector2 pt2)
	{
		if(pt1.x == pt2.x)
		{
			type = FONTION_AFFINE_TYPE.VERTICAL;
			a = pt1.y;
			return;
		}
		if(pt1.y == pt2.y)
		{
			type = FONTION_AFFINE_TYPE.HORIZONTAL;
			b = pt1.x;
			a = 0;
			return;
		}
		
		type = FONTION_AFFINE_TYPE.NORMAL;
		a = (pt2.y-pt1.y)/(pt2.x-pt1.x);
		b = pt1.y - a*pt1.x;		
	}
	
	public float GetY(float x)
	{
		if(type == FONTION_AFFINE_TYPE.VERTICAL)
		{
			Debug.LogWarning("Warning FonctionAffine.GetY() Fonction verticale!");
			return -1;
		}
		
		return (a*x+b);
	}
	
	public float GetX(float y)
	{
		if(type == FONTION_AFFINE_TYPE.HORIZONTAL)
		{
			Debug.LogWarning("Warning FonctionAffine.GetX() Fonction horizontale!");
			return -1;
		}
		
		return (y-b)/a;
	}
	
	public void Display()
	{
		switch(type)
		{
			case FONTION_AFFINE_TYPE.HORIZONTAL:
				Debug.Log ("Y = "+b);
				break;
			case FONTION_AFFINE_TYPE.VERTICAL:
				Debug.Log ("X = "+a);
				break;
			case FONTION_AFFINE_TYPE.NORMAL:
				Debug.Log ("Y = "+a+"X + "+b);
				break;
		}
	}
	
}
