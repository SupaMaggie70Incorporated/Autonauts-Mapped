using UnityEngine;

public class CrudeAnimalBreedingStation : Converter
{
	private PlaySound m_PlaySound;

	public override void Restart()
	{
		base.Restart();
		SetDimensions(new TileCoord(0, -2), new TileCoord(1, 0), new TileCoord(0, 1));
		SetSpawnPoint(new TileCoord(1, 1));
		m_DisplayIngredients = false;
	}

	public override void StartConverting()
	{
		base.StartConverting();
		m_PlaySound = AudioManager.Instance.StartEvent("BuildingBlueprintMaking", this, Remember: true);
	}

	protected override void UpdateConverting()
	{
		if ((int)(m_StateTimer * 60f) % 20 < 10)
		{
			m_ModelRoot.transform.localScale = new Vector3(0.9f, 1.3f, 0.9f);
		}
		else
		{
			m_ModelRoot.transform.localScale = new Vector3(1f, 1f, 1f);
		}
	}

	protected override void EndConverting()
	{
		AudioManager.Instance.StopEvent(m_PlaySound);
		AudioManager.Instance.StartEvent("BuildingToolMakingComplete", this);
		m_ModelRoot.transform.localScale = new Vector3(1f, 1f, 1f);
	}
}
