using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.XR;

namespace cakeslice;

[DisallowMultipleComponent]
[RequireComponent(typeof(Camera))]
[ExecuteInEditMode]
public class OutlineEffect : MonoBehaviour
{
	private static OutlineEffect m_instance;

	private readonly LinkedSet<Outline> outlines = new LinkedSet<Outline>();

	[Range(1f, 6f)]
	public float lineThickness = 1.25f;

	[Range(0f, 10f)]
	public float lineIntensity = 0.5f;

	[Range(0f, 1f)]
	public float fillAmount = 0.2f;

	public Color lineColor0 = Color.red;

	public Color lineColor1 = Color.green;

	public Color lineColor2 = Color.blue;

	public bool additiveRendering;

	public bool backfaceCulling = true;

	[Header("These settings can affect performance!")]
	public bool cornerOutlines;

	public bool addLinesBetweenColors;

	[Header("Advanced settings")]
	public bool scaleWithScreenSize = true;

	[Range(0.1f, 0.9f)]
	public float alphaCutoff = 0.5f;

	public bool flipY;

	public Camera sourceCamera;

	[HideInInspector]
	public Camera outlineCamera;

	private Material outline1Material;

	private Material outline2Material;

	private Material outline3Material;

	private Material outlineEraseMaterial;

	private Shader outlineShader;

	private Shader outlineBufferShader;

	[HideInInspector]
	public Material outlineShaderMaterial;

	[HideInInspector]
	public RenderTexture renderTexture;

	[HideInInspector]
	public RenderTexture extraRenderTexture;

	private CommandBuffer commandBuffer;

	private List<Material> materialBuffer = new List<Material>();

	public static OutlineEffect Instance
	{
		get
		{
			if (object.Equals(m_instance, null))
			{
				return m_instance = Object.FindObjectOfType(typeof(OutlineEffect)) as OutlineEffect;
			}
			return m_instance;
		}
	}

	private OutlineEffect()
	{
	}

	private Material GetMaterialFromID(int ID)
	{
		return ID switch
		{
			0 => outline1Material, 
			1 => outline2Material, 
			_ => outline3Material, 
		};
	}

	private Material CreateMaterial(Color emissionColor)
	{
		Material material = new Material(outlineBufferShader);
		material.SetColor("_Color", emissionColor);
		material.SetInt("_SrcBlend", 5);
		material.SetInt("_DstBlend", 10);
		material.SetInt("_ZWrite", 0);
		material.DisableKeyword("_ALPHATEST_ON");
		material.EnableKeyword("_ALPHABLEND_ON");
		material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
		material.renderQueue = 3000;
		return material;
	}

	private void Awake()
	{
		m_instance = this;
	}

	private void Start()
	{
		CreateMaterialsIfNeeded();
		UpdateMaterialsPublicProperties();
		if (sourceCamera == null)
		{
			sourceCamera = GetComponent<Camera>();
			if (sourceCamera == null)
			{
				sourceCamera = Camera.main;
			}
		}
		if (outlineCamera == null)
		{
			GameObject gameObject = new GameObject("Outline Camera");
			gameObject.transform.parent = sourceCamera.transform;
			outlineCamera = gameObject.AddComponent<Camera>();
			outlineCamera.enabled = false;
		}
		renderTexture = new RenderTexture(sourceCamera.pixelWidth, sourceCamera.pixelHeight, 16, RenderTextureFormat.Default);
		extraRenderTexture = new RenderTexture(sourceCamera.pixelWidth, sourceCamera.pixelHeight, 16, RenderTextureFormat.Default);
		UpdateOutlineCameraFromSource();
		commandBuffer = new CommandBuffer();
		outlineCamera.AddCommandBuffer(CameraEvent.BeforeImageEffects, commandBuffer);
	}

