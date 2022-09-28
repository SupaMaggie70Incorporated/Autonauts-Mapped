using UnityEngine;

public class BuildingLight : Building
{
	private MyLight m_Light;

	public override void Restart()
	{
		base.Restart();
		SetDimensions(new TileCoord(0, 0), new TileCoord(0, 0), new TileCoord(0, 1));
		HideAccessModel();
	}

	public override void PostCreate()
	{
		base.PostCreate();
		Transform newParent = base.transform;
		if ((bool)ObjectUtils.FindDeepChild(base.transform, "LightPoint"))
		{
			newParent = ObjectUtils.FindDeepChild(base.transform, "LightPoint").transform;
		}
		m_Light = LightManager.Instance.LoadLight("Streetlamp", newParent, default(Vector3));
		if (SaveLoadManager.m_Video)
		{
			m_Light.SetActive(Active: false);
		}
	}

	protected new void OnDestroy()
	{
		LightManager.Instance.DestroyLight(m_Light);
		base.OnDestroy();
	}

	public override void SetBlueprint(bool Blueprint, bool Register = false)
	{
		base.SetBlueprint(Blueprint, Register);
		m_Light.SetActive(!Blueprint);
	}

	private void Update()
	{
		m_Light.UpdateIntensity();
	}
}
