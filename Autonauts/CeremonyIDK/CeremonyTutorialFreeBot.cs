using System.Collections.Generic;
using UnityEngine;

public class CeremonyTutorialFreeBot : CeremonyGenericSpeech
{
	private WorkerAssembler m_Assembler;

	public void SetQuest(Quest NewQuest)
	{
		SetQuest(NewQuest, null);
		if (NewQuest.m_ID == Quest.ID.GlueFirstFreeBot)
		{
			SetSpeech("CeremonyTutorialFirstFreeBot");
		}
		else if (NewQuest.m_ID == Quest.ID.GlueSecondFreeBot)
		{
			SetSpeech("CeremonyTutorialSecondFreeBot");
		}
		GetAssembler();
		if ((bool)m_Assembler)
		{
			CreateBot();
			PanTo(m_Assembler, new Vector3(0f, 2f, 0f), 10f, 1f);
			m_Speech.GetButton().SetActive(Active: false);
		}
	}

	private void GetAssembler()
	{
		foreach (KeyValuePair<BaseClass, int> item in CollectionManager.Instance.GetCollection("Converter"))
		{
			if (item.Key.m_TypeIdentifier == ObjectType.WorkerAssembler)
			{
				m_Assembler = item.Key.GetComponent<WorkerAssembler>();
				break;
			}
		}
	}

	private void CreateBot()
	{
		if ((bool)m_Assembler)
		{
			m_Assembler.CreateBot();
		}
	}

	protected override void End()
	{
		base.End();
		ReturnPanTo(1f);
	}

	public void Update()
	{
		if (m_Assembler != null && m_Assembler.m_State == Converter.State.Idle)
		{
			m_Speech.GetButton().SetActive(Active: true);
		}
	}
}
