using System.IO;
using UnityEngine;

namespace Dummiesman;

public class CharWordReader
{
	public char[] word;

	public int wordSize;

	public bool endReached;

	private StreamReader reader;

	private int bufferSize;

	private char[] buffer;

	public char currentChar;

	private int currentPosition;

	private int maxPosition;

	public CharWordReader(StreamReader reader, int bufferSize)
	{
		this.reader = reader;
		this.bufferSize = bufferSize;
		buffer = new char[this.bufferSize];
		word = new char[this.bufferSize];
		MoveNext();
	}

	public void SkipWhitespaces()
	{
		while (char.IsWhiteSpace(currentChar))
		{
			MoveNext();
		}
	}

	public void SkipWhitespaces(out bool newLinePassed)
	{
		newLinePassed = false;
		while (char.IsWhiteSpace(currentChar))
		{
			if (currentChar == '\r' || currentChar == '\n')
			{
				newLinePassed = true;
			}
			MoveNext();
		}
	}

	public void SkipUntilNewLine()
	{
		while (currentChar != 0 && currentChar != '\n' && currentChar != '\r')
		{
			MoveNext();
		}
		SkipNewLineSymbols();
	}

	public void ReadUntilWhiteSpace()
	{
		wordSize = 0;
		while (currentChar != 0 && !char.IsWhiteSpace(currentChar))
		{
			word[wordSize] = currentChar;
			wordSize++;
			MoveNext();
		}
	}

	public void ReadUntilNewLine()
	{
		wordSize = 0;
		while (currentChar != 0 && currentChar != '\n' && currentChar != '\r')
		{
			word[wordSize] = currentChar;
			wordSize++;
			MoveNext();
		}
		SkipNewLineSymbols();
	}

	public bool Is(string other)
	{
		if (other.Length != wordSize)
		{
			return false;
		}
		for (int i = 0; i < wordSize; i++)
		{
			if (word[i] != other[i])
			{
				return false;
			}
		}
		return true;
	}

	public string GetString(int startIndex = 0)
	{
		if (startIndex >= wordSize - 1)
		{
			return string.Empty;
		}
		return new string(word, startIndex, wordSize - startIndex);
	}

	public Vector3 ReadVector()
	{
		SkipWhitespaces();
		float x = ReadFloat();
		SkipWhitespaces();
		float y = ReadFloat();
		SkipWhitespaces(out var newLinePassed);
		float z = 0f;
		if (!newLinePassed)
		{
			z = ReadFloat();
		}
		return new Vector3(x, y, z);
	}

	public int ReadInt()
	{
		int num = 0;
		bool flag = currentChar == '-';
		if (flag)
		{
			MoveNext();
		}
		while (currentChar >= '0' && currentChar <= '9')
		{
			int num2 = currentChar - 48;
			num = num * 10 + num2;
			MoveNext();
		}
		if (!flag)
		{
			return num;
		}
		return -num;
	}

	public float ReadFloat()
	{
		bool num = currentChar == '-';
		if (num)
		{
			MoveNext();
		}
		float num2 = ReadInt();
		if (currentChar == '.' || currentChar == ',')
		{
			MoveNext();
			num2 += ReadFloatEnd();
			if (currentChar == 'e' || currentChar == 'E')
			{
				MoveNext();
				int num3 = ReadInt();
				num2 *= Mathf.Pow(10f, num3);
			}
		}
		if (num)
		{
			num2 = 0f - num2;
		}
		return num2;
	}

	private float ReadFloatEnd()
	{
		float num = 0f;
		float num2 = 0.1f;
		while (currentChar >= '0' && currentChar <= '9')
		{
			int num3 = currentChar - 48;
			num += (float)num3 * num2;
			num2 *= 0.1f;
			MoveNext();
		}
		return num;
	}

	private void SkipNewLineSymbols()
	{
		while (currentChar == '\n' || currentChar == '\r')
		{
			MoveNext();
		}
	}

	public void MoveNext()
	{
		currentPosition++;
		if (currentPosition >= maxPosition)
		{
			if (reader.EndOfStream)
			{
				currentChar = '\0';
				endReached = true;
				return;
			}
			currentPosition = 0;
			maxPosition = reader.Read(buffer, 0, bufferSize);
		}
		currentChar = buffer[currentPosition];
	}
}
