using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace B83.Image.BMP;

public class BMPLoader
{
	private const ushort MAGIC = 19778;

	public bool ReadPaletteAlpha;

	public bool ForceAlphaReadWhenPossible;

	public BMPImage LoadBMP(string aFileName)
	{
		using FileStream aData = File.OpenRead(aFileName);
		return LoadBMP(aData);
	}

	public BMPImage LoadBMP(byte[] aData)
	{
		using MemoryStream aData2 = new MemoryStream(aData);
		return LoadBMP(aData2);
	}

	public BMPImage LoadBMP(Stream aData)
	{
		using BinaryReader aReader = new BinaryReader(aData);
		return LoadBMP(aReader);
	}

	public BMPImage LoadBMP(BinaryReader aReader)
	{
		BMPImage bMPImage = new BMPImage();
		if (!ReadFileHeader(aReader, ref bMPImage.header))
		{
			Debug.LogError("Not a BMP file");
			return null;
		}
		if (!ReadInfoHeader(aReader, ref bMPImage.info))
		{
			Debug.LogError("Unsupported header format");
			return null;
		}
		if (bMPImage.info.compressionMethod != 0 && bMPImage.info.compressionMethod != BMPComressionMode.BI_BITFIELDS && bMPImage.info.compressionMethod != BMPComressionMode.BI_ALPHABITFIELDS && bMPImage.info.compressionMethod != BMPComressionMode.BI_RLE4 && bMPImage.info.compressionMethod != BMPComressionMode.BI_RLE8)
		{
			Debug.LogError("Unsupported image format: " + bMPImage.info.compressionMethod);
			return null;
		}
		long offset = 14 + bMPImage.info.size;
		aReader.BaseStream.Seek(offset, SeekOrigin.Begin);
		if (bMPImage.info.nBitsPerPixel < 24)
		{
			bMPImage.rMask = 31744u;
			bMPImage.gMask = 992u;
			bMPImage.bMask = 31u;
		}
		if (bMPImage.info.compressionMethod == BMPComressionMode.BI_BITFIELDS || bMPImage.info.compressionMethod == BMPComressionMode.BI_ALPHABITFIELDS)
		{
			bMPImage.rMask = aReader.ReadUInt32();
			bMPImage.gMask = aReader.ReadUInt32();
			bMPImage.bMask = aReader.ReadUInt32();
		}
		if (ForceAlphaReadWhenPossible)
		{
			bMPImage.aMask = GetMask(bMPImage.info.nBitsPerPixel) ^ (bMPImage.rMask | bMPImage.gMask | bMPImage.bMask);
		}
		if (bMPImage.info.compressionMethod == BMPComressionMode.BI_ALPHABITFIELDS)
		{
			bMPImage.aMask = aReader.ReadUInt32();
		}
		if (bMPImage.info.nPaletteColors != 0 || bMPImage.info.nBitsPerPixel <= 8)
		{
			bMPImage.palette = ReadPalette(aReader, bMPImage, ReadPaletteAlpha || ForceAlphaReadWhenPossible);
		}
		aReader.BaseStream.Seek(bMPImage.header.offset, SeekOrigin.Begin);
		bool flag = bMPImage.info.compressionMethod == BMPComressionMode.BI_RGB || bMPImage.info.compressionMethod == BMPComressionMode.BI_BITFIELDS || bMPImage.info.compressionMethod == BMPComressionMode.BI_ALPHABITFIELDS;
		if (bMPImage.info.nBitsPerPixel == 32 && flag)
		{
			Read32BitImage(aReader, bMPImage);
		}
		else if (bMPImage.info.nBitsPerPixel == 24 && flag)
		{
			Read24BitImage(aReader, bMPImage);
		}
		else if (bMPImage.info.nBitsPerPixel == 16 && flag)
		{
			Read16BitImage(aReader, bMPImage);
		}
		else if (bMPImage.info.compressionMethod == BMPComressionMode.BI_RLE4 && bMPImage.info.nBitsPerPixel == 4 && bMPImage.palette != null)
		{
			ReadIndexedImageRLE4(aReader, bMPImage);
		}
		else if (bMPImage.info.compressionMethod == BMPComressionMode.BI_RLE8 && bMPImage.info.nBitsPerPixel == 8 && bMPImage.palette != null)
		{
			ReadIndexedImageRLE8(aReader, bMPImage);
		}
		else
		{
			if (!flag || bMPImage.info.nBitsPerPixel > 8 || bMPImage.palette == null)
			{
				Debug.LogError(string.Concat("Unsupported file format: ", bMPImage.info.compressionMethod, " BPP: ", bMPImage.info.nBitsPerPixel));
				return null;
			}
			ReadIndexedImage(aReader, bMPImage);
		}
		return bMPImage;
	}

