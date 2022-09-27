public class StringWinder : StringWinderCrude
{
	protected override void UpdateConverting()
	{
		base.UpdateConverting();
		ConvertVibrate();
	}

	protected override void EndConverting()
	{
		base.EndConverting();
		EndVibrate();
	}
}
