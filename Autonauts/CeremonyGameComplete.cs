using System;
using System.Collections.Generic;
using UnityEngine;

public class CeremonyGameComplete : CeremonyGenericSpeechWithTitle
{
	private enum State
	{
		FocusOnFolk,
		FocusOnEye,
		FocusOnPyramid,
		PyramidActivate,
		PullBack,
		First,
		Second,
		Third,
		Fourth,
		Total
	}

	private State m_State;

	private float m_StateTimer;

	private TranscendBuilding m_Pyramid;

	private float m_Rotation;

	private Vector3 m_OldPosition;

	private float m_OldMusicVolume;

	private float m_Volume;

	private GameObject m_Blocker;

	private GameCompleteBadge m_Badge;

	protected new void Awake()
	{
		base.Awake();
		m_OldMusicVolume = AudioManager.Instance.GetMusicVolume();
		m_Blocker = base.transform.Find("Blocker").gameObject;
		m_Blocker.SetActive(value: false);
		m_Badge = base.transform.Find("Blocker/GameCompleteBadge").GetComponent<GameCompleteBadge>();
		m_Badge.gameObject.SetActive(value: false);
		m_TitleStrip.SetActive(value: false);
		SetSpeech(null);
		GetPyramid();
		SetState(State.FocusOnFolk);
	}

	protected override void End()
	{
		AudioManager.Instance.StartMusic("MusicGame");
		if ((bool)m_Pyramid)
		{
			m_Pyramid.StopConfetti();
		}
		ToggleBotsCelebrate();
		ResetCamera();
		base.End();
	}

	public void GetPyramid()
	{
		Dictionary<BaseClass, int> collection = CollectionManager.Instance.GetCollection("TranscendBuilding");
		if (collection == null)
		{
			return;
		}
		using Dictionary<BaseClass, int>.Enumerator enumerator = collection.GetEnumerator();
		if (enumerator.MoveNext())
		{
			m_Pyramid = enumerator.Current.Key.GetComponent<TranscendBuilding>();
		}
	}

	public override void OnAcceptClicked(BaseGadget NewGadget)
	{
		if (m_State == State.First)
		{
			SetState(State.Second);
		}
		else if (m_State == State.Second)
		{
			SetState(State.Third);
		}
		else if (m_State == State.Third)
		{
			SetState(State.Fourth);
		}
		else if (m_State == State.Fourth)
		{
			End();
		}
	}

	public void ToggleBotsCelebrate()
	{
		Dictionary<BaseClass, int> collection = CollectionManager.Instance.GetCollection("Worker");
		if (collection == null)
		{
			return;
		}
		foreach (KeyValuePair<BaseClass, int> item in collection)
		{
			item.Key.GetComponent<Worker>().ToggleCelebrate();
		}
	}

	private void FocusOnFolk()
	{
		m_OldPosition = CameraManager.Instance.m_CameraPosition;
		CameraManager.Instance.SetState(CameraManager.State.Free);
		if ((bool)m_Pyramid)
		{
			m_Pyramid.SetState(TranscendBuilding.TranscendState.Transcending);
			Vector3 newPosition = m_Pyramid.transform.TransformPoint(new Vector3(0f, 8f, -10f));
			Vector3 position = m_Pyramid.m_TranscendEffect.m_Heart.transform.position;
			CameraManager.Instance.PanTo(newPosition, 3f, position);
			m_Pyramid.StartCompleteAnimation();
		}
	}

	private void FocusOnEye()
	{
		if ((bool)m_Pyramid)
		{
			Vector3 newPosition = m_Pyramid.transform.TransformPoint(new Vector3(0f, 10f, -20f));
			Vector3 lookAtPosition = m_Pyramid.transform.TransformPoint(new Vector3(0f, 2f, 0f));
			CameraManager.Instance.PanTo(newPosition, 0.5f, lookAtPosition, TweenLookAt: true);
			m_Pyramid.TriggerConversionAnimation();
		}
		m_Volume = 1f;
	}

	private void FocusOnPyramid()
	{
		if ((bool)m_Pyramid)
		{
			Vector3 newPosition = m_Pyramid.transform.TransformPoint(new Vector3(0f, 10f, -20f));
			Vector3 lookAtPosition = m_Pyramid.transform.TransformPoint(new Vector3(0f, 2f, 0f));
			CameraManager.Instance.PanTo(newPosition, 2f, lookAtPosition, TweenLookAt: true);
		}
		AudioManager.Instance.StartMusic("MusicEnding");
		AudioManager.Instance.SetMusicVolume(m_OldMusicVolume);
	}

