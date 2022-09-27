using SimpleJSON;
using UnityEngine;

public class TreeMulberry : MyTree
{
	private GameObject m_Worms;

	private bool m_WormsActive;

	private AnimalSilkmoth m_Silkmoth;

	public override void Restart()
	{
		base.Restart();
		if (!ObjectTypeList.m_Loading)
		{
			CollectionManager.Instance.AddCollectable("MulberryTree", this);
		}
	}

	public override void StopUsing(bool AndDestroy = true)
	{
		if ((bool)m_Silkmoth)
		{
			m_Silkmoth.ClearTarget();
			m_Silkmoth = null;
		}
		base.StopUsing(AndDestroy);
	}

	public override string GetHumanReadableName()
	{
		return base.GetHumanReadableName();
	}

	public override void Load(JSONNode Node)
	{
		base.Load(Node);
		CollectionManager.Instance.AddCollectable("MulberryTree", this);
		UpdateModel();
	}

	protected override void GrowingFinished()
	{
		SetState(State.WaitingEmpty);
	}

	protected override void UpdateModel()
	{
		base.UpdateModel();
		Transform transform = m_ModelRoot.transform.Find("Fruit");
		if ((bool)transform)
		{
			transform.gameObject.SetActive(value: false);
			if (m_State == State.Waiting)
			{
				transform.gameObject.SetActive(value: true);
			}
			m_WormsActive = transform.gameObject.activeSelf;
		}
	}

	protected override void CreateChoppedGoodies(TileCoord StartPosition)
	{
		base.CreateChoppedGoodies(StartPosition);
		if (m_WormsActive)
		{
			TileCoordObject.SpawnObjects(m_TileCoord, ObjectType.AnimalSilkworm, 1, 2, AroundTile: true);
		}
		TileCoordObject.SpawnObjects(m_TileCoord, ObjectType.MulberrySeed, 1, 2, AroundTile: true);
		AudioManager.Instance.StartEvent("ObjectCreated", this);
	}

	protected override void CreateHammeredGoodies(TileCoord StartPosition)
	{
		base.CreateHammeredGoodies(StartPosition);
		if (m_WormsActive)
		{
			TileCoordObject.SpawnObjects(m_TileCoord, ObjectType.AnimalSilkworm, 1, 2, AroundTile: true);
		}
		TileCoordObject.SpawnObjects(m_TileCoord, ObjectType.MulberrySeed, 1, 2, AroundTile: true);
		AudioManager.Instance.StartEvent("ObjectCreated", this);
		SetState(State.WaitingEmpty);
		UpdateModel();
	}

	public void SetSilkmoth(AnimalSilkmoth Silkmoth)
	{
		m_Silkmoth = Silkmoth;
		Wake();
	}

	public AnimalSilkmoth GetSilkmoth()
	{
		return m_Silkmoth;
	}

	protected override ActionType GetActionFromMallet(AFO Info)
	{
		Info.m_UseAction = base.UseMallet;
		Info.m_EndAction = base.EndMallet;
		Info.m_FarmerState = Farmer.State.Hammering;
		if (Info.m_ActionType == AFO.AT.Primary)
		{
			if (m_State == State.Waiting || m_State == State.WaitingEmpty)
			{
				if (m_WormsActive)
				{
					Info.m_RequirementsOut = "FNOTreeFull";
				}
				else
				{
					Info.m_RequirementsOut = "FNOTreeEmpty";
				}
				if (Info.m_RequirementsIn == "" || Info.m_RequirementsIn == Info.m_RequirementsOut)
				{
					return ActionType.UseInHands;
				}
				return ActionType.Total;
			}
			return ActionType.Fail;
		}
		return ActionType.Total;
	}

	public override void UpdateStateGrowingFruit()
	{
		base.UpdateStateGrowingFruit();
		if (m_State == State.Waiting && (bool)m_Silkmoth)
		{
			m_Silkmoth.StopUsing();
			m_Silkmoth = null;
		}
	}

	private void UpdateTestSilkmoth()
	{
		if ((bool)m_Silkmoth)
		{
			Vector3 vector = base.transform.position - m_Silkmoth.transform.position;
			vector.y = 0f;
			if (vector.magnitude < 3f)
			{
				SetState(State.GrowingFruit);
			}
		}
	}

	private void UpdateStateWaitingEmpty()
	{
		m_GrownWobble.Update();
		UpdateScale();
		UpdateWobble();
		UpdateTestSilkmoth();
		if (m_GrownWobble.m_Height == 0f && m_WobbleTimer == 0f && m_Silkmoth == null)
		{
			Sleep();
		}
	}

	protected new void Update()
	{
		base.Update();
		if (m_State == State.WaitingEmpty)
		{
			UpdateStateWaitingEmpty();
		}
	}
}
