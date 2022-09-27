using System;
using System.Collections.Generic;
using UnityEngine;

public class FarmerStateFishing : FarmerStateTool
{
	private enum SubState
	{
		Stick,
		PullBack,
		Cast,
		Idle,
		Nibble,
		ReelIn,
		Land,
		Total
	}

	private static float m_CastDelay = 0.5f;

	private SubState m_SubState;

	private Quaternion m_Rotation;

	private float m_SubStateTimer;

	private TileCoord m_TargetTile;

	private ToolFishingRod m_FishingRod;

	private BaseClass m_NewFish;

	private float m_WaitTime;

	private bool m_FishWithBait;

	private static List<ObjectType> m_ShallowNormalFreshFish;

	private static List<ObjectType> m_DeepNormalFreshFish;

	private static List<ObjectType> m_DeepRareFreshFish;

	private static List<ObjectType> m_DeepSuperRareFreshFish;

	private static List<ObjectType> m_ShallowNormalSaltFish;

	private static List<ObjectType> m_DeepNormalSaltFish;

	private static List<ObjectType> m_DeepRareSaltFish;

	private static List<ObjectType> m_DeepSuperRareSaltFish;

	public override bool GetIsAdjacentTile(TileCoord TargetTile, BaseClass Object)
	{
		return false;
	}

	public override string GetNoToolIconName()
	{
		return "Tools/IconToolFishingRod";
	}

	public static bool GetIsToolAcceptable(ObjectType NewType)
	{
		if (NewType != ObjectType.ToolFishingStick)
		{
			return ToolFishingRod.GetIsTypeFishingRod(NewType);
		}
		return true;
	}

	public override bool IsToolAcceptable(Holdable NewObject)
	{
		if (NewObject == null)
		{
			return false;
		}
		return GetIsToolAcceptable(NewObject.m_TypeIdentifier);
	}

	public override void StartState()
	{
		base.StartState();
		m_SafetyDelay = 60f;
		if (m_Farmer.m_FarmerCarry.m_CarryObject[0].m_TypeIdentifier == ObjectType.ToolFishingStick)
		{
			m_ActionSoundName = "ToolFishingStickHit";
			GetFinalRotationTile();
			StandInTargetTile();
			m_TargetTile = m_Farmer.m_FarmerAction.m_CurrentInfo.m_Position;
			m_Farmer.StartAnimation("FarmerFishingStick");
			SetState(SubState.Stick);
		}
		else
		{
			m_Farmer.m_FinalPosition = m_Farmer.transform.position;
			m_TargetTile = m_Farmer.m_FarmerAction.m_CurrentInfo.m_Position;
			TileCoord tileCoord = m_TargetTile - m_Farmer.m_TileCoord;
			float num = Mathf.Atan2(tileCoord.y, tileCoord.x);
			m_Rotation = Quaternion.Euler(0f, num * 57.29578f - 90f, 0f);
			m_Farmer.transform.rotation = m_Rotation;
			m_FishingRod = m_Farmer.m_FarmerCarry.m_ToolModel.GetComponent<ToolFishingRod>();
			float variableAsFloat = VariableManager.Instance.GetVariableAsFloat(m_FishingRod.m_TypeIdentifier, "MinDelay");
			float variableAsFloat2 = VariableManager.Instance.GetVariableAsFloat(m_FishingRod.m_TypeIdentifier, "MaxDelay");
			m_WaitTime = UnityEngine.Random.Range(variableAsFloat, variableAsFloat2);
			SetState(SubState.PullBack);
		}
		m_FishWithBait = !m_Farmer.m_FarmerCarry.m_CarryObject[0].GetComponent<ToolFillable>().GetIsEmpty();
	}

	private List<ObjectType> GetObjectTypes(string ListName)
	{
		List<object> variableAsList = VariableManager.Instance.GetVariableAsList(ListName);
		List<ObjectType> list = new List<ObjectType>();
		foreach (object item in variableAsList)
		{
			ObjectType identifierFromSaveName = ObjectTypeList.Instance.GetIdentifierFromSaveName((string)item);
			list.Add(identifierFromSaveName);
		}
		return list;
	}

	private void CacheFishingVariables()
	{
		if (m_ShallowNormalFreshFish == null)
		{
			m_ShallowNormalFreshFish = GetObjectTypes("ShallowNormalFreshFish");
			m_DeepNormalFreshFish = GetObjectTypes("DeepNormalFreshFish");
			m_DeepRareFreshFish = GetObjectTypes("DeepRareFreshFish");
			m_DeepSuperRareFreshFish = GetObjectTypes("DeepSuperRareFreshFish");
			m_ShallowNormalSaltFish = GetObjectTypes("ShallowNormalSaltFish");
			m_DeepNormalSaltFish = GetObjectTypes("DeepNormalSaltFish");
			m_DeepRareSaltFish = GetObjectTypes("DeepRareSaltFish");
			m_DeepSuperRareSaltFish = GetObjectTypes("DeepSuperRareSaltFish");
		}
	}

