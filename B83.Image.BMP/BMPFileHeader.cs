namespace B83.Image.BMP;

public struct BMPFileHeader
{
	public ushort magic;

	public uint filesize;

	public uint reserved;

	public uint offset;
}
