using System.Collections.Generic;
using UnityEngine;

public class LightManager : MonoBehaviour
{
	public static LightManager Instance;

	private List<MyLight> m_Lights;

	private void Awake()
	{
		Instance = this;
		m_Lights = new List<MyLight>();
		SetupFalloff();
	}

	private void SetupFalloff()
	{
		int num = 16;
		Texture2D texture2D = new Texture2D(num, 1, TextureFormat.ARGB32, mipChain: false, linear: true);
		texture2D.filterMode = FilterMode.Bilinear;
		texture2D.wrapMode = TextureWrapMode.Clamp;
		Color[] array = new Color[num * num];
		new Vector2(0f, num / 2);
		int num2 = 5;
		for (int i = 0; i < num; i++)
		{
			float num3 = 1f - (float)(i + 1) / (float)num;
			array[i] = new Color(num3, num3, num3, num3);
		}
		texture2D.SetPixels(array);
		texture2D.Apply();
		Shader.SetGlobalTexture("_LightTextureNew", texture2D);
	}

	public MyLight LoadLight(string PrefabName, Transform NewParent, Vector3 LocalPosition)
	{
		MyLight component = Object.Instantiate((GameObject)Resources.Load("Prefabs/Lights/" + PrefabName, typeof(GameObject)), base.transform.position, Quaternion.identity, NewParent).GetComponent<MyLight>();
		component.transform.localPosition = LocalPosition;
		m_Lights.Add(component);
		return component;
	}

	public void DestroyLight(MyLight NewLight)
	{
		m_Lights.Remove(NewLight);
		Object.Destroy(NewLight.gameObject);
	}

	public void UpdateLightsEnabled()
	{
		foreach (MyLight light in m_Lights)
		{
			light.UpdateActive();
		}
	}

	public void UpdateLightsIntensity()
	{
		foreach (MyLight light in m_Lights)
		{
			light.UpdateIntensity();
		}
	}

	public void SetDesaturationActive(bool Desaturated)
	{
		foreach (MyLight light in m_Lights)
		{
			light.SetDesaturationActive(Desaturated);
		}
	}
}
