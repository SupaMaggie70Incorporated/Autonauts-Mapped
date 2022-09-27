public class FarmerStateEngaged : FarmerStateBase
{
	private enum EngagedType
	{
		Generic,
		Paddling,
		Converting,
		Cart,
		Building,
		Total
	}

	public FarmerEngagedBase m_Engaged;

	private bool m_EngagedWithVehicle;

	public override bool GetIsAdjacentTile(TileCoord TargetTile, BaseClass Object)
	{
		if (Object.m_TypeIdentifier == ObjectType.Worker)
		{
			return true;
		}
		return base.GetIsAdjacentTile(TargetTile, Object);
	}

	public override void StartState()
	{
		base.StartState();
		m_SafetyDelay = 0f;
		m_Engaged = null;
		if ((bool)m_Farmer.m_EngagedObject.GetComponent<Canoe>())
		{
			m_Engaged = new FarmerEngagedPaddling();
		}
		else if ((bool)m_Farmer.m_EngagedObject.GetComponent<ConverterFoundation>())
		{
			if (m_Farmer.m_EngagedObject.GetComponent<ConverterFoundation>().AreRequrementsMet())
			{
				m_Engaged = new FarmerEngagedBuilding();
			}
			else
			{
				m_Engaged = new FarmerEngagedGeneric();
			}
		}
		else if ((bool)m_Farmer.m_EngagedObject.GetComponent<RockingChair>())
		{
			m_Engaged = new FarmerEngagedRockingChair();
		}
		else if ((bool)m_Farmer.m_EngagedObject.GetComponent<Converter>())
		{
			m_Engaged = new FarmerEngagedConverting();
		}
		else if ((bool)m_Farmer.m_EngagedObject.GetComponent<ResearchStation>())
		{
			m_Engaged = new FarmerEngagedResearch();
		}
		else if (Cart.GetIsTypeCart(m_Farmer.m_EngagedObject.m_TypeIdentifier))
		{
			m_Engaged = new FarmerEngagedCart();
		}
		else if ((bool)m_Farmer.m_EngagedObject.GetComponent<Crane>())
		{
			m_Engaged = new FarmerEngagedCrane();
		}
		else if ((bool)m_Farmer.m_EngagedObject.GetComponent<Sign>())
		{
			m_Engaged = new FarmerEngagedSign();
		}
		else if ((bool)m_Farmer.m_EngagedObject.GetComponent<Minecart>())
		{
			m_Engaged = new FarmerEngagedMinecart();
		}
		else if (m_Farmer.m_EngagedObject.m_TypeIdentifier == ObjectType.Worker)
		{
			m_Engaged = new FarmerEngagedBot();
		}
		else
		{
			m_Engaged = new FarmerEngagedGeneric();
		}
		if (m_Engaged != null)
		{
			m_Engaged.m_Farmer = m_Farmer;
			m_Engaged.Start();
		}
		m_EngagedWithVehicle = m_Farmer.m_EngagedObject.GetComponent<Vehicle>() != null;
		if (m_EngagedWithVehicle)
		{
			DespawnManager.Instance.Remove(m_Farmer.m_EngagedObject.GetComponent<Vehicle>());
		}
	}

	public override void EndState()
	{
		base.EndState();
		if (m_Engaged != null)
		{
			m_Engaged.End();
		}
	}

	public override void UpdateState()
	{
		base.UpdateState();
		if (m_Engaged != null)
		{
			m_Engaged.Update();
		}
		if (m_EngagedWithVehicle)
		{
			RecordingManager.Instance.UpdateObject(m_Farmer);
		}
	}
}
