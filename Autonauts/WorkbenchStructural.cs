public class WorkbenchStructural : Converter
{
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
		UpdateIngredients();
		SquashIngredients();
		DoConvertAnimActionParticles("WorkbenchWoodChips");
		AudioManager.Instance.StartEvent("BuildingWorkbenchImpactMaking", this);
	}

	protected override void UpdateConverting()
	{
		UpdateSquashIngredients();
		UpdateConvertAnimTimer(0.25f);
	}

	protected override void EndConverting()
	{
		EndSquashIngredients();
		AudioManager.Instance.StartEvent("BuildingToolMakingComplete", this);
		QuestManager.Instance.AddEvent(QuestEvent.Type.MakeStructuralPart, m_LastEngagerType == ObjectType.Worker, 0, this);
	}
}
