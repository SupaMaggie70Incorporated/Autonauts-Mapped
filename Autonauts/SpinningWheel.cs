using UnityEngine;

public class SpinningWheel : Converter
{
	private GameObject m_Spindle;

	private PlaySound m_PlaySound;

	private Transform m_Wool;

	private GameObject m_WoolModel;

	public override void Restart()
	{
		base.Restart();
		SetDimensions(new TileCoord(0, 0), new TileCoord(0, 0), new TileCoord(0, 1));
		SetSpawnPoint(new TileCoord(1, 0));
		SetResultToCreate(1);
		m_Spindle = m_ModelRoot.transform.Find("Spindle").gameObject;
		if (m_Wool == null)
		{
			m_Wool = m_ModelRoot.transform.Find("WoolPoint");
			m_WoolModel = ModelManager.Instance.Instantiate(ObjectType.Wool, "", m_Wool, RandomVariants: true);
			m_WoolModel.transform.localPosition = default(Vector3);
			m_WoolModel.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
			m_WoolModel.transform.localScale = default(Vector3);
		}
	}

	public override void StartConverting()
	{
		base.StartConverting();
		m_PlaySound = AudioManager.Instance.StartEvent("BuildingSpinningWheelMaking", this, Remember: true);
	}

	protected override void UpdateConverting()
	{
		m_Spindle.transform.localRotation = Quaternion.Euler(0f, 0f, 1000f * TimeManager.Instance.m_NormalDelta) * m_Spindle.transform.localRotation;
		float conversionPercent = GetConversionPercent();
		m_WoolModel.transform.localScale = new Vector3(conversionPercent, conversionPercent, 1f);
		m_WoolModel.transform.localPosition = new Vector3(0f, 0f, (0f - conversionPercent) * ObjectTypeList.Instance.GetHeight(ObjectType.Wool) * 0.5f);
		m_Wool.transform.localRotation = Quaternion.Euler(0f, 0f, 1400f * TimeManager.Instance.m_NormalDelta) * m_Wool.transform.localRotation;
	}

	protected override void EndConverting()
	{
		AudioManager.Instance.StopEvent(m_PlaySound);
		m_WoolModel.transform.localScale = default(Vector3);
	}
}
