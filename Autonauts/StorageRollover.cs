using UnityEngine;

public class StorageRollover : GeneralRollover
{
	[HideInInspector]
	public BaseClass m_Target;

	private BaseText m_CurrentText;

	private BaseText m_TotalText;

	private BaseImage m_Image;

	private BaseText m_Description;

	protected new void Awake()
	{
		base.Awake();
		m_Target = null;
		m_CurrentText = m_Panel.transform.Find("CountCurrent").GetComponent<BaseText>();
		m_TotalText = m_Panel.transform.Find("CountTotal").GetComponent<BaseText>();
		m_Image = m_Panel.transform.Find("ObjectImage").GetComponent<BaseImage>();
		if ((bool)m_Panel.transform.Find("Description"))
		{
			m_Description = m_Panel.transform.Find("Description").GetComponent<BaseText>();
		}
		Hide();
	}

	private ObjectType GetStoredType()
	{
		ObjectType result = ObjectTypeList.m_Total;
		if ((bool)m_Target.GetComponent<MobileStorage>())
		{
			result = m_Target.GetComponent<MobileStorage>().m_ObjectType;
		}
		if ((bool)m_Target.GetComponent<StationaryEngine>())
		{
			result = ObjectType.Fuel;
		}
		if ((bool)m_Target.GetComponent<Storage>())
		{
			result = m_Target.GetComponent<Storage>().m_ObjectType;
		}
		return result;
	}

	protected override void UpdateTarget()
	{
		SetTargetTitle();
		ObjectType storedType = GetStoredType();
		if (storedType == ObjectTypeList.m_Total)
		{
			m_CurrentText.SetTextFromID("StorageRolloverEmpty");
			m_TotalText.SetTextFromID("StorageRolloverEmpty");
			m_Image.SetSprite("Icons/IconEmpty");
			return;
		}
		int num = 0;
		int num2 = 0;
		if ((bool)m_Target.GetComponent<MobileStorage>())
		{
			num = m_Target.GetComponent<MobileStorage>().GetStoredForDisplay();
			num2 = m_Target.GetComponent<MobileStorage>().GetCapacity();
		}
		if ((bool)m_Target.GetComponent<LinkedSystemEngine>())
		{
			num = m_Target.GetComponent<LinkedSystemEngine>().m_Energy;
			num2 = m_Target.GetComponent<LinkedSystemEngine>().m_EnergyCapacity;
		}
		if ((bool)m_Target.GetComponent<Storage>())
		{
			num = m_Target.GetComponent<Storage>().GetStoredForDisplay();
			num2 = m_Target.GetComponent<Storage>().GetCapacity();
		}
		m_CurrentText.SetText(num.ToString());
		m_TotalText.SetText(num2.ToString());
		m_Image.SetSprite(IconManager.Instance.GetIcon(storedType));
	}

	private void SetTargetTitle()
	{
		m_Title.SetText(m_Target.GetHumanReadableName());
		if ((bool)m_Description)
		{
			float num = 190f;
			string descriptionFromIdentifier = ObjectTypeList.Instance.GetDescriptionFromIdentifier(m_Target.m_TypeIdentifier);
			if (descriptionFromIdentifier != "")
			{
				m_Description.SetText(descriptionFromIdentifier);
				m_Description.SetActive(Active: true);
			}
			else
			{
				m_Description.SetActive(Active: false);
				num -= 45f;
			}
			SetHeight(num);
		}
	}

	public virtual void SetStorageTarget(BaseClass Target)
	{
		if (Target != m_Target)
		{
			m_Target = Target;
			m_Panel.SetActive(Active: false);
			if ((bool)m_Target)
			{
				SetTargetTitle();
				UpdateTarget();
			}
		}
	}

	protected override bool GetTargettingSomething()
	{
		return m_Target != null;
	}
}
