using UnityEngine;

public class HayBalerCrude : Converter
{
	private PlaySound m_PlaySound;

	private GameObject m_Wheel;

	private GameObject m_Weight;

	public override void Restart()
	{
		base.Restart();
		m_DisplayIngredients = true;
		SetDimensions(new TileCoord(0, 0), new TileCoord(0, 0), new TileCoord(0, 1));
		SetSpawnPoint(new TileCoord(1, 0));
		m_Weight = m_ModelRoot.transform.Find("Weight").gameObject;
		m_Wheel = m_ModelRoot.transform.Find("Wheel").gameObject;
	}

	protected override void UpdateIngredients()
	{
		base.UpdateIngredients();
		Vector3 position = m_Weight.transform.position;
		position.y = m_IngredientsRoot.transform.position.y + m_IngredientsHeight;
		m_Weight.transform.position = position;
	}

	public override void StartConverting()
	{
		base.StartConverting();
		m_PlaySound = AudioManager.Instance.StartEvent("BuildingHayBalerMaking", this, Remember: true);
		StartIngredientsDown();
	}

	protected override void UpdateConverting()
	{
		m_Wheel.transform.localRotation = Quaternion.Euler(500f * TimeManager.Instance.m_NormalDelta, 0f, 0f) * m_Wheel.transform.localRotation;
		ConvertVibrate();
		MoveIngredientsDown();
		Vector3 position = m_Weight.transform.position;
		float num = 1f - GetConversionPercent();
		position.y = m_IngredientsRoot.transform.position.y + num * m_IngredientsHeight;
		m_Weight.transform.position = position;
	}

	protected override void EndConverting()
	{
		EndVibrate();
		EndIngredientsDown();
		AudioManager.Instance.StopEvent(m_PlaySound);
		AudioManager.Instance.StartEvent("BuildingToolMakingComplete", this);
	}
}
