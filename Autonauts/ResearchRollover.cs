using UnityEngine;

public class ResearchRollover : Rollover
{
	private ResearchStation m_Target;

	private Quest m_TargetQuest;

	private bool m_ShowHearts;

	private ConverterRequirementIngredient m_ObjectImage;

	private ConverterRequirementIngredient m_Hearts;

	protected new void Awake()
	{
		base.Awake();
		m_Target = null;
		m_TargetQuest = null;
		Hide();
	}

	protected override void UpdateTarget()
	{
		Quest quest = null;
		quest = ((!m_Target) ? m_TargetQuest : QuestManager.Instance.GetQuest(m_Target.m_CurrentResearchQuest));
		if (quest == null)
		{
			return;
		}
		int progress = quest.m_EventsRequired[0].m_Progress;
		int required = quest.m_EventsRequired[0].m_Required;
		m_Hearts.SetRequired(progress, required);
		if ((bool)m_Target)
		{
			int count = 0;
			if ((bool)m_Target.m_CurrentResearchObject)
			{
				count = 1;
			}
			m_ObjectImage.SetRequired(count, 1);
		}
		else if (quest.GetIsComplete())
		{
			m_ObjectImage.SetRequired(1, 1);
		}
		else
		{
			m_ObjectImage.SetRequired(0, 1);
		}
	}

	private void SetupTargetQuest(Quest NewQuest)
	{
		Transform transform = m_Panel.transform;
		BaseText component = transform.Find("Title").GetComponent<BaseText>();
		BaseText component2 = transform.Find("Description").GetComponent<BaseText>();
		m_ObjectImage = transform.Find("RequiredObject").GetComponent<ConverterRequirementIngredient>();
		m_Hearts = transform.Find("Hearts").GetComponent<ConverterRequirementIngredient>();
		BaseImage component3 = transform.Find("ApprovedTick").GetComponent<BaseImage>();
		m_Hearts.SetIngredient(ObjectType.FolkHeart, WhiteText: false);
		if (NewQuest != null)
		{
			component.SetTextFromID(NewQuest.m_Title);
			component2.SetTextFromID(NewQuest.m_Description);
			m_ObjectImage.SetIngredient(NewQuest.m_ObjectTypeRequired, WhiteText: false);
			m_ObjectImage.gameObject.SetActive(value: true);
			m_Hearts.gameObject.SetActive(value: true);
			component3.SetActive(NewQuest.GetIsComplete());
			UpdateTarget();
		}
		else
		{
			string humanReadableNameFromIdentifier = ObjectTypeList.Instance.GetHumanReadableNameFromIdentifier(m_Target.m_TypeIdentifier);
			component.SetText(humanReadableNameFromIdentifier);
			string descriptionFromIdentifier = ObjectTypeList.Instance.GetDescriptionFromIdentifier(m_Target.m_TypeIdentifier);
			component2.SetText(descriptionFromIdentifier);
			m_ObjectImage.gameObject.SetActive(value: false);
			m_Hearts.gameObject.SetActive(value: true);
			component3.SetActive(Active: false);
		}
	}

	public void SetResearchTarget(ResearchStation Target)
	{
		if (Target != m_Target)
		{
			m_Target = Target;
			m_TargetQuest = null;
			m_Panel.SetActive(Active: false);
			if ((bool)m_Target)
			{
				Quest quest = QuestManager.Instance.GetQuest(m_Target.m_CurrentResearchQuest);
				SetupTargetQuest(quest);
				UpdateTarget();
			}
		}
	}

	public void SetResearchTarget(Quest TargetQuest)
	{
		if (TargetQuest != m_TargetQuest)
		{
			m_TargetQuest = TargetQuest;
			m_Target = null;
			m_Panel.SetActive(Active: false);
			if (m_TargetQuest != null)
			{
				SetupTargetQuest(m_TargetQuest);
				UpdateTarget();
			}
		}
	}

	protected override bool GetTargettingSomething()
	{
		if (!(m_Target != null))
		{
			return m_TargetQuest != null;
		}
		return true;
	}
}
