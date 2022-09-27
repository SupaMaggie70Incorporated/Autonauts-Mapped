using System;
using System.IO;
using UnityEngine;

namespace Dummiesman;

public static class DDSLoader
{
	public static Texture2D Load(Stream ddsStream)
	{
		byte[] array = new byte[ddsStream.Length];
		ddsStream.Read(array, 0, (int)ddsStream.Length);
		return Load(array);
	}

	public static Texture2D Load(string ddsPath)
	{
		return Load(File.ReadAllBytes(ddsPath));
	}

	public static Texture2D Load(byte[] ddsBytes)
	{
		try
		{
			if (ddsBytes[4] != 124)
			{
				throw new Exception("Invalid DDS header. Structure length is incrrrect.");
			}
			byte b = ddsBytes[87];
			if (b != 49 && b != 53)
			{
				throw new Exception("Cannot load DDS due to an unsupported pixel format. Needs to be DXT1 or DXT5.");
			}
			int height = ddsBytes[13] * 256 + ddsBytes[12];
			int width = ddsBytes[17] * 256 + ddsBytes[16];
			bool mipChain = ddsBytes[28] > 0;
			TextureFormat textureFormat = ((b == 49) ? TextureFormat.DXT1 : TextureFormat.DXT5);
			int num = 128;
			byte[] array = new byte[ddsBytes.Length - num];
			Buffer.BlockCopy(ddsBytes, num, array, 0, ddsBytes.Length - num);
			Texture2D texture2D = new Texture2D(width, height, textureFormat, mipChain);
			texture2D.LoadRawTextureData(array);
			texture2D.Apply();
			return texture2D;
		}
		catch (Exception ex)
		{
			throw new Exception("An error occured while loading DirectDraw Surface: " + ex.Message);
		}
	}
}