	private static void Read32BitImage(BinaryReader aReader, BMPImage bmp)
	{
		int num = Mathf.Abs(bmp.info.width);
		int num2 = Mathf.Abs(bmp.info.height);
		Color32[] array = (bmp.imageData = new Color32[num * num2]);
		if (aReader.BaseStream.Position + num * num2 * 4 > aReader.BaseStream.Length)
		{
			Debug.LogError("Unexpected end of file.");
			return;
		}
		int shiftCount = GetShiftCount(bmp.rMask);
		int shiftCount2 = GetShiftCount(bmp.gMask);
		int shiftCount3 = GetShiftCount(bmp.bMask);
		int shiftCount4 = GetShiftCount(bmp.aMask);
		byte a = byte.MaxValue;
		for (int i = 0; i < array.Length; i++)
		{
			uint num3 = aReader.ReadUInt32();
			byte r = (byte)((num3 & bmp.rMask) >> shiftCount);
			byte g = (byte)((num3 & bmp.gMask) >> shiftCount2);
			byte b = (byte)((num3 & bmp.bMask) >> shiftCount3);
			if (bmp.bMask != 0)
			{
				a = (byte)((num3 & bmp.aMask) >> shiftCount4);
			}
			array[i] = new Color32(r, g, b, a);
		}
	}

	private static void Read24BitImage(BinaryReader aReader, BMPImage bmp)
	{
		int num = Mathf.Abs(bmp.info.width);
		int num2 = Mathf.Abs(bmp.info.height);
		int num3 = (24 * num + 31) / 32 * 4;
		int num4 = num3 * num2;
		int num5 = num3 - num * 3;
		Color32[] array = (bmp.imageData = new Color32[num * num2]);
		if (aReader.BaseStream.Position + num4 > aReader.BaseStream.Length)
		{
			Debug.LogError("Unexpected end of file. (Have " + (aReader.BaseStream.Position + num4) + " bytes, expected " + aReader.BaseStream.Length + " bytes)");
			return;
		}
		int shiftCount = GetShiftCount(bmp.rMask);
		int shiftCount2 = GetShiftCount(bmp.gMask);
		int shiftCount3 = GetShiftCount(bmp.bMask);
		for (int i = 0; i < num2; i++)
		{
			for (int j = 0; j < num; j++)
			{
				int num6 = aReader.ReadByte() | (aReader.ReadByte() << 8) | (aReader.ReadByte() << 16);
				byte r = (byte)(((uint)num6 & bmp.rMask) >> shiftCount);
				byte g = (byte)(((uint)num6 & bmp.gMask) >> shiftCount2);
				byte b = (byte)(((uint)num6 & bmp.bMask) >> shiftCount3);
				array[j + i * num] = new Color32(r, g, b, byte.MaxValue);
			}
			for (int k = 0; k < num5; k++)
			{
				aReader.ReadByte();
			}
		}
	}

