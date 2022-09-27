using System;
using UnityEngine;

public class StoragePalette : Storage
{
	private GameObject[] m_DisplayedObjects;

	private int m_NumObjectsPerRow;

	private int m_NumRows;

	private int m_NumDepths;

	private int m_MaxObjects;

	private float m_ObjectSpacingX;

	private float m_ObjectSpacingY;

	private float m_ObjectSpacingZ;

	private GameObject m_ObjectModelPrefab;

	private int[][] m_TriangleData;

	private int[] m_SingleObjectTriangleCounts;

	private Mesh[] m_ObjectMesh;

	public static bool GetIsTypeStoragePalette(ObjectType NewType)
	{
		if (NewType == ObjectType.StoragePalette || NewType == ObjectType.StoragePaletteMedium)
		{
			return true;
		}
		return false;
	}

	public override void Restart()
	{
		base.Restart();
		SetDimensions(new TileCoord(0, -1), new TileCoord(1, 0), new TileCoord(0, 1));
	}

	protected new void Awake()
	{
		base.Awake();
		m_ObjectType = ObjectTypeList.m_Total;
		m_Capacity = 100;
		m_MaxLevels = 3;
		m_NumLevels = 1;
	}

	public override void StopUsing(bool AndDestroy = true)
	{
		base.StopUsing(AndDestroy);
		DestroyOldObjectMesh();
		m_ObjectType = ObjectTypeList.m_Total;
	}

	private void CreateRowTriangles(GameObject ModelPrefab)
	{
		Mesh mesh = ModelPrefab.GetComponentInChildren<MeshFilter>().mesh;
		int num = mesh.vertices.Length;
		m_TriangleData = new int[mesh.subMeshCount][];
		m_SingleObjectTriangleCounts = new int[mesh.subMeshCount];
		for (int i = 0; i < mesh.subMeshCount; i++)
		{
			int[] triangles = mesh.GetTriangles(i);
			int[] array = new int[triangles.Length * m_NumObjectsPerRow * m_NumDepths];
			int num2 = triangles.Length;
			m_SingleObjectTriangleCounts[i] = num2;
			for (int j = 0; j < m_NumDepths; j++)
			{
				for (int k = 0; k < m_NumObjectsPerRow; k++)
				{
					for (int l = 0; l < num2; l++)
					{
						array[l + k * num2 + j * (m_NumObjectsPerRow * num2)] = triangles[l] + k * num + j * (m_NumObjectsPerRow * num);
					}
				}
			}
			m_TriangleData[i] = array;
		}
	}

