using System;
using System.Collections.Generic;
using UnityEngine;

public class CeremonyCertificateAcademyEnded : CeremonyBase
{
	private enum State
	{
		Begin,
		Center,
		FirstTime,
		Wait,
		Complete,
		WaitPress,
		QuestMove,
		RevealBlueprints,
		Idle,
		RevealWonderWait,
		RevealWonder,
		RevealWonderWait2,
		IdleWonder,
		Return,
		RevealNewWait,
		RevealNew,
		ObjectsFirstTime,
		TipsFirstTime,
		Total
	}

	private State m_State;

	private float m_StateTimer;

	private Certificate m_Certificate;

	private Transform m_CertificateParent;

	private Vector3 m_CertificateOldPosition;

	private Vector2 m_CertificateStartPosition;

	private Vector2 m_CertificateCurrentPosition;

	private BaseImage m_Fade;

	private CeremonyBlueprint m_DefaultBlueprint;

	private Vector2 m_DefaultPosition;

	private List<CeremonyBlueprint> m_Blueprints;

	private NewThing m_New;

	private int m_CurrentBlueprint;

	private CeremonyBlueprint m_WonderBlueprint;

	private Vector3 m_WonderOldPosition;

	private bool m_WonderOnTop;

	private CeremonySpeech m_SpeechPanel;

	private BaseButtonImage m_AcceptButton;

	private ObjectType m_WonderUnlocked;

	private static float m_BigScale = 1.5f;

	private static float m_FadeAlpha = 0.75f;

	public override void SetQuest(Quest NewQuest, List<ObjectType> UnlockedObjects)
	{
		m_WonderUnlocked = ObjectTypeList.m_Total;
		foreach (ObjectType UnlockedObject in UnlockedObjects)
		{
			if (Wonder.GetIsTypeWonder(UnlockedObject))
			{
				UnlockedObjects.Remove(UnlockedObject);
				m_WonderUnlocked = UnlockedObject;
				break;
			}
		}
		base.SetQuest(NewQuest, UnlockedObjects);
		m_Certificate = Academy.Instance.GetCertificateFromQuest(NewQuest);
		m_Certificate.ForceComplete(Complete: false);
		m_Fade = base.transform.Find("BlockingImage").GetComponent<BaseImage>();
		m_DefaultBlueprint = base.transform.Find("CeremonyBlueprint").GetComponent<CeremonyBlueprint>();
		m_DefaultBlueprint.gameObject.SetActive(value: false);
		m_DefaultPosition = m_DefaultBlueprint.GetComponent<RectTransform>().anchoredPosition;
		m_SpeechPanel = base.transform.Find("SpeechPanel").GetComponent<CeremonySpeech>();
		m_SpeechPanel.SetActive(Active: false);
		m_SpeechPanel.GetButton().SetAction(OnAcceptClicked, null);
		CreateBlueprints();
		CreateWonder();
		if (m_UnlockedObjects.Count > 0)
		{
			CreateNew();
			AttachNewToTopBlueprint();
		}
		else if (m_WonderUnlocked != ObjectTypeList.m_Total)
		{
			CreateNew();
			AttachNewToBlueprint(m_WonderBlueprint);
		}
		SetState(State.Begin);
		if (CeremonyManager.Instance.m_GluePlaying)
		{
			SetUnlockedQuestsVisible(Visible: false);
		}
	}

	private void CreateAcceptButton()
	{
		GameObject original = (GameObject)Resources.Load("Prefabs/Hud/Standard/StandardAcceptButton", typeof(GameObject));
		m_AcceptButton = UnityEngine.Object.Instantiate(original, m_Certificate.transform).GetComponent<BaseButtonImage>();
		m_AcceptButton.SetAction(OnAcceptClicked, m_AcceptButton);
		RectTransform component = m_AcceptButton.GetComponent<RectTransform>();
		component.anchorMin = new Vector2(1f, 0f);
		component.anchorMax = new Vector2(1f, 0f);
		component.pivot = new Vector2(1f, 0f);
		component.anchoredPosition = new Vector2(-10f, 10f);
	}

