using System.Collections.Generic;
using UnityEngine;

public class MaterialManager : MonoBehaviour
{
	public enum MiscType
	{
		TileMapMaterial,
		TileMapWaterMaterial,
		TileMapWalls,
		TileMapBoundary,
		WaterSurf,
		WetSand,
		Total
	}

	public enum BotBulbType
	{
		IdleOn,
		IdleOff,
		WorkingOn,
		WorkingOff,
		ErrorOn,
		ErrorOff,
		Total
	}

	public static MaterialManager Instance;

	public Material[] m_BotBulbMaterials;

	public static Color[] m_BotBulbColours = new Color[3]
	{
		new Color(0.75f, 0.35f, 0f),
		new Color(0f, 0.5f, 0f),
		new Color(1f, 0f, 0f)
	};

	public Material[] m_MiscMaterials;

	public Material[] m_MiscMaterialsDark;

	public Material m_Material;

	public Material m_MaterialTrans;

	public Material m_MaterialBuilding;

	public Material m_MaterialTransBuilding;

	public Material m_MaterialTransBuildingBlueprint;

	public Material m_MaterialHighlight;

	public Material m_MaterialTransHighlight;

	public Material m_MaterialOccluded;

	public Material m_MaterialTransOccluded;

	public Material m_MaterialWindy;

	public Material m_MaterialBelt;

	public Material m_CloudMaterial;

	public Material m_ArcMaterial;

	public Material m_MaterialTranscend;

	public List<Material> m_AllocatedMaterials;

	public Material m_MaterialPulleyOn;

	public Material m_MaterialPulleyOff;

	public Material m_MaterialRed;

	public Material m_MaterialBlack;

	public Material m_MaterialArcIdle;

	public Material m_MaterialArcInactive;

