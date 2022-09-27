using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace Dummiesman;

public class OBJObjectBuilder
{
	private class ObjLoopHash
	{
		public int vertexIndex;

		public int normalIndex;

		public int uvIndex;

		public override bool Equals(object obj)
		{
			if (!(obj is ObjLoopHash))
			{
				return false;
			}
			ObjLoopHash objLoopHash = obj as ObjLoopHash;
			if (objLoopHash.vertexIndex == vertexIndex && objLoopHash.uvIndex == uvIndex)
			{
				return objLoopHash.normalIndex == normalIndex;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return ((3 * 314159 + vertexIndex) * 314159 + normalIndex) * 314159 + uvIndex;
		}
	}

	private OBJLoader _loader;

	private string _name;

	private Dictionary<ObjLoopHash, int> _globalIndexRemap = new Dictionary<ObjLoopHash, int>();

	private Dictionary<string, List<int>> _materialIndices = new Dictionary<string, List<int>>();

	private List<int> _currentIndexList;

	private string _lastMaterial;

	private List<Vector3> _vertices = new List<Vector3>();

	private List<Vector3> _normals = new List<Vector3>();

	private List<Vector2> _uvs = new List<Vector2>();

	private bool recalculateNormals;

	public int PushedFaceCount { get; private set; }

	public GameObject Build()
	{
		GameObject gameObject = new GameObject(_name);
		MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
		int num = 0;
		Material[] array = new Material[_materialIndices.Count];
		foreach (KeyValuePair<string, List<int>> materialIndex in _materialIndices)
		{
			Material value = null;
			if (_loader.Materials == null)
			{
				value = OBJLoaderHelper.CreateNullMaterial();
				value.name = materialIndex.Key;
			}
			else if (!_loader.Materials.TryGetValue(materialIndex.Key, out value))
			{
				value = OBJLoaderHelper.CreateNullMaterial();
				value.name = materialIndex.Key;
				_loader.Materials[materialIndex.Key] = value;
			}
			array[num] = value;
			num++;
		}
		meshRenderer.sharedMaterials = array;
		MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
		num = 0;
		Mesh mesh = new Mesh
		{
			name = _name,
			indexFormat = ((_vertices.Count > 65535) ? IndexFormat.UInt32 : IndexFormat.UInt16),
			subMeshCount = _materialIndices.Count
		};
		mesh.SetVertices(_vertices);
		mesh.SetNormals(_normals);
		mesh.SetUVs(0, _uvs);
		foreach (KeyValuePair<string, List<int>> materialIndex2 in _materialIndices)
		{
			mesh.SetTriangles(materialIndex2.Value, num);
			num++;
		}
		if (recalculateNormals)
		{
			mesh.RecalculateNormals();
		}
		mesh.RecalculateTangents();
		mesh.RecalculateBounds();
		meshFilter.sharedMesh = mesh;
		return gameObject;
	}

	public void SetMaterial(string name)
	{
		if (!_materialIndices.TryGetValue(name, out _currentIndexList))
		{
			_currentIndexList = new List<int>();
			_materialIndices[name] = _currentIndexList;
		}
	}

	public void PushFace(string material, List<int> vertexIndices, List<int> normalIndices, List<int> uvIndices)
	{
		if (vertexIndices.Count < 3)
		{
			return;
		}
		if (material != _lastMaterial)
		{
			SetMaterial(material);
			_lastMaterial = material;
		}
		int[] array = new int[vertexIndices.Count];
		for (int i = 0; i < vertexIndices.Count; i++)
		{
			int num = vertexIndices[i];
			int num2 = normalIndices[i];
			int num3 = uvIndices[i];
			ObjLoopHash key = new ObjLoopHash
			{
				vertexIndex = num,
				normalIndex = num2,
				uvIndex = num3
			};
			int value = -1;
			if (!_globalIndexRemap.TryGetValue(key, out value))
			{
				_globalIndexRemap.Add(key, _vertices.Count);
				value = _vertices.Count;
				_vertices.Add((num >= 0 && num < _loader.Vertices.Count) ? _loader.Vertices[num] : Vector3.zero);
				_normals.Add((num2 >= 0 && num2 < _loader.Normals.Count) ? _loader.Normals[num2] : Vector3.zero);
				_uvs.Add((num3 >= 0 && num3 < _loader.UVs.Count) ? _loader.UVs[num3] : Vector2.zero);
				if (num2 < 0)
				{
					recalculateNormals = true;
				}
			}
			array[i] = value;
		}
		if (array.Length == 3)
		{
			_currentIndexList.AddRange(new int[3]
			{
				array[0],
				array[1],
				array[2]
			});
		}
		else if (array.Length == 4)
		{
			_currentIndexList.AddRange(new int[3]
			{
				array[0],
				array[1],
				array[2]
			});
			_currentIndexList.AddRange(new int[3]
			{
				array[2],
				array[3],
				array[0]
			});
		}
		else if (array.Length > 4)
		{
			for (int num4 = array.Length - 1; num4 >= 2; num4--)
			{
				_currentIndexList.AddRange(new int[3]
				{
					array[0],
					array[num4 - 1],
					array[num4]
				});
			}
		}
		PushedFaceCount++;
	}

	public OBJObjectBuilder(string name, OBJLoader loader)
	{
		_name = name;
		_loader = loader;
	}
}
