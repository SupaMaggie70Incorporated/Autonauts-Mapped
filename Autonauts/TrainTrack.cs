using System.Collections.Generic;
using UnityEngine;

public class TrainTrack : Floor
{
	public static float m_Height = 0.4f;

	[HideInInspector]
	public TrainTrack m_ConnectedUp;

	[HideInInspector]
	public TrainTrack m_ConnectedDown;

	[HideInInspector]
	public TrainTrack m_ConnectedLeft;

	[HideInInspector]
	public TrainTrack m_ConnectedRight;

	protected bool m_ConnectedUpChanged;

	protected bool m_ConnectedDownChanged;

	protected bool m_ConnectedLeftChanged;

	protected bool m_ConnectedRightChanged;

	protected bool m_PlayerDeleted;

	protected bool m_BuffersChanged;

	private BaseClass[] m_Buffers;

	public static bool GetIsTypeTrainTrack(ObjectType NewType)
	{
		if (NewType == ObjectType.TrainTrack || NewType == ObjectType.TrainTrackCurve || NewType == ObjectType.TrainTrackBridge || NewType == ObjectType.TrainTrackPointsLeft || NewType == ObjectType.TrainTrackPointsRight)
		{
			return true;
		}
		return false;
	}

	public override void Restart()
	{
		base.Restart();
		m_ConnectedUp = null;
		m_ConnectedDown = null;
		m_ConnectedLeft = null;
		m_ConnectedRight = null;
		m_ConnectedUpChanged = true;
		m_ConnectedDownChanged = true;
		m_ConnectedLeftChanged = true;
		m_ConnectedRightChanged = true;
		m_PlayerDeleted = false;
		m_BuffersChanged = true;
		BaseClass[] array = (m_Buffers = new TrainTrackBuffer[4]);
	}

	public override void StopUsing(bool AndDestroy = true)
	{
		base.StopUsing(AndDestroy);
		DestroyBuffers();
	}

	public override void PlayerDeleted()
	{
		base.PlayerDeleted();
		m_PlayerDeleted = true;
	}

	public override void PostLoad()
	{
		base.PostLoad();
		Refresh();
		RefreshBuffers();
	}

	protected void DestroyBuffer(int Index)
	{
		if ((bool)m_Buffers[Index])
		{
			m_Buffers[Index].StopUsing();
			m_Buffers[Index] = null;
		}
	}

	protected void DestroyBuffers()
	{
		for (int i = 0; i < 4; i++)
		{
			DestroyBuffer(i);
		}
	}

	protected void CreateBuffer(int Index, Vector3 LocalPosition, Quaternion LocalRotation)
	{
		BaseClass baseClass = ObjectTypeList.Instance.CreateObjectFromIdentifier(ObjectType.TrainTrackBuffer, base.transform.position, base.transform.localRotation);
		baseClass.transform.SetParent(m_ModelRoot.transform);
		baseClass.transform.localPosition = LocalPosition;
		baseClass.transform.localRotation = LocalRotation;
		m_Buffers[Index] = baseClass;
	}

	public virtual void RefreshBuffers(bool Force = false)
	{
	}

	public override void SendAction(ActionInfo Info)
	{
		base.SendAction(Info);
		if (Info.m_Action == ActionType.RefreshFirst)
		{
			RefreshBuffers(Force: true);
		}
		if ((Info.m_Action == ActionType.Refresh || Info.m_Action == ActionType.RefreshFirst) && m_LinkedSystem != null)
		{
			((LinkedSystemTrack)m_LinkedSystem).UpdateCounts();
		}
	}

	public override object GetActionInfo(GetActionInfo Info)
	{
		GetAction action = Info.m_Action;
		if ((uint)(action - 3) <= 1u && m_LinkedSystem != null && !((LinkedSystemTrack)m_LinkedSystem).GetCanDeleteTrack(this))
		{
			return false;
		}
		return base.GetActionInfo(Info);
	}

	protected override void RefreshConnected()
	{
		if ((bool)m_ConnectedUp)
		{
			RefreshManager.Instance.AddObject(m_ConnectedUp);
		}
		if ((bool)m_ConnectedDown)
		{
			RefreshManager.Instance.AddObject(m_ConnectedDown);
		}
		if ((bool)m_ConnectedLeft)
		{
			RefreshManager.Instance.AddObject(m_ConnectedLeft);
		}
		if ((bool)m_ConnectedRight)
		{
			RefreshManager.Instance.AddObject(m_ConnectedRight);
		}
	}

