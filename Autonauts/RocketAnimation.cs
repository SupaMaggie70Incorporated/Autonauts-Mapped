using System.Collections.Generic;
using UnityEngine;

public class RocketAnimation : BaseClass
{
	public static RocketAnimation Instance;

	private GameObject m_Flame;

	private bool m_FlameActive;

	private float m_FlameTimer;

	private float m_FlameVolume;

	private Rocket m_Rocket;

	private PlaySound m_FlameSound;

	private Transmitter m_Transmitter;

	private Animator m_Animator;

	public override void PostCreate()
	{
		Instance = this;
		base.PostCreate();
		m_Animator = GetComponent<Animator>();
		Transform transform = base.transform.Find("ModelController");
		m_Rocket = ObjectTypeList.Instance.CreateObjectFromIdentifier(ObjectType.Rocket, default(Vector3), Quaternion.identity).GetComponent<Rocket>();
		m_Rocket.transform.SetParent(transform);
		m_Rocket.transform.localPosition = default(Vector3);
		m_Flame = m_Rocket.m_ModelRoot.transform.Find("Flame").gameObject;
		m_FlameVolume = 1f;
		m_FlameSound = AudioManager.Instance.StartEventAmbient("Rocket", transform.gameObject, Remember: true, Follow: true);
		if ((bool)CollectionManager.Instance)
		{
			Dictionary<BaseClass, int> collection = CollectionManager.Instance.GetCollection("Transmitter");
			if (collection != null)
			{
				using Dictionary<BaseClass, int>.Enumerator enumerator = collection.GetEnumerator();
				if (enumerator.MoveNext())
				{
					m_Transmitter = enumerator.Current.Key.GetComponent<Transmitter>();
					m_Transmitter.transform.SetParent(transform);
					m_Transmitter.transform.localPosition = default(Vector3);
					m_Transmitter.m_AccessModel.SetActive(value: false);
				}
			}
		}
		m_Animator.Rebind();
	}

	private void OnDestroy()
	{
		if ((bool)AudioManager.Instance)
		{
			AudioManager.Instance.StopEvent(m_FlameSound);
		}
		if ((bool)m_Rocket)
		{
			m_Rocket.StopUsing();
		}
		Instance = null;
	}

	public void StartDescend()
	{
		m_FlameActive = true;
		m_FlameTimer = 0f;
		CameraManager.Instance.SetState(CameraManager.State.Ceremony);
		((CameraStateCeremony)CameraManager.Instance.m_CurrentState).SetTrackObject(base.transform.Find("CameraController").gameObject);
	}

	public void StopFlame()
	{
		if ((bool)m_Transmitter)
		{
			m_Transmitter.transform.SetParent(MapManager.Instance.m_BuildingsRootTransform);
			m_Transmitter.transform.position = m_Transmitter.m_TileCoord.ToWorldPositionTileCentered();
		}
		m_FlameActive = false;
		if ((bool)m_Flame)
		{
			m_Flame.SetActive(value: false);
		}
	}

	public void StartFlame()
	{
		m_FlameActive = true;
		m_Flame.SetActive(value: true);
	}

	public void CameraClose()
	{
		CameraManager.Instance.SetState(CameraManager.State.Free);
		Vector3 position = m_Rocket.transform.position;
		CameraManager.Instance.PanTo(position + new Vector3(0f, 7f, -15f), 0.25f, position);
	}

	public void LidOpen()
	{
		AudioManager.Instance.StartEvent("ShipOpen");
	}

	public void LidClose()
	{
		AudioManager.Instance.StartEvent("ShipClose");
	}

	public void KickPlayer()
	{
		Vector3 position = m_Rocket.m_ModelRoot.transform.Find("Player").position;
		List<BaseClass> players = CollectionManager.Instance.GetPlayers();
		players[0].GetComponent<Farmer>().RocketKicked(position);
		m_Rocket.m_ModelRoot.transform.Find("Player").gameObject.SetActive(value: false);
		AudioManager.Instance.StartEvent("FarmerThrow", players[0].GetComponent<TileCoordObject>());
	}

	public void KickTutorBot()
	{
		if (GameOptionsManager.Instance.m_Options.m_TutorialEnabled)
		{
			Vector3 position = m_Rocket.m_ModelRoot.transform.Find("Player").position;
			TutorBot.Instance.RocketKicked(position);
			AudioManager.Instance.StartEvent("FarmerThrow", TutorBot.Instance);
		}
	}

	public void FadeTextIn()
	{
		CeremonyRocketIntro.Instance.FadeTextIn();
	}

	public void ShowName()
	{
		CeremonyRocketIntro.Instance.ShowName();
	}

	public void FadeTextOut()
	{
		CeremonyRocketIntro.Instance.FadeTextOut();
	}

	public void StartCeremony()
	{
		CeremonyRocketIntro.Instance.RocketFinished();
	}

	public void End()
	{
		StopUsing();
	}

	public void Skip()
	{
		StopFlame();
		if (CameraManager.Instance.m_State != 0)
		{
			CameraManager.Instance.SetState(CameraManager.State.Normal);
			CameraManager.Instance.Focus(m_Transmitter.transform.position);
		}
		List<BaseClass> players = CollectionManager.Instance.GetPlayers();
		if (players != null && players.Count > 0)
		{
			Farmer component = players[0].GetComponent<Farmer>();
			component.gameObject.SetActive(value: true);
			if (component.m_State != 0)
			{
				component.SetState(Farmer.State.None);
			}
		}
		if ((bool)TutorBot.Instance)
		{
			TutorBot.Instance.RocketEnd();
		}
		StopUsing();
	}

	private void Update()
	{
		if (TimeManager.Instance == null)
		{
			return;
		}
		if (TimeManager.Instance.m_NormalTimeEnabled)
		{
			m_Animator.speed = TimeManager.Instance.m_TimeScale;
		}
		else
		{
			m_Animator.speed = 0f;
		}
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
		else
		{
			m_FlameVolume -= TimeManager.Instance.m_NormalDelta;
		}
		m_FlameSound.m_Result.ActingVariation.AdjustVolume(m_FlameVolume);
		float num = base.transform.position.y / 150f;
		m_FlameSound.m_Result.ActingVariation.VarAudio.pitch = 0.75f + num;
	}
}