	private ObjectType GetFishTypeFromList(List<ObjectType> FishList)
	{
		int index = UnityEngine.Random.Range(0, FishList.Count);
		return FishList[index];
	}

	private ObjectType GetFishType()
	{
		ObjectType result = ObjectType.FishSalmon;
		ObjectType lastObjectType = m_Farmer.m_FarmerCarry.GetLastObjectType();
		string text = "FishChance";
		if (m_FishWithBait)
		{
			text += "Bait";
		}
		float variableAsFloat = VariableManager.Instance.GetVariableAsFloat(lastObjectType, text);
		float variableAsFloat2 = VariableManager.Instance.GetVariableAsFloat(lastObjectType, text + "Rare");
		float variableAsFloat3 = VariableManager.Instance.GetVariableAsFloat(lastObjectType, text + "SuperRare");
		Tile.TileType tileType = TileManager.Instance.GetTileType(m_TargetTile);
		CacheFishingVariables();
		float num = UnityEngine.Random.Range(0f, 100f);
		if (num < variableAsFloat3)
		{
			switch (tileType)
			{
			case Tile.TileType.WaterDeep:
				result = GetFishTypeFromList(m_DeepSuperRareFreshFish);
				break;
			case Tile.TileType.SeaWaterDeep:
				result = GetFishTypeFromList(m_DeepSuperRareSaltFish);
				break;
			}
		}
		else if (num < variableAsFloat2 + variableAsFloat3)
		{
			switch (tileType)
			{
			case Tile.TileType.WaterDeep:
				result = GetFishTypeFromList(m_DeepRareFreshFish);
				break;
			case Tile.TileType.SeaWaterDeep:
				result = GetFishTypeFromList(m_DeepRareSaltFish);
				break;
			}
		}
		else if (!(num < variableAsFloat + variableAsFloat2 + variableAsFloat3))
		{
			result = ((tileType != Tile.TileType.WaterShallow && tileType != Tile.TileType.WaterDeep) ? ObjectType.FishingJunkSalt : ObjectType.FishingJunkFresh);
		}
		else
		{
			switch (tileType)
			{
			case Tile.TileType.WaterShallow:
				result = GetFishTypeFromList(m_ShallowNormalFreshFish);
				break;
			case Tile.TileType.WaterDeep:
				result = GetFishTypeFromList(m_DeepNormalFreshFish);
				break;
			case Tile.TileType.SeaWaterShallow:
				result = GetFishTypeFromList(m_ShallowNormalSaltFish);
				break;
			case Tile.TileType.SeaWaterDeep:
				result = GetFishTypeFromList(m_DeepNormalSaltFish);
				break;
			}
		}
		return result;
	}

	private void CreateFish()
	{
		ObjectType fishType = GetFishType();
		m_NewFish = ObjectTypeList.Instance.CreateObjectFromIdentifier(fishType, m_TargetTile.ToWorldPositionTileCentered(), Quaternion.identity);
		float y = UnityEngine.Random.Range(0, 360);
		m_NewFish.transform.localRotation = Quaternion.Euler(0f, y, 0f);
		SpawnAnimationManager.Instance.AddJump(m_NewFish, m_TargetTile, m_Farmer.m_TileCoord, 0f, m_Farmer.m_TileCoord.GetHeightOffGround(), 5f, 0.5f);
		AudioManager.Instance.StartEvent("ToolPaddleSplash", m_Farmer);
		m_Farmer.CreateParticles(m_TargetTile.ToWorldPositionTileCentered(), "Splash");
		if (Fish.GetIsTypeFish(m_NewFish.m_TypeIdentifier))
		{
			QuestManager.Instance.AddEvent(QuestEvent.Type.CatchFish, m_Farmer.m_TypeIdentifier == ObjectType.Worker, 0, m_Farmer);
			BadgeManager.Instance.AddEvent(BadgeEvent.Type.Fish);
		}
	}

	public override void EndState()
	{
		base.EndState();
		if (m_SubState == SubState.Stick)
		{
			StandInTargetTile();
			return;
		}
		m_Farmer.transform.rotation = m_Rotation;
		m_FishingRod.SetLineVisible(Visible: false);
		m_FishingRod.SetFloatVisible(Visible: false);
		if ((bool)m_Farmer.m_FarmerCarry.m_ToolModel)
		{
			m_Farmer.m_FarmerCarry.m_ToolModel.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
		}
		m_Farmer.transform.rotation = m_Rotation;
		m_Farmer.transform.position = m_Farmer.m_FinalPosition;
	}