	public virtual TrainTrack GetConnectedTrackInDirection(TileCoord Current, TileCoord Direction)
	{
		return null;
	}

	public static TrainTrack GetConnectedTrackInDirection(TileCoord Current, bool Vertical, bool Horizontal, TileCoord Direction)
	{
		TileCoord tileCoord = Current + Direction;
		if (tileCoord.y < 0 || tileCoord.y >= TileManager.Instance.m_TilesHigh || tileCoord.x < 0 || tileCoord.x >= TileManager.Instance.m_TilesWide)
		{
			return null;
		}
		Building floor = TileManager.Instance.GetTile(tileCoord).m_Floor;
		if (floor == null)
		{
			return null;
		}
		TrainTrack component = floor.GetComponent<TrainTrack>();
		if (component == null)
		{
			return null;
		}
		if (floor.m_TypeIdentifier == ObjectType.TrainTrackCurve)
		{
			TrainTrackCurve component2 = component.GetComponent<TrainTrackCurve>();
			if (tileCoord == component2.GetStartPosition())
			{
				TileCoord startDirection = component2.GetStartDirection();
				if (Vertical && startDirection.y != 0)
				{
					return component;
				}
				if (Horizontal && startDirection.x != 0)
				{
					return component;
				}
			}
			if (Current == component2.GetStartPosition() && component2.GetStartDirection() == Direction)
			{
				return component;
			}
			if (tileCoord == component2.GetEndPosition())
			{
				TileCoord endDirection = component2.GetEndDirection();
				if (Vertical && endDirection.y != 0)
				{
					return component;
				}
				if (Horizontal && endDirection.x != 0)
				{
					return component;
				}
			}
			if (Current == component2.GetEndPosition() && component2.GetEndDirection() == Direction)
			{
				return component;
			}
			return null;
		}
		if (TrainTrackPoints.GetIsTypeTrainTrackPoints(floor.m_TypeIdentifier))
		{
			TrainTrackPoints component3 = component.GetComponent<TrainTrackPoints>();
			if (tileCoord == component3.GetStartPosition() || tileCoord == component3.GetEndPosition1() || tileCoord == component3.GetEndPosition2() || Current == component3.GetStartPosition() || Current == component3.GetEndPosition1() || Current == component3.GetEndPosition2())
			{
				if (Vertical && (component3.m_Rotation == 0 || component3.m_Rotation == 2))
				{
					return component;
				}
				if (Horizontal && (component3.m_Rotation == 1 || component3.m_Rotation == 3))
				{
					return component;
				}
			}
			return null;
		}
		if (Direction.y != 0 && Vertical && floor.GetComponent<TrainTrackStraight>().GetVertical())
		{
			return component;
		}
		if (Direction.x != 0 && Horizontal && floor.GetComponent<TrainTrackStraight>().GetHorizontal())
		{
			return component;
		}
		return null;
	}

	public override bool CanBuildOn()
	{
		return false;
	}

	public virtual void GetCarriagePosition(float Distance, TileCoord AttachDirection, bool MoveForwards, Vector3 OldPosition, Transform OldTransform, out Vector3 NewPosition, out Quaternion NewRotation, List<TrainTrackPoints> SwitchList, Minecart NewMinecart)
	{
		NewPosition = m_TileCoord.ToWorldPositionTileCentered();
		NewRotation = Quaternion.identity;
	}

	public virtual bool CheckTrackAhead(float Distance, TileCoord MoveDirection, Vector3 OldPosition, List<TrainTrackPoints> SwitchList)
	{
		return true;
	}

	public virtual bool CheckForMinecart(float Distance, TileCoord MoveDirection, Vector3 OldPosition, TrainTrack TestTrack, List<TrainTrackPoints> SwitchList)
	{
		return true;
	}

	public override ActionType GetActionFromObject(AFO Info)
	{
		if (Info.m_ActionType == AFO.AT.Secondary && Info.m_Actioner.GetComponent<Crane>() != null)
		{
			Crane component = Info.m_Actioner.GetComponent<Crane>();
			if ((bool)component.m_HeldObject && !Minecart.GetIsTypeMinecart(component.m_HeldObject.m_TypeIdentifier))
			{
				return ActionType.Fail;
			}
		}
		return base.GetActionFromObject(Info);
	}
}
