using UnityEngine;

namespace Dummiesman;

public class ImageLoaderHelper
{
	public static Texture2D VerifyFormat(Texture2D tex)
	{
		if (tex.format != TextureFormat.ARGB32 && tex.format != TextureFormat.RGBA32 && tex.format != TextureFormat.DXT5)
		{
			return tex;
		}
		Color32[] pixels = tex.GetPixels32();
		bool flag = false;
		Color32[] array = pixels;
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i].a < byte.MaxValue)
			{
				flag = true;
				break;
			}
		}
		if (!flag)
		{
			Texture2D texture2D = new Texture2D(tex.width, tex.height, TextureFormat.RGB24, tex.mipmapCount > 0);
			texture2D.SetPixels32(pixels);
			texture2D.Apply(updateMipmaps: true);
			return texture2D;
		}
		return tex;
	}

	public static void FillPixelArray(Color32[] fillArray, byte[] pixelData, int bytesPerPixel, bool bgra = false)
	{
		if (bgra)
		{
			if (bytesPerPixel == 4)
			{
				for (int i = 0; i < fillArray.Length; i++)
				{
					int num = i * bytesPerPixel;
					fillArray[i] = new Color32(pixelData[num + 2], pixelData[num + 1], pixelData[num], pixelData[num + 3]);
				}
				return;
			}
			for (int j = 0; j < fillArray.Length; j++)
			{
				fillArray[j].r = pixelData[j * 3 + 2];
				fillArray[j].g = pixelData[j * 3 + 1];
				fillArray[j].b = pixelData[j * 3];
			}
		}
		else if (bytesPerPixel == 4)
		{
			for (int k = 0; k < fillArray.Length; k++)
			{
				fillArray[k].r = pixelData[k * 4];
				fillArray[k].g = pixelData[k * 4 + 1];
				fillArray[k].b = pixelData[k * 4 + 2];
				fillArray[k].a = pixelData[k * 4 + 3];
			}
		}
		else
		{
			int num2 = 0;
			for (int l = 0; l < fillArray.Length; l++)
			{
				fillArray[l].r = pixelData[num2++];
				fillArray[l].g = pixelData[num2++];
				fillArray[l].b = pixelData[num2++];
				fillArray[l].a = byte.MaxValue;
			}
		}
	}
}
