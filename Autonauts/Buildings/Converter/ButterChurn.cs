using UnityEngine;

public class ButterChurn : Converter
{
	private GameObject m_Root;

	private PlaySound m_PlaySound;

	private MyParticles m_MilkParticles;

	public override void Restart()
	{
		base.Restart();
		SetDimensions(new TileCoord(0, 0), new TileCoord(0, 0), new TileCoord(0, 1));
		SetSpawnPoint(new TileCoord(1, 0));
		SetResultToCreate(1);
		m_Root = m_ModelRoot.transform.Find("Rotation").gameObject;
	}

	public override void StartConverting()
	{
		base.StartConverting();
		m_PlaySound = AudioManager.Instance.StartEvent("BuildingButterChurnMaking", this, Remember: true);
		m_MilkParticles = ParticlesManager.Instance.CreateParticles("Milk", base.transform.position + new Vector3(0f, 2f, 0f), Quaternion.Euler(-70f, 60f, 0f));
	}

	protected override void UpdateConverting()
	{
		m_Root.transform.localRotation = Quaternion.Euler(1400f * TimeManager.Instance.m_NormalDelta, 0f, 0f) * m_Root.transform.localRotation;
		float x = Random.Range(-1f, 1f);
		Vector3 position = base.transform.position + new Vector3(x, 2f, 0f);
		m_MilkParticles.transform.position = position;
		float x2 = Random.Range(-60, -130);
		m_MilkParticles.transform.rotation = Quaternion.Euler(x2, 0f, 0f);
	}

	protected override void EndConverting()
	{
		AudioManager.Instance.StopEvent(m_PlaySound);
		ParticlesManager.Instance.DestroyParticles(m_MilkParticles, WaitUntilNoParticles: true, StopSpawning: true);
	}
}