	private void SetState(SubState NewState)
	{
		m_SubState = NewState;
		m_SubStateTimer = 0f;
		switch (m_SubState)
		{
		case SubState.PullBack:
			m_Farmer.m_FarmerCarry.m_ToolModel.transform.localRotation = Quaternion.Euler(30f, 0f, 0f);
			m_Farmer.transform.rotation = m_Rotation * Quaternion.Euler(20f, 0f, 0f);
			break;
		case SubState.Cast:
			m_Farmer.m_FarmerCarry.m_ToolModel.transform.localRotation = Quaternion.Euler(-70f, 0f, 0f);
			m_Farmer.transform.rotation = m_Rotation * Quaternion.Euler(-20f, 0f, 0f);
			m_Farmer.transform.position = m_Farmer.m_FinalPosition + new Vector3(0f, 1f, 0f);
			m_FishingRod.SetLineVisible(Visible: true);
			m_FishingRod.SetFloatVisible(Visible: true);
			UpdateFishingRod();
			break;
		case SubState.Idle:
			m_Farmer.m_FarmerCarry.m_ToolModel.transform.localRotation = Quaternion.Euler(-45f, 0f, 0f);
			m_Farmer.transform.rotation = m_Rotation;
			m_Farmer.transform.position = m_Farmer.m_FinalPosition;
			break;
		case SubState.ReelIn:
			m_Farmer.m_FarmerCarry.m_ToolModel.transform.localRotation = Quaternion.Euler(-20f, 0f, 0f);
			m_FishingRod.SetFloatVisible(Visible: false);
			break;
		case SubState.Land:
			m_Farmer.m_FarmerCarry.m_ToolModel.transform.localRotation = Quaternion.Euler(-20f, 0f, 0f);
			m_Farmer.transform.rotation = m_Rotation * Quaternion.Euler(40f, 0f, 0f);
			break;
		case SubState.Nibble:
			break;
		}
	}

	protected override void SendEvents(Actionable TargetObject)
	{
		base.SendEvents(TargetObject);
	}

	public override void DoAnimationAction()
	{
		base.DoAnimationAction();
		if (m_SubState == SubState.Stick)
		{
			m_Farmer.CreateParticles(m_Farmer.m_FinalPosition, "SplashSmall");
		}
	}

	private void WearTool()
	{
		Holdable heldObject = GetHeldObject();
		if (m_FishWithBait && (bool)heldObject && !heldObject.GetComponent<ToolFillable>().GetIsEmpty())
		{
			heldObject.GetComponent<ToolFillable>().Empty(1);
		}
	}

	protected override void ActionSuccess(Actionable TargetObject)
	{
		base.ActionSuccess(TargetObject);
		if (m_SubState == SubState.Stick)
		{
			CreateFish();
			WearTool();
		}
	}

	private void UpdateFishingRod()
	{
		float num = m_SubStateTimer / m_CastDelay;
		Vector3 position = m_FishingRod.m_LinePoint.transform.position;
		Vector3 position2 = (m_TargetTile.ToWorldPositionTileCentered() - position) * num + position;
		position2.y += Mathf.Sin(num * (float)Math.PI) * 7f;
		m_FishingRod.SetFloatPosition(position2, 3f - 2f * num);
	}

	public override void UpdateState()
	{
		base.UpdateState();
		switch (m_SubState)
		{
		case SubState.PullBack:
			if (m_SubStateTimer > 0.25f)
			{
				SetState(SubState.Cast);
			}
			break;
		case SubState.Cast:
			UpdateFishingRod();
			if (m_SubStateTimer > m_CastDelay)
			{
				AudioManager.Instance.StartEvent("ToolPaddleSplash", m_Farmer);
				m_Farmer.CreateParticles(m_FishingRod.GetFloatPosition(), "Splash");
				SetState(SubState.Idle);
			}
			break;
		case SubState.Idle:
			m_FishingRod.SetFloatPosition(m_TargetTile.ToWorldPositionTileCentered(), 1f);
			if (m_SubStateTimer > m_WaitTime)
			{
				SetState(SubState.Nibble);
			}
			break;
		case SubState.Nibble:
		{
			Vector3 position = m_TargetTile.ToWorldPositionTileCentered();
			if ((int)(m_SubStateTimer * 60f) % 12 < 4)
			{
				position.y -= 0.25f;
			}
			m_FishingRod.SetFloatPosition(position, 1f);
			if (m_SubStateTimer > 0.66f)
			{
				SetState(SubState.ReelIn);
			}
			break;
		}
		case SubState.ReelIn:
			if ((int)(m_SubStateTimer * 60f) % 10 < 5)
			{
				m_Farmer.transform.rotation = m_Rotation * Quaternion.Euler(40f, 0f, 0f);
			}
			else
			{
				m_Farmer.transform.rotation = m_Rotation * Quaternion.Euler(20f, 0f, 0f);
			}
			m_FishingRod.SetFloatPosition(m_TargetTile.ToWorldPositionTileCentered(), 0f);
			if (m_SubStateTimer > 1f)
			{
				SetState(SubState.Land);
				CreateFish();
				WearTool();
			}
			break;
		case SubState.Land:
		{
			float num = 0.5f;
			float num2 = m_SubStateTimer / num;
			if ((bool)m_NewFish)
			{
				m_FishingRod.SetFloatPosition(m_NewFish.transform.position, 3f * num2);
			}
			if (m_SubStateTimer > num)
			{
				m_FishingRod.SetLineVisible(Visible: false);
				Actionable targetObject = GetTargetObject();
				SendEvents(targetObject);
				DegradeTool();
				DoEndAction();
			}
			break;
		}
		}
		m_SubStateTimer += TimeManager.Instance.m_NormalDelta;
	}
}