	public void OnPreRender()
	{
		if (commandBuffer == null)
		{
			return;
		}
		CreateMaterialsIfNeeded();
		if (renderTexture == null || renderTexture.width != sourceCamera.pixelWidth || renderTexture.height != sourceCamera.pixelHeight)
		{
			renderTexture = new RenderTexture(sourceCamera.pixelWidth, sourceCamera.pixelHeight, 16, RenderTextureFormat.Default);
			extraRenderTexture = new RenderTexture(sourceCamera.pixelWidth, sourceCamera.pixelHeight, 16, RenderTextureFormat.Default);
			outlineCamera.targetTexture = renderTexture;
		}
		UpdateMaterialsPublicProperties();
		UpdateOutlineCameraFromSource();
		outlineCamera.targetTexture = renderTexture;
		commandBuffer.SetRenderTarget(renderTexture);
		commandBuffer.Clear();
		if (outlines != null)
		{
			foreach (Outline outline in outlines)
			{
				LayerMask layerMask = sourceCamera.cullingMask;
				if (!(outline != null) || (int)layerMask != ((int)layerMask | (1 << outline.originalLayer)))
				{
					continue;
				}
				for (int i = 0; i < outline.Renderer.sharedMaterials.Length; i++)
				{
					Material material = null;
					if (!outline.Renderer.sharedMaterials[i] || !(outline.Renderer.sharedMaterials[i].mainTexture != null))
					{
						material = ((!outline.eraseRenderer) ? GetMaterialFromID(outline.color) : outlineEraseMaterial);
					}
					else
					{
						foreach (Material item in materialBuffer)
						{
							if (item.mainTexture == outline.Renderer.sharedMaterials[i].mainTexture)
							{
								if (outline.eraseRenderer && item.color == outlineEraseMaterial.color)
								{
									material = item;
								}
								else if (item.color == GetMaterialFromID(outline.color).color)
								{
									material = item;
								}
							}
						}
						if (material == null)
						{
							material = ((!outline.eraseRenderer) ? new Material(GetMaterialFromID(outline.color)) : new Material(outlineEraseMaterial));
							material.mainTexture = outline.Renderer.sharedMaterials[i].mainTexture;
							materialBuffer.Add(material);
						}
					}
					if (backfaceCulling)
					{
						material.SetInt("_Culling", 2);
					}
					else
					{
						material.SetInt("_Culling", 0);
					}
					commandBuffer.DrawRenderer(outline.GetComponent<Renderer>(), material, 0, 0);
					MeshFilter component = outline.GetComponent<MeshFilter>();
					if ((bool)component)
					{
						for (int j = 1; j < component.sharedMesh.subMeshCount; j++)
						{
							commandBuffer.DrawRenderer(outline.GetComponent<Renderer>(), material, j, 0);
						}
					}
					SkinnedMeshRenderer component2 = outline.GetComponent<SkinnedMeshRenderer>();
					if ((bool)component2)
					{
						for (int k = 1; k < component2.sharedMesh.subMeshCount; k++)
						{
							commandBuffer.DrawRenderer(outline.GetComponent<Renderer>(), material, k, 0);
						}
					}
				}
			}
		}
		outlineCamera.Render();
	}

	private void OnEnable()
	{
		Outline[] array = Object.FindObjectsOfType<Outline>();
		foreach (Outline obj in array)
		{
			obj.enabled = false;
			obj.enabled = true;
		}
	}

