using System;
using System.IO;
using UnityEngine;

public class SaveFileThumbnail
{
	private static string m_DefaultName = "Thumbnail.jpg";

	public Texture2D m_Texture;

	public SaveFileThumbnail()
	{
		m_Texture = null;
	}

	public void Capture()
	{
		int tilesWide = TileManager.Instance.m_TilesWide;
		int tilesHigh = TileManager.Instance.m_TilesHigh;
		m_Texture = new Texture2D(tilesWide, tilesHigh, TextureFormat.RGBA32, mipChain: false, linear: true);
		m_Texture.filterMode = FilterMode.Point;
		Color32[] array = new Color32[tilesWide * tilesHigh];
		Color32 color = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
		Color32 color2 = GeneralUtils.ColorFromHex(16732068);
		Color32 color3 = GeneralUtils.ColorFromHex(10039402);
		for (int i = 0; i < tilesHigh; i++)
		{
			for (int j = 0; j < tilesWide; j++)
			{
				int num = (tilesHigh - i - 1) * tilesWide + j;
				if (TileManager.Instance.m_Tiles != null)
				{
					int num2 = i * tilesWide + j;
					Tile tile = TileManager.Instance.m_Tiles[num2];
					if ((bool)tile.m_Building || (bool)tile.m_BuildingFootprint)
					{
						array[num] = color2;
						continue;
					}
					if ((bool)tile.m_Floor)
					{
						array[num] = color3;
						continue;
					}
					Tile.TileType tileType = tile.m_TileType;
					array[num] = Tile.m_TileInfo[(int)tileType].m_MapColour;
				}
				else
				{
					array[num] = color;
				}
			}
		}
		m_Texture.SetPixels32(0, 0, tilesWide, tilesHigh, array);
		m_Texture.Apply();
	}

	private static string GetFileName(string NewPath)
	{
		return NewPath + "/" + m_DefaultName;
	}

	public void Save(string NewPath)
	{
		Capture();
		byte[] bytes = m_Texture.EncodeToJPG(97);
		string fileName = GetFileName(NewPath);
		if (!Directory.Exists(NewPath))
		{
			ErrorMessage.LogError("Thumbnail Save - Folder doesn't exist : " + NewPath);
		}
		else
		{
			SaveFile.Save(fileName, bytes);
		}
	}

	public bool Exists(string NewPath)
	{
		return File.Exists(GetFileName(NewPath));
	}

	public void Load(string NewPath)
	{
		string fileName = GetFileName(NewPath);
		if (!File.Exists(fileName))
		{
			ErrorMessage.LogError("Thumbnail Load - File doesn't exist : " + fileName);
			return;
		}
		byte[] data;
		try
		{
			data = File.ReadAllBytes(fileName);
		}
		catch (UnauthorizedAccessException ex)
		{
			ErrorMessage.LogError("Thumbnail Load - UnauthorizedAccessException : " + fileName + " " + ex.ToString());
			return;
		}
		m_Texture = new Texture2D(2, 2);
		m_Texture.LoadImage(data);
	}
}