	private void PyramidActivate()
	{
		if ((bool)m_Pyramid)
		{
			m_Pyramid.CreateConfetti();
		}
		ToggleBotsCelebrate();
	}

	private void PullBack()
	{
		if ((bool)m_Pyramid)
		{
			Vector3 newPosition = m_Pyramid.transform.TransformPoint(new Vector3(0f, 60f, -80f));
			Vector3 lookAtPosition = m_Pyramid.transform.TransformPoint(new Vector3(0f, 2f, 0f));
			CameraManager.Instance.PanTo(newPosition, 3f, lookAtPosition, TweenLookAt: true);
		}
	}

	private void ResetCamera()
	{
		if (CameraManager.Instance.m_State != 0)
		{
			CameraManager.Instance.SetState(CameraManager.State.Normal);
			CameraManager.Instance.Focus(m_OldPosition);
		}
	}

	private void CalcStartRotation()
	{
		m_Rotation = -(float)Math.PI / 2f;
	}

	private void SetState(State NewState)
	{
		m_State = NewState;
		m_StateTimer = 0f;
		switch (m_State)
		{
		case State.FocusOnFolk:
			FocusOnFolk();
			break;
		case State.FocusOnEye:
			FocusOnEye();
			break;
		case State.FocusOnPyramid:
			FocusOnPyramid();
			break;
		case State.PyramidActivate:
			PyramidActivate();
			break;
		case State.PullBack:
			PullBack();
			break;
		case State.First:
			CalcStartRotation();
			m_TitleStrip.SetActive(value: true);
			SetTitle("CeremonyGameCompleteTitle");
			SetSpeech("CeremonyGameCompleteSpeech1");
			AudioManager.Instance.StartEvent("CeremonyFirstBot");
			break;
		case State.Second:
			m_Speech.m_Tutor.SetImages("Ceremonies/GaryIdle2", "Ceremonies/GaryTalk2");
			SetSpeech("CeremonyGameCompleteSpeech2");
			break;
		case State.Third:
			m_Speech.m_Tutor.SetImages("Ceremonies/AaronIdle2", "Ceremonies/AaronTalk2");
			SetSpeech("CeremonyGameCompleteSpeech3");
			break;
		case State.Fourth:
			m_Speech.m_Tutor.SetImages("Tutorial/TutorIdle", "Tutorial/TutorTalk");
			SetSpeech("CeremonyGameCompleteSpeech4");
			m_Blocker.SetActive(value: true);
			m_Badge.gameObject.SetActive(value: true);
			AudioManager.Instance.StartEvent("CeremonyBadgeEarned");
			break;
		}
	}

	private void UpdateRotation()
	{
		if ((bool)m_Pyramid)
		{
			m_Rotation += TimeManager.Instance.m_NormalDelta * 0.25f;
			Vector3 position = default(Vector3);
			position.x = Mathf.Cos(m_Rotation) * 80f;
			position.y = 60f;
			position.z = Mathf.Sin(m_Rotation) * 80f;
			position = m_Pyramid.transform.TransformPoint(position);
			Vector3 lookAtPosition = m_Pyramid.transform.TransformPoint(new Vector3(0f, 2f, 0f));
			CameraManager.Instance.PanTo(position, 0.01f, lookAtPosition);
		}
	}

	private void Update()
	{
		switch (m_State)
		{
		case State.FocusOnFolk:
			if ((bool)m_Pyramid)
			{
				Vector3 position = m_Pyramid.m_TranscendEffect.m_Heart.transform.position;
				CameraManager.Instance.m_LookAtPosition = position;
			}
			if (m_StateTimer > 3f)
			{
				SetState(State.FocusOnEye);
			}
			break;
		case State.FocusOnEye:
			m_Volume -= TimeManager.Instance.m_NormalDelta;
			if (m_Volume < 0f)
			{
				m_Volume = 0f;
			}
			AudioManager.Instance.SetMusicVolume(m_Volume * m_OldMusicVolume);
			if (m_StateTimer > 3f)
			{
				SetState(State.FocusOnPyramid);
			}
			break;
		case State.FocusOnPyramid:
			if (m_StateTimer > 2f)
			{
				SetState(State.PyramidActivate);
			}
			break;
		case State.PyramidActivate:
			if (m_StateTimer > 3f)
			{
				SetState(State.PullBack);
			}
			break;
		case State.PullBack:
			if (m_StateTimer > 3f)
			{
				SetState(State.First);
			}
			break;
		case State.First:
		case State.Second:
		case State.Third:
		case State.Fourth:
			UpdateRotation();
			break;
		}
		m_StateTimer += TimeManager.Instance.m_NormalDelta;
	}
}