	private void OnDestroy()
	{
		if (renderTexture != null)
		{
			renderTexture.Release();
		}
		if (extraRenderTexture != null)
		{
			extraRenderTexture.Release();
		}
		DestroyMaterials();
	}

	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		outlineShaderMaterial.SetTexture("_OutlineSource", renderTexture);
		if (addLinesBetweenColors)
		{
			Graphics.Blit(source, extraRenderTexture, outlineShaderMaterial, 0);
			outlineShaderMaterial.SetTexture("_OutlineSource", extraRenderTexture);
		}
		Graphics.Blit(source, destination, outlineShaderMaterial, 1);
	}

	private void CreateMaterialsIfNeeded()
	{
		if (outlineShader == null)
		{
			outlineShader = Resources.Load<Shader>("OutlineShader");
		}
		if (outlineBufferShader == null)
		{
			outlineBufferShader = Resources.Load<Shader>("OutlineBufferShader");
		}
		if (outlineShaderMaterial == null)
		{
			outlineShaderMaterial = new Material(outlineShader);
			outlineShaderMaterial.hideFlags = HideFlags.HideAndDontSave;
			UpdateMaterialsPublicProperties();
		}
		if (outlineEraseMaterial == null)
		{
			outlineEraseMaterial = CreateMaterial(new Color(0f, 0f, 0f, 0f));
		}
		if (outline1Material == null)
		{
			outline1Material = CreateMaterial(new Color(1f, 0f, 0f, 0f));
		}
		if (outline2Material == null)
		{
			outline2Material = CreateMaterial(new Color(0f, 1f, 0f, 0f));
		}
		if (outline3Material == null)
		{
			outline3Material = CreateMaterial(new Color(0f, 0f, 1f, 0f));
		}
	}

	private void DestroyMaterials()
	{
		foreach (Material item in materialBuffer)
		{
			Object.DestroyImmediate(item);
		}
		materialBuffer.Clear();
		Object.DestroyImmediate(outlineShaderMaterial);
		Object.DestroyImmediate(outlineEraseMaterial);
		Object.DestroyImmediate(outline1Material);
		Object.DestroyImmediate(outline2Material);
		Object.DestroyImmediate(outline3Material);
		outlineShader = null;
		outlineBufferShader = null;
		outlineShaderMaterial = null;
		outlineEraseMaterial = null;
		outline1Material = null;
		outline2Material = null;
		outline3Material = null;
	}

	public void UpdateMaterialsPublicProperties()
	{
		if (!outlineShaderMaterial)
		{
			return;
		}
		float num = 1f;
		if (scaleWithScreenSize)
		{
			num = (float)Screen.height / 360f;
		}
		if (scaleWithScreenSize && num < 1f)
		{
			if (XRSettings.isDeviceActive && sourceCamera.stereoTargetEye != 0)
			{
				outlineShaderMaterial.SetFloat("_LineThicknessX", 0.001f * (1f / (float)XRSettings.eyeTextureWidth) * 1000f);
				outlineShaderMaterial.SetFloat("_LineThicknessY", 0.001f * (1f / (float)XRSettings.eyeTextureHeight) * 1000f);
			}
			else
			{
				outlineShaderMaterial.SetFloat("_LineThicknessX", 0.001f * (1f / (float)Screen.width) * 1000f);
				outlineShaderMaterial.SetFloat("_LineThicknessY", 0.001f * (1f / (float)Screen.height) * 1000f);
			}
		}
		else if (XRSettings.isDeviceActive && sourceCamera.stereoTargetEye != 0)
		{
			outlineShaderMaterial.SetFloat("_LineThicknessX", num * (lineThickness / 1000f) * (1f / (float)XRSettings.eyeTextureWidth) * 1000f);
			outlineShaderMaterial.SetFloat("_LineThicknessY", num * (lineThickness / 1000f) * (1f / (float)XRSettings.eyeTextureHeight) * 1000f);
		}
		else
		{
			outlineShaderMaterial.SetFloat("_LineThicknessX", num * (lineThickness / 1000f) * (1f / (float)Screen.width) * 1000f);
			outlineShaderMaterial.SetFloat("_LineThicknessY", num * (lineThickness / 1000f) * (1f / (float)Screen.height) * 1000f);
		}
		outlineShaderMaterial.SetFloat("_LineIntensity", lineIntensity);
		outlineShaderMaterial.SetFloat("_FillAmount", fillAmount);
		outlineShaderMaterial.SetColor("_LineColor1", lineColor0 * lineColor0);
		outlineShaderMaterial.SetColor("_LineColor2", lineColor1 * lineColor1);
		outlineShaderMaterial.SetColor("_LineColor3", lineColor2 * lineColor2);
		if (flipY)
		{
			outlineShaderMaterial.SetInt("_FlipY", 1);
		}
		else
		{
			outlineShaderMaterial.SetInt("_FlipY", 0);
		}
		if (!additiveRendering)
		{
			outlineShaderMaterial.SetInt("_Dark", 1);
		}
		else
		{
			outlineShaderMaterial.SetInt("_Dark", 0);
		}
		if (cornerOutlines)
		{
			outlineShaderMaterial.SetInt("_CornerOutlines", 1);
		}
		else
		{
			outlineShaderMaterial.SetInt("_CornerOutlines", 0);
		}
		Shader.SetGlobalFloat("_OutlineAlphaCutoff", alphaCutoff);
	}

	private void UpdateOutlineCameraFromSource()
	{
		outlineCamera.CopyFrom(sourceCamera);
		outlineCamera.renderingPath = RenderingPath.Forward;
		outlineCamera.backgroundColor = new Color(0f, 0f, 0f, 0f);
		outlineCamera.clearFlags = CameraClearFlags.Color;
		outlineCamera.rect = new Rect(0f, 0f, 1f, 1f);
		outlineCamera.cullingMask = 0;
		outlineCamera.targetTexture = renderTexture;
		outlineCamera.enabled = false;
		outlineCamera.allowHDR = false;
	}

	public void AddOutline(Outline outline)
	{
		if (!outlines.Contains(outline))
		{
			outlines.Add(outline);
		}
	}

	public void RemoveOutline(Outline outline)
	{
		if (outlines.Contains(outline))
		{
			outlines.Remove(outline);
		}
	}
}
