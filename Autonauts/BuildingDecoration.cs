public class BuildingDecoration : Building
{
	public override void Restart()
	{
		base.Restart();
		int num = 1;
		int num2 = 1;
		int variableAsInt = VariableManager.Instance.GetVariableAsInt(m_TypeIdentifier, "Width", CheckValid: false);
		if (variableAsInt != 0)
		{
			num = variableAsInt;
		}
		variableAsInt = VariableManager.Instance.GetVariableAsInt(m_TypeIdentifier, "Height", CheckValid: false);
		if (variableAsInt != 0)
		{
			num2 = variableAsInt;
		}
		SetDimensions(new TileCoord(0, -(num2 - 1)), new TileCoord(num - 1, 0), new TileCoord(0, 1));
		HideAccessModel();
	}
}
