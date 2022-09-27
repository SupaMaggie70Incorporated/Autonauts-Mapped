using SimpleJSON;
using UnityEngine;

public class TallBoulder : Boulder
{
	private int m_Size;

	public override void RegisterClass()
	{
		base.RegisterClass();
		ObjectTypeList.Instance.SetTypeSleep(ObjectType.TallBoulder);
		ModelManager.Instance.AddModel("Models/Other/TallBoulder2", ObjectType.TallBoulder, RandomVariants: true);
		ModelManager.Instance.AddModel("Models/Other/TallBoulder3", ObjectType.TallBoulder, RandomVariants: true);
	}

	public override void Restart()
	{
		base.Restart();
		UpdateSize();
	}

	private void UpdateSize()
	{
		if (m_Size != 0)
		{
			m_HeightScale = (float)m_Size * 0.5f + 1f;
		}
		else
		{
			m_HeightScale = 1f;
		}
		UpdateWobbler();
	}

	public void SetSize(int Size)
	{
		m_Size = Size;
		m_MaxMined = VariableManager.Instance.GetVariableAsInt(ObjectType.TallBoulder, "MaxMined" + (m_Size + 1));
		UpdateSize();
	}

	public override void Save(JSONNode Node)
	{
		base.Save(Node);
		Node["B"] = m_Size;
	}

	public override void Load(JSONNode Node)
	{
		base.Load(Node);
		int asInt = Node["B"].AsInt;
		SetSize(asInt);
	}

	private void CreateStoneBlockCrude()
	{
		TileCoord randomEmptyTile = TileHelpers.GetRandomEmptyTile(m_TileCoord);
		BaseClass baseClass = ObjectTypeList.Instance.CreateObjectFromIdentifier(ObjectType.StoneBlockCrude, randomEmptyTile.ToWorldPositionTileCentered(), Quaternion.identity);
		SpawnAnimationManager.Instance.AddJump(baseClass, m_TileCoord, randomEmptyTile, 0f, baseClass.transform.position.y, 4f);
	}

	private void StartChisel(AFO Info)
	{
	}

	private void UseChisel(AFO Info)
	{
		m_Wobbler.Go(0.5f, 5f, 0.5f);
		Wake();
	}

	private void EndChisel(AFO Info)
	{
		CreateStoneBlockCrude();
		m_AmountMined += VariableManager.Instance.GetVariableAsInt("CrudeStoneBlockCost");
		if (m_AmountMined >= m_MaxMined)
		{
			StopUsing();
		}
		UpdateWobbler();
	}

	private ActionType GetActionFromChisel(AFO Info)
	{
		Info.m_StartAction = StartChisel;
		Info.m_UseAction = UseChisel;
		Info.m_EndAction = EndChisel;
		Info.m_FarmerState = Farmer.State.StoneCutting;
		if (Info.m_ActionType == AFO.AT.Primary)
		{
			return ActionType.UseInHands;
		}
		return ActionType.Total;
	}

	public override ActionType GetActionFromObject(AFO Info)
	{
		ObjectType objectType = Info.m_ObjectType;
		if (FarmerStateStoneCutting.GetIsToolAcceptable(objectType))
		{
			return GetActionFromChisel(Info);
		}
		if (FarmerStateMining.GetIsToolAcceptable(objectType) || objectType == ObjectType.Rock)
		{
			return ActionType.Total;
		}
		return base.GetActionFromObject(Info);
	}
}