	private Mesh CreateRowMesh(GameObject NewObject, GameObject ModelPrefab)
	{
		GameObject gameObject = ModelPrefab.transform.GetChild(0).gameObject;
		if ((bool)ModelPrefab.GetComponent<MeshFilter>())
		{
			gameObject = ModelPrefab;
		}
		NewObject.transform.localRotation = gameObject.transform.localRotation;
		NewObject.transform.localScale = gameObject.transform.localScale;
		Mesh mesh = gameObject.GetComponent<MeshFilter>().mesh;
		MeshRenderer meshRenderer = NewObject.AddComponent<MeshRenderer>();
		MeshRenderer component = gameObject.GetComponent<MeshRenderer>();
		meshRenderer.sharedMaterials = component.sharedMaterials;
		Mesh mesh2 = new Mesh();
		mesh2.subMeshCount = mesh.subMeshCount;
		Vector3[] array = new Vector3[mesh.vertices.Length * m_NumObjectsPerRow * m_NumDepths];
		Vector3[] array2 = new Vector3[mesh.normals.Length * m_NumObjectsPerRow * m_NumDepths];
		Vector2[] array3 = new Vector2[mesh.uv.Length * m_NumObjectsPerRow * m_NumDepths];
		for (int i = 0; i < m_NumDepths; i++)
		{
			for (int j = 0; j < m_NumObjectsPerRow; j++)
			{
				float z = (float)j * m_ObjectSpacingX - (float)(m_NumObjectsPerRow - 1) * m_ObjectSpacingX * 0.5f;
				float x = (float)i * m_ObjectSpacingZ - (float)(m_NumDepths - 1) * m_ObjectSpacingZ * 0.5f;
				Vector3 vector = NewObject.transform.InverseTransformPoint(new Vector3(x, 0f, z));
				vector.z += gameObject.transform.localPosition.y / gameObject.transform.localScale.z;
				int num = mesh.vertices.Length;
				for (int k = 0; k < num; k++)
				{
					array[k + (j + i * m_NumObjectsPerRow) * num] = mesh.vertices[k] + vector;
				}
				mesh.normals.CopyTo(array2, (j + i * m_NumObjectsPerRow) * mesh.normals.Length);
				mesh.uv.CopyTo(array3, (j + i * m_NumObjectsPerRow) * mesh.uv.Length);
			}
		}
		mesh2.vertices = array;
		mesh2.normals = array2;
		mesh2.uv = array3;
		for (int l = 0; l < mesh.subMeshCount; l++)
		{
			mesh2.SetTriangles(new int[0], l);
		}
		NewObject.AddComponent<MeshFilter>().mesh = mesh2;
		NewObject.AddComponent<MeshCollider>().sharedMesh = mesh2;
		NewObject.layer = base.gameObject.layer;
		return mesh2;
	}

	private void DestroyOldObjectMesh()
	{
		if ((bool)m_ObjectModelPrefab)
		{
			UnityEngine.Object.Destroy(m_ObjectModelPrefab.gameObject);
			m_ObjectModelPrefab = null;
		}
		if (m_DisplayedObjects != null)
		{
			for (int i = 0; i < m_DisplayedObjects.Length; i++)
			{
				UnityEngine.Object.Destroy(m_DisplayedObjects[i].gameObject);
			}
			m_DisplayedObjects = null;
		}
	}

