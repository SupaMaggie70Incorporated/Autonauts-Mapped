using System.Collections.Generic;
using UnityEngine;

public class CeremonySpacePortComplete : CeremonyGenericSpeechWithTitle
{
	public enum State
	{
		Intro,
		Land,
		Talk1,
		Talk2,
		Talk3,
		Talk4
	}

	private State m_State;

	private SpacePort m_SpacePort;

	private Vector3 m_OldPosition;

	private void Start()
	{
		FindSpacePort();
		if (m_SpacePort == null)
		{
			End();
			return;
		}
		m_OldPosition = CameraManager.Instance.m_CameraPosition;
		CameraManager.Instance.SetState(CameraManager.State.Free);
		SetState(State.Intro);
	}

	private void FindSpacePort()
	{
		Dictionary<BaseClass, int> collection = CollectionManager.Instance.GetCollection("SpacePort");
		if (collection == null || collection.Count <= 0)
		{
			return;
		}
		using Dictionary<BaseClass, int>.Enumerator enumerator = collection.GetEnumerator();
		if (enumerator.MoveNext())
		{
			m_SpacePort = enumerator.Current.Key.GetComponent<SpacePort>();
		}
	}

	private void LookAtShip()
	{
		CameraManager.Instance.SetState(CameraManager.State.Free);
		Vector3 position = m_SpacePort.m_Rocket.transform.position;
		position.y = 5f;
		Vector3 newPosition = m_SpacePort.m_Rocket.transform.TransformPoint(new Vector3(0f, 0f, -3.5f));
		newPosition.y = 5f;
		CameraManager.Instance.PanTo(newPosition, 0.5f, position);
	}

	private void ResetCamera()
	{
		if (CameraManager.Instance.m_State != 0)
		{
			CameraManager.Instance.SetState(CameraManager.State.Normal);
			CameraManager.Instance.Focus(m_OldPosition);
		}
	}

	private void SetState(State NewState)
	{
		m_State = NewState;
		switch (m_State)
		{
		case State.Intro:
		{
			Vector3 position = m_SpacePort.m_Rocket.transform.position;
			position.y = 1f;
			Vector3 newPosition = m_SpacePort.m_Rocket.transform.TransformPoint(new Vector3(0f, 0f, -15f));
			newPosition.y = 15f;
			CameraManager.Instance.PanTo(newPosition, 1f, position);
			SetSpeech("CeremonySpacePortCompleteSpeech1");
			AudioManager.Instance.StartEvent("CeremonyFirstResearch");
			break;
		}
		case State.Land:
			ShowSpeech(Show: false);
			m_SpacePort.m_Rocket.SetState(SpacePortRocket.State.Intro);
			break;
		case State.Talk1:
			ShowSpeech(Show: true);
			LookAtShip();
			SetSpeechImages("Ceremonies/GaryIdle", "Ceremonies/GaryTalk");
			SetSpeech("CeremonySpacePortCompleteSpeech2");
			break;
		case State.Talk2:
			SetSpeechImages("Ceremonies/AaronIdle", "Ceremonies/AaronTalk");
			SetSpeech("CeremonySpacePortCompleteSpeech3");
			break;
		case State.Talk3:
			SetSpeechImages("Ceremonies/GaryIdle", "Ceremonies/GaryTalk");
			SetSpeech("CeremonySpacePortCompleteSpeech4");
			break;
		case State.Talk4:
			SetSpeechImages("Ceremonies/AaronIdle", "Ceremonies/AaronTalk");
			SetSpeech("CeremonySpacePortCompleteSpeech5");
			break;
		}
	}

	public override void OnAcceptClicked(BaseGadget NewGadget)
	{
		if (m_State == State.Intro)
		{
			SetState(State.Land);
		}
		else if (m_State == State.Talk1)
		{
			SetState(State.Talk2);
		}
		else if (m_State == State.Talk2)
		{
			SetState(State.Talk3);
		}
		else if (m_State == State.Talk3)
		{
			SetState(State.Talk4);
		}
		else if (m_State == State.Talk4)
		{
			End();
		}
	}

	protected override void End()
	{
		base.End();
		ResetCamera();
	}

	private void Update()
	{
		if (m_State == State.Land && m_SpacePort.m_Rocket.m_State == SpacePortRocket.State.Idle)
		{
			SetState(State.Talk1);
		}
	}
}
