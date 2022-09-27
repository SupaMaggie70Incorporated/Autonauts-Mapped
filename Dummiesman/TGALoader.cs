using System;
using System.IO;
using Dummiesman.Extensions;
using UnityEngine;

namespace Dummiesman;

public class TGALoader
{
	private static int GetBits(byte b, int offset, int count)
	{
		return (b >> offset) & ((1 << count) - 1);
	}

	private static Color32[] LoadRawTGAData(BinaryReader r, int bitDepth, int width, int height)
	{
		Color32[] array = new Color32[width * height];
		byte[] pixelData = r.ReadBytes(width * height * (bitDepth / 8));
		ImageLoaderHelper.FillPixelArray(array, pixelData, bitDepth / 8, bgra: true);
		return array;
	}

	private static Color32[] LoadRLETGAData(BinaryReader r, int bitDepth, int width, int height)
	{
		Color32[] array = new Color32[width * height];
		int num;
		for (int i = 0; i < array.Length; i += num)
		{
			byte b = r.ReadByte();
			int bits = GetBits(b, 7, 1);
			num = GetBits(b, 0, 7) + 1;
			if (bits == 0)
			{
				for (int j = 0; j < num; j++)
				{
					Color32 color = (array[j + i] = ((bitDepth == 32) ? r.ReadColor32RGBA().FlipRB() : r.ReadColor32RGB().FlipRB()));
				}
				continue;
			}
			Color32 color2 = ((bitDepth == 32) ? r.ReadColor32RGBA().FlipRB() : r.ReadColor32RGB().FlipRB());
			for (int k = 0; k < num; k++)
			{
				array[k + i] = color2;
			}
		}
		return array;
	}

	public static Texture2D Load(string fileName)
	{
		using FileStream tGAStream = File.OpenRead(fileName);
		return Load(tGAStream);
	}

	public static Texture2D Load(byte[] bytes)
	{
		using MemoryStream tGAStream = new MemoryStream(bytes);
		return Load(tGAStream);
	}

	public static Texture2D Load(Stream TGAStream)
	{
		using BinaryReader binaryReader = new BinaryReader(TGAStream);
		binaryReader.BaseStream.Seek(2L, SeekOrigin.Begin);
		byte b = binaryReader.ReadByte();
		if (b != 10 && b != 2)
		{
			Debug.LogError($"Unsupported targa image type. ({b})");
			return null;
		}
		binaryReader.BaseStream.Seek(12L, SeekOrigin.Begin);
		short width = binaryReader.ReadInt16();
		short height = binaryReader.ReadInt16();
		int num = binaryReader.ReadByte();
		if (num < 24)
		{
			throw new Exception("Tried to load TGA with unsupported bit depth");
		}
		binaryReader.BaseStream.Seek(1L, SeekOrigin.Current);
		Texture2D texture2D = new Texture2D(width, height, (num == 24) ? TextureFormat.RGB24 : TextureFormat.ARGB32, mipChain: true);
		if (b == 2)
		{
			texture2D.SetPixels32(LoadRawTGAData(binaryReader, num, width, height));
		}
		else
		{
			texture2D.SetPixels32(LoadRLETGAData(binaryReader, num, width, height));
		}
		texture2D.Apply();
		return texture2D;
	}
}
