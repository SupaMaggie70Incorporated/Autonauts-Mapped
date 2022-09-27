using UnityEngine;

namespace Dummiesman;

public static class ImageUtils
{
	public static Texture2D ConvertToNormalMap(Texture2D tex)
	{
		Texture2D texture2D = tex;
		if (tex.format != TextureFormat.RGBA32 && tex.format != TextureFormat.ARGB32)
		{
			texture2D = new Texture2D(tex.width, tex.height, TextureFormat.RGBA32, mipChain: true);
		}
		Color[] pixels = tex.GetPixels();
		for (int i = 0; i < pixels.Length; i++)
		{
			Color color = pixels[i];
			color.a = pixels[i].r;
			color.r = 0f;
			color.g = pixels[i].g;
			color.b = 0f;
			pixels[i] = color;
		}
		texture2D.SetPixels(pixels);
		texture2D.Apply(updateMipmaps: true);
		return texture2D;
	}
}
