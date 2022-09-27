using UnityEngine;

namespace B83.Image.BMP;

public struct BitmapInfoHeader
{
	public uint size;

	public int width;

	public int height;

	public ushort nColorPlanes;

	public ushort nBitsPerPixel;

	public BMPComressionMode compressionMethod;

	public uint rawImageSize;

	public int xPPM;

	public int yPPM;

	public uint nPaletteColors;

	public uint nImportantColors;

	public int absWidth => Mathf.Abs(width);

	public int absHeight => Mathf.Abs(height);
}