	public override void SetObjectType(ObjectType NewType)
	{
		if (m_ObjectType == NewType)
		{
			return;
		}
		DestroyOldObjectMesh();
		base.SetObjectType(NewType);
		SetSign(NewType);
		if (NewType == ObjectTypeList.m_Total || !StorageTypeManager.m_StoragePaletteInformation.ContainsKey(NewType))
		{
			return;
		}
		m_Capacity = 0;
		if (NewType != ObjectTypeList.m_Total)
		{
			m_Capacity = StorageTypeManager.m_StoragePaletteInformation[NewType].m_Capacity;
		}
		switch (NewType)
		{
		case ObjectType.BricksCrudeRaw:
		case ObjectType.BricksCrude:
			m_ObjectSpacingX = 1.3f;
			m_ObjectSpacingY = 1f;
			m_ObjectSpacingZ = 2f;
			m_NumObjectsPerRow = 3;
			m_NumRows = 3;
			m_NumDepths = 3;
			if (m_TypeIdentifier == ObjectType.StoragePaletteMedium)
			{
				m_NumObjectsPerRow += 2;
				m_NumRows += 3;
			}
			break;
		case ObjectType.RoofTilesRaw:
		case ObjectType.RoofTiles:
			m_ObjectSpacingX = 1.3f;
			m_ObjectSpacingY = 1f;
			m_ObjectSpacingZ = 2f;
			m_NumObjectsPerRow = 3;
			m_NumRows = 3;
			m_NumDepths = 3;
			if (m_TypeIdentifier == ObjectType.StoragePaletteMedium)
			{
				m_NumObjectsPerRow++;
				m_NumRows += 3;
			}
			break;
		case ObjectType.StoneBlockCrude:
		case ObjectType.StoneBlock:
		case ObjectType.HayBale:
			m_ObjectSpacingX = 2.1f;
			m_ObjectSpacingY = 1.5f;
			m_ObjectSpacingZ = 3f;
			m_NumObjectsPerRow = 2;
			m_NumRows = 2;
			m_NumDepths = 2;
			if (m_TypeIdentifier == ObjectType.StoragePaletteMedium)
			{
				m_NumObjectsPerRow++;
				m_NumRows += 3;
			}
			break;
		case ObjectType.CottonCloth:
		case ObjectType.BullrushesCloth:
		case ObjectType.SilkCloth:
			m_ObjectSpacingX = 2.1f;
			m_ObjectSpacingY = 1f;
			m_ObjectSpacingZ = 3f;
			m_NumObjectsPerRow = 2;
			m_NumRows = 3;
			m_NumDepths = 2;
			if (m_TypeIdentifier == ObjectType.StoragePaletteMedium)
			{
				m_NumObjectsPerRow++;
				m_NumRows += 5;
			}
			break;
		case ObjectType.Blanket:
			m_ObjectSpacingX = 2.1f;
			m_ObjectSpacingY = 1.5f;
			m_ObjectSpacingZ = 3f;
			m_NumObjectsPerRow = 2;
			m_NumRows = 2;
			m_NumDepths = 2;
			if (m_TypeIdentifier == ObjectType.StoragePaletteMedium)
			{
				m_NumObjectsPerRow++;
				m_NumRows += 3;
			}
			break;
		case ObjectType.WoodenBeam:
		case ObjectType.MetalGirder:
			m_ObjectSpacingX = 1.05f;
			m_ObjectSpacingY = 1.05f;
			m_ObjectSpacingZ = 0f;
			m_NumObjectsPerRow = 4;
			m_NumRows = 3;
			m_NumDepths = 1;
			if (m_TypeIdentifier == ObjectType.StoragePaletteMedium)
			{
				m_NumObjectsPerRow += 2;
				m_NumRows += 4;
			}
			break;
		case ObjectType.MetalPlateCrude:
			m_ObjectSpacingX = 2f;
			m_ObjectSpacingY = 0.4f;
			m_ObjectSpacingZ = 2.2f;
			m_NumObjectsPerRow = 2;
			m_NumRows = 6;
			m_NumDepths = 2;
			if (m_TypeIdentifier == ObjectType.StoragePaletteMedium)
			{
				m_NumObjectsPerRow++;
				m_NumRows += 12;
				m_NumDepths++;
			}
			break;
		default:
			m_ObjectSpacingX = 0.85f;
			m_ObjectSpacingY = 0.7f;
			m_ObjectSpacingZ = 0f;
			m_NumObjectsPerRow = 5;
			m_NumRows = 4;
			m_NumDepths = 1;
			if (m_TypeIdentifier == ObjectType.StoragePaletteMedium)
			{
				m_NumObjectsPerRow += 2;
				m_NumRows += 4;
			}
			break;
		}
		m_MaxObjects = m_NumObjectsPerRow * m_NumRows * m_NumDepths;
		string modelNameFromIdentifier = ObjectTypeList.Instance.GetModelNameFromIdentifier(NewType);
		GameObject gameObject = ModelManager.Instance.Load(NewType, modelNameFromIdentifier, RandomVariants: false);
		m_ObjectModelPrefab = UnityEngine.Object.Instantiate(gameObject, default(Vector3), Quaternion.identity, null);
		m_ObjectModelPrefab.SetActive(value: false);
		CreateRowTriangles(m_ObjectModelPrefab);
		m_DisplayedObjects = new GameObject[m_NumRows];
		m_ObjectMesh = new Mesh[m_NumRows];
		for (int i = 0; i < m_NumRows; i++)
		{
			GameObject gameObject2 = new GameObject("Storage");
			m_ObjectMesh[i] = CreateRowMesh(gameObject2, m_ObjectModelPrefab);
			gameObject2.transform.SetParent(m_ModelRoot.transform);
			GameObject gameObject3 = gameObject;
			gameObject2.transform.localRotation = gameObject3.transform.rotation;
			gameObject2.transform.position = m_ModelRoot.transform.Find("Contents").TransformPoint(new Vector3(0f, 0f, (float)i * m_ObjectSpacingY));
			gameObject2.transform.localRotation *= Quaternion.Euler(0f, 90f, 0f);
			gameObject2.SetActive(value: false);
			m_DisplayedObjects[i] = gameObject2;
		}
	}

