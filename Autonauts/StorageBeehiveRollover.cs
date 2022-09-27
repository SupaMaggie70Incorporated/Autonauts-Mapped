using UnityEngine;

public class StorageBeehiveRollover : GeneralRollover
{
	[HideInInspector]
	public StorageBeehive m_Target;

	private BaseText m_Bees;

	private StandardProgressBar m_Honey;

	private float m_FullTimer;

	protected new void Awake()
	{
		base.Awake();
		m_Target = null;
		m_Bees = m_Panel.transform.Find("Bees").GetComponent<BaseText>();
		m_Honey = m_Panel.transform.Find("HoneyProgressBar").GetComponent<StandardProgressBar>();
		m_FullTimer = 0f;
		Hide();
	}

	protected override void UpdateTarget()
	{
		string humanReadableNameFromIdentifier = ObjectTypeList.Instance.GetHumanReadableNameFromIdentifier(m_Target.m_TypeIdentifier);
		m_Title.SetText(humanReadableNameFromIdentifier);
		string newText = "StorageBeehiveRolloverNoBees";
		if (m_Target.m_Bees.Count != 0)
		{
			newText = "StorageBeehiveRolloverBees";
		}
		m_Bees.SetTextFromID(newText);
		int stored = m_Target.GetStored();
		int capacity = m_Target.GetCapacity();
		float value = (float)stored / (float)capacity;
		m_Honey.SetValue(value);
		if (stored == capacity)
		{
			m_FullTimer += TimeManager.Instance.m_NormalDelta;
			if ((int)(m_FullTimer * 60f) % 8 < 4)
			{
				m_Honey.SetFillColour(new Color(1f, 1f, 0f));
			}
			else
			{
				m_Honey.SetFillColour(new Color(1f, 1f, 1f));
			}
		}
		else
		{
			m_Honey.SetFillColour(new Color(1f, 1f, 0f));
		}
	}

	public void SetStorageTarget(StorageBeehive Target)
	{
		if (Target != m_Target)
		{
			m_Target = Target;
			if ((bool)m_Target)
			{
				UpdateTarget();
			}
		}
	}

	protected override bool GetTargettingSomething()
	{
		return m_Target != null;
	}
}
