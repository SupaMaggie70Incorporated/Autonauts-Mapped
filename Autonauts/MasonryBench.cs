public class MasonryBench : Converter
{
	private PlaySound m_PlaySound;

	public override void Restart()
	{
		base.Restart();
		m_DisplayIngredients = true;
		SetDimensions(new TileCoord(0, 0), new TileCoord(1, 0), new TileCoord(0, 1));
		SetSpawnPoint(new TileCoord(2, 0));
	}

	public override void StartConverting()
	{
		base.StartConverting();
		SquashIngredients();
	}

	public override void DoConvertAnimAction()
	{
		base.DoConvertAnimAction();
		DoConvertAnimActionParticles("StoneChips");
		m_PlaySound = AudioManager.Instance.StartEvent("BuildingWorkbenchAction", this);
		UpdateIngredients();
		SquashIngredients();
	}

	protected override void UpdateConverting()
	{
		base.UpdateConverting();
		UpdateSquashIngredients();
		UpdateConvertAnimTimer(0.25f);
	}

	protected override void EndConverting()
	{
		base.EndConverting();
		EndSquashIngredients();
		AudioManager.Instance.StartEvent("BuildingToolMakingComplete", this);
	}
}
