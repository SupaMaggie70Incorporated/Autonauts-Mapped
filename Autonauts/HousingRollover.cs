using UnityEngine;

public class HousingRollover : FolkRollover
{
	private Housing m_HousingTarget;

	private StandardProgressBar m_HouseHealth;

	private BaseText m_RepairText;

	private BaseImage m_RepairImage;

	private LevelHeart m_LevelHeart;

	protected new void Awake()
	{
		m_HouseHealth = base.transform.Find("BasePanel").Find("HouseHealth").GetComponent<StandardProgressBar>();
		m_RepairText = m_HouseHealth.transform.Find("RepairText").GetComponent<BaseText>();
		m_RepairImage = m_HouseHealth.transform.Find("RepairImage").GetComponent<BaseImage>();
		m_LevelHeart = base.transform.Find("BasePanel").Find("LevelHeart").GetComponent<LevelHeart>();
		base.Awake();
		m_HousingTarget = null;
		Hide();
	}

	private void HouseChanged()
	{
		if (m_Target == null)
		{
			string humanReadableNameFromIdentifier = ObjectTypeList.Instance.GetHumanReadableNameFromIdentifier(m_HousingTarget.m_TypeIdentifier);
			m_Title.SetText(humanReadableNameFromIdentifier);
			m_HouseHealth.SetActive(Active: true);
			UpdateHealth();
			m_LevelHeart.SetActive(Active: true);
			m_LevelHeart.SetValue(BaseClass.GetTierFromType(m_HousingTarget.m_TypeIdentifier));
		}
		else
		{
			m_HouseHealth.SetActive(Active: false);
			m_LevelHeart.SetActive(Active: false);
		}
	}

	public void SetTarget(Housing Target)
	{
		Folk folk = null;
		if ((bool)Target && Target.m_Folks.Count > 0)
		{
			folk = Target.m_Folks[0];
		}
		if (Target != m_HousingTarget || m_Target != folk)
		{
			m_HousingTarget = Target;
			if ((bool)m_HousingTarget)
			{
				SetTarget(folk);
				HouseChanged();
			}
			else
			{
				SetTarget(null, DisableBars: false, PreviousTier: false);
				m_HouseHealth.SetActive(Active: false);
			}
		}
	}

	protected override bool GetTargettingSomething()
	{
		return m_HousingTarget != null;
	}

	private void UpdateHealth()
	{
		float num = 1f - m_HousingTarget.GetUsed();
		m_HouseHealth.SetValue(num);
		if (num == 0f)
		{
			ObjectType repairTypeRequired = m_HousingTarget.GetRepairTypeRequired();
			string humanReadableNameFromIdentifier = ObjectTypeList.Instance.GetHumanReadableNameFromIdentifier(repairTypeRequired);
			int repairCountAdded = m_HousingTarget.m_RepairCountAdded;
			int repairAmountRequired = m_HousingTarget.GetRepairAmountRequired();
			string text = TextManager.Instance.Get("FolkRolloverRepair", humanReadableNameFromIdentifier, repairCountAdded.ToString(), repairAmountRequired.ToString());
			m_RepairText.SetText(text);
			m_RepairText.SetActive(Active: true);
			Sprite icon = IconManager.Instance.GetIcon(repairTypeRequired);
			m_RepairImage.SetSprite(icon);
			m_RepairImage.SetActive(Active: true);
		}
		else
		{
			m_RepairText.SetActive(Active: false);
			m_RepairImage.SetActive(Active: false);
		}
	}

	protected override void UpdateTarget()
	{
		base.UpdateTarget();
		Folk folk = null;
		if (m_HousingTarget.m_Folks.Count > 0)
		{
			folk = m_HousingTarget.m_Folks[0];
		}
		if (m_Target != folk)
		{
			SetTarget(folk);
			HouseChanged();
		}
		if (m_Target == null)
		{
			UpdateHealth();
		}
	}
}
