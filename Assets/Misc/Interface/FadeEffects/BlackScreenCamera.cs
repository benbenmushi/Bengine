using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackScreenCamera : MonoBehaviour
{
	public bool     enableAtStart = false;
	[Range(0, 1)]
	public float intensity = 1;
	private RenderTexture accumTexture;
	private Camera refCam;

	[SerializeField]
	private Shader blackScreenShader;
	private Material blackScreenMaterial = null;

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
		if (this.blackScreenShader == null)
		{
			Debug.LogWarning("ShakeRoundEffect : shakeRoundShader is missing! Disabling.");
			this.enabled = false;
			return;
		}
		if (this.blackScreenMaterial == null)
		{
			blackScreenMaterial = new Material(this.blackScreenShader);
			blackScreenMaterial.SetFloat("_Intensity", intensity);
			blackScreenMaterial.hideFlags = HideFlags.HideAndDontSave;
		}
	}
	void OnDisable()
	{
		if (blackScreenMaterial != null)
			blackScreenMaterial = null;
		DestroyImmediate(this.accumTexture);
	}

	void OnValidate()
	{
		if (blackScreenMaterial != null)
		{
			blackScreenMaterial.SetFloat("_Intensity", intensity);
		}
	}
	void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		// If shakeMaterial is not created by some reason
		if (this.blackScreenMaterial == null)
		{
			// Simply transfer framebuffer to destination
			Graphics.Blit(source, destination);
			return;
		}
		Graphics.Blit(source, destination, this.blackScreenMaterial, 0 /* 0 = Multiplty */);
	}
}
