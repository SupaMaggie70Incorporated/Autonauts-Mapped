using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Dummiesman;

public class OBJLoader
{
	public SplitMode SplitMode = SplitMode.Object;

	internal List<Vector3> Vertices = new List<Vector3>();

	internal List<Vector3> Normals = new List<Vector3>();

	internal List<Vector2> UVs = new List<Vector2>();

	internal Dictionary<string, Material> Materials;

	private FileInfo _objInfo;

	private void LoadMaterialLibrary(string mtlLibPath)
	{
		if (_objInfo != null && File.Exists(Path.Combine(_objInfo.Directory.FullName, mtlLibPath)))
		{
			Materials = new MTLLoader().Load(Path.Combine(_objInfo.Directory.FullName, mtlLibPath));
		}
		else if (File.Exists(mtlLibPath))
		{
			Materials = new MTLLoader().Load(mtlLibPath);
		}
	}

	public GameObject Load(Stream input)
	{
		StreamReader reader = new StreamReader(input);
		Dictionary<string, OBJObjectBuilder> builderDict = new Dictionary<string, OBJObjectBuilder>();
		OBJObjectBuilder currentBuilder = null;
		string material = "default";
		List<int> list = new List<int>();
		List<int> list2 = new List<int>();
		List<int> list3 = new List<int>();
		Action<string> action = delegate(string objectName)
		{
			if (!builderDict.TryGetValue(objectName, out currentBuilder))
			{
				currentBuilder = new OBJObjectBuilder(objectName, this);
				builderDict[objectName] = currentBuilder;
			}
		};
		action("default");
		CharWordReader charWordReader = new CharWordReader(reader, 4096);
		while (true)
		{
			charWordReader.SkipWhitespaces();
			if (charWordReader.endReached)
			{
				break;
			}
			charWordReader.ReadUntilWhiteSpace();
			if (charWordReader.Is("#"))
			{
				charWordReader.SkipUntilNewLine();
			}
			else if (Materials == null && charWordReader.Is("mtllib"))
			{
				charWordReader.SkipWhitespaces();
				charWordReader.ReadUntilNewLine();
				string @string = charWordReader.GetString();
				LoadMaterialLibrary(@string);
			}
			else if (charWordReader.Is("v"))
			{
				Vertices.Add(charWordReader.ReadVector());
			}
			else if (charWordReader.Is("vn"))
			{
				Normals.Add(charWordReader.ReadVector());
			}
			else if (charWordReader.Is("vt"))
			{
				UVs.Add(charWordReader.ReadVector());
			}
			else if (charWordReader.Is("usemtl"))
			{
				charWordReader.SkipWhitespaces();
				charWordReader.ReadUntilNewLine();
				string string2 = charWordReader.GetString();
				material = string2;
				if (SplitMode == SplitMode.Material)
				{
					action(string2);
				}
			}
			else if ((charWordReader.Is("o") || charWordReader.Is("g")) && SplitMode == SplitMode.Object)
			{
				charWordReader.ReadUntilNewLine();
				string string3 = charWordReader.GetString(1);
				action(string3);
			}
			else if (charWordReader.Is("f"))
			{
				while (true)
				{
					charWordReader.SkipWhitespaces(out var newLinePassed);
					if (newLinePassed)
					{
						break;
					}
					int num = int.MinValue;
					int num2 = int.MinValue;
					int num3 = int.MinValue;
					num = charWordReader.ReadInt();
					if (charWordReader.currentChar == '/')
					{
						charWordReader.MoveNext();
						if (charWordReader.currentChar != '/')
						{
							num3 = charWordReader.ReadInt();
						}
						if (charWordReader.currentChar == '/')
						{
							charWordReader.MoveNext();
							num2 = charWordReader.ReadInt();
						}
					}
					if (num > int.MinValue)
					{
						if (num < 0)
						{
							num = Vertices.Count - num;
						}
						num--;
					}
					if (num2 > int.MinValue)
					{
						if (num2 < 0)
						{
							num2 = Normals.Count - num2;
						}
						num2--;
					}
					if (num3 > int.MinValue)
					{
						if (num3 < 0)
						{
							num3 = UVs.Count - num3;
						}
						num3--;
					}
					list.Add(num);
					list2.Add(num2);
					list3.Add(num3);
				}
				currentBuilder.PushFace(material, list, list2, list3);
				list.Clear();
				list2.Clear();
				list3.Clear();
			}
			else
			{
				charWordReader.SkipUntilNewLine();
			}
		}
		GameObject gameObject = new GameObject((_objInfo != null) ? Path.GetFileNameWithoutExtension(_objInfo.Name) : "WavefrontObject");
		gameObject.transform.localScale = new Vector3(-1f, 1f, 1f);
		foreach (KeyValuePair<string, OBJObjectBuilder> item in builderDict)
		{
			if (item.Value.PushedFaceCount != 0)
			{
				item.Value.Build().transform.SetParent(gameObject.transform, worldPositionStays: false);
			}
		}
		return gameObject;
	}

	public GameObject Load(Stream input, Stream mtlInput)
	{
		MTLLoader mTLLoader = new MTLLoader();
		Materials = mTLLoader.Load(mtlInput);
		return Load(input);
	}

	public GameObject Load(string path, string mtlPath)
	{
		_objInfo = new FileInfo(path);
		if (!string.IsNullOrEmpty(mtlPath) && File.Exists(mtlPath))
		{
			MTLLoader mTLLoader = new MTLLoader();
			Materials = mTLLoader.Load(mtlPath);
			using FileStream input = new FileStream(path, FileMode.Open);
			return Load(input);
		}
		using FileStream input2 = new FileStream(path, FileMode.Open);
		return Load(input2);
	}

	public GameObject Load(string path)
	{
		return Load(path, null);
	}
}
