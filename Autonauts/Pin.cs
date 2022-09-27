using UnityEngine;

public class Pin : BaseImage
{
	private CertificateInfo m_Info;

	private Quest m_Quest;

	public void SetQuest(Quest NewQuest, CertificateInfo NewInfo)
	{
		m_Quest = NewQuest;
		m_Info = NewInfo;
		UpdateStatus();
	}

	private bool GetLocked()
	{
		if (m_Info.m_ResearchIDs.Count == 0)
		{
			return false;
		}
		return !QuestManager.Instance.GetQuest(m_Info.m_ResearchIDs[0]).GetIsComplete();
	}

	public void UpdateStatus()
	{
		if (m_Quest.m_Pinned)
		{
			SetRolloverFromID("AcademyUnpin");
		}
		else
		{
			SetRolloverFromID("AcademyPin");
		}
		if (!GetLocked() && !m_Quest.GetIsComplete())
		{
			SetActive(Active: true);
			Color colour = new Color(1f, 1f, 1f, 1f);
			if (m_Quest.m_Pinned)
			{
				colour = ((!m_Indicated) ? new Color(1f, 0f, 0f, 1f) : new Color(0.5f, 0f, 0f, 1f));
			}
			else if (m_Indicated)
			{
				colour = new Color(0.5f, 0.5f, 0.5f, 1f);
			}
			SetColour(colour);
		}
		else
		{
			SetActive(Active: false);
		}
	}

	public override void SetIndicated(bool Indicated)
	{
		base.SetIndicated(Indicated);
		UpdateStatus();
	}
}
