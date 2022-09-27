using UnityEngine;

public class Pumpkin : Holdable
{
	private Wobbler m_Wobbler;

	public override void Restart()
	{
		base.Restart();
		m_Wobbler.Restart();
	}

	protected new void Awake()
	{
		base.Awake();
		m_Wobbler = new Wobbler();
	}

	private void UseMallet(AFO Info)
	{
		m_Wobbler.Go(0.5f, 5f, 0.5f);
		base.enabled = true;
	}

	private void EndMallet(AFO Info)
	{
		BaseClass baseClass = ObjectTypeList.Instance.CreateObjectFromIdentifier(ObjectType.PumpkinRaw, m_TileCoord.ToWorldPositionTileCentered(), Quaternion.identity);
		SpawnAnimationManager.Instance.AddJump(baseClass, m_TileCoord, m_TileCoord, 0f, baseClass.transform.position.y, 4f);
		for (int i = 0; i < 2; i++)
		{
			BaseClass baseClass2 = ObjectTypeList.Instance.CreateObjectFromIdentifier(ObjectType.PumpkinSeeds, m_TileCoord.ToWorldPositionTileCentered(), Quaternion.identity);
			SpawnAnimationManager.Instance.AddJump(baseClass2, m_TileCoord, m_TileCoord, 0f, baseClass2.transform.position.y, 4f);
		}
		StopUsing();
	}

	private ActionType GetActionFromMallet(AFO Info)
	{
		Info.m_UseAction = UseMallet;
		Info.m_EndAction = EndMallet;
		Info.m_FarmerState = Farmer.State.Hammering;
		if (Info.m_ActionType == AFO.AT.Primary)
		{
			return ActionType.UseInHands;
		}
		return ActionType.Total;
	}

	private ActionType GetActionFromFlail(AFO Info)
	{
		Info.m_UseAction = UseMallet;
		Info.m_EndAction = EndMallet;
		Info.m_FarmerState = Farmer.State.Flail;
		if (Info.m_ActionType == AFO.AT.Primary)
		{
			return ActionType.UseInHands;
		}
		return ActionType.Total;
	}

	public override ActionType GetActionFromObject(AFO Info)
	{
		ObjectType objectType = Info.m_ObjectType;
		if (FarmerStateHammering.GetIsToolAcceptable(objectType))
		{
			return GetActionFromMallet(Info);
		}
		if (FarmerStateFlail.GetIsToolAcceptable(objectType))
		{
			return GetActionFromFlail(Info);
		}
		return base.GetActionFromObject(Info);
	}

	private void Update()
	{
		m_Wobbler.Update();
		float height = m_Wobbler.m_Height;
		float y = 1f + height * 0.5f;
		m_ModelRoot.transform.localScale = new Vector3(1f, y, 1f);
		if (m_Wobbler.m_Height == 0f)
		{
			base.enabled = false;
		}
	}
}
