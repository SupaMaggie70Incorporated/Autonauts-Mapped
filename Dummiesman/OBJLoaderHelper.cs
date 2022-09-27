using System.Globalization;
using UnityEngine;

namespace Dummiesman;

public static class OBJLoaderHelper
{
	public static void EnableMaterialTransparency(Material mtl)
	{
		mtl.SetFloat("_Mode", 3f);
		mtl.SetInt("_SrcBlend", 5);
		mtl.SetInt("_DstBlend", 10);
		mtl.SetInt("_ZWrite", 0);
		mtl.DisableKeyword("_ALPHATEST_ON");
		mtl.EnableKeyword("_ALPHABLEND_ON");
		mtl.DisableKeyword("_ALPHAPREMULTIPLY_ON");
		mtl.renderQueue = 3000;
	}

	public static void DisableMaterialTransparency(Material mtl)
	{
		mtl.SetFloat("_Mode", 0f);
		mtl.SetInt("_SrcBlend", 1);
		mtl.SetInt("_DstBlend", 0);
		mtl.SetInt("_ZWrite", 1);
		mtl.DisableKeyword("_ALPHATEST_ON");
		mtl.DisableKeyword("_ALPHABLEND_ON");
		mtl.DisableKeyword("_ALPHAPREMULTIPLY_ON");
		mtl.renderQueue = 2000;
	}

	public static float FastFloatParse(string input)
	{
		if (input.Contains("e") || input.Contains("E"))
		{
			return float.Parse(input, CultureInfo.InvariantCulture);
		}
		float num = 0f;
		int num2 = 0;
		int length = input.Length;
		if (length == 0)
		{
			return float.NaN;
		}
		char c = input[0];
		float num3 = 1f;
		if (c == '-')
		{
			num3 = -1f;
			num2++;
			if (num2 >= length)
			{
				return float.NaN;
			}
		}
		while (true)
		{
			if (num2 >= length)
			{
				return num3 * num;
			}
			c = input[num2++];
			if (c < '0' || c > '9')
			{
				break;
			}
			num = num * 10f + (float)(c - 48);
		}
		if (c != '.' && c != ',')
		{
			return float.NaN;
		}
		float num4 = 0.1f;
		while (num2 < length)
		{
			c = input[num2++];
			if (c < '0' || c > '9')
			{
				return float.NaN;
			}
			num += (float)(c - 48) * num4;
			num4 *= 0.1f;
		}
		return num3 * num;
	}

	public static int FastIntParse(string input)
	{
		int num = 0;
		bool flag = input[0] == '-';
		for (int i = (flag ? 1 : 0); i < input.Length; i++)
		{
			num = num * 10 + (input[i] - 48);
		}
		if (!flag)
		{
			return num;
		}
		return -num;
	}

	public static Material CreateNullMaterial()
	{
		return new Material(Shader.Find("Standard (Specular setup)"));
	}

	public static Vector3 VectorFromStrArray(string[] cmps)
	{
		float x = FastFloatParse(cmps[1]);
		float y = FastFloatParse(cmps[2]);
		if (cmps.Length == 4)
		{
			float z = FastFloatParse(cmps[3]);
			return new Vector3(x, y, z);
		}
		return new Vector2(x, y);
	}

	public static Color ColorFromStrArray(string[] cmps, float scalar = 1f)
	{
		float r = FastFloatParse(cmps[1]) * scalar;
		float g = FastFloatParse(cmps[2]) * scalar;
		float b = FastFloatParse(cmps[3]) * scalar;
		return new Color(r, g, b);
	}
}
