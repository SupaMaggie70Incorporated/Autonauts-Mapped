using UnityEngine;

public class FolkSeedRehydratorRollover : Rollover
{
	[HideInInspector]
	public FolkSeedRehydrator m_Target;

	private ConverterRequirementIngredient m_Folk;

	private BaseProgressBar m_Food;

	private BaseText m_Title;

	private BaseText m_Description;

	protected new void Awake()
	{
		base.Awake();
		m_Target = null;
		m_Title = m_Panel.transform.Find("Title").GetComponent<BaseText>();
		m_Description = m_Panel.transform.Find("Description").GetComponent<BaseText>();
		m_Folk = m_Panel.transform.Find("Ingredient").GetComponent<ConverterRequirementIngredient>();
		m_Food = m_Panel.transform.Find("FoodProgressBar").GetComponent<BaseProgressBar>();
		Hide();
	}

	protected new void Start()
	{
		base.Start();
		m_Folk.Init();
		m_Folk.SetIngredient(ObjectType.FolkSeed, WhiteText: false);
	}

	protected override void UpdateTarget()
	{
		string humanReadableNameFromIdentifier = ObjectTypeList.Instance.GetHumanReadableNameFromIdentifier(m_Target.m_TypeIdentifier);
		m_Title.SetText(humanReadableNameFromIdentifier);
		string descriptionFromIdentifier = ObjectTypeList.Instance.GetDescriptionFromIdentifier(m_Target.m_TypeIdentifier);
		m_Description.SetText(descriptionFromIdentifier);
		if (m_Target.GetFolkSeed())
		{
			m_Folk.SetRequired(1, 1);
		}
		else
		{
			m_Folk.SetRequired(0, 1);
		}
		m_Food.SetValue(m_Target.GetFuel());
	}

	public void SetStorageTarget(FolkSeedRehydrator Target)
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
