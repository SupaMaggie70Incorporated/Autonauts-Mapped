public class HoldableRollover : GeneralRollover
{
	private static float m_NormalHeight = 220f;

	private BaseClass m_Target;

	private Tile.TileType m_TargetType;

	private ObjectType m_TargetObject;

	private BaseImage m_Image;

	private BaseText m_Description;

	private LevelHeart m_LevelHeart;

	private BaseImage m_StorageImage;

	private Weight m_Weight;

	protected new void Awake()
	{
		base.Awake();
		m_Target = null;
		m_TargetType = Tile.TileType.Total;
		m_TargetObject = ObjectTypeList.m_Total;
		m_Image = m_Panel.transform.Find("Panel/ObjectImage").GetComponent<BaseImage>();
		m_Description = m_Panel.transform.Find("Description").GetComponent<BaseText>();
		m_LevelHeart = m_Panel.transform.Find("Panel/LevelHeart").GetComponent<LevelHeart>();
		m_LevelHeart.SetActive(Active: false);
		m_StorageImage = m_Panel.transform.Find("Panel/Storage").GetComponent<BaseImage>();
		m_StorageImage.SetActive(Active: false);
		m_Weight = m_Panel.transform.Find("Panel/Weight").GetComponent<Weight>();
		Hide();
	}

	private void UpdateHeart()
	{
		ObjectType newType = m_TargetObject;
		if ((bool)m_Target)
		{
			newType = m_Target.m_TypeIdentifier;
		}
		if (!BaseClass.GetHasTierFromType(newType))
		{
			m_LevelHeart.SetActive(Active: false);
			return;
		}
		m_LevelHeart.SetActive(Active: true);
		int tierFromType = BaseClass.GetTierFromType(newType);
		m_LevelHeart.SetValue(tierFromType);
	}

	private ObjectType GetStorageTypeDisplayed(ObjectType NewType)
	{
		ObjectType storageType = ObjectTypeList.Instance.GetStorageType(NewType);
		if ((storageType == ObjectTypeList.m_Total || !Storage.GetIsTypeStorage(storageType)) && storageType != ObjectType.FolkSeedPod && storageType != ObjectType.SilkwormStation)
		{
			return ObjectTypeList.m_Total;
		}
		return storageType;
	}

	private void UpdateWeight()
	{
		ObjectType objectType = m_TargetObject;
		if ((bool)m_Target)
		{
			objectType = m_Target.m_TypeIdentifier;
		}
		if (ObjectTypeList.Instance.GetIsBuilding(objectType) || !ObjectTypeList.Instance.GetIsHoldable(objectType))
		{
			m_Weight.SetActive(Active: false);
			return;
		}
		m_Weight.SetActive(Active: true);
		if (Vehicle.GetIsTypeVehicle(objectType) || Carriage.GetIsTypeCarriage(objectType))
		{
			m_Weight.SetNeedCrane(Crane: true);
			return;
		}
		m_Weight.SetNeedCrane(Crane: false);
		if (GetStorageTypeDisplayed(objectType) == ObjectTypeList.m_Total)
		{
			m_Weight.SetActive(Active: false);
			return;
		}
		m_Weight.SetActive(Active: true);
		m_Weight.SetObjectType(objectType);
	}

	private void UpdateStorageImage(ObjectType NewType)
	{
		if (NewType == ObjectTypeList.m_Total)
		{
			m_StorageImage.SetActive(Active: false);
			return;
		}
		ObjectType storageTypeDisplayed = GetStorageTypeDisplayed(NewType);
		if (storageTypeDisplayed == ObjectTypeList.m_Total)
		{
			m_StorageImage.SetActive(Active: false);
			return;
		}
		m_StorageImage.SetActive(Active: true);
		m_StorageImage.SetSprite(IconManager.Instance.GetIcon(storageTypeDisplayed));
	}

	private string GetObjectDescription(ObjectType NewType)
	{
		string result = ObjectTypeList.Instance.GetDescriptionFromIdentifier(NewType);
		int variableAsInt = VariableManager.Instance.GetVariableAsInt(NewType, "UpgradeFrom", CheckValid: false);
		if (variableAsInt != 0)
		{
			ObjectType identifier = (ObjectType)variableAsInt;
			string humanReadableNameFromIdentifier = ObjectTypeList.Instance.GetHumanReadableNameFromIdentifier(identifier);
			result = TextManager.Instance.Get("RolloverBackupDescription", humanReadableNameFromIdentifier);
		}
		return result;
	}

	private void SetDescription(string Description)
	{
		m_Description.SetText(Description);
		float num = m_NormalHeight;
		if (Description == "")
		{
			num -= 60f;
		}
		SetHeight(num);
	}

	public void SetTarget(BaseClass Target)
	{
		if (!(Target != m_Target))
		{
			return;
		}
		m_Target = Target;
		m_TargetType = Tile.TileType.Total;
		m_TargetObject = ObjectTypeList.m_Total;
		if ((bool)Target)
		{
			string text = Target.GetHumanReadableName();
			if (Sign.GetIsTypeSign(Target.m_TypeIdentifier))
			{
				string signName = Target.GetComponent<Sign>().m_SignName;
				text = ((!(signName == "")) ? (text + " - \"" + signName + "\"") : (text + "(" + TextManager.Instance.Get("SignBlank") + ")"));
			}
			m_Title.SetText(text);
			string objectDescription = GetObjectDescription(Target.m_TypeIdentifier);
			SetDescription(objectDescription);
			m_Image.SetActive(Active: true);
			m_Image.SetSprite(Target.GetIcon());
			UpdateStorageImage(Target.m_TypeIdentifier);
			UpdateHeart();
			UpdateWeight();
		}
	}

	public void SetTarget(Tile.TileType TargetType)
	{
		if (TargetType == m_TargetType)
		{
			return;
		}
		if (TargetType == Tile.TileType.Empty)
		{
			TargetType = Tile.TileType.Total;
		}
		m_TargetType = TargetType;
		m_Target = null;
		m_TargetObject = ObjectTypeList.m_Total;
		if (TargetType != Tile.TileType.Total)
		{
			string text = Tile.m_TileInfo[(int)TargetType].m_Name;
			m_Title.SetTextFromID(text);
			if (TextManager.Instance.DoesExist(text + "Description"))
			{
				SetDescription(TextManager.Instance.Get(text + "Description"));
			}
			else
			{
				SetDescription("");
			}
			m_Image.SetActive(Active: true);
			m_Image.SetSprite(Tile.GetIcon(TargetType));
			m_LevelHeart.SetActive(Active: false);
			UpdateStorageImage(ObjectTypeList.m_Total);
			m_Weight.SetActive(Active: false);
		}
	}

	public void SetTarget(ObjectType Target)
	{
		if (Target != m_TargetObject)
		{
			m_TargetObject = Target;
			m_Target = null;
			m_TargetType = Tile.TileType.Total;
			if (m_TargetObject != ObjectTypeList.m_Total)
			{
				string humanReadableNameFromIdentifier = ObjectTypeList.Instance.GetHumanReadableNameFromIdentifier(Target);
				m_Title.SetText(humanReadableNameFromIdentifier);
				string objectDescription = GetObjectDescription(Target);
				SetDescription(objectDescription);
				m_Image.SetActive(Active: true);
				m_Image.SetSprite(IconManager.Instance.GetIcon(Target));
				UpdateHeart();
				UpdateStorageImage(Target);
				UpdateWeight();
			}
		}
	}

	protected override bool GetTargettingSomething()
	{
		if ((bool)m_Target || m_TargetType != Tile.TileType.Total || m_TargetObject != ObjectTypeList.m_Total)
		{
			return true;
		}
		return false;
	}
}
