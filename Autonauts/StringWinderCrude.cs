using UnityEngine;

public class StringWinderCrude : Converter
{
	private GameObject m_Spindle;

	private GameObject m_Stone;

	private Vector3 m_StonePosition;

	private GameObject m_String;

	private PlaySound m_PlaySound;

	public override void Restart()
	{
		base.Restart();
		m_DisplayIngredients = true;
		SetDimensions(new TileCoord(0, 0), new TileCoord(1, 0), new TileCoord(0, 1));
		SetSpawnPoint(new TileCoord(2, 0));
		SetResultToCreate(1);
		m_Spindle = m_ModelRoot.transform.Find("Spindle").gameObject;
		m_Stone = m_ModelRoot.transform.Find("StonePoint").gameObject;
		m_StonePosition = m_Stone.transform.localPosition;
		if (m_String == null)
		{
			GameObject gameObject = m_ModelRoot.transform.Find("StringPoint").gameObject;
			m_String = ModelManager.Instance.Instantiate(ObjectType.StringBall, "", gameObject.transform, RandomVariants: true);
			m_String.transform.localPosition = default(Vector3);
			m_String.transform.rotation = Quaternion.identity;
			m_String.transform.localScale = default(Vector3);
		}
	}

	public override void StartConverting()
	{
		base.StartConverting();
		m_PlaySound = AudioManager.Instance.StartEvent("BuildingStringWinderMaking", this, Remember: true);
		StartIngredientsDown();
	}

	protected override void UpdateConverting()
	{
		MoveIngredientsDown();
		m_Spindle.transform.localRotation = m_Spindle.transform.localRotation * Quaternion.Euler(0f, 0f, 1400f * TimeManager.Instance.m_NormalDelta);
		float conversionPercent = GetConversionPercent();
		m_String.transform.localScale = new Vector3(conversionPercent, conversionPercent, conversionPercent);
		m_String.transform.rotation = m_String.transform.rotation * Quaternion.Euler(0f, 1400f * TimeManager.Instance.m_NormalDelta, 0f);
		if ((int)(m_StateTimer * 60f) % 12 < 6)
		{
			m_Stone.transform.localPosition = m_StonePosition + new Vector3(0f, -0.25f, 0f);
		}
		else
		{
			m_Stone.transform.localPosition = m_StonePosition + new Vector3(0f, 0.25f, 0f);
		}
	}

	protected override void EndConverting()
	{
		EndIngredientsDown();
		AudioManager.Instance.StopEvent(m_PlaySound);
		m_String.transform.localScale = default(Vector3);
	}
}
