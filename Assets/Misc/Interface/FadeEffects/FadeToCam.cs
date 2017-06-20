#if UNITY_EDITOR
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using System;
#endif
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeToCam : MonoBehaviour
{

	public bool     enableAtStart = false;
	[Range(0, 1)]
	public float intensity = 1;
	[ValidateInput("IsSolidColor", "Camera must be solid color")]
	public Camera targetCamera;
	private RenderTexture cachedTexture;
	private Camera refCam;

	[SerializeField]
	private Shader shader;
	private Material material = null;

	bool IsSolidColor(Camera cam)
	{
		return (cam.clearFlags == CameraClearFlags.SolidColor);
	}

	void Awake()
	{
		this.refCam = this.GetComponent<Camera>();
		this.enabled = enableAtStart;
	}
	void OnEnable()
	{
		// Disable if Image Effects is not supported
		if (!SystemInfo.supportsImageEffects)
		{
			Debug.LogWarning("ShakeRoundEffect : Image effects is not supported on this platform! Disabling.");
			this.enabled = false;
			return;
		}
		// Disable if required Render Texture Format is not supported
		if (!SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.ARGB32))
		{
			Debug.LogWarning("ShakeRoundEffect : RenderTextureFormat.ARGB32 is not supported on this platform! Disabling.");
			this.enabled = false;
			return;
		}
		if (this.shader == null)
		{
			Debug.LogWarning("ShakeRoundEffect : shakeRoundShader is missing! Disabling.");
			this.enabled = false;
			return;
		}

		if (cachedTexture != null)
			Destroy(cachedTexture);
		if (targetCamera)
		{
			cachedTexture = new RenderTexture(targetCamera.pixelWidth, targetCamera.pixelHeight, 16);
			targetCamera.targetTexture = cachedTexture;
			targetCamera.enabled = true;
		}
		if (this.material == null)
		{
			material = new Material(this.shader);
			material.SetFloat("_Intensity", intensity);
			material.SetTexture("_Texture", cachedTexture);
			material.hideFlags = HideFlags.HideAndDontSave;
		}
	}
	void OnDisable()
	{
		if (material != null)
			material = null;
		if (cachedTexture != null)
			Destroy(cachedTexture);
	}

	void OnValidate()
	{
		if (material != null)
		{
			material.SetFloat("_Intensity", intensity);
		}
	}
	void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		// If shakeMaterial is not created by some reason
		if (this.material == null)
		{
			// Simply transfer framebuffer to destination
			Graphics.Blit(source, destination);
			return;
		}
		Graphics.Blit(source, destination, this.material, 0 /* 0 = Multiplty */);
	}
}
