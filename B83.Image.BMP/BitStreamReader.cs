using System;
using System.IO;

namespace B83.Image.BMP;

public class BitStreamReader
{
	private BinaryReader m_Reader;

	private byte m_Data;

	private int m_Bits;

	public BitStreamReader(BinaryReader aReader)
	{
		m_Reader = aReader;
	}

	public BitStreamReader(Stream aStream)
		: this(new BinaryReader(aStream))
	{
	}

	public byte ReadBit()
	{
		if (m_Bits <= 0)
		{
			m_Data = m_Reader.ReadByte();
			m_Bits = 8;
		}
		return (byte)((uint)(m_Data >> --m_Bits) & 1u);
	}

	public ulong ReadBits(int aCount)
	{
		ulong num = 0uL;
		if (aCount <= 0 || aCount > 32)
		{
			throw new ArgumentOutOfRangeException("aCount", "aCount must be between 1 and 32 inclusive");
		}
		for (int num2 = aCount - 1; num2 >= 0; num2--)
		{
			num |= (ulong)ReadBit() << num2;
		}
		return num;
	}

	public void Flush()
	{
		m_Data = 0;
		m_Bits = 0;
	}
}
