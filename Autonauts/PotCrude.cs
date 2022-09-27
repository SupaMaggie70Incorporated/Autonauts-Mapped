using UnityEngine;

public class PotCrude : Converter
{
	private GameObject m_Spoon;

	private PlaySound m_PlaySound;

	private MyParticles m_Particles;

	public override void Restart()
	{
		base.Restart();
		SetDimensions(new TileCoord(0, 0), new TileCoord(0, 0), new TileCoord(0, 1));
		SetSpawnPoint(new TileCoord(1, 0));
		m_Spoon = m_ModelRoot.transform.Find("Spoon").gameObject;
	}

	public override void StartConverting()
	{
		base.StartConverting();
		m_PlaySound = AudioManager.Instance.StartEvent("BuildingPotCrudeMaking", this, Remember: true);
		m_Particles = ParticlesManager.Instance.CreateParticles("SpoonSplash", base.transform.position, Quaternion.Euler(-70f, 60f, 0f));
	}

	protected override void UpdateConverting()
	{
		m_Spoon.transform.rotation = Quaternion.Euler(0f, 1400f * TimeManager.Instance.m_NormalDelta, 0f) * m_Spoon.transform.rotation;
		m_Particles.transform.rotation = Quaternion.Euler(0f, 1400f * TimeManager.Instance.m_NormalDelta, 0f) * m_Particles.transform.rotation;
	}

	protected override void EndConverting()
	{
		AudioManager.Instance.StopEvent(m_PlaySound);
		ParticlesManager.Instance.DestroyParticles(m_Particles, WaitUntilNoParticles: true, StopSpawning: true);
	}
}
