using UnityEngine;

public class CeremonyBadgeRevealed : CeremonyBase
{
	private void Awake()
	{
	}

	public void SetBadge(Badge NewBadge)
	{
		Transform obj = base.transform.Find("BadgePanel/Panel");
		BaseText component = obj.Find("Title").GetComponent<BaseText>();
		string newText = "Badge" + NewBadge.m_ID;
		component.SetTextFromID(newText);
		BadgeEvent.Type @event = NewBadge.m_EventsRequired[0].m_Event;
		BaseText component2 = obj.Find("EventDescription").GetComponent<BaseText>();
		string nameFromType = BadgeEvent.GetNameFromType(@event);
		component2.SetTextFromID(nameFromType);
		int count = NewBadge.m_EventsRequired[0].m_Count;
		obj.Find("EventCount").GetComponent<BaseText>().SetText(count.ToString());
		string text = "Badge" + NewBadge.m_ID;
		base.transform.Find("BadgePanel/BadgeIcon").GetComponent<BaseImage>().SetSprite("Badges/" + text);
		StandardAcceptButton component3 = obj.Find("StandardAcceptButton").GetComponent<StandardAcceptButton>();
		component3.SetAction(OnAcceptClicked, component3);
		AudioManager.Instance.StartEvent("CeremonyBadgeEarned");
	}

	private void End()
	{
		Object.Destroy(base.gameObject);
		CeremonyManager.Instance.CeremonyEnded();
	}

	public void OnAcceptClicked(BaseGadget NewGadget)
	{
		End();
	}

	private void Update()
	{
	}
}
