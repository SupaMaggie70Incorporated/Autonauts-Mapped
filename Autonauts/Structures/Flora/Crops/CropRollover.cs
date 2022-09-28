public class CropRollover : GeneralRollover
{
	private Crop m_Target;

	private BaseImage m_Image;

	private BaseText m_Fertiliser;

	private BaseText m_Water;

	private BaseText m_Yield;

	private BaseText m_Tilled;

	private BaseText m_Description;

	protected new void Awake()
	{
		base.Awake();
		m_Target = null;
		m_Image = m_Panel.transform.Find("Panel/ObjectImage").GetComponent<BaseImage>();
		m_Fertiliser = m_Panel.transform.Find("FertiliserValue").GetComponent<BaseText>();
		m_Water = m_Panel.transform.Find("WaterValue").GetComponent<BaseText>();
		m_Tilled = m_Panel.transform.Find("TilledValue").GetComponent<BaseText>();
		m_Yield = m_Panel.transform.Find("YieldValue").GetComponent<BaseText>();
		m_Description = m_Panel.transform.Find("Description").GetComponent<BaseText>();
		Hide();
	}

	public void SetTarget(BaseClass Target)
	{
		if (Target != m_Target)
		{
			if ((bool)Target)
			{
				m_Target = Target.GetComponent<Crop>();
				string descriptionFromIdentifier = ObjectTypeList.Instance.GetDescriptionFromIdentifier(Target.m_TypeIdentifier);
				m_Description.SetText(descriptionFromIdentifier);
			}
			else
			{
				m_Target = null;
			}
			UpdateTarget();
		}
	}

	protected override bool GetTargettingSomething()
	{
		if ((bool)m_Target)
		{
			return true;
		}
		return false;
	}

	protected override void UpdateTarget()
	{
		if (!(m_Target == null))
		{
			m_Title.SetText(m_Target.GetHumanReadableName());
			m_Image.SetSprite(IconManager.Instance.GetIcon(m_Target.m_TypeIdentifier));
			string newText = "CropRolloverYes";
			string newText2 = "CropRolloverNo";
			if (m_Target.m_Fertiliser != ObjectTypeList.m_Total)
			{
				m_Fertiliser.SetTextFromID(newText);
			}
			else
			{
				m_Fertiliser.SetTextFromID(newText2);
			}
			if (m_Target.m_Watered)
			{
				m_Water.SetTextFromID(newText);
			}
			else
			{
				m_Water.SetTextFromID(newText2);
			}
			if (m_Target.m_StartTile == Tile.TileType.SoilTilled)
			{
				m_Tilled.SetTextFromID(newText);
			}
			else
			{
				m_Tilled.SetTextFromID(newText2);
			}
			m_Yield.SetText(m_Target.m_Yield.ToString());
		}
	}
}
