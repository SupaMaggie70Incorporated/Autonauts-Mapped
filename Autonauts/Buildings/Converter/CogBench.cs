using UnityEngine;

public class CogBench : Converter
{
	private PlaySound m_PlaySound;

	private MyParticles m_Particles;

	public override void Restart()
	{
		base.Restart();
		m_DisplayIngredients = true;
		SetDimensions(new TileCoord(0, 0), new TileCoord(0, 0), new TileCoord(0, 1));
		SetSpawnPoint(new TileCoord(1, 0));
	}

	public override void StartConverting()
	{
		base.StartConverting();
		m_PlaySound = AudioManager.Instance.StartEvent("BuildingGearMachineMaking", this, Remember: true);
		m_Particles = ParticlesManager.Instance.CreateParticles("Chips", base.transform.localPosition + new Vector3(-1.26f, -1.08f, 0.03f), base.transform.rotation * Quaternion.Euler(-60f, 90f, 0f));
		StartIngredientsDown();
	}

	protected override void UpdateConverting()
	{
		ConvertVibrate();
		MoveIngredientsDown();
		SpinIngredients(1400f);
	}

	protected override void EndConverting()
	{
		EndVibrate();
		EndIngredientsDown();
		ParticlesManager.Instance.DestroyParticles(m_Particles, WaitUntilNoParticles: true, StopSpawning: true);
		AudioManager.Instance.StopEvent(m_PlaySound);
		AudioManager.Instance.StartEvent("BuildingMakingComplete", this);
	}
}