	public Material m_MaterialArcActive;

	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
			Object.DontDestroyOnLoad(base.gameObject);
		}
		else if (this != Instance)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		Material source = (Material)Resources.Load("Materials/Standard", typeof(Material));
		m_Material = new Material(source);
		m_Material.mainTexture = TextureAtlasManager.Instance.m_Texture;
		m_Material.name = "Big Material";
		source = (Material)Resources.Load("Materials/StandardTrans", typeof(Material));
		m_MaterialTrans = new Material(source);
		m_MaterialTrans.mainTexture = TextureAtlasManager.Instance.m_Texture;
		m_MaterialTrans.name = "Big Material Trans";
		source = (Material)Resources.Load("Materials/StandardBuilding", typeof(Material));
		m_MaterialBuilding = new Material(source);
		m_MaterialBuilding.mainTexture = TextureAtlasManager.Instance.m_Texture;
		m_MaterialBuilding.name = "Big Material Building";
		source = (Material)Resources.Load("Materials/StandardTransBuilding", typeof(Material));
		m_MaterialTransBuilding = new Material(source);
		m_MaterialTransBuilding.mainTexture = TextureAtlasManager.Instance.m_Texture;
		m_MaterialTransBuilding.name = "Big Material Trans Building";
		source = (Material)Resources.Load("Materials/StandardTransBuildingBlueprint", typeof(Material));
		m_MaterialTransBuildingBlueprint = new Material(source);
		m_MaterialTransBuildingBlueprint.mainTexture = TextureAtlasManager.Instance.m_Texture;
		m_MaterialTransBuildingBlueprint.name = "Big Material Trans Building Blueprint";
		source = (Material)Resources.Load("Materials/StandardHighlight", typeof(Material));
		m_MaterialHighlight = new Material(source);
		m_MaterialHighlight.mainTexture = TextureAtlasManager.Instance.m_Texture;
		m_MaterialHighlight.name = "Big Material Highlight";
		source = (Material)Resources.Load("Materials/StandardHighlightTrans", typeof(Material));
		m_MaterialTransHighlight = new Material(source);
		m_MaterialTransHighlight.mainTexture = TextureAtlasManager.Instance.m_Texture;
		m_MaterialTransHighlight.name = "Big Material Trans Highlight";
		source = (Material)Resources.Load("Materials/StandardOccludedHighlight", typeof(Material));
		m_MaterialOccluded = new Material(source);
		m_MaterialOccluded.mainTexture = TextureAtlasManager.Instance.m_Texture;
		m_MaterialOccluded.name = "Big Material Occluded";
		source = (Material)Resources.Load("Materials/StandardOccludedHighlightTrans", typeof(Material));
		m_MaterialTransOccluded = new Material(source);
		m_MaterialTransOccluded.mainTexture = TextureAtlasManager.Instance.m_Texture;
		m_MaterialTransOccluded.name = "Big Material Trans Occluded";
		source = (Material)Resources.Load("Materials/StandardWindy", typeof(Material));
		m_MaterialWindy = new Material(source);
		m_MaterialWindy.mainTexture = TextureAtlasManager.Instance.m_Texture;
		m_MaterialWindy.name = "Big Material Windy";
		m_MaterialPulleyOn = (Material)Resources.Load("Materials/PulleyFlashOn", typeof(Material));
		m_MaterialPulleyOff = (Material)Resources.Load("Materials/PulleyFlashOff", typeof(Material));
		m_MaterialRed = (Material)Resources.Load("Models/Materials/SharedRed", typeof(Material));
		m_MaterialBlack = (Material)Resources.Load("Models/Materials/SharedBlack", typeof(Material));
		source = (Material)Resources.Load("Materials/Belt", typeof(Material));
		m_MaterialBelt = new Material(source);
		source = (Material)Resources.Load("Materials/Clouds", typeof(Material));
		m_CloudMaterial = new Material(source);
		source = (Material)Resources.Load("Materials/Arc", typeof(Material));
		m_ArcMaterial = new Material(source);
		source = (Material)Resources.Load("Materials/StandardBuilding", typeof(Material));
		m_MaterialTranscend = new Material(source);
		m_MaterialTranscend.mainTexture = TextureAtlasManager.Instance.m_Texture;
		m_MaterialTranscend.name = "Transcend Material Building";
		m_MaterialTranscend.shader = Shader.Find("Standard");
		source = (Material)Resources.Load("Materials/ArcBuildingRefIdle", typeof(Material));
		m_MaterialArcIdle = new Material(source);
		source = (Material)Resources.Load("Materials/ArcBuildingRefInactive", typeof(Material));
		m_MaterialArcInactive = new Material(source);
		source = (Material)Resources.Load("Materials/ArcBuildingRefActive", typeof(Material));
		m_MaterialArcActive = new Material(source);
		int num = 6;
		m_BotBulbMaterials = new Material[num];
		for (int i = 0; i < num; i++)
		{
			Color color = m_BotBulbColours[i / 2];
			Material material;
			if ((i & 1) == 0)
			{
				source = (Material)Resources.Load("Materials/WorkerBulbOn", typeof(Material));
				material = new Material(source);
				material.color = color;
				material.SetColor("_EmissionColor", color * 2f);
			}
			else
			{
				source = (Material)Resources.Load("Materials/WorkerBulbOff", typeof(Material));
				material = new Material(source);
				material.color = color * 0.25f;
			}
			m_BotBulbMaterials[i] = material;
		}
		num = 6;
		m_MiscMaterials = new Material[num];
		m_MiscMaterialsDark = new Material[num];
		for (int j = 0; j < num; j++)
		{
			MiscType miscType = (MiscType)j;
			string text = miscType.ToString();
			source = (Material)Resources.Load("Materials/" + text, typeof(Material));
			m_MiscMaterials[j] = new Material(source);
			m_MiscMaterials[j].name = text + "  Material";
			m_MiscMaterialsDark[j] = new Material(source);
			m_MiscMaterialsDark[j].color = new Color(0.5f, 0.5f, 0.5f, 1f);
			m_MiscMaterialsDark[j].name = text + "  Dark";
		}
		UpdateSnow();
		m_AllocatedMaterials = new List<Material>();
	}

	public void UpdateSnow()
	{
		string text = "TileMap";
		if ((bool)SettingsManager.Instance && SettingsManager.Instance.GetIsSnowAvailable())
		{
			text += "Snow";
		}
		Texture mainTexture = (Texture)Resources.Load("Textures/Writable/" + text, typeof(Texture));
		Material misc = GetMisc(MiscType.TileMapMaterial, Dark: false);
		if ((bool)misc)
		{
			misc.mainTexture = mainTexture;
			misc = GetMisc(MiscType.TileMapMaterial, Dark: true);
			misc.mainTexture = mainTexture;
		}
	}

	public Material GetMisc(MiscType NewType, bool Dark)
	{
		if (m_MiscMaterials == null)
		{
			return null;
		}
		if (Dark)
		{
			return m_MiscMaterialsDark[(int)NewType];
		}
		return m_MiscMaterials[(int)NewType];
	}

	public Material AddMaterial(string Name)
	{
		Material material = new Material((Material)Resources.Load(Name, typeof(Material)));
		m_AllocatedMaterials.Add(material);
		return material;
	}

	public void DestroyMaterial(Material OldMaterial)
	{
		m_AllocatedMaterials.Remove(OldMaterial);
		Object.Destroy(OldMaterial);
	}

	private void SetAllocatedMaterialsDesat(float Value)
	{
		foreach (Material allocatedMaterial in m_AllocatedMaterials)
		{
			allocatedMaterial.SetFloat("_Saturation", Value);
		}
	}

	public void ToggleTest()
	{
		if (CheatManager.Instance.m_ShowMaterials)
		{
			m_Material.color = new Color(1f, 0f, 1f);
			m_Material.mainTexture = null;
		}
		else
		{
			m_Material.color = new Color(1f, 1f, 1f);
			m_Material.mainTexture = TextureAtlasManager.Instance.m_Texture;
		}
	}

	public void SetDesaturation(bool Desaturated, bool BuildingsDesaturated)
	{
		float num = 1f;
		if (Desaturated)
		{
			num = 0f;
		}
		m_Material.SetFloat("_Saturation", num);
		m_MaterialTrans.SetFloat("_Saturation", num);
		m_MaterialWindy.SetFloat("_Saturation", num);
		SetAllocatedMaterialsDesat(num);
		num = 1f;
		if (BuildingsDesaturated)
		{
			num = 0f;
		}
		m_MaterialBuilding.SetFloat("_Saturation", num);
		m_MaterialTransBuilding.SetFloat("_Saturation", num);
		DayNightManager.Instance.SetDesaturationActive(Desaturated || BuildingsDesaturated);
		LightManager.Instance.SetDesaturationActive(Desaturated || BuildingsDesaturated);
		GetMisc(MiscType.TileMapWalls, Dark: false).SetFloat("_Saturation", num);
		GetMisc(MiscType.TileMapBoundary, Dark: false).SetFloat("_Saturation", num);
		GetMisc(MiscType.TileMapWalls, Dark: true).SetFloat("_Saturation", num);
		GetMisc(MiscType.TileMapBoundary, Dark: true).SetFloat("_Saturation", num);
	}

	public static bool GetObjectTypeNeedsWindy(ObjectType NewType)
	{
		if (NewType == ObjectType.Grass || NewType == ObjectType.CropCotton || NewType == ObjectType.CropWheat || NewType == ObjectType.Bullrushes || NewType == ObjectType.FlowerWild || NewType == ObjectType.CropCarrot)
		{
			return true;
		}
		return false;
	}
}