	private static void Read16BitImage(BinaryReader aReader, BMPImage bmp)
	{
		int num = Mathf.Abs(bmp.info.width);
		int num2 = Mathf.Abs(bmp.info.height);
		int num3 = (16 * num + 31) / 32 * 4;
		int num4 = num3 * num2;
		int num5 = num3 - num * 2;
		Color32[] array = (bmp.imageData = new Color32[num * num2]);
		if (aReader.BaseStream.Position + num4 > aReader.BaseStream.Length)
		{
			Debug.LogError("Unexpected end of file. (Have " + (aReader.BaseStream.Position + num4) + " bytes, expected " + aReader.BaseStream.Length + " bytes)");
			return;
		}
		int shiftCount = GetShiftCount(bmp.rMask);
		int shiftCount2 = GetShiftCount(bmp.gMask);
		int shiftCount3 = GetShiftCount(bmp.bMask);
		int shiftCount4 = GetShiftCount(bmp.aMask);
		byte a = byte.MaxValue;
		for (int i = 0; i < num2; i++)
		{
			for (int j = 0; j < num; j++)
			{
				uint num6 = (uint)(aReader.ReadByte() | (aReader.ReadByte() << 8));
				byte r = (byte)((num6 & bmp.rMask) >> shiftCount);
				byte g = (byte)((num6 & bmp.gMask) >> shiftCount2);
				byte b = (byte)((num6 & bmp.bMask) >> shiftCount3);
				if (bmp.aMask != 0)
				{
					a = (byte)((num6 & bmp.aMask) >> shiftCount4);
				}
				array[j + i * num] = new Color32(r, g, b, a);
			}
			for (int k = 0; k < num5; k++)
			{
				aReader.ReadByte();
			}
		}
	}

	private static void ReadIndexedImage(BinaryReader aReader, BMPImage bmp)
	{
		int num = Mathf.Abs(bmp.info.width);
		int num2 = Mathf.Abs(bmp.info.height);
		int nBitsPerPixel = bmp.info.nBitsPerPixel;
		int num3 = (nBitsPerPixel * num + 31) / 32 * 4;
		int num4 = num3 * num2;
		int num5 = num3 - (num * nBitsPerPixel + 7) / 8;
		Color32[] array = (bmp.imageData = new Color32[num * num2]);
		if (aReader.BaseStream.Position + num4 > aReader.BaseStream.Length)
		{
			Debug.LogError("Unexpected end of file. (Have " + (aReader.BaseStream.Position + num4) + " bytes, expected " + aReader.BaseStream.Length + " bytes)");
			return;
		}
		BitStreamReader bitStreamReader = new BitStreamReader(aReader);
		for (int i = 0; i < num2; i++)
		{
			for (int j = 0; j < num; j++)
			{
				int num6 = (int)bitStreamReader.ReadBits(nBitsPerPixel);
				if (num6 >= bmp.palette.Count)
				{
					Debug.LogError("Indexed bitmap has indices greater than it's color palette");
					return;
				}
				array[j + i * num] = bmp.palette[num6];
			}
			bitStreamReader.Flush();
			for (int k = 0; k < num5; k++)
			{
				aReader.ReadByte();
			}
		}
	}

	private static void ReadIndexedImageRLE4(BinaryReader aReader, BMPImage bmp)
	{
		int num = Mathf.Abs(bmp.info.width);
		int num2 = Mathf.Abs(bmp.info.height);
		Color32[] array = (bmp.imageData = new Color32[num * num2]);
		int num3 = 0;
		int num4 = 0;
		int num5 = 0;
		while (aReader.BaseStream.Position < aReader.BaseStream.Length - 1)
		{
			int num6 = aReader.ReadByte();
			byte b = aReader.ReadByte();
			if (num6 > 0)
			{
				for (int num7 = num6 / 2; num7 > 0; num7--)
				{
					array[num3++ + num5] = bmp.palette[(b >> 4) & 0xF];
					array[num3++ + num5] = bmp.palette[b & 0xF];
				}
				if ((num6 & 1) > 0)
				{
					array[num3++ + num5] = bmp.palette[(b >> 4) & 0xF];
				}
				continue;
			}
			switch (b)
			{
			case 0:
				num3 = 0;
				num4++;
				num5 = num4 * num;
				continue;
			case 2:
				num3 += aReader.ReadByte();
				num4 += aReader.ReadByte();
				num5 = num4 * num;
				continue;
			case 1:
				return;
			}
			for (int num8 = (int)b / 2; num8 > 0; num8--)
			{
				byte b2 = aReader.ReadByte();
				array[num3++ + num5] = bmp.palette[(b2 >> 4) & 0xF];
				array[num3++ + num5] = bmp.palette[b2 & 0xF];
			}
			if ((b & 1) > 0)
			{
				array[num3++ + num5] = bmp.palette[(aReader.ReadByte() >> 4) & 0xF];
			}
			if ((((b - 1) / 2) & 1) == 0)
			{
				aReader.ReadByte();
			}
		}
	}