	private CeremonyBlueprint CreateBlueprint(ObjectType NewType, Vector2 Position)
	{
		CeremonyBlueprint ceremonyBlueprint = UnityEngine.Object.Instantiate(m_DefaultBlueprint, base.transform);
		ceremonyBlueprint.gameObject.SetActive(value: true);
		ceremonyBlueprint.GetComponent<RectTransform>().anchoredPosition = Position;
		ceremonyBlueprint.Init(NewType, OnAcceptClicked);
		ceremonyBlueprint.gameObject.SetActive(value: false);
		ceremonyBlueprint.m_Button.SetActive(Active: false);
		return ceremonyBlueprint;
	}

	protected void CreateBlueprints()
	{
		_ = base.transform;
		float num = 20f;
		float num2 = 20f;
		float num3 = m_DefaultPosition.x;
		float num4 = m_DefaultPosition.y + (float)m_UnlockedObjects.Count * num2 / 2f;
		m_Blueprints = new List<CeremonyBlueprint>();
		foreach (ObjectType unlockedObject in m_UnlockedObjects)
		{
			CeremonyBlueprint item = CreateBlueprint(unlockedObject, new Vector2(num3, num4));
			m_Blueprints.Add(item);
			num3 += num;
			num4 -= num2;
		}
	}

	protected void CreateWonder()
	{
		if (m_WonderUnlocked != ObjectTypeList.m_Total)
		{
			m_WonderBlueprint = CreateBlueprint(m_WonderUnlocked, m_DefaultPosition);
		}
	}

	protected void CreateNew()
	{
		GameObject original = (GameObject)Resources.Load("Prefabs/Hud/NewThing", typeof(GameObject));
		m_New = UnityEngine.Object.Instantiate(original, default(Vector3), Quaternion.identity, base.transform).GetComponent<NewThing>();
		m_New.transform.localScale = new Vector3(3f, 3f, 3f);
		m_New.UpdateWhilePaused();
	}

	private void AttachNewToBlueprint(CeremonyBlueprint NewBlueprint)
	{
		m_New.transform.SetParent(NewBlueprint.transform.Find("BasePanel"));
		RectTransform component = m_New.GetComponent<RectTransform>();
		component.anchorMin = new Vector2(0f, 0f);
		component.anchorMax = new Vector2(0f, 0f);
		component.pivot = new Vector2(0f, 0f);
		component.anchoredPosition = new Vector2(-10f, 10f);
	}

	protected void AttachNewToTopBlueprint()
	{
		CeremonyBlueprint newBlueprint = m_Blueprints[m_Blueprints.Count - 1];
		AttachNewToBlueprint(newBlueprint);
	}

	private void End()
	{
		SetState(State.Total);
		UnityEngine.Object.Destroy(base.gameObject);
		CeremonyManager.Instance.CeremonyEnded(CheckForMoreCeremonies: false);
		GameStateAutopedia.Instance.CeremonyPlaying(Playing: false, null);
		if (CeremonyManager.Instance.m_GluePlaying)
		{
			CeremonyManager.Instance.GlueEnded();
			return;
		}
		GameStateManager.Instance.PopState();
		CeremonyManager.Instance.DoRevealCertificate(m_Quest, m_UnlockedObjects);
	}

	public void OnAcceptClicked(BaseGadget NewGadget)
	{
		if (m_State == State.WaitPress || m_State == State.FirstTime)
		{
			SetState(State.Return);
		}
		else if (m_State == State.Idle)
		{
			if (m_Blueprints.Count != 0)
			{
				m_New.transform.SetParent(base.transform);
				CeremonyBlueprint ceremonyBlueprint = m_Blueprints[m_Blueprints.Count - 1];
				m_Blueprints.Remove(ceremonyBlueprint);
				UnityEngine.Object.Destroy(ceremonyBlueprint.gameObject);
			}
			if (m_Blueprints.Count == 0)
			{
				if (m_WonderUnlocked != ObjectTypeList.m_Total)
				{
					SetState(State.RevealWonderWait);
				}
				else
				{
					SetState(State.Return);
				}
			}
			else
			{
				AttachNewToTopBlueprint();
			}
		}
		else if (m_State == State.IdleWonder)
		{
			m_SpeechPanel.SetActive(Active: false);
			UnityEngine.Object.Destroy(m_WonderBlueprint.gameObject);
			SetState(State.Return);
		}
		else if (m_State == State.RevealNew)
		{
			if (CeremonyManager.Instance.m_GlueQuest.m_ID == Quest.ID.GlueBasics)
			{
				SetState(State.ObjectsFirstTime);
			}
			else
			{
				End();
			}
		}
		else if (m_State == State.ObjectsFirstTime)
		{
			SetState(State.TipsFirstTime);
		}
		else if (m_State == State.TipsFirstTime)
		{
			End();
		}
	}

