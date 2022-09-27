using UnityEngine;

public class SpacePortRocket : BaseClass
{
	public enum State
	{
		Intro,
		Leave,
		InSpace,
		Return,
		Idle,
		Total
	}

	public State m_State;

	private GameObject m_RocketTrailGround;

	private GameObject m_RocketTrailCollider;

	private GameObject m_RocketTrailParticles;

	private MyLight m_Light;

	private Animator m_Animator;

	private GameObject m_Flame;

	private bool m_FlameActive;

	private float m_FlameTimer;

	private float m_FlameVolume;

	private PlaySound m_FlameSound;

	public override void PostCreate()
	{
		base.PostCreate();
		base.name = "Rocket";
		m_Light = LightManager.Instance.LoadLight("RocketLight", m_ModelRoot.transform, default(Vector3));
		m_Light.transform.localPosition = new Vector3(0f, 0f, 0f);
		GameObject original = (GameObject)Resources.Load("Prefabs/Particles/Rocket/RocketTrailGround", typeof(GameObject));
		m_RocketTrailGround = Object.Instantiate(original, base.transform.position, Quaternion.Euler(90f, 0f, 0f), null);
		original = (GameObject)Resources.Load("Prefabs/Particles/Rocket/RocketTrailCollider", typeof(GameObject));
		m_RocketTrailCollider = Object.Instantiate(original, base.transform.position, Quaternion.identity, null);
		original = (GameObject)Resources.Load("Prefabs/Particles/Rocket/RocketTrailParticles", typeof(GameObject));
		m_RocketTrailParticles = Object.Instantiate(original, base.transform.position, Quaternion.Euler(90f, 0f, 0f), m_ModelRoot.transform);
		m_RocketTrailParticles.transform.localPosition = new Vector3(0f, 0f, 0f) + new Vector3(0f, -3f, 0f);
		m_RocketTrailParticles.GetComponent<MyParticles>().m_Particles.trigger.SetCollider(0, m_RocketTrailCollider.GetComponent<BoxCollider>());
		m_RocketTrailParticles.GetComponent<RocketTrailParticles>().SetChildParticles(m_RocketTrailGround.GetComponent<MyParticles>());
		m_ModelRoot.transform.Find("Player").gameObject.SetActive(value: false);
		m_Flame = m_ModelRoot.transform.Find("Flame").gameObject;
		m_Animator = GetComponent<Animator>();
		m_Animator.Rebind();
	}

	private void OnDestroy()
	{
		LightManager.Instance.DestroyLight(m_Light);
		Object.Destroy(m_RocketTrailGround.gameObject);
		Object.Destroy(m_RocketTrailCollider.gameObject);
	}

	public override void StopUsing(bool AndDestroy = true)
	{
		StopSound();
		base.StopUsing(AndDestroy);
	}

	public void SetState(State NewState)
	{
		State state = m_State;
		if (state == State.InSpace)
		{
			m_ModelRoot.gameObject.SetActive(value: true);
		}
		m_State = NewState;
		switch (m_State)
		{
		case State.Intro:
			m_Animator.Rebind();
			m_Animator.Play("Intro", -1, 0f);
			StartFlame();
			break;
		case State.Leave:
			m_Animator.Rebind();
			m_Animator.Play("Leave", -1, 0f);
			StartFlame();
			break;
		case State.InSpace:
			m_Animator.Rebind();
			m_Animator.Play("InSpace", -1, 0f);
			m_ModelRoot.gameObject.SetActive(value: false);
			m_RocketTrailParticles.SetActive(value: false);
			StopSound();
			break;
		case State.Return:
			m_Animator.Rebind();
			m_Animator.Play("Return", -1, 0f);
			StartFlame();
			break;
		}
	}

	public void StartDescend()
	{
		m_FlameActive = true;
		m_FlameTimer = 0f;
	}

	public void StopFlame()
	{
		m_FlameActive = false;
		if ((bool)m_Flame)
		{
			m_Flame.SetActive(value: false);
		}
		m_RocketTrailParticles.SetActive(value: false);
	}

	private void StopSound()
	{
		if (m_FlameSound != null)
		{
			AudioManager.Instance.StopEvent(m_FlameSound);
			m_FlameSound = null;
		}
	}

	public void StartFlame()
	{
		m_FlameActive = true;
		m_Flame.SetActive(value: true);
		m_RocketTrailParticles.SetActive(value: true);
		if (m_FlameSound == null)
		{
			m_FlameSound = AudioManager.Instance.StartEventAmbient("Rocket", m_ModelRoot.gameObject, Remember: true, Follow: true);
		}
	}

	private void UpdateFlame()
	{
		if (m_FlameActive)
		{
			m_FlameTimer += TimeManager.Instance.m_NormalDelta;
			if ((int)(m_FlameTimer * 60f) % 6 < 3)
			{
				m_Flame.transform.localScale = new Vector3(1f, 1f, 1f);
			}
			else
			{
				m_Flame.transform.localScale = new Vector3(1f, 1f, 0.65f);
			}
			m_FlameVolume = 1f;
		}
		else if (m_FlameVolume > 0f)
		{
			m_FlameVolume -= TimeManager.Instance.m_NormalDelta;
			if (m_FlameVolume < 0f)
			{
				m_FlameVolume = 0f;
				StopSound();
			}
		}
		if (m_FlameSound != null)
		{
			m_FlameSound.m_Result.ActingVariation.AdjustVolume(m_FlameVolume);
			float num = m_ModelRoot.transform.position.y / 150f;
			m_FlameSound.m_Result.ActingVariation.VarAudio.pitch = 0.75f + num;
		}
	}

	private bool IsAnimationPlaying()
	{
		if (m_Animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
		{
			return true;
		}
		return false;
	}

	private void Update()
	{
		if (TimeManager.Instance == null)
		{
			return;
		}
		Vector3 position = base.transform.position;
		position.y = 0.25f;
		m_RocketTrailGround.transform.position = position;
		m_RocketTrailCollider.transform.position = position;
		if (TimeManager.Instance.m_NormalTimeEnabled)
		{
			m_Animator.speed = TimeManager.Instance.m_TimeScale;
		}
		else
		{
			m_Animator.speed = 0f;
		}
		switch (m_State)
		{
		case State.Intro:
			if (!IsAnimationPlaying())
			{
				SetState(State.Idle);
			}
			break;
		case State.Leave:
			if (!IsAnimationPlaying())
			{
				SetState(State.Return);
			}
			break;
		case State.Return:
			if (!IsAnimationPlaying())
			{
				SetState(State.Idle);
			}
			break;
		}
		UpdateFlame();
	}
}
