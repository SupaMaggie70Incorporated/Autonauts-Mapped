using System.Collections.Generic;
using Dummiesman;
using UnityEngine;
using UnityEngine.Rendering;

public class ModelManager : MonoBehaviour
{
	private class TextureInfo
	{
		public Texture m_Texture;

		public int m_RepeatsX;

		public int m_RepeatsY;

		public Rect m_Rect;

		public MeshFilter m_WorstFilter;

		public GameObject m_WorstObject;
	}

	public static ModelManager Instance;

	private static ObjectType m_TestObject = ObjectTypeList.m_Total;

	private static bool m_BigDetailLog = false;

	private static bool m_DetailLog = false;

	private static bool m_Log = false;

	private static bool m_LogWarningsRepeatTextures = false;

	private static bool m_LogErrors = true;

	private static bool m_DebugShowModels = false;

	private Dictionary<ObjectType, ModelInfo> m_Models;

	private Dictionary<string, GameObject> m_ExtraModels;

	private Dictionary<string, ProcessModelInfo> m_ModelsToProcess;

	private bool m_Initted;

	private Material m_MergeMaterial;

	private Material m_MergeMaterialTrans;

	private Dictionary<Texture, TextureInfo> m_TextureInfo;

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
		m_Models = new Dictionary<ObjectType, ModelInfo>();
		m_ExtraModels = new Dictionary<string, GameObject>();
		m_ModelsToProcess = new Dictionary<string, ProcessModelInfo>();
		m_Initted = false;
		if (TextureAtlasManager.Instance != null)
		{
			Init();
		}
	}

	private GameObject LoadNew(string Name, ObjectType NewType = ObjectType.Nothing)
	{
		GameObject gameObject = null;
		bool flag = false;
		if (NewType >= ObjectType.Total && NewType < ObjectTypeList.m_Total)
		{
			string ModelName = "";
			gameObject = ((!ModManager.Instance.IsModelUsingCustomModel(NewType, out ModelName)) ? ((GameObject)Resources.Load(ModelName, typeof(GameObject))) : ModManager.Instance.GetModModel(NewType));
			if (gameObject == null)
			{
				ErrorMessage.LogError("Couldn't find model " + Name);
				return null;
			}
			Name = gameObject.name;
			flag = true;
		}
		else
		{
			gameObject = (GameObject)Resources.Load(Name, typeof(GameObject));
			if (gameObject == null)
			{
				ErrorMessage.LogError("Couldn't find model " + Name);
				return null;
			}
		}
		GameObject gameObject2 = Object.Instantiate(gameObject, base.transform);
		gameObject2.name = Name;
		if (!flag)
		{
			Bounds bounds = ObjectUtils.ObjectBounds(gameObject2);
			if (bounds.size.x > 80f || bounds.size.y > 80f || bounds.size.z > 80f)
			{
				ErrorMessage.LogError("Model " + Name + " is too big");
			}
		}
		if (!m_DebugShowModels)
		{
			gameObject2.SetActive(value: false);
		}
		return gameObject2;
	}

	public GameObject Load(ObjectType NewType, string Name, bool RandomVariants)
	{
		if (NewType >= ObjectType.Total && NewType < ObjectTypeList.m_Total)
		{
			string ModelName = "";
			if (ModManager.Instance.IsModelUsingCustomModel(NewType, out ModelName))
			{
				return ModManager.Instance.GetModModel(NewType);
			}
			return (GameObject)Resources.Load(ModelName, typeof(GameObject));
		}
		if (NewType != ObjectTypeList.m_Total)
		{
			if (!m_Models.ContainsKey(NewType))
			{
				m_Models.Add(NewType, new ModelInfo(RandomVariants: false));
			}
			ModelInfo modelInfo = m_Models[NewType];
			if (RandomVariants && modelInfo.m_VariantModels.Count > 0)
			{
				List<GameObject> variantModels = modelInfo.m_VariantModels;
				int index = Random.Range(0, variantModels.Count);
				return variantModels[index];
			}
			return m_ExtraModels[Name];
		}
		return m_ExtraModels[Name];
	}

	public GameObject Instantiate(ObjectType NewType, string Name, Transform NewParent, bool RandomVariants)
	{
		GameObject gameObject = null;
		if (!m_Initted)
		{
			if (NewType >= ObjectType.Total && NewType < ObjectTypeList.m_Total)
			{
				string ModelName = "";
				if (ModManager.Instance.IsModelUsingCustomModel(NewType, out ModelName))
				{
					gameObject = ModManager.Instance.GetModModel(NewType);
					if (gameObject == null)
					{
						ErrorMessage.LogError("Custom Model cannot be loaded/found - please check LUA");
						gameObject = (GameObject)Resources.Load(Name, typeof(GameObject));
					}
				}
				else
				{
					gameObject = (GameObject)Resources.Load(ModelName, typeof(GameObject));
				}
			}
			else
			{
				gameObject = (GameObject)Resources.Load(Name, typeof(GameObject));
			}
		}
		else if (NewType >= ObjectType.Total && NewType < ObjectTypeList.m_Total)
		{
			string ModelName2 = "";
			if (ModManager.Instance.IsModelUsingCustomModel(NewType, out ModelName2))
			{
				gameObject = ModManager.Instance.GetModModel(NewType);
				gameObject.gameObject.AddComponent<MeshCollider>();
				gameObject.gameObject.GetComponent<MeshCollider>().sharedMesh = gameObject.gameObject.GetComponentInChildren<MeshFilter>().mesh;
			}
			else
			{
				gameObject = Load(NewType, ModelName2, RandomVariants);
			}
		}
		else
		{
			gameObject = Load(NewType, Name, RandomVariants);
		}
		if (gameObject == null)
		{
			ErrorMessage.LogError("Invalid object type " + Name);
		}
		GameObject obj = Object.Instantiate(gameObject, default(Vector3), Quaternion.identity, NewParent);
		obj.name = gameObject.name;
		obj.SetActive(value: true);
		return obj;
	}

	public void AddModel(string Name, ObjectType NewType, bool RandomVariants = false, bool ForceBuilding = false)
	{
		if (GetTypeProcess(NewType, Name))
		{
			if (!m_ModelsToProcess.ContainsKey(Name))
			{
				m_ModelsToProcess.Add(Name, new ProcessModelInfo(NewType, RandomVariants, ForceBuilding));
			}
			return;
		}
		GameObject gameObject = LoadNew(Name);
		if (!m_Models.ContainsKey(NewType))
		{
			m_Models.Add(NewType, new ModelInfo(RandomVariants: false));
		}
		m_Models[NewType].m_VariantModels.Add(gameObject);
		m_ExtraModels[Name] = gameObject;
	}

	private void AddModelType(ObjectType NewType)
	{
		string modelNameFromIdentifier = ObjectTypeList.Instance.GetModelNameFromIdentifier(NewType);
		if (!(modelNameFromIdentifier != ""))
		{
			return;
		}
		if (NewType >= ObjectType.Total && NewType < ObjectTypeList.m_Total)
		{
			GameObject gameObject = LoadNew(modelNameFromIdentifier, NewType);
			if (!m_Models.ContainsKey(NewType))
			{
				m_Models.Add(NewType, new ModelInfo(RandomVariants: false));
			}
			m_Models[NewType].m_VariantModels.Add(gameObject);
			m_ExtraModels[modelNameFromIdentifier] = gameObject;
		}
		else if (GetTypeProcess(NewType, ""))
		{
			AddModel(modelNameFromIdentifier, NewType, RandomVariants: true);
		}
		else
		{
			GameObject gameObject2 = LoadNew(modelNameFromIdentifier);
			if (!m_Models.ContainsKey(NewType))
			{
				m_Models.Add(NewType, new ModelInfo(RandomVariants: false));
			}
			m_Models[NewType].m_VariantModels.Add(gameObject2);
			m_ExtraModels[modelNameFromIdentifier] = gameObject2;
		}
	}

	public void MakeModelList()
	{
		if (ObjectTypeList.Instance == null)
		{
			new ObjectTypeList();
		}
		if (m_TestObject != ObjectTypeList.m_Total)
		{
			AddModelType(m_TestObject);
		}
		else
		{
			for (int i = 0; i < 673; i++)
			{
				ObjectType newType = (ObjectType)i;
				AddModelType(newType);
			}
		}
		AddModel("Models/Buildings/BuildingAccessPoint", ObjectTypeList.m_Total, RandomVariants: false, ForceBuilding: true);
		AddModel("Models/Buildings/BuildingAccessPointIn", ObjectTypeList.m_Total, RandomVariants: false, ForceBuilding: true);
		AddModel("Models/Buildings/BuildingSpawnPoint", ObjectTypeList.m_Total, RandomVariants: false, ForceBuilding: true);
		AddModel("Models/Buildings/BuildingBlueprintTile", ObjectTypeList.m_Total, RandomVariants: false, ForceBuilding: true);
		AddModel("Models/Special/MechanicalPulley", ObjectTypeList.m_Total, RandomVariants: false, ForceBuilding: true);
		AddModel("Models/Special/ResearchVessel", ObjectTypeList.m_Total);
	}

	private void GetUVBounds(Vector2[] UVs, int[] NewIndices, out int iUMin, out int iVMin, out int iUMax, out int iVMax)
	{
		float num = 0f;
		float num2 = 1f;
		float num3 = 0f;
		float num4 = 1f;
		if (UVs.Length != 0)
		{
			num = 10000f;
			num2 = -10000f;
			num3 = 10000f;
			num4 = -10000f;
			foreach (int num5 in NewIndices)
			{
				if (num5 >= UVs.Length)
				{
					Debug.Log("***** " + num5 + " " + UVs.Length);
				}
				if (num > UVs[num5].x)
				{
					num = UVs[num5].x;
				}
				if (num2 < UVs[num5].x)
				{
					num2 = UVs[num5].x;
				}
				if (num3 > UVs[num5].y)
				{
					num3 = UVs[num5].y;
				}
				if (num4 < UVs[num5].y)
				{
					num4 = UVs[num5].y;
				}
			}
		}
		float num6 = 0.01f;
		if (Mathf.Abs(num) < num6)
		{
			num = 0f;
		}
		if (Mathf.Abs(num2 - 1f) < num6)
		{
			num2 = 1f;
		}
		if (Mathf.Abs(num3) < num6)
		{
			num3 = 0f;
		}
		if (Mathf.Abs(num4 - 1f) < num6)
		{
			num4 = 1f;
		}
		if (m_DetailLog)
		{
			Debug.Log("        UV Bounds : " + num + "," + num3 + " " + num2 + "," + num4);
		}
		iUMin = Mathf.FloorToInt(num);
		iUMax = Mathf.CeilToInt(num2);
		iVMin = Mathf.FloorToInt(num3);
		iVMax = Mathf.CeilToInt(num4);
		if (m_DetailLog)
		{
			Debug.Log("        UV Rounded : " + iUMin + "," + iVMin + " " + iUMax + "," + iVMax);
		}
	}

	private bool GetIncludeMesh(MeshFilter NewFilter, Material NewMaterial, bool DebugLog, string ModelName)
	{
		if (NewMaterial == null)
		{
			return false;
		}
		if (NewFilter.name == "Blueprint" || NewMaterial.name == "Blueprint" || NewMaterial.name == "SignText" || NewFilter.name == "Face" || NewMaterial.name.Contains("CertificateSign") || NewMaterial.name.Contains("StorageGeneric") || NewMaterial.name.Contains("StorageLiquid") || NewMaterial.name.Contains("Screen"))
		{
			if (DebugLog && m_DetailLog)
			{
				Debug.Log("Model " + ModelName + " not merged because it has a material called 'blueprint' or 'StorageGeneric' : " + NewMaterial.name);
			}
			return false;
		}
		if (NewMaterial.IsKeywordEnabled("_EMISSION"))
		{
			if (DebugLog && m_DetailLog)
			{
				Debug.Log("Model " + ModelName + " not merged because it has an emmisive material : " + NewMaterial.name);
			}
			return false;
		}
		return true;
	}

	private void ProcessModel(GameObject NewModel)
	{
		if (m_Log)
		{
			Debug.Log("ProcessModel " + NewModel.name);
		}
		MeshFilter[] componentsInChildren = NewModel.GetComponentsInChildren<MeshFilter>();
		foreach (MeshFilter meshFilter in componentsInChildren)
		{
			Mesh mesh = meshFilter.mesh;
			if (m_DetailLog)
			{
				Debug.Log("    Processing mesh " + meshFilter.gameObject.name + " : " + mesh.subMeshCount);
			}
			MeshRenderer component = meshFilter.GetComponent<MeshRenderer>();
			Vector2[] uv = mesh.uv;
			for (int j = 0; j < mesh.subMeshCount; j++)
			{
				if (!GetIncludeMesh(meshFilter, component.sharedMaterials[j], DebugLog: false, NewModel.name))
				{
					continue;
				}
				Texture mainTexture = component.sharedMaterials[j].mainTexture;
				if (component.sharedMaterials[j] == null)
				{
					if (m_LogErrors)
					{
						Debug.Log("*** No material *** model : " + NewModel.name + "  " + meshFilter.name + " submesh : " + j);
					}
					continue;
				}
				if (mainTexture == null)
				{
					if (m_LogErrors)
					{
						Debug.Log("*** No texture map *** model : " + NewModel.name + "  Mesh : " + meshFilter.name + " Submesh : " + j + " material : " + component.sharedMaterials[j].name);
					}
					continue;
				}
				if (m_DetailLog)
				{
					Debug.Log("        Processing sub mesh " + j + " : " + mainTexture.name);
				}
				int[] indices = mesh.GetIndices(j);
				if (m_DetailLog)
				{
					Debug.Log("            " + indices.Length + " Indices");
				}
				GetUVBounds(uv, indices, out var iUMin, out var iVMin, out var iUMax, out var iVMax);
				int num = iUMax - iUMin;
				int num2 = iVMax - iVMin;
				if (num < 1)
				{
					num = 1;
				}
				if (num2 < 1)
				{
					num2 = 1;
				}
				if (m_DetailLog)
				{
					Debug.Log("            UV Repeats : " + num + "," + num2);
				}
				TextureInfo textureInfo;
				if (!m_TextureInfo.ContainsKey(mainTexture))
				{
					textureInfo = new TextureInfo();
					textureInfo.m_Texture = mainTexture;
					textureInfo.m_RepeatsX = num;
					textureInfo.m_RepeatsY = num2;
					textureInfo.m_WorstFilter = meshFilter;
					textureInfo.m_WorstObject = NewModel;
					m_TextureInfo.Add(mainTexture, textureInfo);
					continue;
				}
				textureInfo = m_TextureInfo[mainTexture];
				if (textureInfo.m_RepeatsX < num)
				{
					textureInfo.m_RepeatsX = num;
					textureInfo.m_WorstFilter = meshFilter;
					textureInfo.m_WorstObject = NewModel;
				}
				if (textureInfo.m_RepeatsY < num2)
				{
					textureInfo.m_RepeatsY = num2;
					textureInfo.m_WorstFilter = meshFilter;
					textureInfo.m_WorstObject = NewModel;
				}
			}
		}
	}

	private Texture2D CreateRepeatingTexture(Texture2D OldTexture, int RepeatsX, int RepeatsY, GameObject WorstObject, MeshFilter WorstFilter)
	{
		if (m_LogWarningsRepeatTextures)
		{
			Debug.Log("*** Repeating texture *** " + OldTexture.name + " for Model " + WorstObject.name + " Mesh " + WorstFilter.name + " : " + RepeatsX + "," + RepeatsY + " at " + OldTexture.width + "," + OldTexture.height + " = " + RepeatsX * OldTexture.width + "," + RepeatsY * OldTexture.height);
		}
		Texture2D texture2D = new Texture2D(OldTexture.width * RepeatsX, OldTexture.height * RepeatsY, OldTexture.format, mipChain: false);
		texture2D.name = OldTexture.name + " (Repeating)";
		for (int i = 0; i < RepeatsY; i++)
		{
			for (int j = 0; j < RepeatsX; j++)
			{
				Graphics.CopyTexture(OldTexture, 0, 0, 0, 0, OldTexture.width, OldTexture.height, texture2D, 0, 0, j * OldTexture.width, i * OldTexture.height);
			}
		}
		return texture2D;
	}

	private void BuildTextures()
	{
		if (m_Log)
		{
			Debug.Log("Total Texture Infos " + m_TextureInfo.Count);
		}
		List<Texture2D> list = new List<Texture2D>();
		foreach (KeyValuePair<Texture, TextureInfo> item in m_TextureInfo)
		{
			TextureInfo value = item.Value;
			Texture2D texture2D = (Texture2D)value.m_Texture;
			if (m_DetailLog)
			{
				Debug.Log(texture2D.name + " " + value.m_RepeatsX + "," + value.m_RepeatsY);
			}
			Texture2D texture2D2 = texture2D;
			if (value.m_RepeatsX != 1 || value.m_RepeatsY != 1)
			{
				texture2D2 = CreateRepeatingTexture(texture2D, value.m_RepeatsX, value.m_RepeatsY, value.m_WorstObject, value.m_WorstFilter);
				list.Add(texture2D2);
			}
			TextureAtlasManager.Instance.AddTexture(texture2D2);
		}
		TextureAtlasManager.Instance.Finish();
		Rect[] rectangleResults = TextureAtlasManager.Instance.m_RectangleResults;
		int num = 0;
		foreach (KeyValuePair<Texture, TextureInfo> item2 in m_TextureInfo)
		{
			TextureInfo value2 = item2.Value;
			value2.m_Rect = rectangleResults[num];
			if (m_DetailLog)
			{
				Debug.Log(item2.Key.name + " " + value2.m_Rect);
			}
			num++;
		}
		foreach (Texture2D item3 in list)
		{
			Object.Destroy(item3);
		}
		UpdateSnowTextures();
	}

	private bool GetIsSnowTexture(string Name)
	{
		string[] array = new string[8] { "Bush01", "Bush02", "Bush03", "Lichen", "Tree01", "Tree02", "Tree03", "Tree04" };
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i] == Name)
			{
				return true;
			}
		}
		return false;
	}

	public void UpdateSnowTextures()
	{
	}

	private void MergeMeshes(GameObject NewModel)
	{
		if (m_DetailLog)
		{
			Debug.Log("MergeMeshes " + NewModel.name);
		}
		List<CombineInstance> list = new List<CombineInstance>();
		MeshFilter[] componentsInChildren = NewModel.GetComponentsInChildren<MeshFilter>();
		MeshRenderer[] componentsInChildren2 = NewModel.GetComponentsInChildren<MeshRenderer>();
		MeshFilter[] array = componentsInChildren;
		foreach (MeshFilter meshFilter in array)
		{
			Mesh mesh = meshFilter.mesh;
			MeshRenderer component = meshFilter.GetComponent<MeshRenderer>();
			for (int j = 0; j < mesh.subMeshCount; j++)
			{
				CombineInstance item = default(CombineInstance);
				item.mesh = mesh;
				item.subMeshIndex = j;
				item.transform = meshFilter.transform.localToWorldMatrix;
				list.Add(item);
			}
			Object.Destroy(component);
			Object.Destroy(meshFilter);
		}
		MeshFilter meshFilter2 = NewModel.AddComponent<MeshFilter>();
		meshFilter2.mesh = new Mesh();
		meshFilter2.mesh.CombineMeshes(list.ToArray(), mergeSubMeshes: true);
		NewModel.AddComponent<MeshRenderer>().sharedMaterials = new Material[1] { m_MergeMaterial };
		MeshRenderer[] array2 = componentsInChildren2;
		for (int i = 0; i < array2.Length; i++)
		{
			Object.Destroy(array2[i]);
		}
		array = componentsInChildren;
		for (int i = 0; i < array.Length; i++)
		{
			Object.Destroy(array[i]);
		}
	}

	private bool AdjustModel(GameObject NewModel)
	{
		bool result = true;
		float halfTexelOffset = TextureAtlasManager.Instance.m_HalfTexelOffset;
		if (m_Log)
		{
			Debug.Log("AdjustModel " + NewModel.name);
		}
		MeshFilter[] componentsInChildren = NewModel.GetComponentsInChildren<MeshFilter>();
		foreach (MeshFilter meshFilter in componentsInChildren)
		{
			Mesh mesh = meshFilter.mesh;
			if (m_DetailLog)
			{
				Debug.Log("    Processing mesh " + meshFilter.gameObject.name + " : " + mesh.subMeshCount);
			}
			MeshRenderer component = meshFilter.GetComponent<MeshRenderer>();
			Vector2[] uv = mesh.uv;
			Vector2[] array = new Vector2[mesh.vertices.Length];
			Material[] array2 = new Material[component.sharedMaterials.Length];
			Dictionary<int, int> dictionary = new Dictionary<int, int>();
			bool flag = false;
			for (int j = 0; j < mesh.subMeshCount; j++)
			{
				int[] indices = mesh.GetIndices(j);
				if (!GetIncludeMesh(meshFilter, component.sharedMaterials[j], DebugLog: true, NewModel.name))
				{
					if (m_DetailLog)
					{
						Debug.Log("        Not processing sub mesh " + j);
					}
					array2[j] = component.sharedMaterials[j];
					result = false;
					if (uv.Length != 0)
					{
						if (m_DetailLog)
						{
							Debug.Log("              Copying UVs");
						}
						foreach (int num in indices)
						{
							array[num].x = uv[num].x;
							array[num].y = uv[num].y;
						}
					}
					continue;
				}
				if (StandardShaderUtils.GetRenderMode(component.sharedMaterials[j]) == StandardShaderUtils.BlendMode.Transparent)
				{
					array2[j] = m_MergeMaterialTrans;
					result = false;
				}
				else
				{
					array2[j] = m_MergeMaterial;
				}
				Texture mainTexture = component.sharedMaterials[j].mainTexture;
				if (!mainTexture)
				{
					continue;
				}
				if (m_DetailLog)
				{
					Debug.Log("        Processing sub mesh " + j + " : " + mainTexture.name);
				}
				if (m_DetailLog)
				{
					Debug.Log("            " + indices.Length + " Indices");
				}
				TextureInfo textureInfo = m_TextureInfo[mainTexture];
				Rect rect = textureInfo.m_Rect;
				GetUVBounds(uv, indices, out var iUMin, out var iVMin, out var _, out var _);
				float width = rect.width;
				float height = rect.height;
				float num2 = rect.width / (float)textureInfo.m_RepeatsX;
				float num3 = rect.height / (float)textureInfo.m_RepeatsY;
				float num4 = num2 * (float)(-iUMin) + rect.x;
				float num5 = num3 * (float)(-iVMin) + rect.y;
				float num6 = 1f / (float)textureInfo.m_RepeatsX;
				float num7 = 1f / (float)textureInfo.m_RepeatsY;
				if (uv.Length != 0)
				{
					if (m_DetailLog)
					{
						Debug.Log("              Updating UVs");
					}
					Dictionary<int, int> dictionary2 = new Dictionary<int, int>();
					foreach (int num8 in indices)
					{
						if (!dictionary2.ContainsKey(num8))
						{
							if (m_BigDetailLog)
							{
								Debug.Log("Index " + num8 + " " + uv[num8].x + "," + uv[num8].y);
							}
							array[num8].x = uv[num8].x * num6 * width + num4;
							array[num8].y = uv[num8].y * num7 * height + num5;
							if (uv[num8].x == 0f)
							{
								array[num8].x += halfTexelOffset;
							}
							else if (uv[num8].x == 1f)
							{
								array[num8].x -= halfTexelOffset;
							}
							if (uv[num8].y == 0f)
							{
								array[num8].y += halfTexelOffset;
							}
							else if (uv[num8].y == 1f)
							{
								array[num8].y -= halfTexelOffset;
							}
							dictionary2.Add(num8, 0);
						}
					}
					foreach (KeyValuePair<int, int> item in dictionary2)
					{
						if (!dictionary.ContainsKey(item.Key))
						{
							dictionary.Add(item.Key, 0);
						}
						else
						{
							flag = true;
						}
					}
				}
				else
				{
					if (m_DetailLog)
					{
						Debug.Log("              New UVs " + mesh.name + " " + j);
					}
					foreach (int num9 in indices)
					{
						array[num9].x = rect.x + halfTexelOffset;
						array[num9].y = rect.y + halfTexelOffset;
					}
				}
			}
			if (flag)
			{
				Debug.Log("*** Shared vertices between materials *** Model=" + NewModel.name + " Mesh=" + meshFilter.gameObject.name);
			}
			component.sharedMaterials = array2;
			mesh.uv = array;
		}
		return result;
	}

	private bool GetTypeProcess(ObjectType NewType, string Name)
	{
		if (NewType == ObjectType.WorkerLookAt)
		{
			return false;
		}
		if (Name == "Models/Special/ResearchVessel")
		{
			return false;
		}
		return true;
	}

	private bool GetTypeMerge(ObjectType NewType)
	{
		if ((ObjectTypeList.Instance.GetIsBuilding(NewType) && !Wall.GetIsTypeWall(NewType) && !Floor.GetIsTypeFloor(NewType) && !Window.GetIsTypeWindow(NewType) && NewType != ObjectType.StorageBeehive && NewType != ObjectType.StorageBeehiveCrude) || ToolFillable.GetIsTypeFillable(NewType) || Animal.GetIsTypeAnimal(NewType) || WorkerDrive.GetIsTypeDrive(NewType) || Vehicle.GetIsTypeVehicle(NewType) || NewType == ObjectType.Folk || NewType == ObjectType.WorkerLookAt || NewType == ObjectType.PotClay || NewType == ObjectType.TreePine || NewType == ObjectType.TreeApple || NewType == ObjectType.TreeCoconut || NewType == ObjectType.TreeMulberry || NewType == ObjectType.Bush || NewType == ObjectType.ToolFlail || NewType == ObjectType.ToolFlailCrude || NewType == ObjectType.Canoe || NewType == ObjectType.FlowerPot || NewType == ObjectType.CertificateReward)
		{
			return false;
		}
		return true;
	}

	private void LoadModel(string ModelName, ProcessModelInfo NewInfo)
	{
		if (m_DetailLog)
		{
			Debug.Log("Load " + ModelName);
		}
		GameObject gameObject = LoadNew(ModelName);
		ProcessModel(gameObject);
		NewInfo.m_Model = gameObject;
	}

	private void LoadAllModels()
	{
		foreach (KeyValuePair<string, ProcessModelInfo> item in m_ModelsToProcess)
		{
			LoadModel(item.Key, item.Value);
		}
	}

	private void AdjustModels()
	{
		foreach (KeyValuePair<string, ProcessModelInfo> item in m_ModelsToProcess)
		{
			if (item.Key == "Mod")
			{
				continue;
			}
			string key = item.Key;
			ProcessModelInfo value = item.Value;
			ObjectType type = value.m_Type;
			GameObject model = value.m_Model;
			m_MergeMaterial = MaterialManager.Instance.m_Material;
			m_MergeMaterialTrans = MaterialManager.Instance.m_MaterialTrans;
			if (MaterialManager.GetObjectTypeNeedsWindy(type))
			{
				m_MergeMaterial = MaterialManager.Instance.m_MaterialWindy;
			}
			if ((type != ObjectTypeList.m_Total && ObjectTypeList.Instance.GetIsBuilding(type)) || value.m_ForceBuilding)
			{
				m_MergeMaterial = MaterialManager.Instance.m_MaterialBuilding;
				m_MergeMaterialTrans = MaterialManager.Instance.m_MaterialTransBuilding;
			}
			if (AdjustModel(model) && GetTypeMerge(type))
			{
				MergeMeshes(model);
			}
			if (type != ObjectTypeList.m_Total)
			{
				if (!m_Models.ContainsKey(type))
				{
					m_Models.Add(type, new ModelInfo(RandomVariants: false));
				}
				if (value.m_RandomVariants)
				{
					m_Models[type].m_VariantModels.Add(model);
				}
			}
			m_ExtraModels.Add(key, model);
		}
	}

	private void MergeMesh(MeshRenderer NewRenderer)
	{
		Material[] sharedMaterials = NewRenderer.sharedMaterials;
		List<Material> list = new List<Material>();
		foreach (Material item in sharedMaterials)
		{
			if (!list.Contains(item))
			{
				list.Add(item);
			}
		}
		Material[] array = new Material[list.Count];
		list.CopyTo(array);
		NewRenderer.sharedMaterials = array;
		MeshFilter component = NewRenderer.GetComponent<MeshFilter>();
		Mesh mesh = component.mesh;
		component.mesh = new Mesh();
		int num = array.Length;
		List<CombineInstance>[] array2 = new List<CombineInstance>[num];
		for (int j = 0; j < num; j++)
		{
			array2[j] = new List<CombineInstance>();
		}
		for (int k = 0; k < sharedMaterials.Length; k++)
		{
			int num2 = list.IndexOf(sharedMaterials[k]);
			CombineInstance item2 = default(CombineInstance);
			item2.mesh = mesh;
			item2.subMeshIndex = k;
			item2.transform = Matrix4x4.identity;
			array2[num2].Add(item2);
		}
		CombineInstance[] array3 = new CombineInstance[num];
		Mesh[] array4 = new Mesh[num];
		for (int l = 0; l < num; l++)
		{
			array4[l] = new Mesh();
			array4[l].indexFormat = IndexFormat.UInt16;
			array4[l].CombineMeshes(array2[l].ToArray());
			array3[l].mesh = array4[l];
			array3[l].subMeshIndex = 0;
			array3[l].transform = Matrix4x4.identity;
		}
		component.mesh.indexFormat = IndexFormat.UInt16;
		component.mesh.CombineMeshes(array3, mergeSubMeshes: false);
	}

	private void MergeNodes()
	{
		foreach (KeyValuePair<ObjectType, ModelInfo> model in m_Models)
		{
			foreach (GameObject variantModel in model.Value.m_VariantModels)
			{
				MeshRenderer[] componentsInChildren = variantModel.GetComponentsInChildren<MeshRenderer>();
				foreach (MeshRenderer newRenderer in componentsInChildren)
				{
					MergeMesh(newRenderer);
				}
			}
		}
	}

	public void Init()
	{
		m_Initted = true;
		m_TextureInfo = new Dictionary<Texture, TextureInfo>();
		if (m_Log)
		{
			Debug.Log("*** Loading Models ***");
		}
		LoadAllModels();
		if (m_Log)
		{
			Debug.Log("*** Building Textures ***");
		}
		BuildTextures();
		if (m_Log)
		{
			Debug.Log("*** Adjusting Models ***");
		}
		AdjustModels();
		if (m_Log)
		{
			Debug.Log("*** Merge Nodes ***");
		}
		MergeNodes();
		m_TextureInfo = null;
		if (m_Log)
		{
			Debug.Log("*** All done with models ***");
		}
	}

	public void SetMaterialsNode(Transform OriginalCurrentNode, Transform NewCurrentNode, Material NewMaterial, Material NewMaterialTrans)
	{
		if ((bool)OriginalCurrentNode.GetComponent<MeshRenderer>())
		{
			MeshRenderer component = NewCurrentNode.GetComponent<MeshRenderer>();
			Material[] array = new Material[component.sharedMaterials.Length];
			for (int i = 0; i < array.Length; i++)
			{
				Material material = component.sharedMaterials[i];
				if ((material == MaterialManager.Instance.m_Material || material == MaterialManager.Instance.m_MaterialBuilding || material == MaterialManager.Instance.m_MaterialOccluded || material == MaterialManager.Instance.m_MaterialHighlight) && material != NewMaterial)
				{
					array[i] = NewMaterial;
				}
				else if ((material == MaterialManager.Instance.m_MaterialTrans || material == MaterialManager.Instance.m_MaterialTransBuilding || material == MaterialManager.Instance.m_MaterialTransOccluded || material == MaterialManager.Instance.m_MaterialTransHighlight) && material != NewMaterialTrans)
				{
					array[i] = NewMaterialTrans;
				}
				else
				{
					array[i] = material;
				}
			}
			component.sharedMaterials = array;
		}
		for (int j = 0; j < OriginalCurrentNode.transform.childCount; j++)
		{
			SetMaterialsNode(OriginalCurrentNode.GetChild(j), NewCurrentNode.GetChild(j), NewMaterial, NewMaterialTrans);
		}
	}

	public void SetMaterials(BaseClass NewObject, Material NewMaterial, Material NewMaterialTrans)
	{
		string modelName = NewObject.m_ModelName;
		GameObject gameObject = Load(NewObject.m_TypeIdentifier, modelName, RandomVariants: false);
		SetMaterialsNode(gameObject.transform, NewObject.m_ModelRoot.transform, NewMaterial, NewMaterialTrans);
	}

	public void RestoreStandardMaterialsNode(Transform OriginalCurrentNode, Transform NewCurrentNode)
	{
		MeshRenderer component = OriginalCurrentNode.GetComponent<MeshRenderer>();
		if ((bool)component)
		{
			MeshRenderer component2 = NewCurrentNode.GetComponent<MeshRenderer>();
			if (component.sharedMaterials.Length != 1 || (component.sharedMaterials[0] != null && (bool)component.sharedMaterials[0].mainTexture && !component.sharedMaterials[0].mainTexture.isReadable && component.sharedMaterials[0] != MaterialManager.Instance.m_MaterialTransBuildingBlueprint))
			{
				component2.sharedMaterials = component.sharedMaterials;
			}
		}
		for (int i = 0; i < OriginalCurrentNode.transform.childCount; i++)
		{
			RestoreStandardMaterialsNode(OriginalCurrentNode.GetChild(i), NewCurrentNode.GetChild(i));
		}
	}

	public void RestoreStandardMaterials(BaseClass NewObject)
	{
		if (NewObject == null || NewObject.m_ModelRoot == null)
		{
			return;
		}
		string modelName = NewObject.m_ModelName;
		if (modelName == null || modelName == "")
		{
			return;
		}
		GameObject gameObject = Load(NewObject.m_TypeIdentifier, modelName, RandomVariants: false);
		if (gameObject == null)
		{
			Debug.Log(string.Concat("Can't find prefab for ", NewObject.m_TypeIdentifier, " ", modelName));
		}
		Building component = NewObject.GetComponent<Building>();
		if (component != null && component.m_TypeIdentifier >= ObjectType.Total && component.m_TypeIdentifier < ObjectTypeList.m_Total)
		{
			string ModelName = "";
			if (ModManager.Instance.IsModelUsingCustomModel(component.m_TypeIdentifier, out ModelName))
			{
				ModCustom customModInfo = ModManager.Instance.GetCustomModInfo(component.m_TypeIdentifier);
				List<Material> value = new List<Material>();
				bool flag = false;
				if (customModInfo != null)
				{
					flag = customModInfo.ModOriginalMaterials.TryGetValue(component.m_TypeIdentifier, out value);
				}
				MeshRenderer[] componentsInChildren;
				if (flag)
				{
					int num = 0;
					componentsInChildren = NewObject.GetComponentsInChildren<MeshRenderer>(includeInactive: true);
					foreach (MeshRenderer meshRenderer in componentsInChildren)
					{
						if (meshRenderer.name.Equals("Models/Buildings/BuildingAccessPointIn") || meshRenderer.name.Equals("Models/Buildings/BuildingSpawnPoint"))
						{
							continue;
						}
						for (int j = 0; j < meshRenderer.materials.Length; j++)
						{
							if (num < value.Count && value[num] != null)
							{
								OBJLoaderHelper.DisableMaterialTransparency(meshRenderer.materials[j]);
								meshRenderer.materials[j].color = new Color(value[num].color.r, value[num].color.g, value[num].color.b, 1f);
							}
							num++;
						}
					}
					return;
				}
				componentsInChildren = NewObject.GetComponentsInChildren<MeshRenderer>(includeInactive: true);
				foreach (MeshRenderer meshRenderer2 in componentsInChildren)
				{
					for (int k = 0; k < meshRenderer2.materials.Length; k++)
					{
						OBJLoaderHelper.DisableMaterialTransparency(meshRenderer2.materials[k]);
						meshRenderer2.materials[k].color = new Color(meshRenderer2.materials[k].color.r, meshRenderer2.materials[k].color.g, meshRenderer2.materials[k].color.b, 1f);
					}
				}
				return;
			}
		}
		RestoreStandardMaterialsNode(gameObject.transform, NewObject.m_ModelRoot.transform);
	}

	public void RestoreStandardMaterials(ObjectType NewType, GameObject NewObject, string ModelName)
	{
		GameObject gameObject = Load(NewType, ModelName, RandomVariants: false);
		RestoreStandardMaterialsNode(gameObject.transform, NewObject.transform);
	}

	public void RestoreStandardMaterialsChild(BaseClass NewObject, string ChildName)
	{
		string modelName = NewObject.m_ModelName;
		if (modelName != null && !(modelName == ""))
		{
			GameObject gameObject = Load(NewObject.m_TypeIdentifier, modelName, RandomVariants: false);
			gameObject = gameObject.transform.Find(ChildName).gameObject;
			GameObject gameObject2 = NewObject.m_ModelRoot.transform.Find(ChildName).gameObject;
			RestoreStandardMaterialsNode(gameObject.transform, gameObject2.transform);
		}
	}

	public void SetMaterialsTransparent(GameObject NewObject, bool Transparent)
	{
		Building component = NewObject.GetComponent<Building>();
		MeshRenderer[] componentsInChildren;
		if (component != null && component.m_TypeIdentifier >= ObjectType.Total && component.m_TypeIdentifier < ObjectTypeList.m_Total)
		{
			string ModelName = "";
			if (ModManager.Instance.IsModelUsingCustomModel(component.m_TypeIdentifier, out ModelName))
			{
				ModManager.Instance.GetModModel(component.m_TypeIdentifier);
				componentsInChildren = NewObject.GetComponentsInChildren<MeshRenderer>(includeInactive: true);
				foreach (MeshRenderer meshRenderer in componentsInChildren)
				{
					for (int j = 0; j < meshRenderer.materials.Length; j++)
					{
						OBJLoaderHelper.EnableMaterialTransparency(meshRenderer.materials[j]);
						meshRenderer.materials[j].color = new Color(meshRenderer.materials[j].color.r, meshRenderer.materials[j].color.g, meshRenderer.materials[j].color.b, 0.3f);
					}
				}
				return;
			}
		}
		Material material;
		Material material2;
		Material material3;
		Material material4;
		Material material5;
		Material material6;
		if (Transparent)
		{
			material = MaterialManager.Instance.m_Material;
			material2 = MaterialManager.Instance.m_MaterialTrans;
			material3 = MaterialManager.Instance.m_MaterialBuilding;
			material4 = MaterialManager.Instance.m_MaterialTransBuildingBlueprint;
			material5 = MaterialManager.Instance.m_MaterialTransBuilding;
			material6 = MaterialManager.Instance.m_MaterialTransBuildingBlueprint;
		}
		else
		{
			material = MaterialManager.Instance.m_MaterialTrans;
			material2 = MaterialManager.Instance.m_Material;
			material3 = MaterialManager.Instance.m_MaterialTransBuildingBlueprint;
			material4 = MaterialManager.Instance.m_MaterialBuilding;
			material5 = MaterialManager.Instance.m_MaterialTransBuildingBlueprint;
			material6 = MaterialManager.Instance.m_MaterialTransBuilding;
		}
		componentsInChildren = NewObject.GetComponentsInChildren<MeshRenderer>(includeInactive: true);
		foreach (MeshRenderer meshRenderer2 in componentsInChildren)
		{
			Material[] sharedMaterials = meshRenderer2.sharedMaterials;
			for (int k = 0; k < meshRenderer2.sharedMaterials.Length; k++)
			{
				if (sharedMaterials[k] == material)
				{
					sharedMaterials[k] = material2;
				}
				else if (sharedMaterials[k] == material3)
				{
					sharedMaterials[k] = material4;
				}
				else if (sharedMaterials[k] == material5)
				{
					sharedMaterials[k] = material6;
				}
			}
			meshRenderer2.sharedMaterials = sharedMaterials;
		}
	}

	private void MakeMaterialsHighlight(TileCoordObject NewObject, bool AndOccluded)
	{
		Building component = NewObject.GetComponent<Building>();
		if (component != null && component.m_TypeIdentifier >= ObjectType.Total && component.m_TypeIdentifier < ObjectTypeList.m_Total)
		{
			string ModelName = "";
			if (ModManager.Instance.IsModelUsingCustomModel(component.m_TypeIdentifier, out ModelName))
			{
				ModCustom customModInfo = ModManager.Instance.GetCustomModInfo(component.m_TypeIdentifier);
				List<Material> value = new List<Material>();
				bool flag = false;
				if (customModInfo != null && customModInfo.ModOriginalMaterials.ContainsKey(component.m_TypeIdentifier))
				{
					flag = customModInfo.ModOriginalMaterials.TryGetValue(component.m_TypeIdentifier, out value);
				}
				MeshRenderer[] componentsInChildren = NewObject.GetComponentsInChildren<MeshRenderer>(includeInactive: true);
				foreach (MeshRenderer meshRenderer in componentsInChildren)
				{
					if (meshRenderer.name.Equals("Models/Buildings/BuildingAccessPointIn") || meshRenderer.name.Equals("Models/Buildings/BuildingSpawnPoint"))
					{
						continue;
					}
					for (int j = 0; j < meshRenderer.materials.Length; j++)
					{
						if (!flag)
						{
							value.Add(new Material(meshRenderer.materials[j]));
						}
						meshRenderer.materials[j].color = new Color(1f, 1f, 0f, 1f);
					}
				}
				if (!flag)
				{
					customModInfo?.ModOriginalMaterials.Add(component.m_TypeIdentifier, value);
				}
				return;
			}
		}
		Material newMaterial = MaterialManager.Instance.m_MaterialHighlight;
		Material newMaterialTrans = MaterialManager.Instance.m_MaterialTransHighlight;
		if (AndOccluded)
		{
			newMaterial = MaterialManager.Instance.m_MaterialOccluded;
			newMaterialTrans = MaterialManager.Instance.m_MaterialTransOccluded;
		}
		string modelName = NewObject.m_ModelName;
		if (modelName != null && !(modelName == ""))
		{
			GameObject gameObject = Load(NewObject.m_TypeIdentifier, modelName, RandomVariants: false);
			SetMaterialsNode(gameObject.transform, NewObject.m_ModelRoot.transform, newMaterial, newMaterialTrans);
		}
	}

	public void MakeMaterialsUnique(GameObject NewObject)
	{
		MeshRenderer[] componentsInChildren = NewObject.GetComponentsInChildren<MeshRenderer>();
		foreach (MeshRenderer meshRenderer in componentsInChildren)
		{
			Material[] sharedMaterials = meshRenderer.sharedMaterials;
			Material[] array = new Material[sharedMaterials.Length];
			for (int j = 0; j < sharedMaterials.Length; j++)
			{
				if ((bool)sharedMaterials[j])
				{
					array[j] = Object.Instantiate(sharedMaterials[j]);
				}
			}
			meshRenderer.sharedMaterials = array;
		}
	}

	public void SetSearchTypesHighlight(List<ObjectType> SearchTypes, bool Desaturated, bool AndOccluded)
	{
		foreach (ObjectType SearchType in SearchTypes)
		{
			Plot[] plots = PlotManager.Instance.m_Plots;
			for (int i = 0; i < plots.Length; i++)
			{
				if (!plots[i].m_ObjectDictionary.ContainsKey(SearchType))
				{
					continue;
				}
				foreach (TileCoordObject item in plots[i].m_ObjectDictionary[SearchType])
				{
					if ((bool)item.m_ModelRoot && (!ObjectTypeList.Instance.GetIsBuilding(item.m_TypeIdentifier) || !item.GetComponent<Building>().m_Blueprint))
					{
						if (Desaturated)
						{
							MakeMaterialsHighlight(item, AndOccluded);
						}
						else
						{
							RestoreStandardMaterials(item);
						}
						item.MaterialsChanged();
					}
				}
			}
		}
	}
}
