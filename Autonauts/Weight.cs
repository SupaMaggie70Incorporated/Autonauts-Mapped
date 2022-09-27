public class Weight : BaseGadget
{
	public void SetNeedCrane(bool Crane)
	{
		string sprite = "RolloverWeightHand";
		if (Crane)
		{
			sprite = "GenIcons/GenIconCraneCrude";
		}
		GetComponent<BaseImage>().SetSprite(sprite);
		base.transform.Find("WeightValue").GetComponent<BaseText>().SetActive(!Crane);
	}

	public void SetObjectType(ObjectType NewType)
	{
		string sprite = "RolloverWeightHand";
		if (ToolFillable.GetIsTypeLiquid(NewType))
		{
			sprite = "RolloverWeightBucket";
		}
		GetComponent<BaseImage>().SetSprite(sprite);
		int weight = Holdable.GetWeight(NewType);
		base.transform.Find("WeightValue").GetComponent<BaseText>().SetText(weight.ToString());
	}
}