	private void StartCenter()
	{
		m_CertificateOldPosition = m_Certificate.GetComponent<RectTransform>().anchoredPosition;
		m_CertificateParent = m_Certificate.transform.parent;
		m_Certificate.transform.SetParent(base.transform, worldPositionStays: true);
		m_CertificateStartPosition = m_Certificate.GetComponent<RectTransform>().anchoredPosition;
	}

	private void EndReturn()
	{
		m_Certificate.transform.SetParent(m_CertificateParent);
		m_Certificate.GetComponent<RectTransform>().anchoredPosition = m_CertificateOldPosition;
	}

	private void StartSpeech(string Speech)
	{
		m_SpeechPanel.SetActive(Active: true);
		m_SpeechPanel.transform.SetParent(m_Fade.transform);
		m_SpeechPanel.transform.SetParent(base.transform);
		m_SpeechPanel.SetSpeechFromID(Speech);
	}

	private void StartRevealWonder()
	{
		m_WonderBlueprint.gameObject.SetActive(value: true);
		m_WonderOldPosition = m_Certificate.transform.localPosition;
		m_WonderBlueprint.transform.localPosition = m_WonderOldPosition;
		m_WonderBlueprint.transform.localScale = new Vector3(0.5f, 0.5f);
		AudioManager.Instance.StartEvent("CeremonyBlueprintSwoosh");
	}

	private void SetUnlockedQuestsVisible(bool Visible)
	{
		foreach (Quest.ID item in CeremonyManager.Instance.m_GlueQuest.m_QuestsUnlocked)
		{
			Academy.Instance.SetQuestVisible(item, Visible);
		}
	}

	private void StartRevealNew()
	{
		string speech = "";
		if (CeremonyManager.Instance.m_GlueQuest.m_ID == Quest.ID.GlueBasics)
		{
			speech = "CeremonyFirstQuestUnlocked";
		}
		else if (CeremonyManager.Instance.m_GlueQuest.m_ID == Quest.ID.GlueForestry)
		{
			speech = "CeremonySecondQuestUnlocked";
			LessonButton.SetLumberNew(New: true);
			Academy.Instance.UpdateLumber();
		}
		else if (CeremonyManager.Instance.m_GlueQuest.m_ID == Quest.ID.GlueFirstIndustries)
		{
			speech = "CeremonyThirdQuestUnlocked";
		}
		else if (CeremonyManager.Instance.m_GlueQuest.m_ID == Quest.ID.GlueFinal)
		{
			speech = "CeremonyFinalQuestUnlocked";
		}
		StartSpeech(speech);
		SetUnlockedQuestsVisible(Visible: true);
	}

