public class StorageSeedlingsRollover : StorageRollover
{
	private BaseText m_Text;

	private ConverterRequirementIngredient m_Ingredient;

	private ConverterRequirementFuel m_Fuel;

	protected new void Awake()
	{
		base.Awake();
		m_Target = null;
		m_Ingredient = m_Panel.transform.Find("ConverterRequirementIngredient").GetComponent<ConverterRequirementIngredient>();
		m_Fuel = m_Panel.transform.Find("ConverterRequirementFuel").GetComponent<ConverterRequirementFuel>();
		Hide();
	}

	protected override void UpdateTarget()
	{
		base.UpdateTarget();
		StorageSeedlings component = m_Target.GetComponent<StorageSeedlings>();
		_ = component.m_ObjectType;
		m_Ingredient.SetRequired(component.m_RequiredCount, 1);
		if (component.m_RequiredType == ObjectType.Nothing)
		{
			m_Ingredient.SetIngredient("StorageSeedlingsRolloverNoSeeds", component.m_RequiredType, WhiteText: false);
		}
		else
		{
			m_Ingredient.SetIngredient(component.m_RequiredType, WhiteText: false);
		}
		m_Fuel.UpdateFuel(component.GetFuelPercent(), (int)component.m_Fuel);
	}

	public override void SetStorageTarget(BaseClass Target)
	{
		base.SetStorageTarget(Target);
		if ((bool)m_Target)
		{
			m_Fuel.SetTier(Target.GetComponent<StorageSeedlings>().m_Tier, WhiteText: false);
		}
	}

	protected override bool GetTargettingSomething()
	{
		return m_Target != null;
	}
}
