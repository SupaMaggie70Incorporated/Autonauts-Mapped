using UnityEngine;

public class VehicleAssemblerGood : LinkedSystemConverter
{
	private PlaySound m_PlaySound;

	private MyParticles m_Particles;

	private Transform m_SmokePoint;

	public override void Restart()
	{
		base.Restart();
		m_DisplayIngredients = true;
		SetDimensions(new TileCoord(-2, -1), new TileCoord(2, 1), new TileCoord(-3, 0));
		SetSpawnPoint(new TileCoord(3, 0));
		m_PulleySide = 1;
		m_DisplayIngredients = false;
	}

	public override void PostCreate()
	{
		base.PostCreate();
		m_SmokePoint = ObjectUtils.FindDeepChild(m_ModelRoot.transform, "SmokePoint");
	}

	public override void StartConverting()
	{
		base.StartConverting();
		m_PlaySound = AudioManager.Instance.StartEvent("BuildingWorkbenchMaking", this, Remember: true);
	}

	protected override void UpdateConverting()
	{
		if ((int)(m_StateTimer * 60f) % 20 < 10)
		{
			m_ModelRoot.transform.localScale = new Vector3(0.9f, 1.3f, 0.9f);
		}
		else
		{
			m_ModelRoot.transform.localScale = new Vector3(1f, 1f, 1f);
		}
	}

	protected override void EndConverting()
	{
		AudioManager.Instance.StopEvent(m_PlaySound);
		AudioManager.Instance.StartEvent("BuildingToolMakingComplete", this);
		m_ModelRoot.transform.localScale = new Vector3(1f, 1f, 1f);
	}
}