	private void SetState(State NewState)
	{
		switch (m_State)
		{
		case State.WaitPress:
			UnityEngine.Object.Destroy(m_AcceptButton.gameObject);
			break;
		case State.QuestMove:
			UnityEngine.Object.Destroy(m_Certificate.GetComponent<Swoosher>());
			break;
		case State.Return:
			EndReturn();
			break;
		case State.ObjectsFirstTime:
			TutorialPointerManager.Instance.CeremonyActive(Active: true);
			TutorialPointerManager.Instance.SetType(TutorialPointerManager.Type.Total);
			break;
		case State.TipsFirstTime:
			TutorialPointerManager.Instance.CeremonyActive(Active: true);
			TutorialPointerManager.Instance.SetType(TutorialPointerManager.Type.Total);
			break;
		}
		m_State = NewState;
		m_StateTimer = 0f;
		switch (m_State)
		{
		case State.Center:
			StartCenter();
			break;
		case State.FirstTime:
			StartSpeech("CeremonyAcademyFirstTime");
			break;
		case State.Complete:
			m_Certificate.ForceComplete(Complete: true);
			AudioManager.Instance.StartEvent("CertificateAppear");
			break;
		case State.WaitPress:
			CreateAcceptButton();
			break;
		case State.QuestMove:
		{
			Swoosher swoosher = m_Certificate.gameObject.AddComponent<Swoosher>();
			Vector3 localPosition = m_Certificate.transform.localPosition;
			swoosher.StartSwoosh(localPosition, localPosition + new Vector3(-200f, 0f), m_BigScale, m_BigScale, 0.1f, 0f, null, Local: true);
			swoosher.m_UpdateWhilePaused = true;
			AudioManager.Instance.StartEvent("CeremonyBlueprintSwoosh");
			break;
		}
		case State.RevealWonderWait:
			m_New.gameObject.SetActive(value: false);
			break;
		case State.RevealWonder:
			StartRevealWonder();
			break;
		case State.IdleWonder:
			StartSpeech("CeremonyWonderUnlocked");
			break;
		case State.Return:
			m_CertificateCurrentPosition = m_Certificate.GetComponent<RectTransform>().anchoredPosition;
			break;
		case State.RevealNewWait:
			if ((bool)m_New)
			{
				m_New.gameObject.SetActive(value: false);
			}
			break;
		case State.RevealNew:
			StartRevealNew();
			break;
		case State.ObjectsFirstTime:
			StartSpeech("CeremonyObjectsFirstTime");
			Autopedia.Instance.ObjectsFirstTime();
			break;
		case State.TipsFirstTime:
			StartSpeech("CeremonyTipsFirstTime");
			Autopedia.Instance.TipsFirstTime();
			break;
		case State.Wait:
		case State.RevealBlueprints:
		case State.Idle:
		case State.RevealWonderWait2:
			break;
		}
	}

	private void UpdateCenter()
	{
		float num = 0.125f;
		if (m_StateTimer > num)
		{
			m_StateTimer = num;
		}
		float num2 = m_StateTimer / num;
		Vector2 anchoredPosition = (new Vector2(HudManager.Instance.m_HalfCanvasWidth, 0f - HudManager.Instance.m_HalfCanvasHeight) - m_CertificateStartPosition) * num2 + m_CertificateStartPosition;
		m_Certificate.GetComponent<RectTransform>().anchoredPosition = anchoredPosition;
		float num3 = (m_BigScale - 1f) * num2 + 1f;
		m_Certificate.transform.localScale = new Vector3(num3, num3, 1f);
		float a = num2 * m_FadeAlpha;
		m_Fade.SetColour(new Color(0f, 0f, 0f, a));
		if (m_StateTimer == num)
		{
			if (CeremonyManager.Instance.m_GluePlaying && CeremonyManager.Instance.m_GlueQuest.m_ID == Quest.ID.GlueBasics)
			{
				SetState(State.FirstTime);
			}
			else
			{
				SetState(State.Wait);
			}
		}
	}

	private void UpdateComplete()
	{
		if (m_StateTimer > 0.5f)
		{
			if (m_UnlockedObjects.Count > 0 || m_WonderUnlocked != ObjectTypeList.m_Total)
			{
				SetState(State.QuestMove);
			}
			else
			{
				SetState(State.WaitPress);
			}
		}
	}

	private void UpdateRevealBlueprints()
	{
		if (m_StateTimer > 0.1f)
		{
			m_StateTimer = 0f;
			if (m_CurrentBlueprint >= m_Blueprints.Count)
			{
				SetState(State.Idle);
				return;
			}
			CeremonyBlueprint ceremonyBlueprint = m_Blueprints[m_CurrentBlueprint];
			ceremonyBlueprint.gameObject.SetActive(value: true);
			ceremonyBlueprint.m_Button.SetActive(Active: true);
			Swoosher swoosher = ceremonyBlueprint.gameObject.AddComponent<Swoosher>();
			Vector3 localPosition = m_Certificate.transform.localPosition;
			Vector3 localPosition2 = ceremonyBlueprint.transform.localPosition;
			swoosher.StartSwoosh(localPosition, localPosition2, 0.5f, 1f, 0.1f, 0f, null, Local: true);
			swoosher.m_UpdateWhilePaused = true;
			AudioManager.Instance.StartEvent("CeremonyBlueprintSwoosh");
			m_CurrentBlueprint++;
		}
	}

