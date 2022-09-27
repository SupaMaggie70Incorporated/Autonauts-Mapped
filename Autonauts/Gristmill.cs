using UnityEngine;

public class Gristmill : LinkedSystemConverter
{
	private GameObject m_Handle;

	private GameObject m_Gear;

	private GameObject m_Top;

	private GameObject m_Funnel;

	private PlaySound m_PlaySound;

	private MyParticles m_ChaffParticles;

	public override void Restart()
	{
		base.Restart();
		m_DisplayIngredients = true;
		SetDimensions(new TileCoord(0, -1), new TileCoord(1, 0), new TileCoord(0, 1));
		SetSpawnPoint(new TileCoord(2, 0));
		m_Handle = m_ModelRoot.transform.Find("Crank").gameObject;
		m_Gear = m_ModelRoot.transform.Find("Gear").gameObject;
		m_Top = m_ModelRoot.transform.Find("GrindstoneTop").gameObject;
		if ((bool)m_ModelRoot.transform.Find("Funnel"))
		{
			m_Funnel = m_ModelRoot.transform.Find("Funnel").gameObject;
		}
		m_PulleySide = 0;
	}

	public override void PostCreate()
	{
		base.PostCreate();
	}

	public override void StartConverting()
	{
		base.StartConverting();
		m_PlaySound = AudioManager.Instance.StartEvent("BuildingBenchSaw2Making", this, Remember: true);
		m_ChaffParticles = ParticlesManager.Instance.CreateParticles("Chaff", m_IngredientsRoot.position, Quaternion.Euler(-70f, 60f, 0f));
		StartIngredientsDown();
	}

	protected override void UpdateConverting()
	{
		MoveIngredientsDown();
		m_Handle.transform.localRotation = Quaternion.Euler(1400f * TimeManager.Instance.m_NormalDelta, 0f, 0f) * m_Handle.transform.localRotation;
		m_Gear.transform.localRotation = Quaternion.Euler(-1000f * TimeManager.Instance.m_NormalDelta, 0f, 0f) * m_Gear.transform.localRotation;
		m_Top.transform.localRotation = Quaternion.Euler(0f, 800f * TimeManager.Instance.m_NormalDelta, 0f) * m_Top.transform.localRotation;
		m_ChaffParticles.transform.rotation = Quaternion.Euler(0f, 1400f * TimeManager.Instance.m_NormalDelta, 0f) * m_ChaffParticles.transform.rotation;
		UpdateFunnel(m_Funnel);
	}

	protected override void EndConverting()
	{
		AudioManager.Instance.StopEvent(m_PlaySound);
		ParticlesManager.Instance.DestroyParticles(m_ChaffParticles, WaitUntilNoParticles: true, StopSpawning: true);
		EndFunnel(m_Funnel);
	}
}
