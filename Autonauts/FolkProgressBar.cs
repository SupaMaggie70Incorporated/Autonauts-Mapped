using UnityEngine;

public class FolkProgressBar : StandardProgressBar
{
	public enum Type
	{
		Food,
		Housing,
		Clothing,
		Toy,
		Medicine,
		Education,
		Art,
		Total
	}

	public Type m_Type;

	private static string[] m_TypeImages = new string[7] { "IconFood", "IconHousing", "IconTop", "IconToy", "IconMedicine", "IconEducation", "IconArt" };

	private Folk m_Target;

	private bool m_ProgressDisabled;

	private bool m_PreviousTier;

	private BaseImage m_TierImage;

	private BaseText m_TierText;

	private BaseText m_RepairText;

	private BaseImage m_RepairImage;

	private bool m_Lowest;

	public void SetType(Type NewType)
	{
		m_Type = NewType;
		m_TierImage = base.transform.Find("TierImage").GetComponent<BaseImage>();
		m_TierText = m_TierImage.transform.Find("TierText").GetComponent<BaseText>();
		base.transform.Find("RequirementImage").GetComponent<BaseImage>().SetSprite("Icons/" + m_TypeImages[(int)NewType]);
		m_RepairText = base.transform.Find("RepairText").GetComponent<BaseText>();
		m_RepairImage = base.transform.Find("RepairImage").GetComponent<BaseImage>();
	}

	public void SetTarget(Folk NewTarget, bool ProgressDisabled = false, bool PreviousTier = false)
	{
		m_Target = NewTarget;
		m_ProgressDisabled = ProgressDisabled;
		m_PreviousTier = PreviousTier;
		if ((bool)m_Target)
		{
			UpdateBar(0f);
		}
	}

	public float GetProgress()
	{
		if (m_ProgressDisabled)
		{
			return 1f;
		}
		return m_Type switch
		{
			Type.Food => m_Target.GetFood(), 
			Type.Housing => m_Target.GetHousing(), 
			Type.Clothing => m_Target.GetClothing(), 
			Type.Toy => m_Target.GetToy(), 
			Type.Medicine => m_Target.GetMedicine(), 
			Type.Education => m_Target.GetEducation(), 
			Type.Art => m_Target.GetArt(), 
			_ => 0f, 
		};
	}

	public int GetTier()
	{
		if (m_ProgressDisabled)
		{
			return m_Target.GetTier();
		}
		return m_Type switch
		{
			Type.Food => m_Target.GetFoodTier(), 
			Type.Housing => m_Target.GetHousingTier(), 
			Type.Clothing => m_Target.GetClothingTier(), 
			Type.Toy => m_Target.GetToyTier(), 
			Type.Medicine => m_Target.GetMedicineTier(), 
			Type.Education => m_Target.GetEducationTier(), 
			Type.Art => m_Target.GetArtTier(), 
			_ => 0, 
		};
	}

	private bool GetRequired()
	{
		return m_Type switch
		{
			Type.Food => m_Target.GetIsFoodRequirement(), 
			Type.Housing => m_Target.GetIsHousingRequirement(), 
			Type.Clothing => m_Target.GetIsClothingRequirement(), 
			Type.Toy => m_Target.GetIsToyRequirement(), 
			Type.Medicine => m_Target.GetIsMedicineRequirement(), 
			Type.Education => m_Target.GetIsEducationRequirement(), 
			Type.Art => m_Target.GetIsArtRequirement(), 
			_ => false, 
		};
	}

	public bool GetRequirementLow()
	{
		if (GetTier() < m_Target.GetTier() && GetRequired() && GetProgress() > 0f)
		{
			return true;
		}
		return false;
	}

	private void UpdateTier(float Progress, float Timer)
	{
		int tier = GetTier();
		int tier2 = m_Target.GetTier();
		if (tier < tier2 && Progress > 0f)
		{
			Color color = new Color(1f, 1f, 1f, 1f);
			Color colour = new Color(0f, 0f, 0f, 1f);
			if (tier > tier2)
			{
				m_TierImage.SetSprite("FolkRollover/TierUp");
				color = new Color(1f, 1f, 1f, 1f);
			}
			else
			{
				m_TierImage.SetSprite("FolkRollover/TierDown");
				color = (((int)(Timer * 60f) % 16 >= 8) ? new Color(1f, 1f, 1f, 1f) : new Color(1f, 0f, 0f, 1f));
			}
			if (!m_Lowest)
			{
				color.a = 0.5f;
				colour.a = 0.5f;
			}
			m_TierImage.SetColour(color);
			m_TierImage.SetActive(Active: true);
			m_TierText.SetColour(colour);
			m_TierText.SetText((tier + 1).ToString());
		}
		else
		{
			m_TierImage.SetActive(Active: false);
		}
	}

	private void UpdateVisibility()
	{
		SetActive(GetRequired());
	}

	private void UpdateRepairText()
	{
		if (m_Type != Type.Housing)
		{
			m_RepairText.SetActive(Active: false);
			m_RepairImage.SetActive(Active: false);
			return;
		}
		Housing housing = m_Target.m_Housing;
		if ((bool)housing && housing.m_UsageCount == housing.m_MaxUsageCount)
		{
			m_RepairText.SetActive(Active: true);
			ObjectType repairTypeRequired = housing.GetRepairTypeRequired();
			string humanReadableNameFromIdentifier = ObjectTypeList.Instance.GetHumanReadableNameFromIdentifier(repairTypeRequired);
			int repairCountAdded = housing.m_RepairCountAdded;
			int repairAmountRequired = housing.GetRepairAmountRequired();
			string text = TextManager.Instance.Get("FolkRolloverRepair", humanReadableNameFromIdentifier, repairCountAdded.ToString(), repairAmountRequired.ToString());
			m_RepairText.SetText(text);
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

	public void SetLowest(bool Lowest)
	{
		m_Lowest = Lowest;
	}

	public void UpdateBar(float Timer)
	{
		UpdateVisibility();
		UpdateRepairText();
		float progress = GetProgress();
		SetValue(progress);
		UpdateTier(progress, Timer);
		if (progress == 0f)
		{
			if ((int)(Timer * 60f) % 30 < 15)
			{
				SetBackgroundColour(new Color(1f, 0f, 0f, 1f));
			}
			else
			{
				SetBackgroundColour(new Color(1f, 1f, 1f, 1f));
			}
		}
		else
		{
			SetBackgroundColour(new Color32(99, 99, 99, byte.MaxValue));
		}
	}
}
