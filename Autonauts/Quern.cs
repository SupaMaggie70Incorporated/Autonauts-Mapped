using UnityEngine;

public class Quern : Converter
{
	private GameObject m_Top;

	private PlaySound m_PlaySound;

	private MyParticles m_ChaffParticles;

	public override void Restart()
	{
		base.Restart();
		SetDimensions(new TileCoord(0, -1), new TileCoord(1, 0), new TileCoord(0, 1));
		SetSpawnPoint(new TileCoord(2, 0));
		SetResultToCreate(1);
		m_Top = m_ModelRoot.transform.Find("QuernTop").gameObject;
	}

	public override void StartConverting()
	{
		base.StartConverting();
		m_PlaySound = AudioManager.Instance.StartEvent("BuildingQuernMaking", this, Remember: true);
		m_ChaffParticles = ParticlesManager.Instance.CreateParticles("Chaff", base.transform.position, Quaternion.Euler(-70f, 60f, 0f));
	}

	protected override void UpdateConverting()
	{
		m_Top.transform.rotation = Quaternion.Euler(0f, 1400f * TimeManager.Instance.m_NormalDelta, 0f) * m_Top.transform.rotation;
		m_ChaffParticles.transform.rotation = Quaternion.Euler(0f, 1400f * TimeManager.Instance.m_NormalDelta, 0f) * m_ChaffParticles.transform.rotation;
	}

	protected override void EndConverting()
	{
		ParticlesManager.Instance.DestroyParticles(m_ChaffParticles, WaitUntilNoParticles: true, StopSpawning: true);
		AudioManager.Instance.StopEvent(m_PlaySound);
	}
}
