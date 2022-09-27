using UnityEngine;

public class CeremonyOffworldObjectUnlocked : CeremonyGenericSpeechWithTitle
{
	protected new void Awake()
	{
		base.Awake();
		SetTitle("CeremonyOffworldObjectUnlockedTitle");
		AudioManager.Instance.StartEvent("CeremonyQuestEnded");
		ShowSpeech(Show: true);
		if (Random.Range(0, 2) == 0)
		{
			SetSpeechImages("Ceremonies/GaryIdle", "Ceremonies/GaryTalk");
		}
		else
		{
			SetSpeechImages("Ceremonies/AaronIdle", "Ceremonies/AaronTalk");
		}
	}

	public void SetObjectType(ObjectType NewType)
	{
		CeremonyBlueprint component = base.transform.Find("CeremonyBlueprint").GetComponent<CeremonyBlueprint>();
		component.Init(NewType, OnAcceptClicked);
		component.m_Button.SetActive(Active: false);
		if (ObjectTypeList.Instance.GetIsBuilding(NewType))
		{
			SetSpeech("CeremonyOffworldObjectUnlockedSpeechAlt");
		}
		else
		{
			SetSpeech("CeremonyOffworldObjectUnlockedSpeech");
		}
	}

	protected virtual void EndPlans()
	{
		Object.Destroy(base.gameObject);
		CeremonyManager.Instance.CeremonyEnded();
	}

	public override void OnAcceptClicked(BaseGadget NewGadget)
	{
		EndPlans();
	}
}
