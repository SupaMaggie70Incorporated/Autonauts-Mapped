using UnityEngine;

public class SewingStation : LinkedSystemConverter
{
	private PlaySound m_PlaySound;

	private GameObject m_NeedleArm;

	private GameObject m_Gear;

	public override void Restart()
	{
		base.Restart();
		SetDimensions(new TileCoord(0, 0), new TileCoord(1, 0), new TileCoord(0, 1));
		SetSpawnPoint(new TileCoord(2, 0));
		m_DisplayIngredients = true;
		m_PulleySide = 1;
	}

	public override void PostCreate()
	{
		base.PostCreate();
		m_NeedleArm = m_ModelRoot.transform.Find("NeedleArm").gameObject;
		m_Gear = m_ModelRoot.transform.Find("Gear").gameObject;
	}

	public override void StartConverting()
	{
		base.StartConverting();
		m_PlaySound = AudioManager.Instance.StartEvent("BuildingBlueprintMaking", this, Remember: true);
	}

	protected override void UpdateConverting()
	{
		m_Gear.transform.rotation = Quaternion.Euler(1400f * TimeManager.Instance.m_NormalDelta, 0f, 0f) * m_Gear.transform.rotation;
		if ((int)(m_StateTimer * 60f) % 8 < 4)
		{
			m_NeedleArm.transform.localRotation = Quaternion.Euler(-75f, 90f, 90f);
		}
		else
		{
			m_NeedleArm.transform.localRotation = Quaternion.Euler(-90f, 90f, 90f);
		}
	}

	protected override void EndConverting()
	{
		AudioManager.Instance.StopEvent(m_PlaySound);
		AudioManager.Instance.StartEvent("BuildingToolMakingComplete", this);
		m_NeedleArm.transform.localRotation = Quaternion.Euler(-90f, 0f, 180f);
	}
}
