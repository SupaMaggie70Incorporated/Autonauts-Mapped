using System;
using UnityEngine;

namespace RWDTools;

public static class ExtensionMethods
{
	public static float ToAngleDeg(this Vector2 vector)
	{
		return Mathf.Atan2(vector.x, vector.y) * 57.29578f;
	}

	public static Vector2 DegToVector(this float angle)
	{
		return new Vector2(Mathf.Cos(angle * ((float)Math.PI / 180f)), Mathf.Sin(angle * ((float)Math.PI / 180f)));
	}

	public static bool isPowerOfTwo(this int x)
	{
		if (x != 0)
		{
			return (x & (~x + 1)) == x;
		}
		return false;
	}
}
