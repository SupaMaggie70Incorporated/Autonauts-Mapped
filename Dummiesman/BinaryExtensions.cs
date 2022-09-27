using System.IO;
using UnityEngine;

namespace Dummiesman;

public static class BinaryExtensions
{
	public static Color32 ReadColor32RGBR(this BinaryReader r)
	{
		byte[] array = r.ReadBytes(4);
		return new Color32(array[0], array[1], array[2], byte.MaxValue);
	}

	public static Color32 ReadColor32RGBA(this BinaryReader r)
	{
		byte[] array = r.ReadBytes(4);
		return new Color32(array[0], array[1], array[2], array[3]);
	}

	public static Color32 ReadColor32RGB(this BinaryReader r)
	{
		byte[] array = r.ReadBytes(3);
		return new Color32(array[0], array[1], array[2], byte.MaxValue);
	}

	public static Color32 ReadColor32BGR(this BinaryReader r)
	{
		byte[] array = r.ReadBytes(3);
		return new Color32(array[2], array[1], array[0], byte.MaxValue);
	}
}
