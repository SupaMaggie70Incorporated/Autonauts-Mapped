using System.Collections.Generic;
using System.IO;
using Dummiesman;
using UnityEngine;

public class MTLLoader
{
	public List<string> SearchPaths = new List<string>
	{
		"%FileName%_Textures",
		string.Empty
	};

	private FileInfo _objFileInfo;

	public virtual Texture2D TextureLoadFunction(string path, bool isNormalMap)
	{
		foreach (string searchPath in SearchPaths)
		{
			string text = Path.Combine((_objFileInfo != null) ? searchPath.Replace("%FileName%", Path.GetFileNameWithoutExtension(_objFileInfo.Name)) : searchPath, path);
			if (File.Exists(text))
			{
				Texture2D texture2D = ImageLoader.LoadTexture(text);
				if (isNormalMap)
				{
					texture2D = ImageUtils.ConvertToNormalMap(texture2D);
				}
				return texture2D;
			}
		}
		return null;
	}

	private Texture2D TryLoadTexture(string texturePath, bool normalMap = false)
	{
		texturePath = texturePath.Replace('\\', Path.DirectorySeparatorChar);
		texturePath = texturePath.Replace('/', Path.DirectorySeparatorChar);
		return TextureLoadFunction(texturePath, normalMap);
	}

	private int GetArgValueCount(string arg)
	{
		switch (arg)
		{
		case "-bm":
		case "-clamp":
		case "-blendu":
		case "-blendv":
		case "-imfchan":
		case "-texres":
			return 1;
		case "-mm":
			return 2;
		case "-o":
		case "-s":
		case "-t":
			return 3;
		default:
			return -1;
		}
	}

	private int GetTexNameIndex(string[] components)
	{
		int num;
		for (num = 1; num < components.Length; num++)
		{
			int argValueCount = GetArgValueCount(components[num]);
			if (argValueCount < 0)
			{
				return num;
			}
			num += argValueCount;
		}
		return -1;
	}

	private float GetArgValue(string[] components, string arg, float fallback = 1f)
	{
		string text = arg.ToLower();
		for (int i = 1; i < components.Length - 1; i++)
		{
			string text2 = components[i].ToLower();
			if (text == text2)
			{
				return OBJLoaderHelper.FastFloatParse(components[i + 1]);
			}
		}
		return fallback;
	}

	private string GetTexPathFromMapStatement(string processedLine, string[] splitLine)
	{
		int texNameIndex = GetTexNameIndex(splitLine);
		if (texNameIndex < 0)
		{
			Debug.LogError("texNameCmpIdx < 0 on line " + processedLine + ". Texture not loaded.");
			return null;
		}
		int startIndex = processedLine.IndexOf(splitLine[texNameIndex]);
		return processedLine.Substring(startIndex);
	}

	public Dictionary<string, Material> Load(Stream input)
	{
		StringReader stringReader = new StringReader(new StreamReader(input).ReadToEnd());
		Dictionary<string, Material> dictionary = new Dictionary<string, Material>();
		Material material = null;
		for (string text = stringReader.ReadLine(); text != null; text = stringReader.ReadLine())
		{
			if (!string.IsNullOrWhiteSpace(text))
			{
				string text2 = text.Clean();
				string[] array = text2.Split(' ');
				if (array.Length >= 2 && text2[0] != '#')
				{
					if (array[0] == "newmtl")
					{
						string text3 = text2.Substring(7);
						Material material3 = (dictionary[text3] = new Material(Shader.Find("Standard (Specular setup)"))
						{
							name = text3
						});
						material = material3;
					}
					else if (!(material == null))
					{
						if (array[0] == "Kd" || array[0] == "kd")
						{
							Color color = material.GetColor("_Color");
							Color color2 = OBJLoaderHelper.ColorFromStrArray(array);
							material.SetColor("_Color", new Color(color2.r, color2.g, color2.b, color.a));
						}
						else if (array[0] == "map_Kd" || array[0] == "map_kd")
						{
							string texPathFromMapStatement = GetTexPathFromMapStatement(text2, array);
							if (texPathFromMapStatement != null)
							{
								Texture2D texture2D = TryLoadTexture(texPathFromMapStatement);
								material.SetTexture("_MainTex", texture2D);
								if (texture2D != null && (texture2D.format == TextureFormat.DXT5 || texture2D.format == TextureFormat.ARGB32))
								{
									OBJLoaderHelper.EnableMaterialTransparency(material);
								}
								if (Path.GetExtension(texPathFromMapStatement).ToLower() == ".dds")
								{
									material.mainTextureScale = new Vector2(1f, -1f);
								}
							}
						}
						else if (array[0] == "map_Bump" || array[0] == "map_bump")
						{
							string texPathFromMapStatement2 = GetTexPathFromMapStatement(text2, array);
							if (texPathFromMapStatement2 != null)
							{
								Texture2D texture2D2 = TryLoadTexture(texPathFromMapStatement2, normalMap: true);
								float argValue = GetArgValue(array, "-bm");
								if (texture2D2 != null)
								{
									material.SetTexture("_BumpMap", texture2D2);
									material.SetFloat("_BumpScale", argValue);
									material.EnableKeyword("_NORMALMAP");
								}
							}
						}
						else if (array[0] == "Ks" || array[0] == "ks")
						{
							material.SetColor("_SpecColor", OBJLoaderHelper.ColorFromStrArray(array));
						}
						else if (array[0] == "Ka" || array[0] == "ka")
						{
							material.SetColor("_EmissionColor", OBJLoaderHelper.ColorFromStrArray(array, 0.05f));
							material.EnableKeyword("_EMISSION");
						}
						else if (array[0] == "map_Ka" || array[0] == "map_ka")
						{
							string texPathFromMapStatement3 = GetTexPathFromMapStatement(text2, array);
							if (texPathFromMapStatement3 != null)
							{
								material.SetTexture("_EmissionMap", TryLoadTexture(texPathFromMapStatement3));
							}
						}
						else if (array[0] == "d" || array[0] == "Tr")
						{
							float num = OBJLoaderHelper.FastFloatParse(array[1]);
							if (array[0] == "Tr")
							{
								num = 1f - num;
							}
							if (num < 1f - Mathf.Epsilon)
							{
								Color color3 = material.GetColor("_Color");
								color3.a = num;
								material.SetColor("_Color", color3);
								OBJLoaderHelper.EnableMaterialTransparency(material);
							}
						}
						else
						{
							if (array[0] == "Ns" || array[0] == "ns")
							{
								float num2 = OBJLoaderHelper.FastFloatParse(array[1]);
								num2 /= 1000f;
								material.SetFloat("_Glossiness", num2);
							}
							if (array[0] == "Ke" || array[0] == "ke")
							{
								material.SetColor("_EmissionColor", OBJLoaderHelper.ColorFromStrArray(array));
								material.EnableKeyword("_EMISSION");
							}
						}
					}
				}
			}
		}
		return dictionary;
	}

	public Dictionary<string, Material> Load(string path)
	{
		_objFileInfo = new FileInfo(path);
		SearchPaths.Add(_objFileInfo.Directory.FullName);
		using FileStream input = new FileStream(path, FileMode.Open);
		return Load(input);
	}
}
