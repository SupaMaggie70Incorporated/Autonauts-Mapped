public class CeremonyTutorialComplete : CeremonyGenericSpeech
{
	protected new void Awake()
	{
		base.Awake();
		AudioManager.Instance.StartEvent("CeremonyFirstResearch");
	}

	public void SetQuest(Quest NewQuest)
	{
		m_Quest = NewQuest;
		string speech = NewQuest.m_ID.ToString() + "Complete";
		SetSpeech(speech);
	}

	protected override void End()
	{
		base.End();
		if (m_Quest.m_ID == Quest.ID.TutorialBasics)
		{
			ModeButton.Get(ModeButton.Type.Autopedia).SetNew(New: true);
		}
		TutorialPanelController.Instance.NextQuest();
	}
}