	public override bool GetNewLevelAllowed(Building NewBuilding)
	{
		return GetNewLevelAllowedStacked(NewBuilding);
	}

	public static bool GetIsObjectTypeAcceptable(ObjectType NewType)
	{
		if (StorageTypeManager.m_StoragePaletteInformation.ContainsKey(NewType))
		{
			return true;
		}
		return false;
	}

	protected override bool CanAcceptObject(BaseClass NewObject, ObjectType NewType)
	{
		if (m_ObjectType == ObjectTypeList.m_Total)
		{
			if (!GetIsObjectTypeAcceptable(NewType))
			{
				return false;
			}
		}
		else if (m_ObjectType != NewType)
		{
			if (GetStored() > 0)
			{
				return false;
			}
			if (!GetIsObjectTypeAcceptable(NewType))
			{
				return false;
			}
		}
		return base.CanAcceptObject(NewObject, NewType);
	}

	private void SetObjectsToDisplay(int NumObjects)
	{
		if (m_NumObjectsPerRow == 0)
		{
			return;
		}
		int buildingLevelIndex = GetBuildingLevelIndex();
		NumObjects -= buildingLevelIndex * m_Capacity;
		if (NumObjects < 0)
		{
			NumObjects = 0;
		}
		if (NumObjects > m_MaxObjects)
		{
			NumObjects = m_MaxObjects;
		}
		int num = m_NumObjectsPerRow * m_NumDepths;
		int num2 = NumObjects / num + 1;
		if (NumObjects % num == 0)
		{
			num2--;
		}
		for (int i = 0; i < m_NumRows; i++)
		{
			if (i < num2)
			{
				if (i == num2 - 1 && NumObjects % num != 0)
				{
					for (int j = 0; j < m_SingleObjectTriangleCounts.Length; j++)
					{
						int num3 = NumObjects % num * m_SingleObjectTriangleCounts[j];
						int[] array = new int[num3];
						Array.Copy(m_TriangleData[j], array, num3);
						m_ObjectMesh[i].SetTriangles(array, j);
					}
				}
				else
				{
					for (int k = 0; k < m_SingleObjectTriangleCounts.Length; k++)
					{
						m_ObjectMesh[i].SetTriangles(m_TriangleData[k], k);
					}
				}
				m_DisplayedObjects[i].SetActive(value: true);
			}
			else
			{
				m_DisplayedObjects[i].SetActive(value: false);
			}
		}
	}

	public override void UpdateStored()
	{
		if (m_ObjectType == ObjectTypeList.m_Total)
		{
			return;
		}
		base.UpdateStored();
		int stored = GetStored();
		SetObjectsToDisplay(stored);
		if (m_Levels == null)
		{
			return;
		}
		foreach (Building level in m_Levels)
		{
			if ((bool)level.GetComponent<StoragePalette>())
			{
				level.GetComponent<StoragePalette>().SetObjectsToDisplay(stored);
			}
		}
	}

	public override void SendAction(ActionInfo Info)
	{
		base.SendAction(Info);
		ActionType action = Info.m_Action;
		if ((uint)(action - 41) > 1u)
		{
			return;
		}
		if (GetBuildingLevelIndex() != 0)
		{
			if ((bool)m_ParentBuilding && (bool)m_ParentBuilding.GetComponent<StoragePalette>())
			{
				SetObjectType(m_ParentBuilding.GetComponent<StoragePalette>().m_ObjectType);
			}
		}
		else
		{
			UpdateStored();
		}
	}

	protected override void Update()
	{
		base.Update();
	}
}
