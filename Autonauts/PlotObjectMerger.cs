using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class PlotObjectMerger : MonoBehaviour
{
	private static bool m_DebugLog;

	private static bool m_DebugShowObjects;

	public ObjectType m_Type;

	private Plot m_Plot;

	public bool m_Dirty;

	public bool m_Merge;

	public float m_MergeTimer;

	private bool m_Merged;

	private MeshRenderer m_Renderer;

	private MeshFilter m_Filter;

	private static Mesh[] m_TempMeshes;

	private Material[] m_SharedMaterials;

	public void Init(TileCoordObject NewObject, Plot NewPlot)
	{
		m_Type = NewObject.m_TypeIdentifier;
		m_Plot = NewPlot;
		m_Merge = false;
		m_Dirty = false;
		m_Merged = false;
		m_Filter = GetComponent<MeshFilter>();
		m_Filter.mesh = new Mesh();
		m_Renderer = GetComponent<MeshRenderer>();
		string modelNameFromIdentifier = ObjectTypeList.Instance.GetModelNameFromIdentifier(m_Type);
		MeshRenderer componentInChildren = ModelManager.Instance.Load(m_Type, modelNameFromIdentifier, RandomVariants: false).GetComponentInChildren<MeshRenderer>(includeInactive: true);
		if (m_Renderer != null && componentInChildren != null)
		{
			m_SharedMaterials = new Material[3];
			Material material = MaterialManager.Instance.m_Material;
			if (MaterialManager.GetObjectTypeNeedsWindy(m_Type))
			{
				material = MaterialManager.Instance.m_MaterialWindy;
			}
			else if (ObjectTypeList.Instance.GetIsBuilding(m_Type))
			{
				material = MaterialManager.Instance.m_MaterialBuilding;
			}
			for (int i = 0; i < 3; i++)
			{
				if (m_DebugShowObjects)
				{
					m_SharedMaterials[i] = null;
				}
				else
				{
					m_SharedMaterials[i] = material;
				}
			}
			m_Renderer.sharedMaterials = m_SharedMaterials;
		}
		if (m_TempMeshes == null)
		{
			int num = 8;
			m_TempMeshes = new Mesh[num];
			for (int j = 0; j < num; j++)
			{
				m_TempMeshes[j] = new Mesh();
			}
		}
		base.enabled = false;
	}

	public void MarkDirty(bool Immediate)
	{
		if (Immediate)
		{
			if (m_Merged)
			{
				Break();
			}
			if (!m_Merge)
			{
				base.enabled = false;
			}
			return;
		}
		if (m_DebugLog)
		{
			Debug.Log(string.Concat("Marking ", m_Plot.m_PlotX, ",", m_Plot.m_PlotY, " ", m_Type, " ", (int)TimeManager.Instance.m_TotalRealTime));
		}
		m_Merge = true;
		m_MergeTimer = 0f;
		base.enabled = true;
		if (m_Dirty)
		{
			PlotObjectMergerManager.Instance.RemoveDirty(this);
		}
	}

	private void Break()
	{
		if (m_DebugLog)
		{
			Debug.Log(string.Concat("Breaking ", m_Plot.m_PlotX, ",", m_Plot.m_PlotY, " ", m_Type, " ", (int)TimeManager.Instance.m_TotalRealTime));
		}
		m_Merged = false;
		foreach (TileCoordObject item in m_Plot.m_ObjectDictionary[m_Type])
		{
			if (item.m_Plot.m_Visible)
			{
				MeshRenderer[] componentsInChildren = item.GetComponentsInChildren<MeshRenderer>();
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					componentsInChildren[i].enabled = true;
				}
			}
			item.m_Sleeping = false;
		}
		m_Filter.mesh.Clear();
		m_Merge = false;
	}

	private int CountObject(TileCoordObject NewObject)
	{
		int result = 0;
		if (NewObject.m_SleepRequested)
		{
			result = NewObject.GetComponentInChildren<MeshRenderer>().sharedMaterials.Length;
		}
		MeshRenderer[] componentsInChildren = NewObject.GetComponentsInChildren<MeshRenderer>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].enabled = !NewObject.m_SleepRequested & m_Plot.m_Visible;
		}
		NewObject.m_Sleeping = NewObject.m_SleepRequested;
		return result;
	}

	private void MergeObject(TileCoordObject NewObject, List<CombineInstance>[] combines, int[] vertexCount, Matrix4x4 NewMatrix)
	{
		if (!NewObject.m_SleepRequested)
		{
			return;
		}
		MeshFilter[] componentsInChildren = NewObject.GetComponentsInChildren<MeshFilter>();
		foreach (MeshFilter meshFilter in componentsInChildren)
		{
			for (int j = 0; j < meshFilter.GetComponent<MeshRenderer>().sharedMaterials.Length; j++)
			{
				CombineInstance item = default(CombineInstance);
				item.mesh = meshFilter.sharedMesh;
				item.subMeshIndex = j;
				item.transform = NewMatrix * meshFilter.transform.localToWorldMatrix;
				vertexCount[j] += meshFilter.sharedMesh.vertexCount;
				combines[j].Add(item);
			}
		}
	}

	public void Merge()
	{
		if (m_DebugLog)
		{
			Debug.Log(string.Concat("Merging ", m_Plot.m_PlotX, ",", m_Plot.m_PlotY, " ", m_Type, " ", (int)TimeManager.Instance.m_TotalRealTime));
		}
		m_Merged = true;
		if (m_Renderer == null)
		{
			return;
		}
		List<TileCoordObject> list = m_Plot.m_ObjectDictionary[m_Type];
		int num = 0;
		bool isBuilding = ObjectTypeList.Instance.GetIsBuilding(m_Type);
		foreach (TileCoordObject item in list)
		{
			if (!item.GetIsSavable())
			{
				continue;
			}
			num += CountObject(item);
			if (!isBuilding)
			{
				continue;
			}
			Building component = item.GetComponent<Building>();
			if (component.m_Levels == null)
			{
				continue;
			}
			foreach (Building level in component.m_Levels)
			{
				num += CountObject(level);
			}
		}
		if (num > 0)
		{
			List<CombineInstance>[] array = new List<CombineInstance>[m_Renderer.sharedMaterials.Length];
			int[] array2 = new int[m_Renderer.sharedMaterials.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = new List<CombineInstance>();
				array2[i] = 0;
			}
			Matrix4x4 inverse = base.transform.localToWorldMatrix.inverse;
			_ = new Material[num];
			foreach (TileCoordObject item2 in list)
			{
				if (!item2.GetIsSavable())
				{
					continue;
				}
				MergeObject(item2, array, array2, inverse);
				if (!isBuilding)
				{
					continue;
				}
				Building component2 = item2.GetComponent<Building>();
				if (component2.m_Levels == null)
				{
					continue;
				}
				foreach (Building level2 in component2.m_Levels)
				{
					MergeObject(level2, array, array2, inverse);
				}
			}
			CombineInstance[] array3 = new CombineInstance[m_Renderer.sharedMaterials.Length];
			int num2 = 0;
			for (int j = 0; j < m_Renderer.sharedMaterials.Length; j++)
			{
				if (array2[j] >= 65536)
				{
					m_TempMeshes[j].indexFormat = IndexFormat.UInt32;
				}
				else
				{
					m_TempMeshes[j].indexFormat = IndexFormat.UInt16;
				}
				m_TempMeshes[j].CombineMeshes(array[j].ToArray());
				array3[j].mesh = m_TempMeshes[j];
				array3[j].subMeshIndex = 0;
				array3[j].transform = Matrix4x4.identity;
				num2 += m_TempMeshes[j].vertexCount;
			}
			if (num2 >= 65536)
			{
				if (m_DebugLog)
				{
					Debug.Log(string.Concat("Merging objects of type ", m_Type, " at plot ", m_Plot.m_TileX, ",", m_Plot.m_TileY, " exceeds vertex limit. ", num2));
				}
				m_Filter.mesh.indexFormat = IndexFormat.UInt32;
			}
			else
			{
				m_Filter.mesh.indexFormat = IndexFormat.UInt16;
			}
			m_Filter.mesh.CombineMeshes(array3, mergeSubMeshes: false);
		}
		else
		{
			m_Filter.mesh.Clear();
		}
		m_Merge = false;
	}

	public void FinishMerge()
	{
		if (m_Merge && !m_Dirty)
		{
			PlotObjectMergerManager.Instance.NewDirty(this);
			m_Merge = false;
			base.enabled = false;
		}
	}

	private void Update()
	{
		if (m_Merge && !m_Dirty)
		{
			m_MergeTimer += Time.deltaTime;
			if (m_MergeTimer > 2.5f)
			{
				PlotObjectMergerManager.Instance.NewDirty(this);
				m_Merge = false;
				base.enabled = false;
			}
		}
	}
}
