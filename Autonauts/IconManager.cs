using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class IconManager : MonoBehaviour
{
	public enum ImageFilterMode
	{
		Nearest,
		Biliner,
		Average
	}

	public static IconManager Instance;

	private static int m_Width = 128;

	private static int m_Height = 128;

	public Sprite[] m_Sprites;

	private RenderTexture m_RenderTexture;

	private Texture2D m_FinalTexture;

	private Camera m_RenderCamera;

	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
			UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		}
		else if (this != Instance)
		{
			UnityEngine.Object.Destroy(base.gameObject);
			return;
		}
		m_RenderCamera = GameObject.Find("Render Camera").GetComponent<Camera>();
	}

	public static Texture2D ResizeTexture(Texture2D pSource, ImageFilterMode pFilterMode, float pScale)
	{
		Color[] pixels = pSource.GetPixels(0);
		Vector2 vector = new Vector2(pSource.width, pSource.height);
		float num = Mathf.RoundToInt((float)pSource.width * pScale);
		float num2 = Mathf.RoundToInt((float)pSource.height * pScale);
		Texture2D texture2D = new Texture2D((int)num, (int)num2, TextureFormat.RGBA32, mipChain: false);
		int num3 = (int)num * (int)num2;
		Color[] array = new Color[num3];
		Vector2 vector2 = new Vector2(vector.x / num, vector.y / num2);
		Vector2 vector3 = default(Vector2);
		for (int i = 0; i < num3; i++)
		{
			float num4 = (float)i % num;
			float num5 = Mathf.Floor((float)i / num);
			vector3.x = num4 / num * vector.x;
			vector3.y = num5 / num2 * vector.y;
			switch (pFilterMode)
			{
			case ImageFilterMode.Nearest:
			{
				vector3.x = Mathf.Round(vector3.x);
				vector3.y = Mathf.Round(vector3.y);
				int num15 = (int)(vector3.y * vector.x + vector3.x);
				array[i] = pixels[num15];
				break;
			}
			case ImageFilterMode.Biliner:
			{
				float t = vector3.x - Mathf.Floor(vector3.x);
				float t2 = vector3.y - Mathf.Floor(vector3.y);
				int num11 = (int)(Mathf.Floor(vector3.y) * vector.x + Mathf.Floor(vector3.x));
				int num12 = (int)(Mathf.Floor(vector3.y) * vector.x + Mathf.Ceil(vector3.x));
				int num13 = (int)(Mathf.Ceil(vector3.y) * vector.x + Mathf.Floor(vector3.x));
				int num14 = (int)(Mathf.Ceil(vector3.y) * vector.x + Mathf.Ceil(vector3.x));
				array[i] = Color.Lerp(Color.Lerp(pixels[num11], pixels[num12], t), Color.Lerp(pixels[num13], pixels[num14], t), t2);
				break;
			}
			case ImageFilterMode.Average:
			{
				int num6 = (int)Mathf.Max(Mathf.Floor(vector3.x - vector2.x * 0.5f), 0f);
				int num7 = (int)Mathf.Min(Mathf.Ceil(vector3.x + vector2.x * 0.5f), vector.x);
				int num8 = (int)Mathf.Max(Mathf.Floor(vector3.y - vector2.y * 0.5f), 0f);
				int num9 = (int)Mathf.Min(Mathf.Ceil(vector3.y + vector2.y * 0.5f), vector.y);
				Color color = default(Color);
				float num10 = 0f;
				for (int j = num8; j < num9; j++)
				{
					for (int k = num6; k < num7; k++)
					{
						color += pixels[(int)((float)j * vector.x + (float)k)];
						num10 += 1f;
					}
				}
				array[i] = color / num10;
				break;
			}
			}
		}
		texture2D.SetPixels(array);
		texture2D.Apply();
		return texture2D;
	}

	private Color CopyRGB(Color NewColour, Color OldColour)
	{
		NewColour.r = OldColour.r;
		NewColour.g = OldColour.g;
		NewColour.b = OldColour.b;
		return NewColour;
	}

	public void FilterEdges(Texture2D pSource)
	{
		int width = pSource.width;
		int height = pSource.height;
		Color[] pixels = pSource.GetPixels(0);
		Color[] array = new Color[width * height];
		float num = 0.1f;
		for (int i = 0; i < height; i++)
		{
			for (int j = 0; j < width; j++)
			{
				int num2 = i * width + j;
				Color color = pixels[num2];
				if (color.a < num)
				{
					if (j > 0 && pixels[num2 - 1].a >= num)
					{
						color = CopyRGB(color, pixels[num2 - 1]);
					}
					else if (j < width - 1 && pixels[num2 + 1].a >= num)
					{
						color = CopyRGB(color, pixels[num2 + 1]);
					}
					else if (i > 0 && pixels[num2 - width].a >= num)
					{
						color = CopyRGB(color, pixels[num2 - width]);
					}
					else if (i < height - 1 && pixels[num2 + width].a >= num)
					{
						color = CopyRGB(color, pixels[num2 + width]);
					}
					else if (j > 1 && pixels[num2 - 2].a >= num)
					{
						color = CopyRGB(color, pixels[num2 - 2]);
					}
					else if (j < width - 2 && pixels[num2 + 2].a >= num)
					{
						color = CopyRGB(color, pixels[num2 + 2]);
					}
					else if (i > 1 && pixels[num2 - width * 2].a >= num)
					{
						color = CopyRGB(color, pixels[num2 - width * 2]);
					}
					else if (i < height - 2 && pixels[num2 + width * 2].a >= num)
					{
						color = CopyRGB(color, pixels[num2 + width * 2]);
					}
				}
				array[num2] = color;
			}
		}
		pSource.SetPixels(array);
		pSource.Apply();
	}

	private Texture2D RenderModelToTexture(GameObject NewModel, ObjectType NewType)
	{
		Camera renderCamera = m_RenderCamera;
		renderCamera.gameObject.SetActive(value: true);
		if (m_RenderTexture == null)
		{
			m_RenderTexture = new RenderTexture(m_Width * 2, m_Height * 2, 24);
			m_FinalTexture = new Texture2D(m_Width * 2, m_Height * 2, TextureFormat.RGBA32, mipChain: false);
			m_FinalTexture.filterMode = FilterMode.Point;
			m_FinalTexture.wrapMode = TextureWrapMode.Clamp;
		}
		renderCamera.targetTexture = m_RenderTexture;
		RenderTexture.active = m_RenderTexture;
		renderCamera.Render();
		m_FinalTexture.ReadPixels(new Rect(0f, 0f, m_Width * 2, m_Height * 2), 0, 0);
		m_FinalTexture.Apply();
		FilterEdges(m_FinalTexture);
		RenderTexture.active = null;
		renderCamera.gameObject.SetActive(value: false);
		return ResizeTexture(m_FinalTexture, ImageFilterMode.Average, 0.5f);
	}

	private Sprite CreateSprite(string ModelName, ObjectType NewType, string FileName = "")
	{
		GameObject gameObject = ModelManager.Instance.Instantiate(NewType, ModelName, null, RandomVariants: false);
		if (Converter.GetIsTypeConverter(NewType) && (bool)gameObject.transform.Find("Blueprint"))
		{
			gameObject.transform.Find("Blueprint").GetComponent<MeshRenderer>().enabled = false;
		}
		if (Converter.GetIsTypeConverter(NewType) && (bool)gameObject.transform.Find("Plane"))
		{
			Texture value = (Texture)Resources.Load("Textures/Original/IconEmpty", typeof(Texture));
			gameObject.transform.Find("Plane").GetComponent<MeshRenderer>().material.SetTexture("_MainTex", value);
		}
		if (Housing.GetIsTypeHouse(NewType) && (bool)gameObject.transform.Find("Dead"))
		{
			gameObject.transform.Find("Dead").gameObject.SetActive(value: false);
		}
		if (Housing.GetIsTypeHouse(NewType) && ObjectUtils.FindDeepChild(gameObject.transform, "Scaffolding") != null)
		{
			ObjectUtils.FindDeepChild(gameObject.transform, "Scaffolding").gameObject.SetActive(value: false);
		}
		m_RenderCamera.transform.position = default(Vector3);
		m_RenderCamera.aspect = 1f;
		Bounds bounds = ObjectUtils.ObjectBoundsFromVerticesInCameraSpace(gameObject, m_RenderCamera);
		float num = 0f;
		num = ((!(bounds.size.x > bounds.size.y)) ? (1f / bounds.size.y) : (1f / bounds.size.x));
		float x = bounds.center.x;
		float y = bounds.center.y;
		float z = bounds.center.z;
		Vector3 vector = m_RenderCamera.ViewportToWorldPoint(new Vector3(x, y, z)) * (0f - num);
		Vector3 vector2 = new Vector3(-100f, 0f, 0f);
		gameObject.transform.localPosition = vector2 + vector;
		gameObject.transform.localScale = new Vector3(num, num, num);
		vector2 += m_RenderCamera.transform.TransformPoint(new Vector3(0f, 0f, -10f));
		m_RenderCamera.transform.position = vector2;
		Texture2D texture2D = RenderModelToTexture(gameObject, NewType);
		if (FileName.Length > 0)
		{
			OutputTexture(FileName, texture2D);
		}
		Sprite result = Sprite.Create(texture2D, new Rect(0f, 0f, m_Width, m_Height), new Vector2(0.5f, 0.5f));
		UnityEngine.Object.DestroyImmediate(gameObject);
		return result;
	}

	private void OutputTexture(string FileName, Texture2D NewTexture)
	{
		byte[] bytes = NewTexture.EncodeToPNG();
		try
		{
			File.WriteAllBytes(FileName, bytes);
		}
		catch (UnauthorizedAccessException ex)
		{
			ErrorMessage.LogError("Thumbnail Save - UnauthorizedAccessException : " + FileName + " " + ex.ToString());
		}
	}

	private Sprite LoadSprite(string FileName)
	{
		return (Sprite)Resources.Load(FileName, typeof(Sprite));
	}

	private Dictionary<ObjectType, string> GetSpecialCases()
	{
		return new Dictionary<ObjectType, string>
		{
			{
				ObjectType.Worker,
				"IconBotCustom"
			},
			{
				ObjectType.BasicWorker,
				"IconBot"
			},
			{
				ObjectType.Nothing,
				"IconNone"
			},
			{
				ObjectType.Empty,
				"IconEmpty"
			},
			{
				ObjectType.Water,
				"IconWater"
			},
			{
				ObjectType.SeaWater,
				"IconSeaWater"
			},
			{
				ObjectType.Milk,
				"IconMilk"
			},
			{
				ObjectType.Honey,
				"IconHoney"
			},
			{
				ObjectType.Soil,
				"IconSoil"
			},
			{
				ObjectType.FishAny,
				"IconFishAny"
			},
			{
				ObjectType.HatAny,
				"IconHat"
			},
			{
				ObjectType.TopAny,
				"IconTop"
			},
			{
				ObjectType.WorkerFrameAny,
				"IconWorkerFrameAny"
			},
			{
				ObjectType.WorkerHeadAny,
				"IconWorkerHeadAny"
			},
			{
				ObjectType.WorkerDriveAny,
				"IconWorkerDriveAny"
			},
			{
				ObjectType.Fuel,
				"IconFuel"
			},
			{
				ObjectType.HeartAny,
				"IconHeartAny"
			},
			{
				ObjectType.ResearchStationCrude,
				"IconResearchStationCrude"
			},
			{
				ObjectType.ResearchStationCrude2,
				"IconResearchStationCrude2"
			},
			{
				ObjectType.ResearchStationCrude3,
				"IconResearchStationCrude3"
			},
			{
				ObjectType.ResearchStationCrude4,
				"IconResearchStationCrude4"
			},
			{
				ObjectType.ResearchStationCrude5,
				"IconResearchStationCrude5"
			},
			{
				ObjectType.ResearchStationCrude6,
				"IconResearchStationCrude6"
			}
		};
	}

	public void Init()
	{
		int total = (int)ObjectTypeList.m_Total;
		m_Sprites = new Sprite[total];
		Dictionary<ObjectType, string> specialCases = GetSpecialCases();
		for (int i = 0; i < total; i++)
		{
			ObjectType objectType = (ObjectType)i;
			Sprite sprite = null;
			if (specialCases.ContainsKey(objectType))
			{
				string fileName = "Textures/Hud/Icons/" + specialCases[objectType];
				sprite = LoadSprite(fileName);
			}
			else
			{
				string modelNameFromIdentifier = ObjectTypeList.Instance.GetModelNameFromIdentifier(objectType);
				if (modelNameFromIdentifier != null && modelNameFromIdentifier != "")
				{
					string iconPath = GetIconPath(objectType);
					sprite = ((objectType < ObjectType.Total) ? LoadSprite(iconPath) : CreateSprite(modelNameFromIdentifier, objectType));
				}
			}
			m_Sprites[i] = sprite;
		}
	}

	public Sprite GetIcon(ObjectType NewType)
	{
		return m_Sprites[(int)NewType];
	}

	private string GetIconPath(ObjectType NewType)
	{
		return "Textures/Hud/GenIcons/GenIcon" + NewType;
	}
}
