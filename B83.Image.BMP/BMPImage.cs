using System.Collections.Generic;
using UnityEngine;

namespace B83.Image.BMP;

public class BMPImage
{
	public BMPFileHeader header;

	public BitmapInfoHeader info;

	public uint rMask = 16711680u;

	public uint gMask = 65280u;

	public uint bMask = 255u;

	public uint aMask;

	public List<Color32> palette;

	public Color32[] imageData;

	public Texture2D ToTexture2D()
	{
		Texture2D texture2D = new Texture2D(info.absWidth, info.absHeight);
		texture2D.SetPixels32(imageData);
		texture2D.Apply();
		return texture2D;
	}
}
