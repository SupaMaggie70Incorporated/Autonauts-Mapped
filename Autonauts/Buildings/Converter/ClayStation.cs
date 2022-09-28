using UnityEngine;

public class ClayStation : LinkedSystemConverter
{
	private GameObject m_Spindle;

	private PlaySound m_PlaySound;

	private MyParticles m_Particles;

	public override void Restart()
	{
		base.Restart();
		m_DisplayIngredients = true;
		SetDimensions(new TileCoord(0, 0), new TileCoord(0, 0), new TileCoord(0, 1));
		SetSpawnPoint(new TileCoord(1, 0));
		m_Spindle = m_ModelRoot.transform.Find("Spindle").gameObject;
		m_Spindle.transform.localRotation = ObjectUtils.m_ModelRotator;
		m_PulleySide = 1;
	}

	public override void StartConverting()
	{
		base.StartConverting();
		m_PlaySound = AudioManager.Instance.StartEvent("BuildingPottersWheelMaking", this, Remember: true);
		m_Particles = ParticlesManager.Instance.CreateParticles("Clay", m_IngredientsRoot.transform.position, Quaternion.Euler(-70f, 60f, 0f));
		StartIngredientsDown();
	}

	protected override void UpdateConverting()
	{
		MoveIngredientsDown();
		float y = 800f * TimeManager.Instance.m_NormalDelta;
		m_Spindle.transform.rotation = Quaternion.Euler(0f, y, 0f) * m_Spindle.transform.rotation;
		SpinIngredients(800f);
		m_Particles.transform.rotation = Quaternion.Euler(0f, 1400f * TimeManager.Instance.m_NormalDelta, 0f) * m_Particles.transform.rotation;
	}

	protected override void EndConverting()
	{
		EndIngredientsDown();
		AudioManager.Instance.StopEvent(m_PlaySound);
		ParticlesManager.Instance.DestroyParticles(m_Particles, WaitUntilNoParticles: true, StopSpawning: true);
		BadgeManager.Instance.AddEvent(BadgeEvent.Type.Pottery);
	}
}
