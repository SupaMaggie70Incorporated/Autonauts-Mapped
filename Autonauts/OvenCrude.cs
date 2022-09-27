using UnityEngine;

public class OvenCrude : Fueler
{
	private PlaySound m_PlaySound;

	private MyParticles m_Particles;

	private MyLight m_FireLight;

	public override void Restart()
	{
		base.Restart();
		SetDimensions(new TileCoord(0, -1), new TileCoord(1, 0), new TileCoord(0, 1));
		SetSpawnPoint(new TileCoord(1, 1));
		m_PlaySound = null;
		if (m_Particles == null && (bool)ParticlesManager.Instance)
		{
			m_Particles = ParticlesManager.Instance.CreateParticles("OvenCrudeSmoke", default(Vector3), Quaternion.identity);
		}
		m_Particles.Clear();
		m_Particles.Play();
		m_Particles.transform.rotation = Quaternion.Euler(-90f, 0f, 0f);
		UpdatePosition();
		m_DisplayIngredients = true;
		m_Tier = BurnableInfo.Tier.Crude;
		m_Capacity = 150f;
	}

	public override void PostCreate()
	{
		base.PostCreate();
		if (m_ModelRoot.transform.Find("FirePoint") == null)
		{
			_ = base.transform;
		}
		m_FireLight = LightManager.Instance.LoadLight("GenericFire", base.transform, new Vector3(Tile.m_Size / 2f, 2f, 0f));
	}

	public override void StopUsing(bool AndDestroy = true)
	{
		m_Particles.Stop();
		m_Particles = null;
		base.StopUsing(AndDestroy);
	}

	protected new void OnDestroy()
	{
		LightManager.Instance.DestroyLight(m_FireLight);
		if ((bool)m_Particles)
		{
			ParticlesManager.Instance.DestroyParticles(m_Particles);
			m_Particles = null;
		}
		base.OnDestroy();
	}

	private void UpdatePosition()
	{
		Transform transform = m_ModelRoot.transform.Find("SmokePoint");
		if (transform == null)
		{
			transform = base.transform;
		}
		m_Particles.transform.position = transform.position;
	}

	public override void SendAction(ActionInfo Info)
	{
		ActionType action = Info.m_Action;
		if ((uint)(action - 41) <= 1u)
		{
			UpdatePosition();
		}
		base.SendAction(Info);
	}

	public override void SetBlueprint(bool Blueprint, bool Register = false)
	{
		base.SetBlueprint(Blueprint, Register);
		m_Particles.gameObject.SetActive(!Blueprint);
		m_FireLight.SetActive(!Blueprint);
	}

	public override void StartConverting()
	{
		base.StartConverting();
		m_BaseConversionDelay = VariableManager.Instance.GetVariableAsFloat(ObjectType.OvenCrude, "ConversionDelay");
		UpdateConversionDelay();
		AudioManager.Instance.StopEvent(m_PlaySound);
		m_PlaySound = AudioManager.Instance.StartEvent("BuildingFurnaceMaking", this, Remember: true);
		MyParticles newParticles = ParticlesManager.Instance.CreateParticles("ClayFurnaceSmoke", base.transform.localPosition, Quaternion.identity);
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

	protected override BaseClass CreateNewItem()
	{
		BaseClass result = base.CreateNewItem();
		QuestManager.Instance.AddEvent(QuestEvent.Type.UseOvenCrude, m_LastEngagerType == ObjectType.Worker, 0, this);
		return result;
	}

	protected new void Update()
	{
		base.Update();
		if (GetIsSavable() && m_State != State.Converting && m_PlaySound == null)
		{
			m_PlaySound = AudioManager.Instance.StartEvent("BuildingOvenCrudeIdle", this, Remember: true);
		}
	}
}
