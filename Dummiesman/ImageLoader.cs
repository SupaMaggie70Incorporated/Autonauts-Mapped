using System;
using System.IO;
using B83.Image.BMP;
using UnityEngine;

namespace Dummiesman;

public class ImageLoader
{
	public enum TextureFormat
	{
		DDS,
		TGA,
		BMP,
		PNG,
		JPG,
		CRN
	}

	public static void SetNormalMap(ref Texture2D tex)
	{
		Color[] pixels = tex.GetPixels();
		for (int i = 0; i < pixels.Length; i++)
		{
			Color color = pixels[i];
			color.r = pixels[i].g;
			color.a = pixels[i].r;
			pixels[i] = color;
		}
		tex.SetPixels(pixels);
		tex.Apply(updateMipmaps: true);
	}

	public static Texture2D LoadTexture(Stream stream, TextureFormat format)
	{
		switch (format)
		{
		case TextureFormat.BMP:
			return new BMPLoader().LoadBMP(stream).ToTexture2D();
		case TextureFormat.DDS:
			return DDSLoader.Load(stream);
		case TextureFormat.PNG:
		case TextureFormat.JPG:
		{
			byte[] array = new byte[stream.Length];
			stream.Read(array, 0, (int)stream.Length);
			Texture2D texture2D = new Texture2D(1, 1);
			texture2D.LoadImage(array);
			return texture2D;
		}
		case TextureFormat.TGA:
			return TGALoader.Load(stream);
		default:
			return null;
		}
	}

	public static Texture2D LoadTexture(string fn)
	{
		if (!File.Exists(fn))
		{
			return null;
		}
		byte[] array = File.ReadAllBytes(fn);
		string text = Path.GetExtension(fn).ToLower();
		string fileName = Path.GetFileName(fn);
		Texture2D texture2D = null;
		switch (text)
		{
		case ".png":
		case ".jpg":
		case ".jpeg":
			texture2D = new Texture2D(1, 1);
			texture2D.LoadImage(array);
			break;
		case ".dds":
			texture2D = DDSLoader.Load(array);
			break;
		case ".tga":
			texture2D = TGALoader.Load(array);
			break;
		case ".bmp":
			texture2D = new BMPLoader().LoadBMP(array).ToTexture2D();
			break;
		case ".crn":
		{
			byte[] array2 = array;
			ushort width = BitConverter.ToUInt16(new byte[2]
			{
				array2[13],
				array2[12]
			}, 0);
			ushort height = BitConverter.ToUInt16(new byte[2]
			{
				array2[15],
				array2[14]
			}, 0);
			byte b = array2[18];
			UnityEngine.TextureFormat textureFormat = UnityEngine.TextureFormat.RGB24;
			if (b == 0)
			{
				textureFormat = UnityEngine.TextureFormat.DXT1Crunched;
			}
			else if (b == 2)
			{
				textureFormat = UnityEngine.TextureFormat.DXT5Crunched;
			}
			else
			{
				if (b != 12)
				{
					Debug.LogError("Could not load crunched texture " + fileName + " because its format is not supported (" + b + "): " + fn);
					break;
				}
				textureFormat = UnityEngine.TextureFormat.ETC2_RGBA8Crunched;
			}
			texture2D = new Texture2D(width, height, textureFormat, mipChain: true);
			texture2D.LoadRawTextureData(array2);
			texture2D.Apply(updateMipmaps: true);
			break;
		}
		default:
			Debug.LogError("Could not load texture " + fileName + " because its format is not supported : " + fn);
			break;
		}
		if (texture2D != null)
		{
			texture2D = ImageLoaderHelper.VerifyFormat(texture2D);
			texture2D.name = Path.GetFileNameWithoutExtension(fn);
			texture2D.filterMode = FilterMode.Point;
		}
		return texture2D;
	}
}
