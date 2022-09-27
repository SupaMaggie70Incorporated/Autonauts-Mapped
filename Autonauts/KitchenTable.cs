using UnityEngine;

public class KitchenTable : Converter
{
	private PlaySound m_PlaySound;

	private GameObject m_Spoon;

	private MyParticles m_Particles;

	private bool m_SpoonAnimation;

	public override void Restart()
	{
		base.Restart();
		SetDimensions(new TileCoord(0, 0), new TileCoord(1, 0), new TileCoord(0, 1));
		SetSpawnPoint(new TileCoord(2, 0));
		m_Spoon = m_ModelRoot.transform.Find("Spoon").gameObject;
	}

	public override void StartConverting()
	{
		base.StartConverting();
		m_PlaySound = AudioManager.Instance.StartEvent("BuildingBlueprintMaking", this, Remember: true);
		m_SpoonAnimation = true;
		if (m_Results[m_ResultsToCreate][0].m_Type == ObjectType.FishRaw)
		{
			m_SpoonAnimation = false;
		}
		if (m_SpoonAnimation)
		{
			m_Particles = ParticlesManager.Instance.CreateParticles("SpoonSplash", base.transform.position, Quaternion.Euler(-70f, 60f, 0f));
		}
	}

	protected override void UpdateConverting()
	{
		if (m_SpoonAnimation)
		{
			m_Spoon.transform.rotation = Quaternion.Euler(0f, 1400f * TimeManager.Instance.m_NormalDelta, 0f) * m_Spoon.transform.rotation;
			m_Particles.transform.rotation = Quaternion.Euler(0f, 1400f * TimeManager.Instance.m_NormalDelta, 0f) * m_Particles.transform.rotation;
		}
		else if ((int)(m_StateTimer * 60f) % 20 < 10)
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
		if (m_SpoonAnimation)
		{
			ParticlesManager.Instance.DestroyParticles(m_Particles, WaitUntilNoParticles: true, StopSpawning: true);
		}
		else
		{
			m_ModelRoot.transform.localScale = new Vector3(1f, 1f, 1f);
		}
	}
}
