using UnityEngine;

internal class Noise
{
	private static int[] p = new int[256]
	{
		151, 160, 137, 91, 90, 15, 131, 13, 201, 95,
		96, 53, 194, 233, 7, 225, 140, 36, 103, 30,
		69, 142, 8, 99, 37, 240, 21, 10, 23, 190,
		6, 148, 247, 120, 234, 75, 0, 26, 197, 62,
		94, 252, 219, 203, 117, 35, 11, 32, 57, 177,
		33, 88, 237, 149, 56, 87, 174, 20, 125, 136,
		171, 168, 68, 175, 74, 165, 71, 134, 139, 48,
		27, 166, 77, 146, 158, 231, 83, 111, 229, 122,
		60, 211, 133, 230, 220, 105, 92, 41, 55, 46,
		245, 40, 244, 102, 143, 54, 65, 25, 63, 161,
		1, 216, 80, 73, 209, 76, 132, 187, 208, 89,
		18, 169, 200, 196, 135, 130, 116, 188, 159, 86,
		164, 100, 109, 198, 173, 186, 3, 64, 52, 217,
		226, 250, 124, 123, 5, 202, 38, 147, 118, 126,
		255, 82, 85, 212, 207, 206, 59, 227, 47, 16,
		58, 17, 182, 189, 28, 42, 223, 183, 170, 213,
		119, 248, 152, 2, 44, 154, 163, 70, 221, 153,
		101, 155, 167, 43, 172, 9, 129, 22, 39, 253,
		19, 98, 108, 110, 79, 113, 224, 232, 178, 185,
		112, 104, 218, 246, 97, 228, 251, 34, 242, 193,
		238, 210, 144, 12, 191, 179, 162, 241, 81, 51,
		145, 235, 249, 14, 239, 107, 49, 192, 214, 31,
		181, 199, 106, 157, 184, 84, 204, 176, 115, 121,
		50, 45, 127, 4, 150, 254, 138, 236, 205, 93,
		222, 114, 67, 29, 24, 72, 243, 141, 128, 195,
		78, 66, 215, 61, 156, 180
	};

	private static float F2 = 0.5f * (Mathf.Sqrt(3f) - 1f);

	private static float G2 = (3f - Mathf.Sqrt(3f)) / 6f;

	public static Grad[] grad3 = new Grad[12]
	{
		new Grad(1f, 1f),
		new Grad(-1f, 1f),
		new Grad(1f, -1f),
		new Grad(-1f, -1f),
		new Grad(1f, 0f),
		new Grad(-1f, 0f),
		new Grad(1f, 0f),
		new Grad(-1f, 0f),
		new Grad(0f, 1f),
		new Grad(0f, -1f),
		new Grad(0f, 1f),
		new Grad(0f, -1f)
	};

	private int[] perm;

	private Grad[] gradP;

	private int m_Iterations;

	private float m_Scale;

	public float m_Threshold;

	public Noise(int seed, int Iterations, float Scale, float Threshold)
	{
		m_Iterations = Iterations;
		m_Scale = Scale;
		m_Threshold = Threshold;
		perm = new int[512];
		gradP = new Grad[512];
		if (seed > 0 && seed < 1)
		{
			seed *= 65536;
		}
		if (seed < 256)
		{
			seed |= seed << 8;
		}
		for (int i = 0; i < 256; i++)
		{
			int num = 0;
			num = (((i & 1) == 0) ? (p[i] ^ ((seed >> 8) & 0xFF)) : (p[i] ^ (seed & 0xFF)));
			perm[i] = (perm[i + 256] = num);
			gradP[i] = (gradP[i + 256] = grad3[num % 12]);
		}
	}

	public float simplex2(float x, float y)
	{
		float num = (x + y) * F2;
		int num2 = (int)Mathf.Floor(x + num);
		int num3 = (int)Mathf.Floor(y + num);
		float num4 = (float)(num2 + num3) * G2;
		float num5 = x - (float)num2 + num4;
		float num6 = y - (float)num3 + num4;
		int num7;
		int num8;
		if (num5 > num6)
		{
			num7 = 1;
			num8 = 0;
		}
		else
		{
			num7 = 0;
			num8 = 1;
		}
		float num9 = num5 - (float)num7 + G2;
		float num10 = num6 - (float)num8 + G2;
		float num11 = num5 - 1f + 2f * G2;
		float num12 = num6 - 1f + 2f * G2;
		num2 &= 0xFF;
		num3 &= 0xFF;
		Grad grad = gradP[num2 + perm[num3]];
		Grad grad2 = gradP[num2 + num7 + perm[num3 + num8]];
		Grad grad3 = gradP[num2 + 1 + perm[num3 + 1]];
		float num13 = 0.5f - num5 * num5 - num6 * num6;
		float num14;
		if (num13 < 0f)
		{
			num14 = 0f;
		}
		else
		{
			num13 *= num13;
			num14 = num13 * num13 * (grad.x * num5 + grad.y * num6);
		}
		float num15 = 0.5f - num9 * num9 - num10 * num10;
		float num16;
		if (num15 < 0f)
		{
			num16 = 0f;
		}
		else
		{
			num15 *= num15;
			num16 = num15 * num15 * (grad2.x * num9 + grad2.y * num10);
		}
		float num17 = 0.5f - num11 * num11 - num12 * num12;
		float num18;
		if (num17 < 0f)
		{
			num18 = 0f;
		}
		else
		{
			num17 *= num17;
			num18 = num17 * num17 * (grad3.x * num11 + grad3.y * num12);
		}
		return 70f * (num14 + num16 + num18);
	}

	public float GetNoise(float x, float y)
	{
		x *= m_Scale;
		y *= m_Scale;
		float num = 1f;
		float num2 = 0f;
		float num3 = 0f;
		for (int i = 0; i < m_Iterations; i++)
		{
			num3 += simplex2(x / num, y / num) * num;
			num2 += num;
			num *= 0.5f;
		}
		return (num3 / num2 + 1f) / 2f;
	}
}
