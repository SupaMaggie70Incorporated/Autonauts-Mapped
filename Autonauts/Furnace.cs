using UnityEngine;

public class Furnace : Fueler
{
	private PlaySound m_PlaySound;

	private MyLight m_FireLight;

	private Transform m_SmokePoint;

	public override void Restart()
	{
		base.Restart();
		SetDimensions(new TileCoord(0, -1), new TileCoord(1, 0), new TileCoord(0, 1));
		SetSpawnPoint(new TileCoord(1, 1));
		m_PlaySound = null;
		m_Tier = BurnableInfo.Tier.Super;
		m_Capacity = 1000f;
	}

	protected new void Awake()
	{
		base.Awake();
		m_FireLight = LightManager.Instance.LoadLight("GenericFire", base.transform, new Vector3(Tile.m_Size / 2f, 1f, 0f));
	}

	public override void PostCreate()
	{
		base.PostCreate();
		m_SmokePoint = m_ModelRoot.transform.Find("SmokePoint");
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
		m_BaseConversionDelay = VariableManager.Instance.GetVariableAsFloat("Furnace.ConversionDelay");
		UpdateConversionDelay();
		if (m_PlaySound != null)
		{
			AudioManager.Instance.StopEvent(m_PlaySound);
		}
		m_PlaySound = AudioManager.Instance.StartEvent("BuildingFurnaceMaking", this, Remember: true);
		MyParticles newParticles = ParticlesManager.Instance.CreateParticles("ClayFurnaceSmoke", m_SmokePoint.position, Quaternion.identity);
		ParticlesManager.Instance.DestroyParticles(newParticles, WaitUntilNoParticles: true);
	}

	protected override void UpdateConverting()
	{
		if ((int)(m_StateTimer * 60f) % 15 < 7)
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
		m_PlaySound = null;
		m_ModelRoot.transform.localScale = new Vector3(1f, 1f, 1f);
		CheckBurnedIngredients();
	}

	protected new void Update()
	{
		base.Update();
		if (GetIsSavable() && m_State != State.Converting && m_PlaySound == null)
		{
			m_PlaySound = AudioManager.Instance.StartEvent("BuildingFurnaceIdle", this, Remember: true);
		}
	}
}
