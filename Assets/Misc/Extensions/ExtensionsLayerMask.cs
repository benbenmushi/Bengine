using UnityEngine;
using System.Collections;

public static class ExtensionsLayerMask
{
	public static bool IsInLayerMask(this LayerMask mask, int layer)
	{
		return mask.value.IsInMask(layer);
	}
	public static bool IsInLayerMask(this LayerMask mask, Collider collider)
	{
		return collider != null && collider.gameObject != null && mask.value.IsInMask(collider.gameObject.layer);
	}
	public static bool IsInMask(this int mask, int layer)
	{
		return (mask & (1 << layer)) > 0;
	}
}