	private static void ReadIndexedImageRLE8(BinaryReader aReader, BMPImage bmp)
	{
		int num = Mathf.Abs(bmp.info.width);
		int num2 = Mathf.Abs(bmp.info.height);
		Color32[] array = (bmp.imageData = new Color32[num * num2]);
		int num3 = 0;
		int num4 = 0;
		int num5 = 0;
		while (aReader.BaseStream.Position < aReader.BaseStream.Length - 1)
		{
			int num6 = aReader.ReadByte();
			byte b = aReader.ReadByte();
			if (num6 > 0)
			{
				for (int num7 = num6; num7 > 0; num7--)
				{
					array[num3++ + num5] = bmp.palette[b];
				}
				continue;
			}
			switch (b)
			{
			case 0:
				num3 = 0;
				num4++;
				num5 = num4 * num;
				continue;
			case 2:
				num3 += aReader.ReadByte();
				num4 += aReader.ReadByte();
				num5 = num4 * num;
				continue;
			case 1:
				return;
			}
			for (int num8 = b; num8 > 0; num8--)
			{
				array[num3++ + num5] = bmp.palette[aReader.ReadByte()];
			}
			if ((b & 1) > 0)
			{
				aReader.ReadByte();
			}
		}
	}

	private static int GetShiftCount(uint mask)
	{
		for (int i = 0; i < 32; i++)
		{
			if ((mask & (true ? 1u : 0u)) != 0)
			{
				return i;
			}
			mask >>= 1;
		}
		return -1;
	}

	private static uint GetMask(int bitCount)
	{
		uint num = 0u;
		for (int i = 0; i < bitCount; i++)
		{
			num <<= 1;
			num |= 1u;
		}
		return num;
	}

	private static bool ReadFileHeader(BinaryReader aReader, ref BMPFileHeader aFileHeader)
	{
		aFileHeader.magic = aReader.ReadUInt16();
		if (aFileHeader.magic != 19778)
		{
			return false;
		}
		aFileHeader.filesize = aReader.ReadUInt32();
		aFileHeader.reserved = aReader.ReadUInt32();
		aFileHeader.offset = aReader.ReadUInt32();
		return true;
	}

	private static bool ReadInfoHeader(BinaryReader aReader, ref BitmapInfoHeader aHeader)
	{
		aHeader.size = aReader.ReadUInt32();
		if (aHeader.size < 40)
		{
			return false;
		}
		aHeader.width = aReader.ReadInt32();
		aHeader.height = aReader.ReadInt32();
		aHeader.nColorPlanes = aReader.ReadUInt16();
		aHeader.nBitsPerPixel = aReader.ReadUInt16();
		aHeader.compressionMethod = (BMPComressionMode)aReader.ReadInt32();
		aHeader.rawImageSize = aReader.ReadUInt32();
		aHeader.xPPM = aReader.ReadInt32();
		aHeader.yPPM = aReader.ReadInt32();
		aHeader.nPaletteColors = aReader.ReadUInt32();
		aHeader.nImportantColors = aReader.ReadUInt32();
		int num = (int)(aHeader.size - 40);
		if (num > 0)
		{
			aReader.ReadBytes(num);
		}
		return true;
	}

	public static List<Color32> ReadPalette(BinaryReader aReader, BMPImage aBmp, bool aReadAlpha)
	{
		uint num = aBmp.info.nPaletteColors;
		if (num == 0)
		{
			num = (uint)(1 << (int)aBmp.info.nBitsPerPixel);
		}
		List<Color32> list = new List<Color32>((int)num);
		for (int i = 0; i < num; i++)
		{
			byte b = aReader.ReadByte();
			byte g = aReader.ReadByte();
			byte r = aReader.ReadByte();
			byte a = aReader.ReadByte();
			if (!aReadAlpha)
			{
				a = byte.MaxValue;
			}
			list.Add(new Color32(r, g, b, a));
		}
		return list;
	}
}
