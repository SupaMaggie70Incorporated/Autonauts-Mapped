using UnityEngine;

public class ClayFurnace : Fueler
{
	private PlaySound m_PlaySound;

	private MyLight m_FireLight;

	private Transform m_SmokePoint;

	private MyParticles m_Particles;

	private MyParticles m_Sparks;

	public override void Restart()
	{
		base.Restart();
		SetDimensions(new TileCoord(0, -1), new TileCoord(1, 0), new TileCoord(0, 1));
		SetSpawnPoint(new TileCoord(1, 1));
		m_PlaySound = null;
		if (m_Particles == null && (bool)ParticlesManager.Instance)
		{
			m_Particles = ParticlesManager.Instance.CreateParticles("ClayFurnaceIdle", m_SmokePoint.position, Quaternion.Euler(-90f, 0f, 0f));
		}
		m_Particles.Clear();
		m_Particles.Stop();
		UpdateLights();
		m_Tier = BurnableInfo.Tier.Normal;
		m_Capacity = 240f;
	}

	protected new void Awake()
	{
		base.Awake();
		m_FireLight = LightManager.Instance.LoadLight("GenericFire", base.transform, new Vector3(Tile.m_Size / 2f, 1f, 0f));
		m_FireLight.SetActive(Active: false);
	}

	public override void PostCreate()
	{
		base.PostCreate();
		m_SmokePoint = m_ModelRoot.transform.Find("SmokePoint");
	}

	public override void StopUsing(bool AndDestroy = true)
	{
		if ((bool)m_Particles)
		{
			ParticlesManager.Instance.DestroyParticles(m_Particles);
			m_Particles = null;
		}
		base.StopUsing(AndDestroy);
	}

	protected new void OnDestroy()
	{
		LightManager.Instance.DestroyLight(m_FireLight);
		if ((bool)m_Particles)
		{
			ParticlesManager.Instance.DestroyParticles(m_Particles);
		}
		base.OnDestroy();
	}

	protected override void FuelChanged()
	{
		base.FuelChanged();
		if (m_Fuel > 0f)
		{
			if (!m_Particles.m_Particles.isPlaying)
			{
				m_Particles.Play();
			}
			m_FireLight.SetActive(SettingsManager.Instance.m_LightsEnabled);
		}
		else if (m_State != State.Converting)
		{
			if (m_Particles.m_Particles.isPlaying)
			{
				m_Particles.Stop();
			}
			m_FireLight.SetActive(Active: false);
		}
	}

	public override void SetBlueprint(bool Blueprint, bool Register = false)
	{
		base.SetBlueprint(Blueprint, Register);
	}

	public override void StartConverting()
	{
		base.StartConverting();
		m_BaseConversionDelay = VariableManager.Instance.GetVariableAsFloat("ClayFurnace.ConversionDelay");
		UpdateConversionDelay();
		if (m_PlaySound != null)
		{
			AudioManager.Instance.StopEvent(m_PlaySound);
		}
		m_PlaySound = AudioManager.Instance.StartEvent("BuildingFurnaceMaking", this, Remember: true);
		MyParticles newParticles = ParticlesManager.Instance.CreateParticles("ClayFurnaceSmoke", m_SmokePoint.position, Quaternion.Euler(-90f, 0f, 0f));
		ParticlesManager.Instance.DestroyParticles(newParticles, WaitUntilNoParticles: true);
		m_Sparks = ParticlesManager.Instance.CreateParticles("FurnaceSparks", m_SmokePoint.position + new Vector3(0f, -0.5f, 0f), Quaternion.Euler(-90f, 0f, 0f));
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
		ParticlesManager.Instance.DestroyParticles(m_Sparks);
		AudioManager.Instance.StopEvent(m_PlaySound);
		m_PlaySound = null;
		m_ModelRoot.transform.localScale = new Vector3(1f, 1f, 1f);
		CheckBurnedIngredients();
		FuelChanged();
	}

	private void UpdateLights()
	{
		_ = m_Blueprint;
	}

	public override void SendAction(ActionInfo Info)
	{
		switch (Info.m_Action)
		{
		case ActionType.Refresh:
		case ActionType.RefreshFirst:
			m_Particles.transform.position = m_SmokePoint.position;
			break;
		case ActionType.UpdateLights:
			UpdateLights();
			break;
		}
		base.SendAction(Info);
	}

	private void UpdateFuel()
	{
		m_Particles.Play();
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
