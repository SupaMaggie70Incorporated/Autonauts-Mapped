using UnityEngine;

public class CookingPotCrude : Fueler
{
	private GameObject m_Spoon;

	private MyParticles m_SplashParticles;

	private MyLight m_FireLight;

	private PlaySound m_PlaySound;

	public override void Restart()
	{
		base.Restart();
		SetDimensions(new TileCoord(0, 0), new TileCoord(0, 0), new TileCoord(0, 1));
		SetSpawnPoint(new TileCoord(1, 0));
		m_Spoon = m_ModelRoot.transform.Find("Spoon").gameObject;
		m_Tier = BurnableInfo.Tier.Crude;
		m_Capacity = 150f;
		CreateBubbles(2f);
		CreateSmoke();
	}

	public override void PostCreate()
	{
		base.PostCreate();
		m_FireLight = LightManager.Instance.LoadLight("GenericFire", base.transform, new Vector3(0f, 1f, 0f));
	}

	protected new void OnDestroy()
	{
		LightManager.Instance.DestroyLight(m_FireLight);
		base.OnDestroy();
	}

	public override void SetBlueprint(bool Blueprint, bool Register = false)
	{
		base.SetBlueprint(Blueprint, Register);
		m_FireLight.SetActive(!Blueprint);
	}

	public override void StartConverting()
	{
		base.StartConverting();
		m_PlaySound = AudioManager.Instance.StartEvent("BuildingPotCrudeMaking", this, Remember: true);
		m_SplashParticles = ParticlesManager.Instance.CreateParticles("SpoonSplash", base.transform.position, Quaternion.Euler(-70f, 60f, 0f));
		m_BubblesParticles.SetSpeed(3f);
	}

	protected override void UpdateConverting()
	{
		m_Spoon.transform.rotation = Quaternion.Euler(0f, 1400f * TimeManager.Instance.m_NormalDelta, 0f) * m_Spoon.transform.rotation;
		m_SplashParticles.transform.rotation = Quaternion.Euler(0f, 1400f * TimeManager.Instance.m_NormalDelta, 0f) * m_SplashParticles.transform.rotation;
	}

	protected override void EndConverting()
	{
		AudioManager.Instance.StopEvent(m_PlaySound);
		ParticlesManager.Instance.DestroyParticles(m_SplashParticles, WaitUntilNoParticles: true, StopSpawning: true);
		CheckBurnedIngredients();
		m_BubblesParticles.SetSpeed(1f);
	}
}
