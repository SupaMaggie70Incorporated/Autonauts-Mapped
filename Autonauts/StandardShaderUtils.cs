using UnityEngine;

public static class StandardShaderUtils
{
	public enum BlendMode
	{
		Opaque,
		Cutout,
		Fade,
		Transparent
	}

	public static BlendMode GetRenderMode(Material standardShaderMaterial)
	{
		int @int = standardShaderMaterial.GetInt("_SrcBlend");
		int int2 = standardShaderMaterial.GetInt("_DstBlend");
		bool flag = standardShaderMaterial.IsKeywordEnabled("_ALPHATEST_ON");
		if (@int == 1 && int2 == 0)
		{
			if (flag)
			{
				return BlendMode.Cutout;
			}
			return BlendMode.Opaque;
		}
		return BlendMode.Transparent;
	}

	public static void ChangeRenderMode(Material standardShaderMaterial, BlendMode blendMode)
	{
		switch (blendMode)
		{
		case BlendMode.Opaque:
			standardShaderMaterial.SetInt("_SrcBlend", 1);
			standardShaderMaterial.SetInt("_DstBlend", 0);
			standardShaderMaterial.SetInt("_ZWrite", 1);
			standardShaderMaterial.DisableKeyword("_ALPHATEST_ON");
			standardShaderMaterial.DisableKeyword("_ALPHABLEND_ON");
			standardShaderMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
			standardShaderMaterial.renderQueue = -1;
			break;
		case BlendMode.Cutout:
			standardShaderMaterial.SetInt("_SrcBlend", 1);
			standardShaderMaterial.SetInt("_DstBlend", 0);
			standardShaderMaterial.SetInt("_ZWrite", 1);
			standardShaderMaterial.EnableKeyword("_ALPHATEST_ON");
			standardShaderMaterial.DisableKeyword("_ALPHABLEND_ON");
			standardShaderMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
			standardShaderMaterial.renderQueue = 2450;
			break;
		case BlendMode.Fade:
			standardShaderMaterial.SetInt("_SrcBlend", 5);
			standardShaderMaterial.SetInt("_DstBlend", 10);
			standardShaderMaterial.SetInt("_ZWrite", 0);
			standardShaderMaterial.DisableKeyword("_ALPHATEST_ON");
			standardShaderMaterial.EnableKeyword("_ALPHABLEND_ON");
			standardShaderMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
			standardShaderMaterial.renderQueue = 3000;
			break;
		case BlendMode.Transparent:
			standardShaderMaterial.SetInt("_SrcBlend", 1);
			standardShaderMaterial.SetInt("_DstBlend", 10);
			standardShaderMaterial.SetInt("_ZWrite", 0);
			standardShaderMaterial.DisableKeyword("_ALPHATEST_ON");
			standardShaderMaterial.DisableKeyword("_ALPHABLEND_ON");
			standardShaderMaterial.EnableKeyword("_ALPHAPREMULTIPLY_ON");
			standardShaderMaterial.renderQueue = 3000;
			break;
		}
	}

	public static void MakeObjectTransparent(GameObject NewObject)
	{
		ModelManager.Instance.SetMaterialsTransparent(NewObject, Transparent: true);
	}

	public static void MakeObjectTransparent(Building NewObject)
	{
		ModelManager.Instance.SetMaterialsTransparent(NewObject.gameObject, Transparent: true);
	}

	public static void RestoreOldMaterials(Building NewObject)
	{
		ModelManager.Instance.SetMaterialsTransparent(NewObject.gameObject, Transparent: false);
	}
}