	private void UpdateRevealWonder()
	{
		float num = 0.5f;
		if (m_StateTimer > num)
		{
			m_StateTimer = num;
		}
		float num2 = (0f - Mathf.Cos(m_StateTimer / num * (float)Math.PI)) * 0.5f + 0.5f;
		if (num2 > 0.5f && !m_WonderOnTop)
		{
			m_WonderOnTop = true;
			m_WonderBlueprint.transform.SetParent(m_Fade.transform, worldPositionStays: true);
			m_WonderBlueprint.transform.SetParent(base.transform, worldPositionStays: true);
		}
		Vector3 localPosition = (default(Vector3) - m_WonderOldPosition) * num2 + m_WonderOldPosition;
		localPosition.x += Mathf.Sin(num2 * (float)Math.PI) * 350f;
		m_WonderBlueprint.transform.localPosition = localPosition;
		float num3 = 1f * num2 + 0.5f;
		m_WonderBlueprint.transform.localScale = new Vector3(num3, num3);
		if (m_StateTimer >= 0.5f)
		{
			AttachNewToBlueprint(m_WonderBlueprint);
			m_New.gameObject.SetActive(value: true);
			SetState(State.RevealWonderWait2);
		}
	}

	private void UpdateReturn()
	{
		float num = 0.125f;
		if (m_StateTimer > num)
		{
			m_StateTimer = num;
		}
		float num2 = m_StateTimer / num;
		Vector2 anchoredPosition = (m_CertificateStartPosition - m_CertificateCurrentPosition) * num2 + m_CertificateCurrentPosition;
		m_Certificate.GetComponent<RectTransform>().anchoredPosition = anchoredPosition;
		float num3 = (1f - m_BigScale) * num2 + m_BigScale;
		m_Certificate.transform.localScale = new Vector3(num3, num3, 1f);
		float a = (1f - num2) * m_FadeAlpha;
		m_Fade.SetColour(new Color(0f, 0f, 0f, a));
		if (m_StateTimer == num)
		{
			EndReturn();
			if (CeremonyManager.Instance.m_GluePlaying)
			{
				SetState(State.RevealNewWait);
			}
			else
			{
				End();
			}
		}
	}

	public bool IsPlaying()
	{
		if (m_State == State.Idle || m_State == State.RevealBlueprints)
		{
			return false;
		}
		return true;
	}

	private void Update()
	{
		switch (m_State)
		{
		case State.Begin:
			if (m_StateTimer > 0.5f)
			{
				SetState(State.Center);
			}
			break;
		case State.Center:
			UpdateCenter();
			break;
		case State.Wait:
			if (m_StateTimer > 0.5f)
			{
				SetState(State.Complete);
			}
			break;
		case State.Complete:
			UpdateComplete();
			break;
		case State.QuestMove:
			if (m_StateTimer > 0.25f)
			{
				if (m_UnlockedObjects.Count > 0)
				{
					SetState(State.RevealBlueprints);
				}
				else
				{
					SetState(State.RevealWonder);
				}
			}
			break;
		case State.RevealBlueprints:
			UpdateRevealBlueprints();
			break;
		case State.RevealWonderWait:
			if (m_StateTimer > 0.5f)
			{
				SetState(State.RevealWonder);
			}
			break;
		case State.RevealWonder:
			UpdateRevealWonder();
			break;
		case State.RevealWonderWait2:
			if (m_StateTimer > 0.25f)
			{
				SetState(State.IdleWonder);
			}
			break;
		case State.Return:
			UpdateReturn();
			break;
		case State.RevealNewWait:
			if (m_StateTimer > 0.5f)
			{
				SetState(State.RevealNew);
			}
			break;
		}
		m_StateTimer += TimeManager.Instance.m_PauseDelta;
	}
}
