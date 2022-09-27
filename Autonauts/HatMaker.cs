using UnityEngine;

public class HatMaker : Converter
{
	private PlaySound m_PlaySound;

	private Transform m_HeadRoot;

	public override void Restart()
	{
		base.Restart();
		m_DisplayIngredients = true;
		SetDimensions(new TileCoord(0, 0), new TileCoord(1, 0), new TileCoord(0, 1));
		SetSpawnPoint(new TileCoord(2, 0));
	}

	public override void PostCreate()
	{
		base.PostCreate();
		m_HeadRoot = m_ModelRoot.transform.Find("Head");
	}

	public override void StartConverting()
	{
		base.StartConverting();
		SquashIngredients();
	}

	public override void DoConvertAnimAction()
	{
		base.DoConvertAnimAction();
		m_PlaySound = AudioManager.Instance.StartEvent("BuildingWorkbenchAction", this);
		UpdateIngredients();
		float scale = (1f - GetConversionPercent()) * 0.8f + 0.2f;
		SquashIngredients(scale);
		m_HeadRoot.transform.localScale = new Vector3(0.9f, 1.3f, 0.9f);
	}

	protected override void UpdateConverting()
	{
		base.UpdateConverting();
		UpdateSquashIngredients();
		UpdateConvertAnimTimer(0.25f);
		if (m_SquashIngredientsTimer == 0f)
		{
			m_HeadRoot.transform.localScale = new Vector3(1f, 1f, 1f);
		}
	}

	protected override void EndConverting()
	{
		base.EndConverting();
		EndSquashIngredients();
		AudioManager.Instance.StartEvent("BuildingToolMakingComplete", this);
		m_HeadRoot.transform.localScale = new Vector3(1f, 1f, 1f);
	}
}
