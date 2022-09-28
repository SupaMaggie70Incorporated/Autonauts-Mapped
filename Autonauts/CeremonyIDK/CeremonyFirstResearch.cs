using System.Collections.Generic;
using UnityEngine;

public class CeremonyFirstResearch : CeremonyGenericSpeechWithTitle
{
	protected new void Awake()
	{
		base.Awake();
		SetTitle("CeremonyFirstResearch");
		SetSpeech("CeremonyFirstResearchSpeech1");
		AudioManager.Instance.StartEvent("CeremonyFirstResearch");
		foreach (KeyValuePair<BaseClass, int> item in CollectionManager.Instance.GetCollection("ResearchStation"))
		{
			if (item.Key.GetComponent<ResearchStation>().GetIsSavable() && item.Key.m_TypeIdentifier == ObjectType.ResearchStationCrude)
			{
				PanTo(item.Key, new Vector3(0f, 2f, 0f), 10f, 1f);
				break;
			}
		}
	}

	protected override void End()
	{
		base.End();
		ModeButton.Get(ModeButton.Type.Autopedia).SetNew(New: true);
		ReturnPanTo(1f);
		CeremonyManager.Instance.DoResearchLevelUnlocked(m_Quest, m_UnlockedObjects);
	}
}
